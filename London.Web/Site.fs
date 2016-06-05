namespace London.Web

open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.Sitelets

module Site =

    type MainHtml = {
        Title : string
        Body : Doc list
    }
 
    let template =
        Content.Template<MainHtml>("~/Main.html")
            .With("title", fun x -> x.Title)
            .With("body", fun x -> x.Body)

    let page =
        Content.WithTemplate template { Title = " Data expenses"; Body = [ text "Hello" ] }

    let app = Application.SinglePage(fun _ -> page)
