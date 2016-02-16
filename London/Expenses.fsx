#I __SOURCE_DIRECTORY__
#load "../packages/Deedle/Deedle.fsx"

open System
open System.IO
open Deedle

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

type Expense = {
    Date: DateTime
    Title: string
    Amount: decimal
}

let df =
    Directory.GetFiles(Environment.CurrentDirectory + "/data")
    |> Array.map (fun path -> Frame.ReadCsv(path, hasHeaders = false))
    |> Array.map (fun df -> df |> Frame.indexColsWith [ "Date"; "Title"; "Amount" ])
    |> Array.map (fun df -> df.GetRows())
    |> Seq.collect (fun s -> s.Values)
    |> Seq.map (fun s -> s?Date, s?Title, s?Amount)
    |> Seq.map (fun (date, title, amount) -> 
        { Date = string date |> DateTime.Parse
          Title = string title
          Amount = string amount |> decimal })
    |> Frame.ofRecords

let ``sorted expenses`` =
    df.Columns.[ ["Date"; "Title"; "Amount"] ]
    |> Frame.getRows
    |> Series.sortBy(fun s -> s?Amount)
    |> Frame.ofRows

let ``cumulated expenses grouped by title`` =
    let x =
        df.Columns.[ [ "Title"; "Amount" ] ]
        |> Frame.groupRowsByString("Title")
        |> Frame.getNumericCols
        |> Series.mapValues (Stats.levelSum fst)
    x?Amount
    |> Series.observations
    |> Seq.sortBy snd
    |> Seq.toList

``cumulated expenses grouped by title`` 
|> List.iter (fun (title, amount) -> printfn "%50s %.2f" title amount)

let ``average expenses grouped by title`` =
    let x =
        df.Columns.[ [ "Title"; "Amount" ] ]
        |> Frame.groupRowsByString("Title")
        |> Frame.getNumericCols
        |> Series.mapValues (Stats.levelMean fst)
    x?Amount
    |> Series.observations
    |> Seq.sortBy snd
    |> Seq.toList

``average expenses grouped by title``
|> List.iter (fun (title, amount) -> printfn "%50s %.2f" title amount)


//Make window count
