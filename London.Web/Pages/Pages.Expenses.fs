namespace London.Web.Pages

open System
open System.IO
open WebSharper
open WebSharper.Remoting
open London.Core
open London.Core.Dataframe
open Deedle
open London.Web.Pages.Common

module Expenses =
    module Rpcs =
        [<Rpc>]
        let get sortBy: Async<_> =
            Dataframe.agent.Get()
            |> ExpenseDataFrame.GetFrame
            |> ExpenseDataFrame.GetAllExpenses sortBy
            |> async.Return
                
    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next
        open WebSharper.UI.Next.Client
        open WebSharper.JavaScript
        open London.Web.Templates       
        
        let page =
            async {
                let! expenses = Rpcs.get "Date"
                return Card.Doc [ CardTable.Doc (List.mapi Expense.ToTableRow (expenses |> List.sortByDescending (fun e -> e.Date))) ]
            }
            |> Doc.Async