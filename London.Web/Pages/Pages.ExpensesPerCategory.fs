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
            expenses
            |> ExpenseDataFrame.GetExpensesPerCategory
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
                        |> List.mapi (fun cardIndex (category, subCategory) ->
                            Card.Doc(
                               category,
                               subCategory
                               |> List.mapi (fun contentIndex (month, amount, values) ->
                                    Card.Item.Doc(
                                        "card-" + string cardIndex + "-content-" + string contentIndex,
                                        month,
                                        amount 
                                        |> parseFloat 2
                                        |> string,
                                        [ Table.Doc (List.map Expense.ToTableRow values) ]))))
                        |> Doc.Concat
            }
            |> Doc.Async