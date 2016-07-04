#load "_references.fsx"
open _references
open System
open Deedle
open London.Core

(*
    Experimentation
*)

df.Columns.[ [ "Date"; "Label"; "Amount" ] ]
|> Frame.rows
|> Series.observations
|> Seq.iter(fun v -> printfn "%A" v)

df
|> Frame.groupRowsUsing (fun _ c -> 
    let date = c.GetAs<DateTime>("Date")
    date.Year, date.Month)
|> Frame.nest
|> Series.mapValues (fun frame -> frame.RowCount, frame |> Stats.sum |> Series.get "Amount")
|> Series.observations
|> Seq.iter(fun v -> printfn "%A" v)

(*
    Call from Shared library
*)
df
|> ExpenseDataFrame.GetNumberTransactionsAndSumPerMonth
|> Seq.iter(fun x -> printfn "%A" x)