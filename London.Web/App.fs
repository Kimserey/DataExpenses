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
            View.Map (function ExpensesPerMonth -> "Expenses per month" | _ -> "Expenses per category") route.View, 
            "Data expenses", 
            [ Nav.Category.Doc("Expenses", 
                [ aAttr 
                    [ attr.href ""
                      on.click(fun _ _ -> 
                        route.Value <- ExpensesPerMonth
                        toggleSideMenu()) ] 
                    [ text "Per month" ]

                  aAttr 
                    [ attr.href ""
                      on.click(fun _ _ -> 
                        route.Value <- ExpensesPerCategory
                        toggleSideMenu()) ]
                    [ text "Per cateogry" ] ]) ])

    let main =
        Doc.BindView 
            (function
             | ExpensesPerMonth -> ExpensesPerMonth.Client.page
             | _ -> ExpensesPerCategory.Client.page)
            route.View