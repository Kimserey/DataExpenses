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

df
|> Frame.pivotTable
    (fun _ r ->
        r.GetAs<string>("Category"))
    (fun _ r ->
        let date = r.GetAs<DateTime>("Date")
        new DateTime(date.Year, date.Month, 1))
    (Stats.sum >> Series.get "Amount")
|> Frame.fillMissingWith 0.
|> Frame.getNumericCols
|> Series.mapValues (fun s ->
    s 
    |> Series.mapValues (fun v -> v * 100. / (Stats.sum s)))
|> Series.sortByKey
|> Series.observations
|> Seq.iter(fun v -> printfn "%A" v)

(*
    Call from Shared library
*)
df
|> ExpenseDataFrame.GetCategoryRatioPerMonth
|> Seq.iter(fun x -> printfn "%A" x)