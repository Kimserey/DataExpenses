#load "_references.fsx"
open _references
open System
open Deedle
open London.Core

module GradientDescent =

    type Options = {
        LearningRate: float
        Dataset: List<float * float>
        Iterations: int
    } with
        static member Default dataset = { 
            LearningRate = 0.006
            Dataset = dataset
            Iterations = 5000 
        }

    type ModelResult = {
        Estimate: float -> float
        Cost: Cost
        Thethas: float list
        ThethaCalculationSteps: ThethaCalculationSteps
    }
    and ThethaCalculationSteps = ThethaCalculationSteps of float list list
        with override x.ToString() = match x with ThethaCalculationSteps v -> sprintf "Thethas: %A" v
    and Cost = Cost of float
        with override x.ToString() = match x with Cost v -> sprintf "Cost: %.4f%%" v
                  
    let costFunc thethas (data: List<float * float>): float =
        match thethas with
        | thetha0::thetha1::_ ->
            let sum = 
                [0..data.Length - 1] 
                |> List.map (fun i -> data.[i])
                |> List.map (fun (x, y) -> thetha0 + thetha1 * x - y)
                |> List.sum

            (1./float data.Length) * (Math.Pow(sum, 2.))
        | _ -> failwith "Could not compute cost function, thethas are not in correct format."

    let next thethas (options: Options) =
        match thethas with
        | thetha0::thetha1::_ ->
            let thetha0' = 
                let sum = 
                    [0..options.Dataset.Length - 1] 
                    |> List.map (fun i -> options.Dataset.[i])
                    |> List.map (fun (x, y) -> thetha0 + thetha1 * x - y)
                    |> List.sum

                thetha0 - (options.LearningRate * (1./float options.Dataset.Length) * sum)

            let thetha1' =
                let sum =
                    [0..options.Dataset.Length - 1]
                    |> List.map (fun i -> options.Dataset.[i])
                    |> List.map (fun (x, y) -> (thetha0 + thetha1 * x - y) * x)
                    |> List.sum
        
                thetha1 - (options.LearningRate * (1./float options.Dataset.Length) * sum)

            [thetha0'; thetha1']
        | _ -> failwith "Could not compute next thethas, thethas are not in correct format."

    let train options =
        [0..options.Iterations]
        |> List.scan (fun thethas _ -> next thethas options) [0.; 0.]

    let createModel options =
        let interationSteps = train options

        match List.last interationSteps with
        | thetha0::thetha1::_ as thethas->
            { Estimate = fun  x -> thetha0 + thetha1 * x
              Cost = costFunc thethas options.Dataset |> Cost
              Thethas = thethas
              ThethaCalculationSteps = ThethaCalculationSteps interationSteps }
        | _ -> failwith "Failed to create model. Could not compute thethas."
    
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
    |> Series.skip 1
    |> Series.mapValues (fun values ->
        let toList =
            Series.observations 
            >> Seq.map (fun (day, value) -> float day, value) 
            >> Seq.toList

        let modelResult = 
            GradientDescent.createModel 
                { LearningRate = 0.005
                  Dataset = toList values
                  Iterations = 8000 }
        
        let totalDays = [1..31]

        modelResult.Cost,
        toList values,
        totalDays
        |> List.map float
        |> List.map (fun x -> x, modelResult.Estimate x))
    |> Series.observations
    |> Seq.map (fun ((month, year), (_, originals, estimateResults)) ->
        { Name = sprintf "%s %i" (CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName month) year
          Originals = originals |> List.map DataItem.FromTuple
          Estimates = estimateResults |> List.map DataItem.FromTuple  })
    |> Seq.toList

let app = 
    GET >=> choose
        [ path "/data" >=> JSON (getDataset (fun (c:ObjectSeries<_>) -> c.GetAs<string>("Category") <> (string Category.RentAndBills))) ]

startWebServer defaultConfig app