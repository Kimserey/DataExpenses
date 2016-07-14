#load "_references.fsx"
open _references
open System
open Deedle
open London.Core

(*
    Experimentation
*)

let frame =
    df
    |> Frame.pivotTable
        (fun _ c ->  c.GetAs<DateTime>("Date").Day)
        (fun _ c -> 
            let d = c.GetAs<DateTime>("Date")
            d.Month, d.Year)
        (fun frame -> frame |> Stats.sum |> Series.get "Amount")
    |> Frame.realignRows [1..31]
    |> Frame.fillMissingWith 0.
    |> Frame.getNumericCols
    |> Series.mapValues Stats.expandingSum
    |> Frame.ofColumns

// Apply linear regression to July 2016 to predict expenses of the monht
// Least square cost cost function = 1/2N * Sum (estimate - real value) square
// Estimate = Theta1 * x + Theta0
let alpha = 0.01
let epsilon = 0.01
let data =
    [ (1., 143.)
      (2., 243.)
      (3., 263.)
      (4., 303.)
      (5., 343.)
      (6., 350.)
      (7., 351.)
      (8., 373.)
      (9., 403.)
      (10., 401.) ]
let m =
    data.Length

let costFunc thethas: float =
    match thethas with
    | thetha0::thetha1::_ ->
        let sum = 
            [0..m - 1] 
            |> List.map (fun i -> data.[i])
            |> List.map (fun (x, y) -> thetha0 + thetha1 * x - y)
            |> List.sum

        (1./float m) * (Math.Pow(sum, 2.))
    | _ -> failwith "Could not compute cost function, thethas are not in correct format."

let next thethas =
    match thethas with
    | thetha0::thetha1::_ ->
        let thetha0' = 
            let sum = 
                [0..m - 1] 
                |> List.map (fun i -> data.[i])
                |> List.map (fun (x, y) -> thetha0 + thetha1 * x - y)
                |> List.sum

            thetha0 - alpha * (1./float m) * sum

        let thetha1' =
            let sum =
                [0..m - 1]
                |> List.map (fun i -> data.[i])
                |> List.map (fun (x, y) -> (thetha0 + thetha1 * x - y) * x)
                |> List.sum
        
            thetha1 - alpha * (1./float m) * sum

        [thetha0'; thetha1']
    | _ -> thethas

let compute iterations =
    [0..iterations]
    |> List.scan (fun thethas _ -> next thethas) [0.; 0.]

let thethas = 
    compute 5000
    |> List.last

let cost =
    costFunc thethas

printfn "%A  - cost: %.4f%%" thethas (cost * 100.)

printfn "%A" ([1..10] |> List.map (fun i -> thethas.[0] + thethas.[1] * float i))

(*
    Call from Shared library
*)
df
|> ExpenseDataFrame.FromFrame
|> ExpenseDataFrame.GetDailyExpandingSumPerMonth
|> Seq.iter(fun x -> printfn "%A" x)