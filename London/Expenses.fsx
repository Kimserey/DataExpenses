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

let dfs =
    Directory.GetFiles(Environment.CurrentDirectory + "/data")
    |> Array.map (fun path -> Frame.ReadCsv(path, hasHeaders = false))
    |> Array.map (fun df -> df |> Frame.indexColsWith [ "Date"; "Title"; "Amount" ])

let records =
    dfs
    |> Array.map (fun df -> df.GetRows())
    |> Seq.collect (fun s -> s.Values)
    |> Seq.map (fun s -> s?Date, s?Title, s?Amount)
    |> Seq.map (fun (date, title, amount) -> 
        { Date = string date |> DateTime.Parse
          Title = string title
          Amount = string amount |> decimal })
    |> Frame.ofRecords