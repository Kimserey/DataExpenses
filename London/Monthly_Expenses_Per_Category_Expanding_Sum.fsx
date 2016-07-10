#load "_references.fsx"
open _references
open System
open Deedle
open London.Core

(*
    Experimentation
*)

// TODO - Join on full date range

df
|> Frame.pivotTable
    (fun _ c ->  c.GetAs<DateTime>("Date").Date)
    (fun _ c -> c.GetAs<string>("Category"))
    (fun frame -> frame |> Stats.sum |> Series.get "Amount")
|> Frame.fillMissingWith 0.
|> Frame.sortRowsByKey
|> Frame.groupRowsUsing (fun (d: DateTime) _ -> d.Month, d.Year)
|> Frame.nest
|> Series.mapValues (Frame.getNumericCols >> (Series.mapValues Stats.expandingSum) >> Frame.ofColumns)
|> Frame.unnest

(*
    Call from Shared library
*)
df
|> ExpenseDataFrame.GetExpendingMean Category.Supermarket
|> Seq.iter(fun x -> printfn "%A" x)