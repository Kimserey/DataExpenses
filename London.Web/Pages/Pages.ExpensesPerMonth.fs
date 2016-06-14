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
        let get(): Async<Map<(Month * Year), Expense list>> =
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
                        |> Map.toList
                        |> List.sortByDescending (fun ((Month (_, month), Year year), _) -> month + year * 100)
                        |> List.mapi (fun cardIndex ((Month (month, _), Year year), expenses) ->
                            Card.Doc(
                               month + " " + string year,
                               expenses
                               |> List.groupBy (fun e -> e.Category)
                               |> List.sortBy  fst
                               |> List.mapi (fun contentIndex (category, values) ->
                                    Card.Item.Doc(
                                        "card-" + string cardIndex + "-content-" + string contentIndex,
                                        category,
                                        values 
                                        |> List.sumBy (fun e -> e.Amount) 
                                        |> parseFloat 2
                                        |> string,
                                        [ Table.Doc (List.map Expense.ToTableRow values) ]))))
                        |> Doc.Concat
            }
            |> Doc.Async