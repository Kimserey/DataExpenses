﻿namespace London.Core

open System
open System.Globalization
open Deedle
open GradientDescent

type Day = Day of int
type Month = Month of (string * int)
type Year = Year of int
type Title = Title of string
type Sum = Sum of float

type Original = Original of (float * float) list
type Approximation = Approximation of (float * float) list

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

    static member FromFrame frame =
        { Frame = frame }

    (** 
        - Load all data from .csv files into Expenses (assuming no duplicate in .csv)
        - Load all data to a dataframe
        - Label the stores
    **)
    static member FromFile config files =
        let frame =
            files
            |> Seq.map (fun (path: string) -> Frame.ReadCsv(path, hasHeaders = false, culture = "en-GB"))
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

        { Frame = 
            match config with
            | "demo" -> 
                frame 
                |> Frame.filterRowValues(fun c -> 
                    let cat = c.GetAs<string>("Category") 
                    cat <> string Cash 
                    && cat <> string RentAndBills 
                    && cat <> string BankTransfer)
            | _ -> 
                frame }

    static member GetExpensesPerMonth exp: List<Month * Year * Sum * List<Title * Sum * List<Expense>>> =
        let sum frame =
            frame
            |> Frame.getCol "Amount"
            |> Series.observations
            |> Seq.sumBy snd
            |> Sum
        
        exp
        |> ExpenseDataFrame.GetFrame
        |> Frame.groupRowsByString "Category"
        |> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month, c.GetAs<DateTime>("Date").Year)
        |> Frame.nest
        |> Series.observations
        |> Seq.map (fun ((m, y), frame) ->
            Month (monthToString m, m),
            Year y,
            sum frame,
            frame
            |> Frame.nest
            |> Series.observations
            |> Seq.map (fun (category, frame) ->
                Title category,
                sum frame,
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
        let sum frame =
            frame
            |> Frame.getCol "Amount"
            |> Series.observations
            |> Seq.sumBy snd
            |> Sum

        exp
        |> ExpenseDataFrame.GetFrame
        |> Frame.groupRowsUsing(fun _ c ->  monthToString (c.GetAs<DateTime>("Date").Month) + " " + string (c.GetAs<DateTime>("Date").Year))
        |> Frame.groupRowsByString "Category"
        |> Frame.nest
        |> Series.observations
        |> Seq.map (fun (category, frame) -> 
            Title category,
            sum frame,
            frame
            |> Frame.nest
            |> Series.observations
            |> Seq.map (fun (monthAndYear, frame) ->
                Title monthAndYear,
                sum frame,
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
        |> ExpenseDataFrame.GetFrame
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
        |> ExpenseDataFrame.GetFrame
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
            |> ExpenseDataFrame.GetFrame
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

    // Gt all expenses
    static member GetAllExpensesChart exp =
        let pivotTable =
            exp
            |> ExpenseDataFrame.GetFrame
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
        |> ExpenseDataFrame.GetFrame
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

    static member GetDaySpanExpenses (category: Category) exp: seq<float * int> =
        exp
        |> ExpenseDataFrame.GetFrame
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

    static member GetBinaryExpenses (category: Category) exp: seq<DateTime * float * int> =
        let mean =
                exp
                |> ExpenseDataFrame.GetFrame
                |> Frame.filterRowValues(fun c -> c.GetAs<string>("Category") = (string category))
                |> Frame.getCol "Amount"
                |> Stats.mean
        
        exp
        |> ExpenseDataFrame.GetFrame
        |> Frame.filterRowValues(fun c -> c.GetAs<string>("Category") = "Supermarket")
        |> Frame.groupRowsBy "Date"
        |> Frame.getNumericCols
        |> Series.mapValues (Stats.levelSum fst)
        |> Series.get "Amount"
        |> Series.sortByKey
        |> Series.mapValues (fun v -> if unbox<float> v <= mean then v, 0 else v, 1)
        |> Series.observations
        |> Seq.map (fun (k, (v, v')) -> k, v, v')

    static member GetExpandingMean (category: Category) exp =
        exp
        |> ExpenseDataFrame.GetFrame
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
        |> ExpenseDataFrame.GetFrame
        |> Frame.pivotTable
            (fun _ r ->
                r.GetAs<string>("Category"))
            (fun _ r ->
                let date = r.GetAs<DateTime>("Date")
                new DateTime(date.Year, date.Month, 1))
            (Stats.sum >> Series.get "Amount")
        |> Frame.fillMissingWith 0.
        |> Frame.getNumericCols
        |> Series.mapValues (fun s -> s |> Series.mapValues (fun v -> v * 100. / (Stats.sum s)))
        |> Series.sortByKey
        |> Series.observations
        |> Seq.map (fun (k, v) -> k.ToString("MMM yyyy"), Series.observations v |> Seq.toList)
        |> Seq.toList

    static member GetLabelsPerMonth exp =
        //Needed to pass Deedle type check for fillMissingWith
        let empty: Expense list = []

        exp
        |> ExpenseDataFrame.GetFrame
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

            (Title <| k.ToString("MMM yyyy"), k),
            Sum (obs |> List.collect snd |> List.sumBy (fun e -> e.Amount)),
            obs
            |> List.map (fun (shop, (expenses: Expense list)) -> Title shop, Sum (expenses |> List.sumBy (fun e -> e.Amount)), expenses)
            |> List.filter (fun (_, Sum sum, _) -> sum > 0.))
        |> Seq.toList

    static member GetNumberTransactionsAndSumPerMonth exp =
        exp
        |> ExpenseDataFrame.GetFrame
        |> Frame.groupRowsUsing (fun _ c -> 
            let date = c.GetAs<DateTime>("Date")
            date.Year, date.Month)
        |> Frame.nest
        |> Series.mapValues (fun frame -> 
            frame.RowCount,
            frame |> Stats.sum |> Series.get "Amount")
        |> Series.observations
        |> Seq.map snd
        |> Seq.toList

    static member GetDailyExpandingSumPerMonthPerCategory exp =
        let allDates =
            let dates =
                exp
                |> ExpenseDataFrame.GetFrame
                |> Frame.getCol "Date"
                |> Series.mapValues DateTime.Parse
    
            // Here I know that the dates will never be None
            // otherwise something is seriously wrong in the data
            let min = 
                let d = Stats.min dates |> Option.get
                new DateTime(d.Year, d.Month, 1)
            let max = 
                let d = Stats.max dates |> Option.get
                (new DateTime(d.Year, d.Month + 1, 1)).AddDays(-1.)
    
            [0..(max - min).Days]
            |> List.map (float >> min.AddDays)

        exp
        |> ExpenseDataFrame.GetFrame
        |> Frame.pivotTable
            (fun _ c ->  c.GetAs<DateTime>("Date").Date)
            (fun _ c -> c.GetAs<string>("Category"))
            (fun frame -> frame |> Stats.sum |> Series.get "Amount")
        |> Frame.realignRows allDates
        |> Frame.fillMissingWith 0.
        |> Frame.sortRowsByKey
        |> Frame.groupRowsUsing (fun (d: DateTime) _ -> d.Month, d.Year)
        |> Frame.nest
        |> Series.mapValues (Frame.getNumericCols >> (Series.mapValues Stats.expandingSum) >> Frame.ofColumns)
        |> Series.observations
        |> Seq.collect (fun ((month, year), frame) ->
            frame 
            |> Frame.getNumericCols
            |> Series.observations
            |> Seq.map (fun (category, series) ->
                Title category,
                Month (monthToString month, month),
                Year year,
                series
                |> Series.observations
                |> Seq.toList)
            |> Seq.toList)
        |> Seq.toList

    static member GetDailyExpandingSumPerMonth exp =
        exp
        |> ExpenseDataFrame.GetFrame
        |> Frame.pivotTable
            (fun _ c ->  c.GetAs<DateTime>("Date").Day)
            (fun _ c -> 
                let d = c.GetAs<DateTime>("Date")
                d.Month, d.Year)
            (fun frame -> frame |> Stats.sum |> Series.get "Amount")
        |> Frame.realignRows [1..31]
        |> Frame.fillMissingWith 0.
        |> Frame.getNumericCols
        |> Series.mapValues Stats.expandingSum
        |> Series.observations
        |> Seq.map (fun ((month, year), series) ->
            Month (monthToString month, month),
            Year year,
            series 
            |> Series.observations
            |> Seq.map (fun (day, value) ->
                Day day,
                Sum value)
            |> Seq.toList)
        |> Seq.toList

    static member GetCumulatedExpenses (filterOut: Category list) exp =
        exp
        |> ExpenseDataFrame.GetFrame
        |> Frame.groupRowsBy "Date"
        |> Frame.sortRowsByKey
        |> Frame.filterRowValues (fun c -> filterOut |> List.map string |> List.contains (c.GetAs<string>("Category")) |> not)
        |> Frame.getNumericCols
        |> Series.get "Amount"
        |> Stats.levelSum fst
        |> Series.groupInto 
            (fun (date: DateTime) _ -> date.Month, date.Year)
            (fun _ s -> 
                s 
                |> Series.sortByKey
                |> Series.mapKeys (fun (date: DateTime) -> date.Day)
                |> Series.realign [1..(if s.FirstKey().Month = DateTime.Now.Month then DateTime.Now.Day else 31)] 
                |> Series.fillMissingWith 0.
                |> Stats.expandingSum)
        |> Series.skip 1
        |> Series.mapValues (fun values ->
            let toList =
                Series.observations 
                >> Seq.map (fun (day, value) -> day, value) 
                >> Seq.toList

            toList values)
        |> Series.observations
        |> Seq.map (fun ((month, year), originals) ->
            sprintf "%s %i" (CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName month) year,
            originals)
        |> Seq.toList

module Dataframe =
    open System.IO

    type ExpenseMessage =
        | Get of replyChannel: AsyncReplyChannel<ExpenseDataFrame>
        | Refresh of dataDirectory: string option 

    type ExpenseAgent = {
        Get: unit -> ExpenseDataFrame
        Refresh: string option -> unit
    }

    type State = {
        DataDirectory: string
        DataFrameState: DataFrameState
    } with
        static member Default =
            { DataDirectory = ""
              DataFrameState = NotReady }

        static member SetDir dir x =
            { x with DataDirectory = dir }

        static member BecomeReady frame x =
            { x with DataFrameState = Ready frame }
    
    and DataFrameState =
        | Ready of ExpenseDataFrame
        | NotReady

    let build dataDirectory =
        Directory.GetFiles(dataDirectory,"*.csv")
        |> ExpenseDataFrame.FromFile "debug"
    
    let agent =
        let mailbox =
            MailboxProcessor.Start(fun inbox ->
                let rec loop state =
                    async {
                        let! msg = inbox.Receive()

                        match msg with
                        | Get replyChannel ->

                            match state.DataFrameState with
                            | Ready expenses  ->
                                // The frame is ready, returns it and wait for next message
                                replyChannel.Reply expenses
                                return! loop state

                            | NotReady ->
                                // The frame is not ready, builds the frame and returns the result and wait for next message
                                let expenses = build state.DataDirectory
                                replyChannel.Reply expenses
                                return! state 
                                        |> State.BecomeReady expenses
                                        |> loop

                        | Refresh (Some newDir) ->
                            // Refresh the frame using the directory provided and wait for the next message
                            let expenses = build newDir
                            return! state     
                                    |> State.SetDir newDir 
                                    |> State.BecomeReady expenses
                                    |> loop

                        | Refresh None ->
                            // Refresh the frame using the current directory set and wait for the next message
                            let expenses = build state.DataDirectory
                            return! state 
                                    |> State.BecomeReady expenses
                                    |> loop
                    }
                loop State.Default)

        { Get = fun () -> mailbox.PostAndReply Get
          Refresh = fun dataDirectory -> mailbox.Post (Refresh dataDirectory) }
