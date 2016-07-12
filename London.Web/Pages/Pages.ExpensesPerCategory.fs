namespace London.Web.Pages

open System
open System.IO
open WebSharper
open WebSharper.Remoting
open London.Core
open London.Core.Dataframe
open London.Web.Pages.Common
open Deedle

module ExpensesPerCategory =
    module Rpcs =
        [<Rpc>]
        let getTotalSums(): Async<_> =
            Dataframe.agent.Get()
            |> ExpenseDataFrame.GetFrame
            |> ExpenseDataFrame.GetExpensesPerCategory
            |> async.Return
        
        [<Rpc>]
        let getExpandingSums(): Async<_> =
            Dataframe.agent.Get()
            |> ExpenseDataFrame.GetFrame
            |> ExpenseDataFrame.GetCategoryExpandingSumForEachDayOfTheMonth
            |> async.Return
            
    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next
        open WebSharper.UI.Next.Html
        open WebSharper.UI.Next.Client
        open WebSharper.JavaScript
        open WebSharper.JQuery
        open London.Web.Templates

        let makeTotalChart category subCategory =
            divAttr 
                [ attr.``class`` "chart-card"
                  on.afterRender (fun el -> 
                    JQuery
                        .Of(el)
                        .LineChart(
                        {
                            Chart = { Type = "spline"; ZoomType = "" }
                            Title = { Text = category + " - Monthly sum" }
                            XAxis = 
                             { Categories = 
                                 subCategory 
                                 |> List.map (fun (Title title, _, _) -> title) 
                                 |> Array.ofList }
                            YAxis = { Title = { Text = "Amount" } }
                            Series = [| { Name = "Total"; Data = subCategory |> List.map (fun (_, Sum sum, _) -> sum) |> Array.ofList } |]
                            Tooltip =
                             {
                                 HeaderFormat = "<span style=\"font-size: 10px\">{point.key}</span><br/>"
                                 PointFormat = "{point.y} GBP"
                             }
                            PlotOptions = { Spline = { Marker = { Enabled = true } } }
                        }) |> ignore) ] [] :> Doc 

        let makeDailyExpandedSum category expandingSums =
            divAttr 
                [ attr.``class`` "chart-card"
                  on.afterRender (fun el -> 
                    let data =
                     expandingSums
                     |> List.filter (fun (Title c, _, _, _) -> c = category)
                    
                    JQuery
                        .Of(el)
                        .LineChart(
                        {
                            Chart = { Type = "spline"; ZoomType = "xy" }
                            Title = { Text = category + " - Daily sum" }
                            XAxis = 
                             { Categories = 
                                 [1..31]
                                 |> List.map string
                                 |> Array.ofList }
                            YAxis = { Title = { Text = "Amount" } }
                            PlotOptions = { Spline = { Marker = { Enabled = false } } }
                            Series = 
                             data
                             |> List.map (fun (_, Month (m, _), Year y, values) -> 
                                 let series: LineSeries =
                                     { Name = sprintf "%s %i" m y
                                       Data = values |> List.map snd |> Array.ofList }
                                 series)
                             |> Array.ofList
                            Tooltip = 
                             { 
                                 HeaderFormat = "<span style=\"font-size: 10px\">Day {point.key}</span><br/>"
                                 PointFormat = "<span style=\"color:{point.color}\">► </span> {series.name}: <b>{point.y} GBP</b><br/>" 
                             }
                        }) |> ignore) ] [] :> Doc 
        let page =
            async {
                let! expenses = Rpcs.getTotalSums()
                let! expandingSums = Rpcs.getExpandingSums()

                return expenses
                        |> List.sortBy (fun (Title title, _ , _) -> title)
                        |> List.mapi (fun cardIndex (Title category, Sum sum, subCategory) ->
                            let totalChart = makeTotalChart category subCategory
                            let dailyExpandedSum = makeDailyExpandedSum category expandingSums

                            Card.Doc 
                                [ CardList.Doc(
                                       category,
                                       sum.JS.ToFixed 2,
                                       subCategory
                                       |> List.mapi (fun contentIndex (Title month, Sum sum, values) ->
                                            CardList.Item.Doc(
                                                "card-" + string cardIndex + "-content-" + string contentIndex,
                                                month,
                                                sum.JS.ToFixed 2,
                                                [ CardTable.Doc (List.mapi Expense.ToTableRow values) ]))) 
                                  
                                  Tabs.Doc(
                                       Links = 
                                        [
                                            Tabs.ActiveTabLink.Doc(
                                                TargetId = "chart-total-" + string cardIndex, 
                                                Title = "Monthly total")
                                            Tabs.InactiveTabLink.Doc(
                                                TargetId = "chart-daily-expanded-sum-" + string cardIndex, 
                                                Title = "Daily sum")
                                        ],
                                       Content =
                                        [
                                            Tabs.ActiveContent.Doc(
                                                Id = "chart-total-" + string cardIndex, 
                                                Body = [ totalChart ])
                                            Tabs.InactiveContent.Doc(
                                                Id = "chart-daily-expanded-sum-" + string cardIndex,
                                                Body = [ dailyExpandedSum ])
                                        ])
                            ])
                        |> Doc.Concat
            }
            |> Doc.Async