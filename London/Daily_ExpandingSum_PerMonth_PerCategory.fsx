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
    let min = 
        let d = Stats.min dates |> Option.get
        new DateTime(d.Year, d.Month, 1)
    let max = 
        let d = Stats.max dates |> Option.get
        (new DateTime(d.Year, d.Month + 1, 1)).AddDays(-1.)
    
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
|> ExpenseDataFrame.FromFrame
|> ExpenseDataFrame.GetDailyExpandingSumPerMonthPerCategory
|> Seq.iter(fun x -> printfn "%A" x)