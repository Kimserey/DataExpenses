namespace London.Web

open WebSharper
open WebSharper.Resources

module Resources =
    
    type Style() =
        inherit BaseResource("style.css")

    [<assembly:System.Web.UI.WebResource("style.css", "text/css")>]
    [<assembly:Require(typeof<Style>)>]
    do()