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
    
    type Endpoint =
    | ExpensesPerMonth
    | ExpensesPerCategory

    let route = 
        RouteMap.Create 
            (function
             | ExpensesPerMonth -> [ "per-month" ]
             | _ -> [ "per-category" ])
            (function
             | [ "per-month" ] -> ExpensesPerMonth
             | _ -> ExpensesPerCategory)
        |> RouteMap.Install

    let nav = 
        Nav.Doc(
            "Expenses", 
            "List design", 
            [ Nav.Category.Doc("Expenses", 
                [ aAttr [ on.click(fun _ _ -> route.Value <- ExpensesPerMonth) ] [ text "Per month" ]
                  aAttr [ on.click(fun _ _ -> route.Value <- ExpensesPerCategory) ] [ text "Per cateogry" ] ])
              Nav.Category.Doc("Dates", [ a [ text "1" ]; a [ text "2" ] ]) ])

    let main =
        Doc.BindView 
            (function
             | ExpensesPerMonth -> ExpensesPerMonth.Client.page
             | _ -> ExpensesPerCategory.Client.page)
            route.View