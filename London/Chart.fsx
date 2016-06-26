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


type Test = {
    Date: DateTime
    Label: string
    Amount: float
} with
    static member Create(date, label, amount) =
        { Date = date; Label = label; Amount = amount }
    override x.ToString() =
        sprintf "%s - %10s - %.2f" ( x.Date.ToShortDateString()) x.Label x.Amount

let testDf =
    [ Test.Create(new DateTime(2016, 2, 1), "Supermarket", 15.)
      Test.Create(new DateTime(2016, 2, 10), "Supermarket", 25.)
      Test.Create(new DateTime(2016, 2, 16), "Clothes", 15.)
      Test.Create(new DateTime(2016, 3, 10), "Supermarket", 65.) ]
    |> Frame.ofRecords

testDf
|> Frame.rows