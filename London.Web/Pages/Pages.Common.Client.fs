namespace London.Web.Pages

open System
open London.Core
open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.JavaScript
open WebSharper.JQuery
open London.Web.Templates    

[<JavaScript; AutoOpen>]
module Common =
    type Expense with
        static member ToTableRow (index: int) x =
            CardTable.Row.Doc (string index, x.Date.ToLongDateString(), x.Label, x.Amount.JS.ToFixed 2, x.Category)

    [<Direct "simpleUI.toggleSideMenu()">]
    let toggleSideMenu() = X<unit>

    type JQuery with
        [<Inline """$0.highcharts($1)""">]
        member x.Highcharts (chart: HighchartsOptions) = X<JQuery>
    and HighchartsOptions = {
        [<Name "chart">]
        Chart: Chart
        [<Name "title">]
        Title: Title
        [<Name "xAxis">]
        XAxis: XAxis
        [<Name "yAxis">]
        YAxis: YAxis
        [<Name "series">]
        Series: Series []
    }
    and Chart = {
        [<Name "type">]
        Type: string
    }
    and XAxis = {
        [<Name "categories">]
        Categories: string []
    }
    and YAxis = {
        [<Name "title">]
        Title: Title
    }
    and Title = {
        [<Name "text">]
        Text: string
    }
    and Series = {
        [<Name "name">]
        Name: string
        [<Name "data">]
        Data: float []
    }