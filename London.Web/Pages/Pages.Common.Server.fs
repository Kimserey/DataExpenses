namespace London.Web.Pages.Common

open System
open System.IO
open London.Core
open WebSharper
open Deedle

[<AutoOpen>]
module Server =
    type Month = Month of (string * int)
    type Year = Year of int

    type Expense = {
        Date: DateTime
        Label: string
        Amount: float
        Category: string
    }
        
    let expenses =
        Directory.GetFiles("data","*.csv")
        |> ExpenseDataFrame.FromFile 
        |> ExpenseDataFrame.GetFrame

    let getExpensesPerMonth() =
        expenses
        |> Frame.filterRowValues(fun c -> c?Amount < 0.)
        |> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month, c.GetAs<DateTime>("Date").Year)
        |> Frame.sortRows "Date"
        |> Frame.nest
        |> Series.observations
        |> Seq.map (fun ((m, y), df) ->
            (Month (monthToString m, m), Year y), 
            df
            |> Frame.rows
            |> Series.observations
            |> Seq.map(fun (_, s) -> 
                { Date = s.GetAs<DateTime>("Date")
                  Label = s.GetAs<string>("Label")
                  Amount = s?Amount
                  Category = s.GetAs<string>("Category") })
            |> Seq.toList)
        |> Map.ofSeq

    let getExpensesPerCategory() =
        expenses
        |> Frame.filterRowValues(fun c -> c?Amount < 0.)
        |> Frame.groupRowsUsing(fun _ c -> c.GetAs<string>("Category"))
        |> Frame.sortRows "Category"
        |> Frame.nest
        |> Series.observations
        |> Seq.map (fun (category, df) ->
            category, 
            df
            |> Frame.rows
            |> Series.observations
            |> Seq.map(fun (_, s) -> 
                { Date = s.GetAs<DateTime>("Date")
                  Label = s.GetAs<string>("Label")
                  Amount = s?Amount
                  Category = category })
            |> Seq.toList)
        |> Map.ofSeq