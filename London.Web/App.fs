namespace London.Web

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html 
open London.Web.Pages

[<JavaScript>]
type Endpoint =
| Expenses
| ExpensesPerMonth
| ExpensesPerCategory
    with
        override x.ToString() =
            match x with 
            | Expenses -> "Expenses"
            | ExpensesPerMonth -> "Expenses per month"
            | ExpensesPerCategory -> "Expenses per category"
            
        static member Page x =
            match x with
            | Expenses -> Expenses.Client.page
            | ExpensesPerMonth -> ExpensesPerMonth.Client.page
            | ExpensesPerCategory -> ExpensesPerCategory.Client.page

        static member Route x =
            match x with
            | Expenses            -> [ "expenses" ]
            | ExpensesPerMonth    -> [ "per-month" ]
            | ExpensesPerCategory -> [ "per-category" ]

        static member ReverseRoute x =
            match x with
            | [ "expenses" ]  -> Expenses
            | [ "per-month" ] -> ExpensesPerMonth
            | [ "per-category" ]
            | _ -> ExpensesPerCategory

[<JavaScript>]
module App =

    let route = 
        RouteMap.Create Endpoint.Route Endpoint.ReverseRoute
        |> RouteMap.Install

    let nav = 
        Nav.Doc(
            View.Map string route.View, 
            "London expenses", 
            [ Nav.Category.Doc("Expenses", 
                [ aAttr 
                    [ attr.href ""
                      on.click(fun _ _ -> 
                        route.Value <- Expenses
                        toggleSideMenu()) ]
                    [ text "All" ]
                    
                  aAttr 
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
        Doc.BindView Endpoint.Page route.View