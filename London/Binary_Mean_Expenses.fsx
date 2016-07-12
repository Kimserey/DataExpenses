#load "_references.fsx"
open _references
open System
open Deedle
open London.Core

(*
    Experimentation
*)
let mean = 
    df.Columns.[ [ "Date"; "Amount"; "Category" ] ]
    |> Frame.filterRowValues(fun c -> c?Amount < 0. && c.GetAs<string>("Category") = "Supermarket")
    |> Frame.getCol "Amount"
    |> Stats.mean

df.Columns.[ [ "Date"; "Amount"; "Category" ] ]
|> Frame.filterRowValues(fun c -> c?Amount < 0. && c.GetAs<string>("Category") = "Supermarket")
|> Frame.groupRowsBy "Date"
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum fst)
|> Series.get "Amount"
|> Series.sortByKey
|> Series.mapValues (fun v -> if unbox<float> v <= mean then v, 0 else v, 1)
|> Series.observations
|> Seq.iter(fun v -> printfn "%A" v)


(*
    Call from Shared library
*)
df
|> ExpenseDataFrame.FromFrame
|> ExpenseDataFrame.GetBinaryExpenses Category.Supermarket
|> Seq.iter(fun x -> printfn "%A" x)