namespace London.Web.Pages

open System
open System.IO
open London.Core
open Deedle

[<AutoOpen>]
module Common =
    let expenses =
        Directory.GetFiles("data","*.csv")
        |> ExpenseDataFrame.FromFile 
        |> ExpenseDataFrame.GetFrame