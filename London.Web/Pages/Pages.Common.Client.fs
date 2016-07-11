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
        member x.BarChart (options: BarChartOptions) = X<JQuery>

    // Common options
    and Chart = {
        [<Name "type">]
        Type: string
        [<Name "zoomType">]
        ZoomType: string
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
        [<Name "headerFormat">]
        HeaderFormat: string
        [<Name "pointFormat">]
        PointFormat: string    
    }
    and Legend = {
        [<Name "x">]
        X: float
        [<Name "y">]
        Y: float
        [<Name "align">]
        Align: string
        [<Name "verticalAlign">]
        VerticalAlign: string
        [<Name "floating">]
        Floating: string
        [<Name "borderColor">]
        BorderColor: string
        [<Name "borderWidth">]
        BorderWidth: string
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
        [<Name "plotOptions">]
        PlotOptions: SplinePlotOptions
    }
    and LineSeries = {
        [<Name "name">]
        Name: string
        [<Name "data">]
        Data: float []
    }
    and SplinePlotOptions = {
        [<Name "spline">]
        Spline: SplinePlotOptionsSpline
    }
    and SplinePlotOptionsSpline = {
        [<Name "marker">]
        Marker: LinePlotOptionsMarker
    }
    and LinePlotOptionsMarker = {
        [<Name "enabled">]
        Enabled: bool
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
    and BarChartOptions = {
        [<Name "chart">]
        Chart: Chart
        [<Name "title">]
        Title: Title
        [<Name "xAxis">]
        XAxis: XAxis
        [<Name "yAxis">]
        YAxis: YAxis
        [<Name "series">]
        Series: BarSeries []
        [<Name "tooltip">]
        Tooltip: Tooltip
        [<Name "plotOptions">]
        PlotOptions: BarPlotOptions
    }
    and BarSeries = {
        [<Name "name">]
        Name: string
        [<Name "data">]
        Data: float []
    }
    and BarPlotOptions = {
        [<Name "bar">]
        Bar: BarPlotOptionsBar
    }
    and BarPlotOptionsBar = {
        [<Name "pointWidth">]
        PointWidth: float
    }