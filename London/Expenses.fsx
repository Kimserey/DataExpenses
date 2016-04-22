#I __SOURCE_DIRECTORY__
#load "../packages/Deedle/Deedle.fsx"

open System
open System.IO
open System.Globalization
open Deedle

[<AutoOpen>]
module Common =

    type Expense = {
        Date: DateTime
        Title: string
        Amount: decimal
    }

    let printTitle title =
        printfn "\n%s:\n" title

    let monthToString mth =
        CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mth)

(** 
    Script boot up
    --------------
    - Set environment as current file script location
    - Load all data from .csv files into Expenses (assuming no duplicate in .csv)
    - Load all data to a dataframe
**)

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let df = 
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
    |> Frame.ofRecords

(** 
    All expenses - pretty display
    ------------------------------------------
    October
      28/10/2015              SOMETHING     -35.40
      26/10/2015         SOMETHING ELSE     -24.03
    November
      30/11/2015    SOMETHING SOMETHING    -73.43
      02/11/2015        SOMETHING AGAIN    -192.50
**)
df
|> Frame.filterRows(fun _ c -> c?Amount < 0.)
|> Frame.groupRowsUsing(fun _ c -> (c.Get("Date") :?> DateTime).Month)
|> Frame.nest
|> Series.observations
|> Seq.iter (fun (m, df) ->
    printfn "%s" (monthToString m)
    df
    |> Frame.rows
    |> Series.observations
    |> Seq.iter (fun (_, s) -> 
        printfn "  %s %50s %10.2f" 
            ((s.Get("Date") :?> DateTime).ToShortDateString()) 
            (string <| s.Get("Title"))
            s?Amount))

(**
    Total monthly expenses - pretty display
    ---------------------------------------
    - Group by month number
    - Get numeric column (amount)
    - Execute Sum on the first level (monthly group level)
    - Take the single value of the observations and take the data from it
    - Map month number to month name

    Output:
      October : -69.43
     November : -198.72
**)
df
|> Frame.filterRows(fun _ c -> c?Amount < 0.)
|> Frame.groupRowsUsing(fun _ c -> (c.Get("Date") :?> DateTime).Month)
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum fst)
|> Series.observations
|> Seq.head
|> snd
|> Series.map(fun k t -> monthToString k, t)
|> Series.observations
|> Seq.iter (fun (_, (month, amount)) -> printfn "%10s : %.2f" month amount)


(**
    Top three monthly expenses - frame
    ----------------------------------
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

(** 
    Top three montly expenses - pretty display
    ------------------------------------------
    October
      28/10/2015              SOMETHING     -35.40
      26/10/2015         SOMETHING ELSE     -24.03
    November
      30/11/2015    SOMETHING SOMETHING    -73.43
      02/11/2015        SOMETHING AGAIN    -192.50
**)
df
|> Frame.filterRows(fun _ c -> c?Amount < 0.)
|> Frame.groupRowsUsing(fun _ c -> (c.Get("Date") :?> DateTime).Month)
|> Frame.nest
|> Series.observations
|> Seq.map (fun (k, df) -> 
    (monthToString k, 
     df 
     |> Frame.sortRows "Amount"
     |> Frame.take 3))
|> Seq.iter (fun (m, df) ->
    printfn "%s" m
    df
    |> Frame.rows
    |> Series.observations
    |> Seq.iter (fun (_, s) -> 
        printfn "  %s %50s %10.2f" 
            ((s.Get("Date") :?> DateTime).ToShortDateString()) 
            (string <| s.Get("Title"))
            s?Amount))



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
