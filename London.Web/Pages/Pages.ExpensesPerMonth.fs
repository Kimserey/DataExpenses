namespace London.Web.Pages

open System
open System.IO
open WebSharper
open WebSharper.Remoting
open London.Core
open London.Core.Dataframe
open Deedle
open London.Web.Pages.Common

module ExpensesPerMonth =
    module Rpcs =
        [<Rpc>]
        let get(): Async<_> =
            expenses
            |> ExpenseDataFrame.GetExpensesPerMonth
            |> async.Return

        [<Rpc>]
        let getRatio(): Async<_> =
            expenses
            |> ExpenseDataFrame.GetCategoryRatioPerMonth
            |> List.map (fun (d, x) -> DateTime.ParseExact(d, "MMM yyyy", Globalization.CultureInfo.InvariantCulture), x)
            |> async.Return

    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next
        open WebSharper.UI.Next.Html
        open WebSharper.UI.Next.Client
        open WebSharper.JQuery
        open WebSharper.JavaScript
        open London.Web.Templates       
        
        let page =
            async {
                let! expensesPerMonth = Rpcs.get()
                let! ratio = Rpcs.getRatio()

                return expensesPerMonth
                        |> List.sortByDescending (fun (Month (_, month), Year year, _, _) -> month + year * 100)
                        |> List.mapi (fun cardIndex (Month (month, m), Year year, Sum sum, expenses) ->
                            Card.Doc [
                               CardList.Doc(
                                   month + " " + string year,
                                   sum.JS.ToFixed 2,
                                   expenses
                                   |> List.sortBy (fun (Title c, _, _) -> c)
                                   |> List.mapi (fun contentIndex (Title category, Sum sum, expenses) ->
                                        CardList.Item.Doc(
                                            "card-" + string cardIndex + "-content-" + string contentIndex,
                                            category,
                                            sum.JS.ToFixed 2,
                                            [ CardTable.Doc (List.mapi Expense.ToTableRow expenses) ])))
                                
                               ratio
                               |> List.tryFind (fun (d, x) -> d.Month = m && d.Year = year)
                               |> Option.map (fun (_, expenses) ->
                                   divAttr 
                                    [ on.afterRender (fun el -> 
                                        JQuery
                                            .Of(el)
                                            .PieChart(
                                            {
                                                Chart = { Type = "pie"; ZoomType = ""; MarginBottom = 0. }
                                                Title = { Text = "" }
                                                XAxis = { Categories = expenses |> List.map fst |> Array.ofList }
                                                YAxis = { Title = { Text = "Amount" } }
                                                Series = [| { Name = "Total"; Data = expenses |> List.map (fun (k, v) -> { Name = k; Y = v }) |> List.toArray } |]
                                                PlotOptions = { Pie = { DataLabels = { Enabled = true } } }
                                                Tooltip = { PointFormat = "{point.y:.2f}%" }
                                            }) |> ignore) ] [] :> Doc)
                               |> Option.toList
                               |> Doc.Concat
                            ])
                        |> Doc.Concat
            }
            |> Doc.Async