namespace London.Web.Pages

open System
open System.IO
open WebSharper
open WebSharper.Remoting
open London.Core
open London.Core.Dataframe
open London.Web.Pages.Common
open Deedle

module LabelsPerMonth =
    module Rpcs =
        [<Rpc>]
        let get(): Async<_> =
            expenses
            |> ExpenseDataFrame.GetLabelsPerMonth
            |> async.Return
                
    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next
        open WebSharper.UI.Next.Html
        open WebSharper.UI.Next.Client
        open WebSharper.JavaScript
        open WebSharper.JQuery
        open London.Web.Templates
        
        let page =
            async {
                let! labels = Rpcs.get()
                return labels
                        |> List.mapi (fun cardIndex (Title date, labels) ->
                            Card.Doc 
                                [ CardList.Doc(
                                       date,
                                       "-",
                                       labels
                                       |> List.mapi (fun contentIndex (Title label, Sum sum) ->
                                            CardList.Item.Doc(
                                                "card-" + string cardIndex + "-content-" + string contentIndex,
                                                label,
                                                sum.JS.ToFixed 2,
                                                []))) 
                                  
//                                  divAttr 
//                                   [ on.afterRender (fun el -> 
//                                       JQuery
//                                           .Of(el)
//                                           .LineChart(
//                                           {
//                                               Chart = { Type = "spline" }
//                                               Title = { Text = "" }
//                                               XAxis = { Categories = subCategory |> List.map (fun (Title title, _, _) -> title) |> Array.ofList }
//                                               YAxis = { Title = { Text = "Amount" } }
//                                               Series = [| { Name = "Total"; Data = subCategory |> List.map (fun (_, Sum sum, _) -> sum) |> Array.ofList } |]
//                                               Tooltip = { PointFormat = "{point.y:.2f} GBP" }
//                                           }) |> ignore) ] [] :> Doc 
                            ])
                        |> Doc.Concat
            }
            |> Doc.Async