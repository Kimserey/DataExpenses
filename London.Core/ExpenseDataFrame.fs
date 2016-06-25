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
} with
    override x.ToString() = x.Title

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
            |> Seq.filter (fun e -> e.Amount < 0.)
            |> Seq.map (fun e -> { e with Amount = Math.Abs e.Amount })
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
            |> Seq.map (fun (monthAndYear, frame) ->
                Title monthAndYear,
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
        
    // Get all expenses
    static member GetAllExpenses sortBy exp =
        exp
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
        
    // Get all expenses for a certain category
    static member GetExpenses (category: Category) sortBy exp =
        exp
        |> Frame.filterRowValues(fun c -> c.GetAs<string>("Category") = (string category))
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


    // Get all expenses for a certain category
    static member GetSmoothExpenses (category: Category) sortBy exp =
        let frame = 
            exp
            |> Frame.filterRowValues(fun c -> c.GetAs<string>("Category") = (string category))

        let mean, std =
            Stats.mean (frame |> Frame.getCol "Amount" |> Series.mapValues unbox<float>), Stats.stdDev (frame |> Frame.getCol "Amount")

        frame
        |> Frame.sortRows sortBy
        |> Frame.filterRowValues (fun r -> r?Amount <= mean + std)
        |> Frame.rows
        |> Series.observations
        |> Seq.map (fun (_, s) -> 
            { Date = s.GetAs<DateTime>("Date")
              Title = s.GetAs<string>("Title")
              Label = s.GetAs<string>("Label")
              Amount = s?Amount
              Category = s.GetAs<string>("Category") })
        |> Seq.toList

    // Gt all expens
    static member GetAllExpensesChart exp =
        let pivotTable =
            exp
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
            |> Seq.map (fun (date, value) -> date,  unbox<float> value) 
            |> Seq.sortBy fst
            |> Seq.toList)
        |> Seq.toList

    static member GetExpenseLevelCount exp =
        exp
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

    static member GetDaySpanExpenses (category: Category) (exp: Frame<_, string>): seq<float * int> =
        exp
        |> Frame.filterRowValues(fun c -> c.GetAs<string>("Category") = "Supermarket")
        |> Frame.groupRowsBy "Date"
        |> Frame.getNumericCols
        |> Series.mapValues (Stats.levelSum fst)
        |> Series.get "Amount"
        |> Series.sortByKey
        |> Series.windowInto 2
            (fun (s: Series<DateTime, float>) ->
                match s |> Series.observations |> Seq.toList with
                | [ (d1, p1); (d2, _) ] -> p1, (d2 - d1).TotalDays
                | _ ->  failwith "incomplete window are skipped by deedle")
        |> Series.observations
        |> Seq.map (fun (_, (days, value)) -> days, int value)
        |> Seq.sortBy fst

    static member GetBinaryExpenses (category: Category) (exp: Frame<_, string>): seq<DateTime * float * int> =
        let mean =
                exp
                |> Frame.filterRowValues(fun c -> c.GetAs<string>("Category") = (string category))
                |> Frame.getCol "Amount"
                |> Stats.mean
        
        exp
        |> Frame.filterRowValues(fun c -> c.GetAs<string>("Category") = "Supermarket")
        |> Frame.groupRowsBy "Date"
        |> Frame.getNumericCols
        |> Series.mapValues (Stats.levelSum fst)
        |> Series.get "Amount"
        |> Series.sortByKey
        |> Series.mapValues (fun v -> if unbox<float> v <= mean then v, 0 else v, 1)
        |> Series.observations
        |> Seq.map (fun (k, (v, v')) -> k, v, v')

    static member GetExpendingMean (category: Category) exp =
        exp
        |> Frame.filterRowValues(fun c -> c.GetAs<string>("Category") = string category)
        |> Frame.groupRowsBy "Date"
        |> Frame.getNumericCols
        |> Series.mapValues (Stats.levelSum fst)
        |> Series.get "Amount"
        |> Series.sortByKey
        |> Stats.expandingMean
        |> Series.observations

    static member GetCategoryRatioPerMonth exp =
         exp
        |> Frame.pivotTable
            (fun _ r ->
                let date = r.GetAs<DateTime>("Date")
                new DateTime(date.Year, date.Month, 1))
            (fun _ c ->
                c.GetAs<string>("Category"))
            (fun frame ->
                frame
                |> Frame.getNumericCols
                |> Series.get "Amount"
                |> Stats.sum)
        |> Frame.fillMissingWith 0.
        |> Frame.mapRowValues (fun c ->
            let total = c |> Series.values |> Seq.cast<float> |> Seq.sum
            c |> Series.mapValues (fun v -> unbox v * 100. / total))
        |> Series.sortByKey
        |> Series.observations
        |> Seq.map (fun (k, v) -> k.ToString("MMM yyyy"), Series.observations v |> Seq.toList)
        |> Seq.toList

    static member GetLabelsPerMonth exp =
        //Needed to pass Deedle type check for fillMissingWith
        let empty: Expense list = []

        exp
        |> Frame.pivotTable
            (fun _ r ->
                let date = r.GetAs<DateTime>("Date")
                new DateTime(date.Year, date.Month, 1))
            (fun _ c -> c.GetAs<string>("Label"))
            (fun frame -> 
                frame
                |> Frame.rows
                |> Series.dropMissing
                |> Series.observations
                |> Seq.map (fun (_, s) ->  
                    { Date = s.GetAs<DateTime>("Date")
                      Label = s.GetAs<string>("Label")
                      Title = s.GetAs<string>("Title")
                      Amount = s?Amount
                      Category = s.GetAs<string>("Category") })
                |> Seq.toList)
        |> Frame.fillMissingWith empty
        |> Frame.getRows
        |> Series.sortByKey
        |> Series.observations
        |> Seq.map (fun (k, v) -> 
            let obs =
                v
                |> Series.sortByKey
                |> Series.observations
                |> Seq.toList

            Title <| k.ToString("MMM yyyy"),
            Sum (obs |> List.collect snd |> List.sumBy (fun e -> e.Amount)),
            obs
            |> List.map (fun (shop, (expenses: Expense list)) -> Title shop, Sum (expenses |> List.sumBy (fun e -> e.Amount)), expenses)
            |> List.filter (fun (_, Sum sum, _) -> sum > 0.))
        |> Seq.toList

module Dataframe =
    open System.IO

    let expenses =
        Directory.GetFiles("data","*.csv")
        |> ExpenseDataFrame.FromFile
        |> ExpenseDataFrame.GetFrame
