namespace London.Web

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Server
open London.Core
open London.Core.Dataframe

module Sitelet =
    
    type EndPoint =
    | [<EndPoint "/">] App
    | [<EndPoint "/api">] Api of ApiEndPoint

    and ApiEndPoint =
        | [<EndPoint "/expenses">] Expenses of string

    type Chart<'label, 'value> = {
        Title: string
        Labels: 'label list
        Values: 'value list
    }

    let sitelet =
        Sitelet.Infer (fun ctx ->
            function
            | App ->
                Templates.Index.Doc(
                    Title = "London expenses",
                    Nav = [ client <@ App.nav @> ],
                    Main = [ client <@ App.main @> ])
                |> Content.Page
            
            | Api (Expenses sortBy) ->
                
                // CORS - authorize request
                let headers =
                    ctx.Request.Headers
                    |> Seq.tryFind (fun h -> h.Name = "Origin")
                    |> Option.map (fun origin -> Http.Header.Custom "Access-Control-Allow-Origin" origin.Value)
                    |> Option.toList

                let expenses =
                    expenses
                    |> ExpenseDataFrame.GetAllExpenses sortBy

                { Title = "All expenses"
                  Labels = expenses |> List.map (fun e -> e.Date.Date) |> List.sort |> List.map (fun e -> e.ToShortDateString())
                  Values = expenses |> List.map (fun e -> e.Amount) }
                |> Content.Json
                |> Content.WithHeaders headers)