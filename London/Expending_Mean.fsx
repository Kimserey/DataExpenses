#load "_references.fsx"
open _references
open System
open Deedle
open London.Core

(*
    Experimentation
*)

df.Columns.[ [ "Date"; "Amount"; "Category" ] ]
|> Frame.filterRowValues(fun c -> c?Amount < 0. && c.GetAs<string>("Category") = "Supermarket")
|> Frame.groupRowsBy "Date"
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum fst)
|> Series.get "Amount"
|> Series.sortByKey
|> Stats.expandingMean
|> Series.observations
|> Seq.iter(fun v -> printfn "%A" v)

(*
    Call from Shared library
*)
df
|> ExpenseDataFrame.GetExpendingMean Category.Supermarket
|> Seq.iter(fun x -> printfn "%A" x)