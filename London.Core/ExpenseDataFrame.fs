namespace London.Core

open System
open Deedle

type Month = Month of (string * int)
type Year = Year of int
type Title = Title of string
type Sum = Sum of float

type Expense = {
    Date: DateTime
    Title: string
    Label: string
    Amount: float
    Category: string
}

type ExpenseDataFrame = {
    Frame: Frame<int, string>
} with
    static member GetFrame x = x.Frame

    (** 
        - Load all data from .csv files into Expenses (assuming no duplicate in .csv)
        - Load all data to a dataframe
        - Label the stores
    **)
    static member FromFile files =
        let frame =
            files
            |> Seq.map (fun (path: string) -> Frame.ReadCsv(path, hasHeaders = false))
            |> Seq.map (fun df -> df |> Frame.indexColsWith [ "Date"; "Title"; "Amount" ])
            |> Seq.collect (fun df -> df |> Frame.rows |> Series.observations)
            |> Seq.map (fun (_, s) ->
                { Date = s.GetAs<DateTime>("Date")
                  Title = s.GetAs<string>("Title")
                  Amount = s?Amount
                  Label = ""
                  Category = "" })
            |> Frame.ofRecords

        frame.ReplaceColumn(
            "Label", 
            frame
            |> Frame.getCol "Title" 
            |> Series.mapValues ((fun title -> (title, Other)) >> labelStore >> fst))
        
        frame.ReplaceColumn(
            "Category", 
            frame 
            |> Frame.getCol "Title" 
            |> Series.mapValues ((fun title -> (title, Other)) >> labelStore >> snd >> string))
        
        frame.AddColumn(
            "MonthName", 
            frame
            |> Frame.getCol "Date" 
            |> Series.mapValues (fun date -> DateTime.Parse(date).Month |> monthToString))

        frame.AddColumn(
            "ExpenseLevel", 
            frame
            |> Frame.getCol "Amount" 
            |> Series.mapValues (fun (amount: float) ->
                if amount >= -20.   then 1
                elif amount >= -50. then 2
                elif amount >= -70. then 3
                elif amount >= -90. then 4
                else 5))

        { Frame = frame }

    static member GetSum exp =
        exp
        |> Frame.getCol "Amount"
        |> Series.observations
        |> Seq.sumBy snd
        |> Sum

    static member GetExpensesPerMonth exp: List<Month * Year * Sum * List<Title * Sum * List<Expense>>> =
        exp
        |> Frame.filterRowValues(fun c -> c?Amount < 0.)
        |> Frame.groupRowsByString "Category"
        |> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month, c.GetAs<DateTime>("Date").Year)
        |> Frame.sortRows "Date"
        |> Frame.nest
        |> Series.observations
        |> Seq.map (fun ((m, y), frame) ->
            Month (monthToString m, m),
            Year y,
            ExpenseDataFrame.GetSum frame,
            frame
            |> Frame.nest
            |> Series.observations
            |> Seq.map (fun (category, frame) ->
                Title category,
                ExpenseDataFrame.GetSum frame,
                frame
                |> Frame.rows
                |> Series.observations
                |> Seq.map(fun (_, s) -> 
                    { Date = s.GetAs<DateTime>("Date")
                      Title = s.GetAs<string>("Title")
                      Label = s.GetAs<string>("Label")
                      Amount = s?Amount
                      Category = s.GetAs<string>("Category") })
                |> Seq.toList)
            |> Seq.toList)
        |> Seq.toList

    static member GetExpensesPerCategory exp: List<Title * Sum * List<Title * Sum * List<Expense>>> =
        exp
        |> Frame.filterRowValues(fun c -> c?Amount < 0.)
        |> Frame.groupRowsUsing(fun _ c ->  monthToString (c.GetAs<DateTime>("Date").Month) + " " + string (c.GetAs<DateTime>("Date").Year))
        |> Frame.groupRowsByString "Category"
        |> Frame.nest
        |> Series.observations
        |> Seq.map (fun (category, frame) -> 
            Title category,
            ExpenseDataFrame.GetSum frame,
            frame
            |> Frame.nest
            |> Series.observations
            |> Seq.map (fun (month, frame) ->
                Title month,
                ExpenseDataFrame.GetSum frame,
                frame
                |> Frame.rows
                |> Series.observations
                |> Seq.map(fun (_, s) -> 
                    { Date = s.GetAs<DateTime>("Date")
                      Title = s.GetAs<string>("Title")
                      Label = s.GetAs<string>("Label")
                      Amount = s?Amount
                      Category = s.GetAs<string>("Category") })
                |> Seq.toList)
            |> Seq.toList)
        |> Seq.toList

    static member GetAllExpenses sortBy exp =
        exp
        |> Frame.filterRowValues(fun c -> c?Amount < 0.)
        |> Frame.sortRows sortBy
        |> Frame.rows
        |> Series.observations
        |> Seq.map (fun (_, s) -> 
            { Date = s.GetAs<DateTime>("Date")
              Title = s.GetAs<string>("Title")
              Label = s.GetAs<string>("Label")
              Amount = s?Amount
              Category = s.GetAs<string>("Category") })
        |> Seq.toList

    //Temporary name
    static member GetAllExpensesChart exp =
        let pivotTable =
            exp
            |> Frame.filterRowValues(fun c -> c?Amount < 0.)
            |> Frame.pivotTable
                (fun _ r -> r.GetAs<DateTime>("Date"))
                (fun _ r -> r.GetAs<string>("Category"))
                (fun frame -> frame?Amount |> Stats.sum)

        pivotTable.RowKeys
        |> Seq.sort
        |> Seq.toList,
        pivotTable
        |> Frame.fillMissingWith 0.
        |> Frame.cols
        |> Series.observations
        |> Seq.map (fun (title, series) -> 
            title, 
            series 
            |> Series.observations
            |> Seq.map (fun (date, value) -> date,  Math.Abs(unbox<float> value)) 
            |> Seq.sortBy fst
            |> Seq.toList)
        |> Seq.toList

    static member GetExpenseLevelCount exp =
        exp
        |> Frame.filterRowValues(fun c -> c?Amount < 0.)
        |> Frame.pivotTable
            (fun _ r -> r.GetAs<int>("ExpenseLevel"))
            (fun _ r -> r.GetAs<string>("Category"))
            (fun frame -> frame |> Frame.countRows)
        |> Frame.sortRowsByKey
        |> Frame.fillMissingWith 0
        |> Frame.getCols
        |> Series.observations
        |> Seq.map (fun (category, levels: Series<int, int>) ->
            category,
            levels
            |> Series.observations
            |> Seq.toList)
        |> Seq.toList

module Dataframe =
    open System.IO

    let expenses =
        Directory.GetFiles("data","*.csv")
        |> ExpenseDataFrame.FromFile
        |> ExpenseDataFrame.GetFrame
