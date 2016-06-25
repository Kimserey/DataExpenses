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
|> Frame.rows
|> Series.observations
|> Seq.iter(fun v -> printfn "%A" v)


df.Columns.[ [ "Date"; "Amount"; "Category" ] ]
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.pivotTable
    (fun _ r ->
        let date = r.GetAs<DateTime>("Date")
        new DateTime(date.Year, date.Month, 1))
    (fun _ c ->
        c.GetAs<string>("Category"))
    (fun frame ->
        frame
        |> Frame.getNumericCols
        |> Series.get "Amount"
        |> Stats.sum)
|> Frame.fillMissingWith 0.
|> Frame.mapRowValues (fun c ->
    let total = c |> Series.values |> Seq.cast<float> |> Seq.sum
    c |> Series.mapValues (fun v -> unbox v * 100. / total))
|> Series.sortByKey
|> Series.observations
|> Seq.iter(fun v -> printfn "%A" v)

(*
    Call from Shared library
*)
df
|> ExpenseDataFrame.GetCategoryRatioPerMonth
|> Seq.iter(fun x -> printfn "%A" x)