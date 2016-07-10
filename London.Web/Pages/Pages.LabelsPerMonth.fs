namespace London.Web.Pages

open System
open System.IO
open WebSharper
open WebSharper.Remoting
open London.Core
open London.Core.Dataframe
open London.Web.Pages.Common
open Deedle

module LabelsPerMonth =
    module Rpcs =
        [<Rpc>]
        let get(): Async<_> =
            Dataframe.agent.Get()
            |> ExpenseDataFrame.GetFrame
            |> ExpenseDataFrame.GetLabelsPerMonth
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
                let! months = Rpcs.get()
                
                return months
                       |> List.sortByDescending (fun ((_, date), _, _) -> date)
                       |> List.mapi (fun cardIndex ((Title title, date), Sum sum, labels) ->
                                Card.Doc 
                                    [ CardList.Doc(
                                           title,
                                           sum.JS.ToFixed 2,
                                           labels
                                           |> List.mapi (fun contentIndex (Title label, Sum sum, expenses) ->
                                                CardList.Item.Doc(
                                                    "card-" + string cardIndex + "-content-" + string contentIndex,
                                                    label,
                                                    sum.JS.ToFixed 2,
                                                    [ CardTable.Doc (List.mapi Expense.ToTableRow expenses) ]))) 
                                      divAttr 
                                        [ attr.style "height: 100%;"
                                          on.afterRender (fun el -> 
                                            JQuery
                                                .Of(el)
                                                .BarChart(
                                                {
                                                    Chart = { Type = "bar"; ZoomType = "" }
                                                    Title = { Text = "" }
                                                    XAxis =
                                                        { Categories = 
                                                            labels 
                                                            |> List.map (fun (Title title, _, _) -> title) |> Array.ofList }
                                                    YAxis = { Title = { Text = "Amount" } }
                                                    PlotOptions = { Bar = { PointWidth = 10. } }
                                                    Series = 
                                                        [| { Name = title
                                                             Data = 
                                                                labels 
                                                                |> List.map (fun (_, Sum sum, _) -> sum) 
                                                                |> List.toArray } |]
                                                    Tooltip =
                                                        { 
                                                            HeaderFormat = "<span style=\"font-size: 10px\">{point.key}</span><br/>"
                                                            PointFormat = "{series.name}: {point.y} GBP" 
                                                        }
                                                }) |> ignore) ] [] :> Doc ])
                       |> Doc.Concat
                        
            }
            |> Doc.Async