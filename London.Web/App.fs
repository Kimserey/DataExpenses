namespace London.Web

open WebSharper
open WebSharper.UI.Next.Html 
open WebSharper.UI.Next.Server
open London.Web.Pages

module App =

    module Rpcs =
        open London.Core
    
        [<Rpc>]
        let refreshDataframe(): Async<unit> =
            Dataframe.agent.Refresh None
            |> async.Return
            
    
    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next
        open WebSharper.UI.Next.Client
        open WebSharper.UI.Next.Html 
    
        type Endpoint =
        | Expenses
        | ExpensesPerMonth
        | ExpensesPerCategory
        | LabelsPerMonth
            with
                override x.ToString() =
                    match x with 
                    | Expenses -> "Expenses"
                    | ExpensesPerMonth -> "Expenses per month"
                    | ExpensesPerCategory -> "Expenses per category"
                    | LabelsPerMonth -> "Labels per month"
                    
                static member Page x =
                    match x with
                    | Expenses -> Expenses.Client.page
                    | ExpensesPerMonth -> ExpensesPerMonth.Client.page
                    | ExpensesPerCategory -> ExpensesPerCategory.Client.page
                    | LabelsPerMonth -> LabelsPerMonth.Client.page
    
                static member Route x =
                    match x with
                    | Expenses            -> [ "expenses" ]
                    | ExpensesPerMonth    -> [ "per-month" ]
                    | ExpensesPerCategory -> [ "per-category" ]
                    | LabelsPerMonth      -> [ "per-labels" ]
    
                static member ReverseRoute x =
                    match x with
                    | [ "expenses" ]  -> Expenses
                    | [ "per-month" ] -> ExpensesPerMonth
                    | [ "per-labels" ] -> LabelsPerMonth
                    | [ "per-category" ] -> ExpensesPerCategory
                    | _ -> ExpensesPerMonth
    
    
        let route = 
            RouteMap.Create Endpoint.Route Endpoint.ReverseRoute
            |> RouteMap.Install
    
        let nav = 
            let link title endpoint =
                aAttr [ attr.href ""; on.click(fun _ e -> e.PreventDefault(); Var.Set route endpoint; toggleSideMenu()) ] [ text title ]
            
            Nav.Doc(
                View.Map string route.View, 
                "Ldn expenses", 
                [ Nav.Category.Doc("fa-area-chart", "Expenses",
                    [ link "All" Expenses
                      link "Per month" ExpensesPerMonth
                      link "Per category" ExpensesPerCategory
                      link "Per label" LabelsPerMonth ]) ],
                fun _ _ -> Rpcs.refreshDataframe() |> Async.Start)
    
        let main =
            Doc.BindView Endpoint.Page route.View