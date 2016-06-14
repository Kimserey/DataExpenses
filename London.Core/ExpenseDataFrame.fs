﻿namespace London.Core

open System
open Deedle

type Month = Month of (string * int)

type Year = Year of int

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
            |> Seq.map (fun df -> df.GetRows())
            |> Seq.collect (fun s -> s.Values)
            |> Seq.map (fun s -> s?Date, s?Title, s?Amount)
            |> Seq.map (fun (date, title, amount) -> 
                { Date = string date |> DateTime.Parse
                  Title = string title
                  Amount = string amount |> float
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

        { Frame = frame }

    static member GetExpensesPerMonth exp =
        exp
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
                  Title = s.GetAs<string>("Title")
                  Label = s.GetAs<string>("Label")
                  Amount = s?Amount
                  Category = s.GetAs<string>("Category") })
            |> Seq.toList)
        |> Map.ofSeq

    static member GetExpensesPerCategory exp: Map<string, List<string * float * List<Expense>>> =
        exp
        |> Frame.filterRowValues(fun c -> c?Amount < 0.)
        |> Frame.groupRowsUsing(fun _ c ->  monthToString (c.GetAs<DateTime>("Date").Month) + " " + string (c.GetAs<DateTime>("Date").Year))
        |> Frame.groupRowsByString "Category"
        |> Frame.nest
        |> Series.observations
        |> Seq.map (fun (category, frame) -> 
            category,
            frame
            |> Frame.nest
            |> Series.observations
            |> Seq.map (fun (month, frame) ->
                month,
                frame
                |> Frame.getCol "Amount"
                |> Series.observations
                |> Seq.sumBy snd,
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
        |> Map.ofSeq

module Dataframe =
    open System.IO

    let expenses =
        Directory.GetFiles("data","*.csv")
        |> ExpenseDataFrame.FromFile
        |> ExpenseDataFrame.GetFrame