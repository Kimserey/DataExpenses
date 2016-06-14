namespace London.Core

open System
open Deedle

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
                  Amount = string amount |> decimal })
            |> Frame.ofRecords

        frame.AddColumn(
            "Label", 
            frame
            |> Frame.getCol "Title" 
            |> Series.mapValues ((fun title -> (title, Other)) >> labelStore >> fst))
        
        frame.AddColumn(
            "Category", 
            frame 
            |> Frame.getCol "Title" 
            |> Series.mapValues ((fun title -> (title, Other)) >> labelStore >> snd >> string))
        
        frame.AddColumn(
            "MonthName", 
            frame
            |> Frame.getCol "Date" 
            |> Series.mapValues (fun date -> monthToString (DateTime.Parse (string date)).Month))

        { Frame = frame }

and private Expense = {
    Date: DateTime
    Title: string
    Amount: decimal
}
