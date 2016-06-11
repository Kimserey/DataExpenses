namespace London.Web

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

[<JavaScript; AutoOpen>]
module Templates =    
    type Card  = Templating.Template<"Templates/template-card.html">
    type Nav   = Templating.Template<"Templates/template-nav.html">
    type Table = Templating.Template<"Templates/template-expenses-table.html">