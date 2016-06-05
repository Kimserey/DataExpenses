namespace London.Web

open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Server

module Site =
    
    type MainTemplate = Templating.Template<"Main.html">

    let page = Content.Page(MainTemplate.Doc(title = "Data expenses", body = [ client <@ Client.page() @> ]))

    let app = Application.SinglePage(fun _ -> page)
