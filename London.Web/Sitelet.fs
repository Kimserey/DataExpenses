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

    let sitelet dataDirectory =
        
        let expenseDataframe =
            London.Core.Dataframe.expenses dataDirectory

        Sitelet.Infer (fun ctx ->
            function
            | Home ->
                Templates.Index.Doc(
                    Title = "London expenses",
                    Nav = [ client <@ App.nav @> ],
                    Main = [ client <@ App.main @> ])
                |> Content.Page
            
            | Api Expenses          -> expenseDataframe |> Api.allExpenses ctx
            | Api Supermarket       -> expenseDataframe |> Api.expensesForCategory Category.Supermarket ctx
            | Api SmoothSupermarket -> expenseDataframe |> Api.smoothExpensesForCategory Category.Supermarket ctx
            | Api LevelCounts       -> expenseDataframe |> Api.expenseLevelsCount ctx
            | Api DaySpan           -> expenseDataframe |> Api.daySpanExpenses ctx
            | Api Binary            -> expenseDataframe |> Api.binaryExpenses ctx
            | Api Expending         -> expenseDataframe |> Api.expendingExpenses ctx
            | Api Ratio             -> expenseDataframe |> Api.categoryRatioPerMonth ctx
            | Api Labels            -> expenseDataframe |> Api.labelsPerMonth ctx)