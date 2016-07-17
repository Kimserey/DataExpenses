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
    } 
    and Cost = Cost of float
        with override x.ToString () = match x with Cost v -> sprintf "Cost: %.4f%%" v
                  
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
        match train options |> List.last with
        | thetha0::thetha1::_ as thethas->
            { Estimate = fun  x -> thetha0 + thetha1 * x
              Cost = costFunc thethas options.Dataset |> Cost
              Thethas = thethas }
        | _ -> failwith "Failed to create model. Could not compute thethas."
    
module Estimation =
    open GradientDescent

    let dataset =
        df
        |> Frame.groupRowsBy "Date"
        |> Frame.sortRowsByKey
        |> Frame.getNumericCols
        |> Series.get "Amount"
        |> Stats.levelSum fst
        |> Series.groupInto 
            (fun (date: DateTime) _ -> date.Month, date.Year)
            (fun _ s -> 
                s 
                |> Series.sortByKey
                |> Series.mapKeys (fun (date: DateTime) -> date.Day)
                |> Stats.expandingSum
                |> Series.observations
                |> Seq.map (fun (day, value) -> float day, value)
                |> Seq.toList)
    
    let estimates =
        dataset
        |> Series.mapValues (fun values ->
            let modelResult = 
                GradientDescent.createModel ({ LearningRate = 0.005; Dataset = values; Iterations = 5000 })
            
            modelResult.Cost,
            values
            |> List.map (fst >> modelResult.Estimate))


(*
    Webserver test  
*)

#I __SOURCE_DIRECTORY__
#r "../packages/Suave/lib/net40/Suave.dll"
#r "../packages/Newtonsoft.Json/lib/net40/Newtonsoft.Json.dll"

open Suave
open Suave.Files
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Writers
open Newtonsoft.Json
open Newtonsoft.Json.Serialization


let getData month =
    df
    |> Frame.filterRowValues (fun c -> 
        let date = c.GetAs<DateTime>("Date")
        date.Month = month && date.Year = 2016)
    |> Frame.groupRowsBy "Date"
    |> Frame.sortRowsByKey
    |> Frame.getNumericCols
    |> Series.get "Amount"
    |> Stats.levelSum fst
    |> Stats.expandingSum
    |> Series.observations
    |> Seq.map (fun ((date: DateTime), value) ->
        float date.Day, value)
    |> Seq.toList

let training    = getData 4
let validation  = getData 5
let validation' = getData 6

let costs = 
    GradientDescent.train (GradientDescent.Options.Default validation)
    |> List.chunkBySize 100
    |> List.map List.last
    |> List.mapi (fun i thethas -> float (i * 100), GradientDescent.costFunc thethas training)

let thethas = 
    GradientDescent.train (GradientDescent.Options.Default validation)
    |> List.last

printfn "Cost with training data:    %10.4f" (GradientDescent.costFunc thethas training)
printfn "Cost with validation data:  %10.4f" (GradientDescent.costFunc thethas validation)
printfn "Cost with validation' data: %10.4f" (GradientDescent.costFunc thethas validation')    

let JSON v =
  OK (JsonConvert.SerializeObject(v, new JsonSerializerSettings(ContractResolver = new CamelCasePropertyNamesContractResolver())))
  >=> setMimeType "application/json; charset=utf-8"
  >=> setHeader "Access-Control-Allow-Origin" "*"
  >=> setHeader "Access-Control-Allow-Headers" "content-type"

type Data = {
    X: float
    Y: float
} with
    static member FromTuple (x, y) = { X = x; Y = y }
    
let app = 
    GET >=> choose
        [ path "/data" 
            >=> JSON 
                [ 
                    validation 
                    |> List.map Data.FromTuple
                    
                    validation'
                    |> List.map Data.FromTuple

                    validation 
                    |> List.map fst 
                    |> List.map (fun i -> i, thethas.[0] + thethas.[1] * i) 
                    |> List.map Data.FromTuple
                ]

          path "/costs"
            >=> JSON
                [
                    costs
                    |> List.map Data.FromTuple
                ]
        ]

startWebServer defaultConfig app