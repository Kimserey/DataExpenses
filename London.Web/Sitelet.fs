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
    | [<EndPoint "/">] App
    | [<EndPoint "/api">] Api of ApiEndPoint

    and ApiEndPoint =
        | [<EndPoint "/expenses">] Expenses

    let sitelet =
        Sitelet.Infer (fun ctx ->
            function
            | App          -> App.content ctx
            | Api Expenses -> Api.content ctx)