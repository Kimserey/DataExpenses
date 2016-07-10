namespace London.Web

open System
open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Server
open London.Core
open London.Core.Dataframe

(**
    Lots of Json endpoints to be consumed by Ajax calls.

    This is mainly for me to prototype a proper graph and
    work on the UI before placing it as a template and use a WebSharper RPC
**)
module Api =
    
    type Chart<'T> = {
        Title: string
        Labels: string list
        DataSeriesList: DataSeries<'T> list
    } 

    and DataSeries<'T> = {
        Title: string
        Values: 'T list
    }

    let addCORSHeader ctx =
        ctx.Request.Headers
        |> Seq.tryFind (fun h -> h.Name = "Origin")
        |> Option.map (fun origin -> Http.Header.Custom "Access-Control-Allow-Origin" origin.Value)
        |> Option.toList

    type Content with
        static member JsonWithCORS ctx = 
            Content.Json
            >> Content.WithHeaders (addCORSHeader ctx)

    let allExpenses ctx expenses =
        let labels, expenses =
            expenses
            |> ExpenseDataFrame.GetAllExpensesChart
            
        { Title = "All expenses"
          Labels = labels |> List.map (fun d -> d.ToShortDateString())
          DataSeriesList = expenses |> List.map (fun (title, expenses) -> { Title = title; Values = expenses |> List.map snd }) }
        |> Content.JsonWithCORS ctx


    let expensesForCategory category ctx expenses =
        let expenses =
            expenses
            |> ExpenseDataFrame.GetExpenses category "Date"
            |> List.sortBy (fun e -> e.Date)
            
        { Title = string category + " - Expenses"
          Labels = expenses |> List.map (fun e -> e.Date.ToShortDateString())
          DataSeriesList = [ { Title = string category + " - Expenses"; Values = expenses } ] }
        |> Content.JsonWithCORS ctx


    let smoothExpensesForCategory category ctx expenses =
        let expenses =
            expenses
            |> ExpenseDataFrame.GetSmoothExpenses category "Date"
            |> List.sortBy (fun e -> e.Date)
            
        { Title = string category + " - Expenses"
          Labels = expenses |> List.map (fun e -> e.Date.ToShortDateString())
          DataSeriesList = [ { Title = string category + " - Expenses"; Values = expenses } ] }
        |> Content.JsonWithCORS ctx
        

    let expenseLevelsCount ctx expenses =
        let counts =
            expenses
            |> ExpenseDataFrame.GetExpenseLevelCount

        { Title = "All expenses"
          Labels = [ "0-20"; "21-50"; "51-70"; "71-90"; "91-max" ]
          DataSeriesList = counts |> List.map (fun (category, counts) -> { Title = category; Values = counts |> List.map snd }) }
        |> Content.JsonWithCORS ctx


    let daySpanExpenses ctx expenses =
        expenses
        |> ExpenseDataFrame.GetDaySpanExpenses Category.Supermarket
        |> Seq.toList
        |> Content.JsonWithCORS ctx


    let binaryExpenses ctx expenses=
        expenses
        |> ExpenseDataFrame.GetBinaryExpenses Category.Supermarket
        |> Seq.toList
        |> Content.JsonWithCORS ctx


    let expandingExpenses ctx expenses =
        expenses
        |> ExpenseDataFrame.GetExpandingMean Category.Supermarket
        |> Seq.toList
        |> Content.Json
        |> Content.WithHeaders (addCORSHeader ctx)
        
    let categoryRatioPerMonth ctx expenses =
        expenses
        |> ExpenseDataFrame.GetCategoryRatioPerMonth
        |> Content.JsonWithCORS ctx

        
    let labelsPerMonth ctx expenses =
        expenses
        |> ExpenseDataFrame.GetLabelsPerMonth
        |> Content.JsonWithCORS ctx

        
    let transactionsAndSumPerMonth ctx expenses =
        expenses
        |> ExpenseDataFrame.GetNumberTransactionsAndSumPerMonth
        |> Content.JsonWithCORS ctx

    /// Serializable type used for Expanding sum for each day of the month per category
    type ExpandedSumCategory = {
        Category: string
        ExpandedSums: ExpandedSum list
    } and ExpandedSum = {
        Month: int
        MonthReadable: string
        Year: int
        Values: ExpandedValue list
    } and ExpandedValue = {
        DayOfMonth: int
        Date: DateTime
        Value: float
    }

    let categoryExpandingSumForEachDayOfTheMonth ctx expenses =
        expenses
        |> ExpenseDataFrame.GetCategoryExpandingSumForEachDayOfTheMonth
        |> List.groupBy (fun (Title category, _,_,_) -> category)
        |> List.map (fun (key, values) ->
            { Category = key
              ExpandedSums =
                values
                |> List.map (fun (_, Month (monthReadable, month), Year year, values) -> 
                    { Month = month
                      MonthReadable = monthReadable
                      Year = year
                      Values = 
                        values 
                        |> List.map (fun (date, value) ->
                            { DayOfMonth = date.Day
                              Date = date
                              Value = value }) }) })
        |> Content.JsonWithCORS ctx
