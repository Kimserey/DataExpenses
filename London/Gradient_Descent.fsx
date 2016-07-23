#load "_references.fsx"
open _references
open System
open Deedle
open London.Core
open London.Core.GradientDescent

(*
    Webserver test  
*)

#I __SOURCE_DIRECTORY__
#r "../packages/Suave/lib/net40/Suave.dll"
#r "../packages/Newtonsoft.Json/lib/net40/Newtonsoft.Json.dll"

open System.Globalization
open Suave
open Suave.Http
open Suave.Files
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Writers
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open GradientDescent
open System.Net


let JSON v =
  OK (JsonConvert.SerializeObject(v, new JsonSerializerSettings(ContractResolver = new CamelCasePropertyNamesContractResolver())))
  >=> setMimeType "application/json; charset=utf-8"
  >=> setHeader "Access-Control-Allow-Origin" "*"
  >=> setHeader "Access-Control-Allow-Headers" "content-type"

type Dataset ={
    Name: string
    Originals: DataItem list
    Estimates: DataItem list
}
and DataItem = {
    X: float
    Y: float
} with
    static member FromTuple (x, y) = { X = x; Y = y }
    
type Series 
    with 
        static member ToList =
            Series.observations 
            >> Seq.map (fun (day, value) -> float day, value) 
            >> Seq.toList

type ThethaSteps = {
    IterationNum: int
    Cost: float
    Estimations: DataItem list
}

type ThethaCost = {
    Cost: float
    Thetha0: float
    Thetha1: float
}

let defaultSettings data =
    { LearningRate = 0.0025
      Dataset = Series.ToList data
      Iterations = 10 }

let getData predicate =
    df
    |> Frame.groupRowsBy "Date"
    |> Frame.sortRowsByKey
    |> Frame.filterRowValues predicate
    |> Frame.getNumericCols
    |> Series.get "Amount"
    |> Stats.levelSum fst
    |> Series.groupInto 
        (fun (date: DateTime) _ -> date.Month, date.Year)
        (fun _ s -> 
            s 
            |> Series.sortByKey
            |> Series.mapKeys (fun (date: DateTime) -> date.Day)
            |> Series.realign [1..(if s.FirstKey().Month = DateTime.Now.Month then DateTime.Now.Day else 31)] 
            |> Series.fillMissingWith 0.
            |> Stats.expandingSum)

let getDataset predicate =
    getData predicate
    |> Series.mapValues (fun values ->
        let modelResult = 
            GradientDescent.createModel  (defaultSettings values)
        
        let totalDays = [1..31]

        modelResult.Cost,
        Series.ToList values,
        totalDays
        |> List.map (fun x -> float x, modelResult.Estimate (float x)))
    |> Series.observations
    |> Seq.map (fun ((month, year), (_, originals, estimateResults)) ->
        { Name = sprintf "%s %i" (CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName month) year
          Originals = originals |> List.map DataItem.FromTuple
          Estimates = estimateResults |> List.map DataItem.FromTuple  })
    |> Seq.toList


let getThethaSteps predicate =
    getData predicate
    |> Series.mapValues (fun values ->
        GradientDescent.estimate (defaultSettings values)
        |> List.mapi (fun i thethas -> (i, thethas))
        |> List.choose (fun (i, thethas: float list) -> 
            match thethas with
            | thetha0::thetha1::_ -> 
                Some { IterationNum = i
                       Cost = Cost.Value <| Cost.Compute (Series.ToList values, thethas)
                       Estimations = 
                            [1..31] 
                            |> List.map (fun i ->  
                                { X = float i
                                  Y = thetha0 + thetha1 * (float i) }) }
            | _ -> 
                None))
    |> Series.observations
    |> Seq.head
    |> snd

let getThethas predicate =
    getData predicate
    |> Series.mapValues (fun values ->
        [ for t0 in [0.0..0.1..45.0] do
            yield [ for t1 in [5.5..0.1..8.5] do
                        let cost = Cost.Value <| Cost.Compute (Series.ToList values, [float t0; float t1])
                        yield { Cost = cost
                                Thetha0 = float t0
                                Thetha1 = float t1  }] ])
    |> Series.observations
    |> Seq.head
    |> snd


let predicate (c:ObjectSeries<_>) =
    c.GetAs<string>("Category") = (string Category.Supermarket)
    && c.GetAs<DateTime>("Date").Month = 5 
    && c.GetAs<DateTime>("Date").Year = 2016

let data    = getDataset predicate
let steps   = getThethaSteps predicate
let thethas = getThethas predicate

let app = 
    GET >=> choose
        [ path "/data" >=> JSON data 
          path "/steps" >=> JSON steps
          path "/thethas" >=> JSON thethas ]

startWebServer defaultConfig app