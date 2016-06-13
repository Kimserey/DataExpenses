namespace London.Web.Pages

open System
open System.IO
open London.Core
open Deedle

[<AutoOpen>]
module Common =
    open WebSharper
    open WebSharper.JavaScript 
        
    [<Inline "parseFloat($1).toFixed($0)">]
    let parseFloat (d: int) (x: float) = X<float>

    let expenses =
        Directory.GetFiles("data","*.csv")
        |> ExpenseDataFrame.FromFile 
        |> ExpenseDataFrame.GetFrame