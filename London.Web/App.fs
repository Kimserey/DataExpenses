namespace London.Web

open WebSharper
open WebSharper.UI.Next.Html 
open WebSharper.UI.Next.Server
open London.Web.Pages

[<JavaScript>]
module App =
    open WebSharper.JavaScript
    open WebSharper.JQuery
    open WebSharper.UI.Next
    open WebSharper.UI.Next.Client
    open WebSharper.UI.Next.Html 

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


    let route = 
        RouteMap.Create Endpoint.Route Endpoint.ReverseRoute
        |> RouteMap.Install

    let nav = 
        let link title endpoint =
            aAttr [ attr.href ""; on.click(fun _ _ -> Var.Set route endpoint; toggleSideMenu()) ] [ text title ]

        Nav.Doc(
            View.Map string route.View, 
            "London expenses", 
            [ Nav.Category.Doc("Expenses", 
                [ link "All" Expenses
                  link "Per month" ExpensesPerMonth
                  link "Per category" ExpensesPerCategory ]) ])

    let main =
        Doc.BindView Endpoint.Page route.View