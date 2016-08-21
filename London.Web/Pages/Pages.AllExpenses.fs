namespace London.Web.Pages

open System
open System.IO
open WebSharper
open WebSharper.JQuery
open WebSharper.Remoting
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open London.Core
open London.Core.Dataframe
open Deedle
open London.Web.Pages.Common

module Expenses =
    module Rpcs =
        [<Rpc>]
        let getCumulatedExpenses(): Async<_> =
            let data =
                Dataframe.agent.Get()
                |> ExpenseDataFrame.GetCumulatedExpenses []
            
            data |> async.Return

        [<Rpc>]
        let get sortBy: Async<_> =
            Dataframe.agent.Get()
            |> ExpenseDataFrame.GetAllExpenses sortBy
            |> async.Return
                
    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next
        open WebSharper.UI.Next.Client
        open WebSharper.JavaScript
        open London.Web.Templates     
        
        let makeChart key title dailyCumulatedExpenses selector =
            divAttr 
                [ attr.id key
                  attr.``class`` "chart-card"
                  on.afterRender (fun el -> 
                    JQuery
                        .Of(el)
                        .LineChart(
                        {
                            Chart = { Type = "spline"; ZoomType = "xy" }
                            Title = { Text = title }
                            XAxis = 
                             { Categories = 
                                 [1..31]
                                 |> List.map string
                                 |> Array.ofList }
                            YAxis = { Title = { Text = "Amount" } }
                            PlotOptions = { Spline = { Marker = { Enabled = false } } }
                            Series = 
                                dailyCumulatedExpenses
                                |> List.map selector
                                |> List.toArray
                            Tooltip = 
                             { 
                                 HeaderFormat = "<span style=\"font-size: 10px\">Day {point.key}</span><br/>"
                                 PointFormat = "<span style=\"color:{point.color}\">► </span> {series.name}: <b>{point.y} GBP</b><br/>" 
                             }
                        }) |> ignore) ] [] :> Doc  
        
        let page =
            async {
                let! expenses = Rpcs.get "Date"
                let! dailyCumulatedExpenses = Rpcs.getCumulatedExpenses()

                return Card.Doc 
                        [ Tabs.Doc(
                            Links = [
                                Tabs.ActiveTabLink.Doc(
                                    TargetId = "list",
                                    Title = "List")
                                Tabs.InactiveTabLink.Doc(
                                    TargetId = "chart",
                                    Title = "Chart")
                            ],
                            Content =[
                                Tabs.ActiveContent.Doc(
                                    Id = "list",
                                    Body = [ CardTable.Doc (List.mapi Expense.ToTableRow (expenses |> List.sortByDescending (fun e -> e.Date)))  ])
                                Tabs.InactiveContent.Doc(
                                    Id = "chart",
                                    Body = [ makeChart "cumul" "Cumulated expenses" dailyCumulatedExpenses (fun (name, data) -> { Name = name; Data = data |> List.map snd |> Array.ofList } : LineSeries) ])
                            ]) ]
            }
            |> Doc.Async