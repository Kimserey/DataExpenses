namespace London.Web.Pages

open System
open System.IO
open WebSharper
open WebSharper.Remoting
open London.Core
open Deedle

module ExpensesPerMonth =
    
    [<JavaScript; AutoOpen>]
    module Domain =
        type Month = Month of (string * int)
        type Year = Year of int

        type Expense = {
            Date: DateTime
            Label: string
            Amount: float
            Category: string
        }


    module Rpcs =
        [<Rpc>]
        let get(): Async<Map<(Month * Year), Expense list>> =
            let res =
                expenses
                |> Frame.filterRowValues(fun c -> c?Amount < 0.)
                |> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month, c.GetAs<DateTime>("Date").Year)
                |> Frame.nest
                |> Series.observations
                |> Seq.map (fun ((m, y), df) ->
                    (Month (monthToString m, m), Year y), 
                    df
                    |> Frame.rows
                    |> Series.observations
                    |> Seq.map(fun (_, s) -> 
                        { Date = s.GetAs<DateTime>("Date")
                          Label = s.GetAs<string>("Label")
                          Amount = s?Amount
                          Category = s.GetAs<string>("Category") })
                    |> Seq.toList)
                |> Map.ofSeq
            async.Return res
                
    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next
        open WebSharper.UI.Next.Client
        open London.Web.Templates       

        type Expense with
            static member ToTableRow x =
                Table.Row.Doc (x.Date.ToShortDateString(), x.Label, string x.Amount, x.Category)

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