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
        static member ToTableRow (index: int) x =
            CardTable.Row.Doc (string index, x.Date.ToLongDateString(), x.Label, x.Amount.JS.ToFixed 2, x.Category)

    [<Direct "simpleUI.toggleSideMenu()">]
    let toggleSideMenu() = X<unit>