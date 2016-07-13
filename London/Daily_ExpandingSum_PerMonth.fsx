#load "_references.fsx"
open _references
open System
open Deedle
open London.Core

(*
    Experimentation
*)

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

(*
    Call from Shared library
*)
df
|> ExpenseDataFrame.FromFrame
|> ExpenseDataFrame.GetDailyExpandingSumPerMonth
|> Seq.iter(fun x -> printfn "%A" x)