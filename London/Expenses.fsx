#I __SOURCE_DIRECTORY__
#load "../packages/Deedle/Deedle.fsx"

open System
open System.IO
open System.Globalization
open System.Text.RegularExpressions
open Deedle


[<AutoOpen>]
module Common =

    type Expense = {
        Date: DateTime
        Title: string
        Amount: decimal
    }

    type Category =
        | DepartmentStore
        | Supermarket
        | AsianSupermarket
        | Clothing
        | Restaurant
        | Electronics
        | FastFood
        | SweetAndSavoury
        | HealthAndBeauty
        | Online
        | Cash
        | Other
        with
            override x.ToString() = sprintf "%A" x

    let printTitle title =
        printfn "\n%s:\n" title

    let monthToString mth =
        CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mth)
       
(**
    Label stores
    ------------
    Label expenses with a Store name when Title match regex containing store name
    Will be useful to aggregate cost per store
**)
let labelStore =
    let label regex label category (str, initialCategory) =
        if Regex.IsMatch(str, regex, RegexOptions.IgnoreCase) 
        then (label, category)
        else (str, initialCategory)

    (**      Regex pattern                                          Label                                Category
             ^^^^^^^^^^^^^                                          ^^^^^                                ^^^^^^^^     **)
    label    ".*CASH.*"                                             "CASH WITHDRAW"                      Cash

    >> label ".*BHS.*"                                              "BHS"                                DepartmentStore
    >> label ".*HOUSE OF FRASER.*"                                  "HOUSE OF FRASER"                    DepartmentStore
    >> label ".*TIGER.*"                                            "TIGER"                              DepartmentStore
    >> label ".*(JOHN LEWIS|JOHNLEWIS).*"                           "JOHN LEWIS"                         DepartmentStore
    >> label ".*TK MAXX.*"                                          "TK MAXX"                            DepartmentStore

    >> label ".*ALDI.*"                                             "ALDI"                               Supermarket
    >> label ".*WAITROSE.*"                                         "WAITROSE"                           Supermarket
    >> label ".*NISA.*"                                             "NISA"                               Supermarket
    >> label ".*ASDA.*"                                             "ASDA"                               Supermarket
    >> label ".*SAINSBURYS.*"                                       "SAINSBURYS"                         Supermarket
    >> label ".*TESCO.*"                                            "TESCO"                              Supermarket
    >> label ".*(M&S|MARKS & SPENCER|MARKS & SPEN|MARKS&SPENCER).*" "M&S"                                Supermarket
    >> label ".*WILKO.*"                                            "WILKO"                              Supermarket

    >> label ".*CURRYS*"                                            "CURRYS"                             Electronics

    >> label ".*BURGER KING.*"                                      "BURGER KING"                        FastFood
    >> label ".*PIZZA HUT.*"                                        "PIZZA HUT"                          FastFood
    >> label ".*MCDONALDS.*"                                        "MCDONALDS"                          FastFood

    >> label ".*SPORTSDIRECT.*"                                     "SPORTSDIRECT"                       Clothing
    >> label ".*UNIQLO.*"                                           "UNIQLO"                             Clothing
    >> label ".*PRIMARK.*"                                          "PRIMARK"                            Clothing
    >> label ".*DEBENHAMS.*"                                        "DEBENHAMS"                          Clothing

    >> label ".*SPECSAVERS.*"                                       "SPECSAVERS"                         HealthAndBeauty
    >> label ".*THE BODY SHOP.*"                                    "THE BODY SHOP"                      HealthAndBeauty
    >> label ".*CRABTREE AND EVELY.*"                               "CRABTREE AND EVELYN"                HealthAndBeauty
    >> label ".*GMC PHARMACY.*"                                     "PHARMACY"                           HealthAndBeauty
    >> label ".*REGIS INTERNATIONA.*"                               "SUPERCUTS"                          HealthAndBeauty
    >> label ".*SUPERDRUG.*"                                        "SUPERDRUG"                          HealthAndBeauty
    >> label ".*BOOTS.*"                                            "BOOTS"                              HealthAndBeauty

    >> label ".*LOON FUNG.*"                                        "LOON FUNG"                          AsianSupermarket
    >> label ".*ASIAN MARKET.*"                                     "ASIAN MARKET"                       AsianSupermarket

    >> label ".*NAM PO.*"                                           "NAM PO DIM SUM"                     SweetAndSavoury
    >> label ".*PRET A MANGER.*"                                    "PRET A MANGER"                      SweetAndSavoury
    >> label ".*POTBELLY.*"                                         "POTBELLY SANDWICH"                  SweetAndSavoury
    >> label ".*MR SIMMS OLDE SWEE.*"                               "MR SIMMS OLDE SWEET CHOCOLATE"      SweetAndSavoury
    >> label ".*COFFEE REPUBLIC.*"                                  "COFFEE REPUBLIC"                    SweetAndSavoury
    >> label ".*PASTY SHOP.*"                                       "PASTY SHOP PUFF"                    SweetAndSavoury
    >> label ".*AVID.*"                                             "AVID CHOCOLATE"                     SweetAndSavoury
    >> label ".*EAT.*"                                              "EAT"                                SweetAndSavoury
    >> label ".*LOLAS.*"                                            "LOLAS"                              SweetAndSavoury

    >> label ".*ITUNES.COM/BILL.*"                                  "APPLE APP STORE"                    Online
    >> label ".*AMAZON.*"                                           "AMAZON"                             Online
    >> label ".*(GOOGLE|Non-Sterling Transaction Fee).*"            "GOOGLE ACCOUNT (OR RELATED)"        Online
    >> label ".*PAYPAL.*"                                           "PAYPAL"                             Online

    >> label ".*WASABI.*"                                           "WASABI JAP"                         Restaurant

    >> label ".*PET SUPPLIES.*"                                     "PET SUPPLIES GIFT"                  Other
    >> label ".*AUDIBLE.*"                                          "AUDIBLE"                            Other
    >> label ".*POST OFFICE.*"                                      "POST OFFICE"                        Other
    >> label ".*DISNEY STORE.*"                                     "DISNEY STORE"                       Other
    >> label ".*(NRGGYM|HARLANDS).*"                                "NRGGYM"                             Other
    >> label ".*(LUL TICKET MACHINE|DLR).*"                         "UNDERGROUND / DLR"                  Other
    >> label "^[A-Z0-9]{8}\sGB[A-Z0-9]{14}\s"                       "FUND TRANSFER (OR RELATED)"         Other
    >> label ".*(LONDON & SOUTH EAS|GWR BURNHAM TO BURHAM).*"       "RAILWAY"                            Other
    >> label ".*EURO LIVERPOOL.*"                                   "EEA"                                Other
    >> label ".*FOXTONS.*"                                          "FOXTONS"                            Other
    >> label ".*GIFFGAFF.*"                                         "GIFFGAFF"                           Other

