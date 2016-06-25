namespace London.Web

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Server
open London.Core
open London.Core.Dataframe

module Sitelet =
    
    type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/api">] Api of ApiEndPoint

    and ApiEndPoint =
        | [<EndPoint "/expenses">]            Expenses
        | [<EndPoint "/supermarket">]         Supermarket
        | [<EndPoint "/smoothsupermarket">]   SmoothSupermarket
        | [<EndPoint "/levelcounts">]         LevelCounts
        | [<EndPoint "/dayspan">]             DaySpan
        | [<EndPoint "/binaryexpenses">]      Binary
        | [<EndPoint "/expending">]           Expending
        | [<EndPoint "/ratio">]               Ratio
        | [<EndPoint "/labels">]              Labels

    let sitelet =
        Sitelet.Infer (fun ctx ->
            function
            | Home ->
                Templates.Index.Doc(
                    Title = "London expenses",
                    Nav = [ client <@ App.nav @> ],
                    Main = [ client <@ App.main @> ])
                |> Content.Page
            
            | Api Expenses          -> Api.allExpenses ctx
            | Api Supermarket       -> Api.expensesForCategory Category.Supermarket ctx
            | Api SmoothSupermarket -> Api.smoothExpensesForCategory Category.Supermarket ctx
            | Api LevelCounts       -> Api.expenseLevelsCount ctx
            | Api DaySpan           -> Api.daySpanExpenses ctx
            | Api Binary            -> Api.binaryExpenses ctx
            | Api Expending         -> Api.expendingExpenses ctx
            | Api Ratio             -> Api.categoryRatioPerMonth ctx
            | Api Labels            -> Api.labelsPerMonth ctx)