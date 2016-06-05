namespace London.Web

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client

[<JavaScript>]
module Client =
    
    let page() = 
        Doc.Button "Say hello " [] (fun () -> 
            async {
                let! hello = Rpcs.Hello.sayHello()
                JS.Alert hello
            } |> Async.Start)