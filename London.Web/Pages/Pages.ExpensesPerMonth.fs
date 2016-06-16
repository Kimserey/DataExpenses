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
            expenses
            |> ExpenseDataFrame.GetExpensesPerMonth
            |> async.Return
                
    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next
        open WebSharper.UI.Next.Client
        open WebSharper.JavaScript
        open London.Web.Templates       
        
        let page =
            async {
                let! expensesPerMonth = Rpcs.get()
                return expensesPerMonth
                        |> List.sortByDescending (fun (Month (_, month), Year year, _, _) -> month + year * 100)
                        |> List.mapi (fun cardIndex (Month (month, _), Year year, Sum sum, expenses) ->
                            Card.Doc [
                               CardList.Doc(
                                   month + " " + string year,
                                   sum.JS.ToFixed 2,
                                   expenses
                                   |> List.sortBy (fun (Title c, _, _) -> c)
                                   |> List.mapi (fun contentIndex (Title category, Sum sum, expenses) ->
                                        CardList.Item.Doc(
                                            "card-" + string cardIndex + "-content-" + string contentIndex,
                                            category,
                                            sum.JS.ToFixed 2,
                                            [ CardTable.Doc (List.mapi Expense.ToTableRow expenses) ]))) ])
                        |> Doc.Concat
            }
            |> Doc.Async