namespace London.Web

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Server
open London.Core
open London.Core.Dataframe

module Api =
    
    type Chart<'T> = {
        Title: string
        Labels: string list
        DataSeriesList: DataSeries<'T> list
    } 

    and DataSeries<'T> = {
        Title: string
        Values: 'T list
    }

    let content ctx =
        // CORS - authorize request
        let headers =
            ctx.Request.Headers
            |> Seq.tryFind (fun h -> h.Name = "Origin")
            |> Option.map (fun origin -> Http.Header.Custom "Access-Control-Allow-Origin" origin.Value)
            |> Option.toList

        let labels, expenses =
            expenses
            |> ExpenseDataFrame.GetAllExpensesChart
            
        { Title = "All expenses"
          Labels = labels |> List.map (fun d -> d.ToShortDateString())
          DataSeriesList = expenses |> List.map (fun (title, expenses) -> { Title = title; Values = expenses |> List.map snd }) }
        |> Content.Json
        |> Content.WithHeaders headers