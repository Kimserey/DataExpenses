namespace London.Web

open WebSharper
open WebSharper.Resources

module Resources =
    [<assembly:System.Web.UI.WebResource("style.css", "text/css")>]
    do()