(** 
    Script boot up, massage and label data
    --------------------------------------
    - Set environment as current file script location
    - Load all data from .csv files into Expenses (assuming no duplicate in .csv)
    - Load all data to a dataframe
    - Label the stores
**)

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let df = 
    let frame =
        Directory.GetFiles(Environment.CurrentDirectory + "/data","*.csv")
        |> Array.map (fun path -> Frame.ReadCsv(path, hasHeaders = false))
        |> Array.map (fun df -> df |> Frame.indexColsWith [ "Date"; "Title"; "Amount" ])
        |> Array.map (fun df -> df.GetRows())
        |> Seq.collect (fun s -> s.Values)
        |> Seq.map (fun s -> s?Date, s?Title, s?Amount)
        |> Seq.map (fun (date, title, amount) -> 
            { Date = string date |> DateTime.Parse
              Title = string title
              Amount = string amount |> decimal })
        |> Frame.ofRecords

    frame.AddColumn(
        "Label", 
        frame
        |> Frame.getCol "Title" 
        |> Series.mapValues ((fun title -> (title, Other)) >> labelStore >> fst))
    frame.AddColumn(
        "Category", 
        frame 
        |> Frame.getCol "Title" 
        |> Series.mapValues ((fun title -> (title, Other)) >> labelStore >> snd >> string))
    frame

