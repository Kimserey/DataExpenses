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
|> Series.observations
|> Seq.iter(fun x -> printfn "%A" x)

df.Columns.[ [ "Date"; "Amount"; "Category" ] ]
|> Frame.filterRowValues(fun c -> c?Amount < 0. && c.GetAs<string>("Category") = "Supermarket")
|> Frame.sortRows "Date"
|> Frame.groupRowsBy "Date"
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum fst)
|> Series.get "Amount"
|> Series.sortByKey
|> Series.window 2
|> Series.observations
|> Seq.iter(fun x -> printfn "%A" x)

df.Columns.[ [ "Date"; "Amount"; "Category" ] ]
|> Frame.filterRowValues(fun c -> c?Amount < 0. && c.GetAs<string>("Category") = "Supermarket")
|> Frame.groupRowsBy "Date"
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum fst)
|> Series.get "Amount"
|> Series.sortByKey
|> Series.windowInto 2
    (fun (s: Series<DateTime, float>) ->
        match s |> Series.observations |> Seq.toList with
        | [ (d1, p1); (d2, _) ] -> (d2 - d1).TotalDays, p1
        | _ ->  failwith "incomplete window are skipped by deedle")
|> Series.observations
|> Seq.map snd
|> Seq.sortBy fst
|> Seq.iter(fun x -> printfn "%A" x)


(*
    Call from library
*)
df
|> ExpenseDataFrame.FromFrame
|> ExpenseDataFrame.GetDaySpanExpenses Category.Supermarket
|> Seq.iter(fun x -> printfn "%A" x)