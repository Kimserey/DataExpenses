namespace London.Web.Pages

open System
open London.Core
open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.JavaScript
open London.Web.Templates    

[<JavaScript; AutoOpen>]
module Common =   
    type Expense with
        static member ToTableRow x =
            CardTable.Row.Doc (x.Date.ToLongDateString(), x.Label, string x.Amount, x.Category)

    [<Inline "parseFloat($1).toFixed($0)">]
    let parseFloat (d: int) (x: float) = X<float>

    [<Direct "simpleUI.toggleSideMenu()">]
    let toggleSideMenu() = X<unit>