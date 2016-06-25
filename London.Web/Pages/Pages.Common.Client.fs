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
        member x.LineChart (options: LineChartOptions) = X<JQuery>
        [<Inline """$0.highcharts($1)""">]
        member x.PieChart (options: PieChartOptions) = X<JQuery>
        [<Inline """$0.highcharts($1)""">]
        member x.ColumnChart (options: ColumnChartOptions) = X<JQuery>

    // Common options
    and Chart = {
        [<Name "type">]
        Type: string
        [<Name "zoomType">]
        ZoomType: string option
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
    and PlotOptionsDataLabels = {
        [<Name "enabled">]
        Enabled: bool
    }
    and Tooltip = {
        [<Name "pointFormat">]
        PointFormat: string    
    }

    // Line specific options
    and LineChartOptions = {
        [<Name "chart">]
        Chart: Chart
        [<Name "title">]
        Title: Title
        [<Name "xAxis">]
        XAxis: XAxis
        [<Name "yAxis">]
        YAxis: YAxis
        [<Name "series">]
        Series: LineSeries []
        [<Name "tooltip">]
        Tooltip: Tooltip
    }
    and LineSeries = {
        [<Name "name">]
        Name: string
        [<Name "data">]
        Data: float []
    }
    
    // Pie specific options
    and PieChartOptions = {
        [<Name "chart">]
        Chart: Chart
        [<Name "title">]
        Title: Title
        [<Name "xAxis">]
        XAxis: XAxis
        [<Name "yAxis">]
        YAxis: YAxis
        [<Name "series">]
        Series: PieSeries []
        [<Name "plotOptions">]
        PlotOptions: PiePlotOptions
        [<Name "tooltip">]
        Tooltip: Tooltip
    }
    and PieSeries = {
        [<Name "name">]
        Name: string
        [<Name "data">]
        Data: PieData []
    }
    and PieData = {
        [<Name "name">]
        Name: string
        [<Name "y">]
        Y: float
    }
    and PiePlotOptions = {
        [<Name "pie">]
        Pie: PiePlotOptionsPie
    }
    and PiePlotOptionsPie = {
        [<Name "datalabels">]
        DataLabels: PlotOptionsDataLabels
    }

    // Column specific options
    and ColumnChartOptions = {
        [<Name "chart">]
        Chart: Chart
        [<Name "title">]
        Title: Title
        [<Name "xAxis">]
        XAxis: XAxis
        [<Name "yAxis">]
        YAxis: YAxis
        [<Name "series">]
        Series: ColumnSeries []
        [<Name "tooltip">]
        Tooltip: Tooltip
    }
    and ColumnSeries = {
        [<Name "name">]
        Name: string
        [<Name "data">]
        Data: float []
    }