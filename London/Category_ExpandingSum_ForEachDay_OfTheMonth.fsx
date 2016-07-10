#load "_references.fsx"
open _references
open System
open Deedle
open London.Core

(*
    Experimentation
*)

let allDates =
    let dates =
        df
        |> Frame.getCol "Date"
        |> Series.mapValues DateTime.Parse
    
    // Here I know that the dates will never be None
    // otherwise something is seriously wrong in the data
    let min = Stats.min dates |> Option.get
    let max = Stats.max dates |> Option.get
    
    [0..(max - min).Days]
    |> List.map (float >> min.AddDays)

df
|> Frame.pivotTable
    (fun _ c ->  c.GetAs<DateTime>("Date").Date)
    (fun _ c -> c.GetAs<string>("Category"))
    (fun frame -> frame |> Stats.sum |> Series.get "Amount")
|> Frame.realignRows allDates
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
|> ExpenseDataFrame.GetCategoryExpandingSumForEachDayOfTheMonth
|> Seq.iter(fun x -> printfn "%A" x)