#I __SOURCE_DIRECTORY__
#load "../packages/Deedle/Deedle.fsx"
#r "../London.Core/bin/Debug/London.Core.dll"

open System
open System.IO
open System.Globalization
open System.Text.RegularExpressions
open Deedle
open London.Core

(** 
    Script boot up, massage and label data
    --------------------------------------
    - Set environment as current file script location
    - Load all data from .csv files into Expenses (assuming no duplicate in .csv)
    - Load all data to a dataframe
    - Label the stores
**)

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let df =
    ExpenseDataFrame.FromFile <| Directory.GetFiles(Environment.CurrentDirectory + "/data","*.csv")
    |> ExpenseDataFrame.GetFrame

(** 
    All expenses - pretty display
    -----------------------------
    October
      28/10/2015              SOMETHING     -35.40  CATEGORY
      26/10/2015         SOMETHING ELSE     -24.03  CATEGORY
    November
      30/11/2015    SOMETHING SOMETHING    -73.43   CATEGORY
      02/11/2015        SOMETHING AGAIN    -192.50  CATEGORY
**)
df
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month)
|> Frame.nest
|> Series.observations
|> Seq.iter (fun (m, df) ->
    printfn "%s" (monthToString m)
    df
    |> Frame.rows
    |> Series.observations
    |> Seq.iter (fun (_, s) -> 
        printfn "  %s %50s %10.2f %50s" 
            (s.GetAs<DateTime>("Date").ToShortDateString()) 
            (s.GetAs<string>("Label"))
            s?Amount
            (s.GetAs<string>("Category"))))


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
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month)
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
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month)
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
|> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month)
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
            (s.GetAs<DateTime>("Date").ToShortDateString()) 
            (s.GetAs<string>("Title"))
            s?Amount))

(**
    All sorted expenses - pretty display
    ------------------------------------
    01/12/2015              SOMETHING    -51.00
    09/02/2016    SOMETHING SOMETHING    -30.00
    13/01/2016        AGAIN SOMETHING     -4.00
**)
df
|> Frame.rows
|> Series.sortBy(fun s -> s?Amount)
|> Series.observations
|> Seq.iter (fun (_, s) -> 
    printfn "%s %50s %10.2f" 
        (s.GetAs<DateTime>("Date").ToShortDateString()) 
        (s.GetAs<string>("Title"))
        s?Amount)

(**
    Grouped by title sorted expenses - pretty display
    -------------------------------------------------
    - Dropped all columns except Title and Amount (Prepare for group by)
    - Group by Title
    - Get numeric columns (useful for the compiler to know that all columns are float type)
    - Run a sum on the first level (at this stage, the frame is indexed on two levels - Title - Id)

    Output:
                SOMETHING    -35.57
      SOMETHING SOMETHING    -29.99
          AGAIN SOMETHING    -29.00
**)
df.Columns.[ [ "Title"; "Amount" ] ]
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsByString("Title")
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum fst)
|> Series.observations
|> Seq.collect (fun (_, s) -> s |> Series.observations)
|> Seq.sortBy snd
|> Seq.iter (fun (title, amount) -> printfn "%50s %10.2f" title amount)

(**
    Grouped by title sorted expenses by name - pretty display
    ---------------------------------------------------------
    - Dropped all columns except Title and Amount (Prepare for group by)
    - Group by Title
    - Get numeric columns (useful for the compiler to know that all columns are float type)
    - Run a sum on the first level (at this stage, the frame is indexed on two levels - Title - Id)

    Output:
                SOMETHING    -35.57
      SOMETHING SOMETHING    -29.99
          AGAIN SOMETHING    -29.00
**)
df.Columns.[ [ "Title"; "Amount" ] ]
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsByString("Title")
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum fst)
|> Series.observations
|> Seq.collect (fun (_, s) -> s |> Series.observations)
|> Seq.sortBy fst
|> Seq.iter (fun (title, amount) -> printfn "%50s %10.2f" title amount)


