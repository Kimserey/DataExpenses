#I __SOURCE_DIRECTORY__
#load "../packages/Deedle/Deedle.fsx"

open System
open System.IO
open System.Globalization
open Deedle

let printTitle title =
    printfn "\n%s:\n" title

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

type Expense = {
    Date: DateTime
    Title: string
    Amount: decimal
} with
    override x.ToString() =
        sprintf "Title:%s\nDate:%A\nAmount:%.2f\n" x.Title x.Date x.Amount

// Print the path location
printTitle "Environment path"
printfn "- %s" (Environment.CurrentDirectory + "/data")

// Print the files
printTitle "Files in data path"
Array.iter (printfn " - %s") <| Directory.GetFiles(Environment.CurrentDirectory + "/data","*.csv")

// Get all records
let records =
    Directory.GetFiles(Environment.CurrentDirectory + "/data","*.csv")
    |> Array.map (fun path -> Frame.ReadCsv(path, hasHeaders = false))
    |> Array.map (fun df -> df |> Frame.indexColsWith [ "Date"; "Title"; "Amount" ])
    |> Array.map (fun df -> df.GetRows())
    |> Seq.collect (fun s -> s.Values)
    |> Seq.map (fun s -> s?Date, s?Title, s?Amount)
    |> Seq.map (fun (date, title, amount) -> 
        { Date = string date |> DateTime.Parse
          Title = string title
          Amount = string amount |> decimal })

// Show duplicate records
printTitle "Duplicate records found:"
records
|> Seq.groupBy id
|> Seq.filter (fun (k, v) -> v |> Seq.toList |> List.length > 1)
|> Seq.iter (fun (k, _) -> printfn "%s" (string k))

// Concat all records to a dataframe
let df = 
    records
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


// Display total monthly expenses
df
|> Frame.filterRows(fun _ c -> c?Amount < 0.)
|> Frame.groupRowsUsing(fun _ c -> (c.Get("Date") :?> DateTime).Month)
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum fst)
|> Series.observations
|> Seq.head
|> snd
|> Series.map(fun k t -> (CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(k), t))
|> Series.observations
|> Seq.iter (fun (_, (month, amount)) -> printfn "%10s : %.2f" month amount)


(**
    Top three expenses for each month
    ---------------------------------
    - Group by the month number
    - Execute operation on Nested frame
    - Transform from observations to Seq to manipulate the data
    - Transform back to observations and unest the frame to get back original format
**)
df
|> Frame.filterRows(fun _ c -> c?Amount < 0.)
|> Frame.groupRowsUsing(fun _ c -> (c.Get("Date") :?> DateTime).Month)
|> Frame.nest
|> Series.observations
|> Seq.map (fun (k, df) -> 
    (k, 
     df 
     |> Frame.sortRows "Amount"
     |> Frame.take 3))
|> Series.ofObservations
|> Frame.unnest