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
open Suave.Files
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Writers
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open GradientDescent


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

let defaultSettings data =
    { LearningRate = 0.005
      Dataset = Series.ToList data
      Iterations = 8000 }

let getDataset filterRowPredicate =
    df
    |> Frame.groupRowsBy "Date"
    |> Frame.sortRowsByKey
    |> Frame.filterRowValues filterRowPredicate
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
    |> Series.mapValues (fun values ->
        let modelResult = 
            GradientDescent.createModel  (defaultSettings values)
        
        let totalDays = [1..31]

        modelResult.Cost,
        Series.ToList values,
        totalDays
        |> List.map float
        |> List.map (fun x -> x, modelResult.Estimate x))
    |> Series.observations
    |> Seq.map (fun ((month, year), (_, originals, estimateResults)) ->
        { Name = sprintf "%s %i" (CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName month) year
          Originals = originals |> List.map DataItem.FromTuple
          Estimates = estimateResults |> List.map DataItem.FromTuple  })
    |> Seq.toList

type ThethaSteps = {
    IterationNum: int
    Cost: float
    Estimations: float list
}

let getThethaSteps filterRowPredicate =
    df
    |> Frame.groupRowsBy "Date"
    |> Frame.sortRowsByKey
    |> Frame.filterRowValues filterRowPredicate
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
    |> Series.mapValues (fun values ->
        GradientDescent.estimate (defaultSettings values)
        |> List.mapi (fun i thethas -> (i, thethas))
        |> List.chunkBySize 1000
        |> List.map List.last
        |> List.choose (fun (i, thethas: float list) -> 
            match thethas with
            | thetha0::thetha1::_ -> Some { IterationNum = i; Cost = Cost.Value <| Cost.Compute (Series.ToList values, thethas); Estimations = [1..31] |> List.map (fun i ->  thetha0 + thetha1 * (float i)) }
            | _                   -> None))
    |> Series.observations
    |> Seq.map snd
    |> Seq.toList

let predicate (c:ObjectSeries<_>) =
    c.GetAs<string>("Category") = (string Category.Supermarket)
    && c.GetAs<DateTime>("Date").Month = 5 
    && c.GetAs<DateTime>("Date").Year = 2016

let data  = getDataset predicate
let steps = getThethaSteps predicate

let app = 
    GET >=> choose
        [ path "/data" >=> JSON data 
          path "/steps" >=> JSON steps ]

startWebServer defaultConfig app