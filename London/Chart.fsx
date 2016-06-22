#load "_references.fsx"
open _references
open System
open Deedle
open London.Core

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

df
|> ExpenseDataFrame.GetSmoothExpenses Category.Supermarket "Date"
|> List.iter (fun e -> printfn "%10s %5.2f" (e.Date.ToShortDateString()) e.Amount)

