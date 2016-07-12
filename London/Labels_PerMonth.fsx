#load "_references.fsx"
open _references
open System
open Deedle
open London.Core

(*
    Experimentation
*)

df.Columns.[ [ "Date"; "Label"; "Amount" ] ]
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.rows
|> Series.observations
|> Seq.iter(fun v -> printfn "%A" v)

let empty: Expense list = []

df
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.pivotTable
    (fun _ r ->
        let date = r.GetAs<DateTime>("Date")
        new DateTime(date.Year, date.Month, 1))
    (fun _ c -> c.GetAs<string>("Label"))
    (fun frame -> 
        frame
        |> Frame.rows
        |> Series.dropMissing
        |> Series.observations
        |> Seq.map (fun (_, s) ->  
            { Date = s.GetAs<DateTime>("Date")
              Label = s.GetAs<string>("Label")
              Title = s.GetAs<string>("Title")
              Amount = s?Amount
              Category = s.GetAs<string>("Category") })
        |> Seq.toList)
|> Frame.fillMissingWith empty
|> Frame.getRows
|> Series.sortByKey
|> Series.observations
|> Seq.iter(fun v -> printfn "%A" v)

(*
    Call from Shared library
*)
df
|> ExpenseDataFrame.FromFrame
|> ExpenseDataFrame.GetLabelsPerMonth
|> Seq.iter(fun x -> printfn "%A" x)