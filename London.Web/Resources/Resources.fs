namespace London.Web

open WebSharper
open WebSharper.Resources

module Resources =

    type Fontawesome() =
        inherit BaseResource("https://use.fontawesome.com/269e7d57ca.js")
        
    type Highcharts() =
        inherit BaseResource("https://code.highcharts.com/highcharts.js")
        
    [<assembly:Require(typeof<Fontawesome>);
      assembly:Require(typeof<Highcharts>);
      assembly:System.Web.UI.WebResource("SimpleUI.css", "text/css");
      assembly:System.Web.UI.WebResource("SimpleUI.js", "text/javascript");
      assembly:System.Web.UI.WebResource("Prescript.js", "text/javascript");
      assembly:System.Web.UI.WebResource("Postscript.js", "text/javascript")>]
    do()