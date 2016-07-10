namespace London.Web

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

[<JavaScript; AutoOpen>]
module Templates =    
    
    type CardTable = Templating.Template<"Templates/template-card-table.html">
    
    type CardList = Templating.Template<"Templates/template-card-list.html">
    
    type Card = Templating.Template<"Templates/template-card.html">
    
    type Tabs   = Templating.Template<"Templates/template-card-tabs.html">

    type Nav   = Templating.Template<"Templates/template-nav.html">

    type Index = Templating.Template<"Templates/index.html">