(** 
    All expenses - pretty display
    -----------------------------
    October
      28/10/2015              SOMETHING     -35.40
      26/10/2015         SOMETHING ELSE     -24.03
    November
      30/11/2015    SOMETHING SOMETHING    -73.43
      02/11/2015        SOMETHING AGAIN    -192.50
**)
df
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month)
|> Frame.nest
|> Series.observations
|> Seq.iter (fun (m, df) ->
    printfn "%s" (monthToString m)
    df
    |> Frame.rows
    |> Series.observations
    |> Seq.iter (fun (_, s) -> 
        printfn "  %s %50s %10.2f" 
            (s.GetAs<DateTime>("Date").ToShortDateString()) 
            (s.GetAs<string>("Title"))
            s?Amount))


(**
    Total monthly expenses - pretty display
    ---------------------------------------
    - Group by month number
    - Get numeric column (amount)
    - Execute Sum on the first level (monthly group level)
    - Take the single value of the observations and take the data from it
    - Map month number to month name

    Output:
      October : -69.43
      November : -198.72
**)
df
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month)
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum fst)
|> Series.observations
|> Seq.head
|> snd
|> Series.map(fun k t -> monthToString k, t)
|> Series.observations
|> Seq.iter (fun (_, (month, amount)) -> printfn "%10s : %.2f" month amount)


(**
    Top three monthly expenses - frame
    ----------------------------------
    - Group by the month number
    - Execute operation on Nested frame
    - Transform from observations to Seq to manipulate the data
    - Transform back to observations and unest the frame to get back original format
**)
df
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month)
|> Frame.nest
|> Series.observations
|> Seq.map (fun (k, df) -> 
    (k, 
     df 
     |> Frame.sortRows "Amount"
     |> Frame.take 3))
|> Series.ofObservations
|> Frame.unnest

(** 
    Top three montly expenses - pretty display
    ------------------------------------------
    October
      28/10/2015              SOMETHING     -35.40
      26/10/2015         SOMETHING ELSE     -24.03
    November
      30/11/2015    SOMETHING SOMETHING    -73.43
      02/11/2015        SOMETHING AGAIN    -192.50
**)
df
|> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month)
|> Frame.nest
|> Series.observations
|> Seq.map (fun (k, df) -> 
    (monthToString k, 
     df 
     |> Frame.sortRows "Amount"
     |> Frame.take 3))
|> Seq.iter (fun (m, df) ->
    printfn "%s" m
    df
    |> Frame.rows
    |> Series.observations
    |> Seq.iter (fun (_, s) -> 
        printfn "  %s %50s %10.2f" 
            (s.GetAs<DateTime>("Date").ToShortDateString()) 
            (s.GetAs<string>("Title"))
            s?Amount))

(**
    All sorted expenses - pretty display
    ------------------------------------
    01/12/2015              SOMETHING    -51.00
    09/02/2016    SOMETHING SOMETHING    -30.00
    13/01/2016        AGAIN SOMETHING     -4.00
**)
df
|> Frame.rows
|> Series.sortBy(fun s -> s?Amount)
|> Series.observations
|> Seq.iter (fun (_, s) -> 
    printfn "%s %50s %10.2f" 
        (s.GetAs<DateTime>("Date").ToShortDateString()) 
        (s.GetAs<string>("Title"))
        s?Amount)

(**
    Grouped by title sorted expenses - pretty display
    -------------------------------------------------
    - Dropped all columns except Title and Amount (Prepare for group by)
    - Group by Title
    - Get numeric columns (useful for the compiler to know that all columns are float type)
    - Run a sum on the first level (at this stage, the frame is indexed on two levels - Title - Id)

    Output:
                SOMETHING    -35.57
      SOMETHING SOMETHING    -29.99
          AGAIN SOMETHING    -29.00
**)
df.Columns.[ [ "Title"; "Amount" ] ]
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsByString("Title")
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum fst)
|> Series.observations
|> Seq.collect (fun (_, s) -> s |> Series.observations)
|> Seq.sortBy snd
|> Seq.iter (fun (title, amount) -> printfn "%50s %10.2f" title amount)

