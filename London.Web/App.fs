namespace London.Web

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html 
open London.Web.Pages

[<JavaScript>]
module App =
    let nav = 
        Nav.Doc(
            "Expenses", 
            "List design", 
            [ Nav.Category.Doc("Expenses", [ a [ text "1" ]; a [ text "2" ] ])
              Nav.Category.Doc("Dates", [ a [ text "1" ]; a [ text "2" ] ]) ])

    let main =
        ExpensesPerMonth.Client.page