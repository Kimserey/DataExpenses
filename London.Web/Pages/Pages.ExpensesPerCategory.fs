namespace London.Web.Pages

open System
open System.IO
open WebSharper
open WebSharper.Remoting
open London.Core
open London.Web.Pages.Common
open Deedle

module ExpensesPerCategory =
    module Rpcs =
        [<Rpc>]
        let get(): Async<Map<string, Expense list>> =
            getExpensesPerCategory()
            |> async.Return
                
    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next
        open WebSharper.UI.Next.Client
        open WebSharper.JavaScript
        open London.Web.Templates
        
        let page =
            async {
                let! expenses = Rpcs.get()
                return expenses
                        |> Map.toList
                        |> List.mapi (fun cardIndex (category, expenses) ->
                            Card.Doc(
                               category,
                               expenses
                               |> List.groupBy (fun e -> string e.Date.Month + " " + string e.Date.Year)
                               |> List.mapi (fun contentIndex (month, values) ->
                                    Card.Item.Doc(
                                        "card-" + string cardIndex + "-content-" + string contentIndex,
                                        month,
                                        values 
                                        |> List.sumBy (fun e -> e.Amount) 
                                        |> parseFloat 2
                                        |> string,
                                        [ Table.Doc (List.map Expense.ToTableRow values) ]))))
                        |> Doc.Concat
            }
            |> Doc.Async