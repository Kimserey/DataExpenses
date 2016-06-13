namespace London.Web

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html 

[<JavaScript>]
module App =
    let nav = 
        Nav.Doc(
            "Expenses", 
            "List design", 
            [ Nav.Category.Doc("Expenses", [ a [ text "1" ]; a [ text "2" ] ])
              Nav.Category.Doc("Dates", [ a [ text "1" ]; a [ text "2" ] ]) ])

    let main =
        [ Card.Doc(
            Title = "April 2016",
            Items = 
                [ Card.Item.Doc(
                    ContentId = "content-31", 
                    Category = "Supermarket", 
                    Amount =  "20.00",
                    Content = 
                        [ Table.Doc
                            [ Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket") 
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")] ])
                  
                  Card.Item.Doc(
                    ContentId = "content-32", 
                    Category = "Supermarket", 
                    Amount =  "20.00",
                    Content = 
                        [ Table.Doc
                            [ Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket") 
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")] ])
                  
                  Card.Item.Doc(
                    ContentId = "content-33", 
                    Category = "Supermarket", 
                    Amount =  "20.00",
                    Content = 
                        [ Table.Doc
                            [ Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket") 
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")] ]) ])
          
          Card.Doc(
            Title = "March 2016",
            Items = 
                [ Card.Item.Doc(
                    ContentId = "content-2", 
                    Category = "Supermarket", 
                    Amount =  "20.00",
                    Content = 
                        [ Table.Doc
                            [ Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket") 
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")] ]) ])

          Card.Doc(
            Title = "February 2016",
            Items = 
                [ Card.Item.Doc(
                    ContentId = "content-1", 
                    Category = "Supermarket", 
                    Amount =  "20.00",
                    Content = 
                        [ Table.Doc
                            [ Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket") 
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")
                              Table.Row.Doc("02/01/2015", "Waitrose", "-21.01", "Supermarket")] ]) ]) ]
        |> Doc.Concat
