namespace London.Web

open WebSharper
open WebSharper.Resources

module Resources =

    type Fontawesome() =
        inherit BaseResource("https://use.fontawesome.com/269e7d57ca.js")
        
    [<assembly:Require(typeof<Fontawesome>);
      assembly:System.Web.UI.WebResource("SimpleUI.css", "text/css");
      assembly:System.Web.UI.WebResource("SimpleUI.js", "text/javascript")>]
    do()