(**
    Grouped by title sorted expenses by name - pretty display
    ---------------------------------------------------------
    - Dropped all columns except Title and Amount (Prepare for group by)
    - Group by Title
    - Get numeric columns (useful for the compiler to know that all columns are float type)
    - Run a sum on the first level (at this stage, the frame is indexed on two levels - Title - Id)

    Output:
                SOMETHING    -35.57
      SOMETHING SOMETHING    -29.99
          AGAIN SOMETHING    -29.00
**)
df.Columns.[ [ "Title"; "Amount" ] ]
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsByString("Title")
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum fst)
|> Series.observations
|> Seq.collect (fun (_, s) -> s |> Series.observations)
|> Seq.sortBy fst
|> Seq.iter (fun (title, amount) -> printfn "%50s %10.2f" title amount)


(**
    Grouped by labeled title sorted expenses - pretty display
    ---------------------------------------------------------
    - Map Title column to change Title to store name
    - Group by Title
    - Apply sum on first level

    Output:
                SOMETHING    -35.57
      SOMETHING SOMETHING    -29.99
          AGAIN SOMETHING    -29.00
**)
df
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsByString "Label"
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum fst)
|> Series.observations
|> Seq.collect (snd >> Series.observations)
|> Seq.sortBy snd
|> Seq.iter (fun (title, amount) -> 
    printfn "%50s %10.2f" title amount)

(**
    Grouped by labeled title per month - pretty display
    ---------------------------------------------------
    - Group by Label
    - Group by Month
    - Get Amount column
    - Level sum by flattening the keys and summing using Month and Label

    Output:
    February
                SOMETHING    -35.57
      SOMETHING SOMETHING    -29.99
          AGAIN SOMETHING    -29.00
    Total: -661.08 GBP
    -----------------------------------------------------------------
    March
                SOMETHING    -35.57
      SOMETHING SOMETHING    -29.99
          AGAIN SOMETHING    -29.00
    Total: -661.08 GBP
    -----------------------------------------------------------------
**)
df.Columns.[ [ "Date"; "Label"; "Amount" ]]
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsByString "Label"
|> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month)
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum (Pair.flatten3 >> Pair.get1And2Of3))
|> Series.observations
|> Seq.collect (snd >> Series.observations)
|> Seq.map (fun ((month, title), amount) -> month, title, amount)
|> Seq.groupBy (fun (month, _, _) -> month)
|> Seq.iter (fun (month, values) ->
    printfn "%s" (monthToString month)
    values
    |> Seq.sortBy (fun (_, _, amount) -> amount)
    |> Seq.iter (fun (_, title, amount) ->
        printfn "%50s %10.2f" title amount)
    printfn "Total: %.2f GBP" (values |> Seq.sumBy (fun (_, _, amount) -> amount))
    printfn "-----------------------------------------------------------------")

(**
    Grouped by category per month - pretty display
    ---------------------------------------------------
    - Group by category
    - Group by Month
    - Get Amount column
    - Level sum by flattening the keys and summing using Month and Label

    Output:
    February
                SOMETHING    -35.57
      SOMETHING SOMETHING    -29.99
          AGAIN SOMETHING    -29.00
    Total: -661.08 GBP
    -----------------------------------------------------------------
    March
                SOMETHING    -35.57
      SOMETHING SOMETHING    -29.99
          AGAIN SOMETHING    -29.00
    Total: -661.08 GBP
    -----------------------------------------------------------------
**)
df.Columns.[ [ "Date"; "Category"; "Amount" ]]
|> Frame.filterRowValues(fun c -> c?Amount < 0.)
|> Frame.groupRowsByString "Category"
|> Frame.groupRowsUsing(fun _ c -> c.GetAs<DateTime>("Date").Month)
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelSum (Pair.flatten3 >> Pair.get1And2Of3))
|> Series.observations
|> Seq.collect (snd >> Series.observations)
|> Seq.map (fun ((month, title), amount) -> month, title, amount)
|> Seq.groupBy (fun (month, _, _) -> month)
|> Seq.iter (fun (month, values) ->
    printfn "%s" (monthToString month)
    values
    |> Seq.sortBy (fun (_, _, amount) -> amount)
    |> Seq.iter (fun (_, title, amount) ->
        printfn "%50s %10.2f" title amount)
    printfn "Total: %.2f GBP" (values |> Seq.sumBy (fun (_, _, amount) -> amount))
    printfn "-----------------------------------------------------------------")
