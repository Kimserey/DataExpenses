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
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.pivotTable
    (fun _ r -> r.GetAs<int>("ExpenseLevel"))
    (fun _ r -> r.GetAs<string>("Category"))
    (fun frame -> frame |> Frame.countRows)
|> Frame.sortRowsByKey
|> Frame.fillMissingWith 0