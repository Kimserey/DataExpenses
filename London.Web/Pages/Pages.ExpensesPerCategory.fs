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
                        |> List.mapi (fun cardIndex (Title category, Sum sum, subCategory) ->
                            Card.Doc 
                                [ CardList.Doc(
                                       category,
                                       sum |> parseFloat 2 |> string,
                                       subCategory
                                       |> List.mapi (fun contentIndex (Title month, Sum sum, values) ->
                                            CardList.Item.Doc(
                                                "card-" + string cardIndex + "-content-" + string contentIndex,
                                                month,
                                                sum |> parseFloat 2 |> string,
                                                [ CardTable.Doc (List.mapi Expense.ToTableRow values) ]))) ])
                        |> Doc.Concat
            }
            |> Doc.Async