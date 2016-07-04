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
        let get(): Async<_> =
            Dataframe.agent.Get()
            |> ExpenseDataFrame.GetFrame
            |> ExpenseDataFrame.GetExpensesPerCategory
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
                let! expenses = Rpcs.get()
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
                                   [ on.afterRender (fun el -> 
                                       JQuery
                                           .Of(el)
                                           .LineChart(
                                           {
                                               Chart = { Type = "spline"; ZoomType = ""; MarginBottom = 0. }
                                               Title = { Text = "" }
                                               XAxis = 
                                                { Categories = 
                                                    subCategory 
                                                    |> List.map (fun (Title title, _, _) -> title) 
                                                    |> Array.ofList }
                                               YAxis = { Title = { Text = "Amount" } }
                                               Series = [| { Name = "Total"; Data = subCategory |> List.map (fun (_, Sum sum, _) -> sum) |> Array.ofList } |]
                                               Tooltip = { PointFormat = "{point.y:.2f} GBP" }
                                           }) |> ignore) ] [] :> Doc 
                            ])
                        |> Doc.Concat
            }
            |> Doc.Async