(**
    Grouped by labeled title sorted expenses - pretty display
    ---------------------------------------------------------
    - Map Title column to change Title to store name
    - Group by Title
    - Apply sum on first level

    Output:
                SOMETHING    -35.57
      SOMETHING SOMETHING    -29.99
          AGAIN SOMETHING    -29.00
**)
df
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsByString "Label"
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum fst)
|> Series.observations
|> Seq.collect (snd >> Series.observations)
|> Seq.sortBy snd
|> Seq.iter (fun (title, amount) -> 
    printfn "%50s %10.2f" title amount)

(**
    Grouped by labeled title per month - pretty display
    ---------------------------------------------------
    - Group by Label
    - Group by Month
    - Get Amount column
    - Level sum by flattening the keys and summing using Month and Label

    Output:
    February
                SOMETHING    -35.57
      SOMETHING SOMETHING    -29.99
          AGAIN SOMETHING    -29.00
    Total: -661.08 GBP
    -----------------------------------------------------------------
    March
                SOMETHING    -35.57
      SOMETHING SOMETHING    -29.99
          AGAIN SOMETHING    -29.00
    Total: -661.08 GBP
    -----------------------------------------------------------------
**)
let showExpensesPerLabel (categories: Category list) =
    categories
    |> List.map string
    |> List.iter (fun category ->
        printfn "%s" category
        df.Columns.[ [ "Date"; "Label"; "Category"; "Amount" ]]
        |> Frame.filterRowValues (fun c -> c?Amount < 0. && c.GetAs<string>("Category") = category)
        |> Frame.groupRowsByString "Label"
        |> Frame.groupRowsUsing (fun _ c -> c.GetAs<DateTime>("Date").Month)
        |> Frame.mapRowKeys Pair.flatten3
        |> Frame.getNumericCols
        |> Series.mapValues (Stats.levelSum Pair.get1And2Of3)
        |> Series.observations
        |> Seq.collect (snd >> Series.observations)
        |> Seq.map (fun ((month, title), amount) -> month, title, amount)
        |> Seq.groupBy (fun (month, _, _) -> month)
        |> Seq.iter (fun (month, values) ->
            printfn "%20s" (monthToString month)
            values
            |> Seq.sortBy (fun (_, _, amount) -> amount)
            |> Seq.iter (fun (_, title, amount) ->
                printfn "%50s %10.2f GBP" title amount)
            printfn "%50s %10.2f GBP" "TOTAL" (values |> Seq.sumBy (fun (_, _, amount) -> amount))))

showExpensesPerLabel 
    [ Category.Supermarket
      Category.Restaurant
      Category.SweetAndSavoury 
      Category.Other ]

(**
    Grouped by category per month showing total - pretty display
    ------------------------------------------------------------
    - Group by category
    - Group by Month
    - Get Amount column
    - Level sum by flattening the keys and summing using Month and Label

    Output:
    February
                SOMETHING    -35.57
      SOMETHING SOMETHING    -29.99
          AGAIN SOMETHING    -29.00
    Total: -661.08 GBP
    -----------------------------------------------------------------
    March
                SOMETHING    -35.57
      SOMETHING SOMETHING    -29.99
          AGAIN SOMETHING    -29.00
    Total: -661.08 GBP
    -----------------------------------------------------------------
**)
df.Columns.[ [ "Date"; "Category"; "Amount" ]]
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsByString "Category"
|> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month)
|> Frame.mapRowKeys Pair.flatten3
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum Pair.get1And2Of3)
|> Series.observations
|> Seq.collect (snd >> Series.observations)
|> Seq.map (fun ((month, title), amount) -> month, title, amount)
|> Seq.groupBy (fun (month, _, _) -> month)
|> Seq.iter (fun (month, values) ->
    printfn "%s" (monthToString month)
    values
    |> Seq.sortBy (fun (_, _, amount) -> amount)
    |> Seq.iter (fun (_, title, amount) ->
        printfn "%50s %10.2f GBP" title amount)
    printfn "%50s %10.2f GBP" "TOTAL" (values |> Seq.sumBy (fun (_, _, amount) -> amount)))

