#load "_references.fsx"
open _references
open System
open Deedle
open London.Core

(*
    Experimentation
*)

df.Columns.[ [ "Date"; "Label"; "Amount" ] ]
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.rows
|> Series.observations
|> Seq.iter(fun v -> printfn "%A" v)


df.Columns.[ [ "Date"; "Label"; "Amount" ] ]
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.pivotTable
    (fun _ r ->
        let date = r.GetAs<DateTime>("Date")
        new DateTime(date.Year, date.Month, 1))
    (fun _ c ->
        c.GetAs<string>("Label"))
    (fun frame ->
        frame
        |> Frame.getNumericCols
        |> Series.get "Amount"
        |> Stats.sum
        |> Math.Abs)
|> Frame.fillMissingWith 0.
|> Frame.getRows
|> Series.sortByKey
|> Series.observations
|> Seq.iter(fun v -> printfn "%A" v)

(*
    Call from Shared library
*)
df
|> ExpenseDataFrame.GetLabelsPerMonth
|> Seq.iter(fun x -> printfn "%A" x)