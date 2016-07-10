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
        
        let page =
            async {
                let! expenses = Rpcs.getTotalSums()
                let! expandingSums = Rpcs.getExpandingSums()

                return expenses
                        |> List.sortBy (fun (Title title, _ , _) -> title)
                        |> List.mapi (fun cardIndex (Title category, Sum sum, subCategory) ->
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
                                  
                                  divAttr 
                                   [ attr.``class`` "chart-card"
                                     on.afterRender (fun el -> 
                                       JQuery
                                           .Of(el)
                                           .LineChart(
                                           {
                                               Chart = { Type = "spline"; ZoomType = "" }
                                               Title = { Text = "Sum of all " + category + " expenses per month" }
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
                                                    PointFormat = "{point.y:.2f} GBP"
                                                }
                                               PlotOptions = { Spline = { Marker = { Enabled = true } } }
                                           }) |> ignore) ] [] :> Doc 
                                  
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
                                               Title = { Text = "Daily expanding sum of " + category }
                                               XAxis = Unchecked.defaultof<XAxis>
                                               YAxis = Unchecked.defaultof<YAxis>
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

                            ])
                        |> Doc.Concat
            }
            |> Doc.Async