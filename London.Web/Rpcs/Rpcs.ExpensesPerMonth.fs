namespace London.Web.Rpcs

open System
open System.IO
open WebSharper
open WebSharper.Remoting
open London.Core
open Deedle

module ExpensesPerMonth =
    
    type Month = Month of (string * int)
    type Year = Year of int

    type Expense = {
        Date: DateTime
        Label: string
        Amount: float
        Category: string
    }

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