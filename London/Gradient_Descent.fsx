#load "_references.fsx"
open _references
open System
open Deedle
open London.Core

(*
    Experimentation
*)

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

// Apply linear regression to July 2016 to predict expenses of the monht
// Least square cost cost function = 1/2N * Sum (estimate - real value) square
// Estimate = Theta1 * x + Theta0

// learning rate alpha
let alpha = 0.006

let costFunc thethas (data: List<_>): float =
    match thethas with
    | thetha0::thetha1::_ ->
        let sum = 
            [0..data.Length - 1] 
            |> List.map (fun i -> data.[i])
            |> List.map (fun (x, y) -> thetha0 + thetha1 * x - y)
            |> List.sum

        (1./float data.Length) * (Math.Pow(sum, 2.))
    | _ -> failwith "Could not compute cost function, thethas are not in correct format."

let next thethas (data: List<_>) =
    match thethas with
    | thetha0::thetha1::_ ->
        let thetha0' = 
            let sum = 
                [0..data.Length - 1] 
                |> List.map (fun i -> data.[i])
                |> List.map (fun (x, y) -> thetha0 + thetha1 * x - y)
                |> List.sum

            thetha0 - (alpha * (1./float data.Length) * sum)

        let thetha1' =
            let sum =
                [0..data.Length - 1]
                |> List.map (fun i -> data.[i])
                |> List.map (fun (x, y) -> (thetha0 + thetha1 * x - y) * x)
                |> List.sum
        
            thetha1 - (alpha * (1./float data.Length) * sum)

        [thetha0'; thetha1']
    | _ -> failwith "Could not compute next thethas, thethas are not in correct format."

let train iterations data =
    [0..iterations]
    |> List.scan (fun thethas _ -> next thethas data) [0.; 0.]
    
let allThetasIterations = 
    train 5000 validation
    
let costs = 
    allThetasIterations
    |> List.chunkBySize 100
    |> List.map List.last
    |> List.mapi (fun i thethas -> float (i * 100), costFunc thethas training)

let thethas = 
    allThetasIterations |> List.last

printfn "Cost with training data:    %10.4f" (costFunc thethas training)
printfn "Cost with validation data:  %10.4f" (costFunc thethas validation)
printfn "Cost with validation' data: %10.4f" (costFunc thethas validation')    

(*
    Call from Shared library
*)
//df
//|> ExpenseDataFrame.FromFrame
//|> ExpenseDataFrame.GetDailyExpandingSumPerMonth
//|> Seq.iter(fun x -> printfn "%A" x)

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