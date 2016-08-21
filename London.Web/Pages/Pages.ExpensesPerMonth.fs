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
            Dataframe.agent.Get()
            |> ExpenseDataFrame.GetExpensesPerMonth
            |> async.Return

        [<Rpc>]
        let getRatio(): Async<_> =
            Dataframe.agent.Get()
            |> ExpenseDataFrame.GetCategoryRatioPerMonth
            |> List.map (fun (d, x) -> DateTime.ParseExact(d, "MMM yyyy", Globalization.CultureInfo.InvariantCulture), x)
            |> async.Return

        [<Rpc>]
        let getDailyExpandingSum(): Async<_> =
            Dataframe.agent.Get()
            |> ExpenseDataFrame.GetDailyExpandingSumPerMonth
            |> async.Return

    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next
        open WebSharper.UI.Next.Html
        open WebSharper.UI.Next.Client
        open WebSharper.JQuery
        open WebSharper.JavaScript
        open London.Web.Templates       
        
        let makeRatioChart ratios month year =
            ratios
            |> List.tryFind (fun (d: DateTime, _) -> d.Month = month && d.Year = year)
            |> Option.map (fun (_, expenses) ->
                divAttr 
                    [ attr.``class`` "chart-card"
                      on.afterRender (fun el -> 
                        JQuery
                            .Of(el)
                            .PieChart(
                            {
                                Chart = { Type = "pie"; ZoomType = "" }
                                Title = { Text = "" }
                                XAxis = { Categories = expenses |> List.map fst |> Array.ofList }
                                YAxis = { Title = { Text = "Amount" } }
                                Series = [| { Name = "Total"; Data = expenses |> List.map (fun (k, v) -> { Name = k; Y = v }) |> List.toArray } |]
                                PlotOptions = { Pie = { DataLabels = { Enabled = true } } }
                                Tooltip = 
                                    { 
                                        HeaderFormat = "<span style=\"font-size: 10px\">{point.key}</span><br/>"
                                        PointFormat = "{point.y:.2f}%" 
                                    }
                            }) |> ignore) ] [] :> Doc)
            |> Option.toList
            |> Doc.Concat


        let makeSumChart sums month year =
            divAttr 
                [ attr.``class`` "chart-card"
                  on.afterRender (fun el -> 
                    JQuery
                        .Of(el)
                        .LineChart(
                        {
                            Chart = { Type = "spline"; ZoomType = "xy" }
                            Title = { Text = "Daily expanding sum" }
                            XAxis = 
                             { Categories = 
                                 [1..31]
                                 |> List.map string
                                 |> Array.ofList }
                            YAxis = { Title = { Text = "Amount" } }
                            PlotOptions = { Spline = { Marker = { Enabled = false } } }
                            Series = 
                                [| { Name = "Sums"
                                     Data = 
                                        sums 
                                        |> List.filter (fun (Month (_, m), Year y, _) -> m = month && y = year) 
                                        |> List.collect (fun (_, _, value) -> value)
                                        |> List.map (fun (_, Sum value) -> value)
                                        |> Array.ofList } |]
                            Tooltip = 
                             { 
                                 HeaderFormat = "<span style=\"font-size: 10px\">Day {point.key}</span><br/>"
                                 PointFormat = "<span style=\"color:{point.color}\">► </span> {series.name}: <b>{point.y} GBP</b><br/>" 
                             }
                        }) |> ignore) ] [] :> Doc 

        let page =
            async {
                let! expensesPerMonth = Rpcs.get()
                let! ratios = Rpcs.getRatio()
                let! sums = Rpcs.getDailyExpandingSum()

                return expensesPerMonth
                        |> List.sortByDescending (fun (Month (_, month), Year year, _, _) -> month + year * 100)
                        |> List.mapi (fun cardIndex (Month (monthString, month), Year year, Sum sum, expenses) ->
                            Card.Doc [
                               CardList.Doc(
                                   monthString + " " + string year,
                                   sum.JS.ToFixed 2,
                                   expenses
                                   |> List.sortBy (fun (Title c, _, _) -> c)
                                   |> List.mapi (fun contentIndex (Title category, Sum sum, expenses) ->
                                        CardList.Item.Doc(
                                            "card-" + string cardIndex + "-content-" + string contentIndex,
                                            category,
                                            sum.JS.ToFixed 2,
                                            [ CardTable.Doc (List.mapi Expense.ToTableRow expenses) ])))
                               Tabs.Doc(
                                       Links = 
                                        [
                                            Tabs.ActiveTabLink.Doc(
                                                TargetId = "chart-ratios-" + string cardIndex, 
                                                Title = "Ratio")
                                            Tabs.InactiveTabLink.Doc(
                                                TargetId = "chart-daily-expanding-sum-" + string cardIndex, 
                                                Title = "Daily expanding sum")
                                        ],
                                       Content =
                                        [
                                            Tabs.ActiveContent.Doc(
                                                Id = "chart-ratios-" + string cardIndex, 
                                                Body = [ makeRatioChart ratios month year])
                                            Tabs.InactiveContent.Doc(
                                                Id = "chart-daily-expanding-sum-" + string cardIndex,
                                                Body = [ makeSumChart sums month year])
                                        ])
                            ])
                        |> Doc.Concat
            }
            |> Doc.Async