(**
    Grouped by category per month showing expenses under category - pretty display
    ------------------------------------------------------------------------------
    April
        Supermarket
              04/04/2016              M&S   -19.57 GBP
              04/04/2016            TESCO   -67.05 GBP
              11/04/2016              M&S   -10.00 GBP
              11/04/2016       SAINSBURYS    -4.45 GBP
                                    TOTAL   -58.15 GBP
        Cash
              13/04/2016    CASH WITHDRAW   -50.00 GBP
                                    TOTAL   -58.15 GBP
**)
df.Columns.[ [ "Date"; "Category"; "Label"; "Amount" ]]
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsByString "Category"
|> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month)
|> Frame.nest
|> Series.observations
|> Seq.iter (fun (m, frame) -> 
    printfn "%s" (monthToString m)
    frame
    |> Frame.nest
    |> Series.observations
    |> Seq.iter (fun (category, frame) ->
        printfn "%25s" category 
        frame
        |> Frame.rows
        |> Series.observations
        |> Seq.map    (fun (_, s) -> s.GetAs<DateTime>("Date").ToShortDateString(), s.GetAs<string>("Label"), s?Amount)
        |> Seq.sortBy (fun (date, _, _) -> date)
        |> Seq.iter   (fun (date, label, amount)  -> printfn "%35s %40s %8.2f GBP" date label amount)
        printfn "%76s %8.2f GBP" 
            "TOTAL"
            (frame
             |> Frame.getCol "Amount" 
             |> Series.observations 
             |> Seq.sumBy snd)))

(**
    Expenses per month for a category - pretty display
    --------------------------------------------------
    Supermarket
            October   -24.03 GBP
           December   -83.32 GBP
    AsianSupermarket
            January   -14.55 GBP
           February   -15.14 GBP
    SweetAndSavoury
            October   -35.40 GBP
           November   -31.69 GBP
**)
let showExpensesPerMonth (categories: Category list) =
    categories
    |> List.map string
    |> List.iter (fun category ->
        printfn "%s" category
        df.Columns.[ [ "Date"; "Category"; "Amount" ] ]
        |> Frame.filterRowValues(fun c -> 
            c?Amount < 0. 
            && c.GetAs<string>("Category") = category)
        |> Frame.groupRowsUsing(fun _ c -> 
            c.GetAs<DateTime>("Date").Month)
        |> Frame.getNumericCols
        |> Series.mapValues (Stats.levelSum fst)
        |> Series.observations
        |> Seq.collect (snd >> Series.observations)
        |> Seq.iter (fun (month, value) -> 
            printfn "%15s %8.2f GBP" (monthToString month) value))

showExpensesPerMonth 
    [ Category.Supermarket
      Category.AsianSupermarket
      Category.SweetAndSavoury
      Category.Restaurant ]

(**
    Expenses per month for each day of the week - pretty display
    ------------------------------------------------------------
    October
                  Monday   -24.03 GBP
               Wednesday   -35.40 GBP
                Saturday   -10.00 GBP
    November
                  Sunday   -30.00 GBP
               Wednesday   -17.06 GBP
                Thursday   -16.52 GBP
**)
df.Columns.[ [ "Date"; "Amount" ]]
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month)
|> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").DayOfWeek)
|> Frame.mapRowKeys Pair.flatten3
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum Pair.get1And2Of3)
|> Series.observations
|> Seq.collect (snd >> Series.observations)
|> Seq.map (fun ((day, month), amount) -> day, month, amount)
|> Seq.groupBy (fun (_, m, _) -> m)
|> Seq.iter (fun (month, group) ->
    printfn "%s" (monthToString month)
    group
    |> Seq.sort
    |> Seq.iter (fun (d, _, a) ->
        printfn "%20s %8.2f GBP" (d.ToString()) a))