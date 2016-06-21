#I __SOURCE_DIRECTORY__
#load "../packages/Deedle/Deedle.fsx"
#r "../London.Core/bin/Debug/London.Core.dll"

open System
open System.IO
open System.Globalization
open System.Text.RegularExpressions
open Deedle
open London.Core

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let df =
    ExpenseDataFrame.FromFile <| Directory.GetFiles(Environment.CurrentDirectory + "/data","*.csv")
    |> ExpenseDataFrame.GetFrame

df
|> ExpenseDataFrame.GetAllExpensesChart
|> snd
|> List.iter (fun (title, exp) ->
    printfn "%s" title
    exp
    |> List.iter (fun (date, amount) -> printfn "%10s %A" (date.ToShortDateString()) amount))

df
|> ExpenseDataFrame.GetExpenseLevelCount
|> List.iter (fun (category, counts) ->
    printfn "%s" category
    counts
    |> List.iter (fun (level, count) -> printfn "%10i %5i" level count))


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
|> Frame.groupRowsBy "Date"
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum fst)
|> Series.get "Amount"
|> Series.sortByKey
|> Series.window 2
|> Series.observations
|> Seq.iter(fun x -> printfn "%A" x)