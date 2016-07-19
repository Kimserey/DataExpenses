﻿namespace London.Core
    
open System
open System.Globalization
open System.Text.RegularExpressions

[<AutoOpen>]
module Domain =
    
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
        | RentAndBills
        | Leisure
        | BankTransfer
        | Transport
        with
            override x.ToString() =
                match x with
                | DepartmentStore -> "Department store"
                | Supermarket -> "Supermarket"
                | AsianSupermarket -> "Asian supermarket"
                | Clothing -> "Clothing"
                | Restaurant -> "Restaurant"
                | Electronics -> "Electronics"
                | FastFood -> "Fast food"
                | SweetAndSavoury -> "Sweet and savoury"
                | HealthAndBeauty -> "Health and beauty"
                | Online -> "Online"
                | Cash -> "Cash"
                | Other -> "Other"
                | RentAndBills -> "Rent And Bills"
                | Leisure -> "Leisure"
                | BankTransfer -> "Bank Transfer"
                | Transport -> "Transport"

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

        (**      Regex pattern                                                          Label                                Category
                 ^^^^^^^^^^^^^                                                          ^^^^^                                ^^^^^^^^     **)
        label    ".*CASH.*"                                                             "CASH WITHDRAW"                      Cash

        >> label ".*BHS.*"                                                              "BHS"                                DepartmentStore
        >> label ".*HOUSE OF FRASER.*"                                                  "HOUSE OF FRASER"                    DepartmentStore
        >> label ".*TIGER.*"                                                            "TIGER"                              DepartmentStore
        >> label ".*(JOHN LEWIS|JOHNLEWIS).*"                                           "JOHN LEWIS"                         DepartmentStore
        >> label ".*TK MAXX.*"                                                          "TK MAXX"                            DepartmentStore
        >> label ".*TFS STORES.*"                                                       "PERFUM TFS STORES"                  DepartmentStore

        >> label ".*ALDI.*"                                                             "ALDI"                               Supermarket
        >> label ".*WAITROSE.*"                                                         "WAITROSE"                           Supermarket
        >> label ".*NISA.*"                                                             "NISA"                               Supermarket
        >> label ".*ASDA.*"                                                             "ASDA"                               Supermarket
        >> label ".*OCADO.*"                                                            "OCADO"                              Supermarket
        >> label ".*SAINSBURY.*"                                                        "SAINSBURY"                          Supermarket
        >> label ".*TESCO.*"                                                            "TESCO"                              Supermarket
        >> label ".*(M&S|MARKS & SPENCER|MARKS & SPEN|MARKS&SPENCER).*"                 "M&S"                                Supermarket
        >> label ".*WILKO.*"                                                            "WILKO"                              Supermarket
        >> label ".*POUNDLAND.*"                                                        "POUNDLAND"                              Supermarket

        >> label ".*CURRYS*"                                                            "CURRYS"                             Electronics
        >> label ".*CARPHONE*"                                                          "CARPHONE"                           Electronics

        >> label ".*BURGER KING.*"                                                      "BURGER KING"                        FastFood
        >> label ".*PIZZA HUT.*"                                                        "PIZZA HUT"                          FastFood
        >> label ".*MCDONALDS.*"                                                        "MCDONALDS"                          FastFood
        >> label ".*KFC.*"                                                              "KFC"                                FastFood

        >> label ".*SPORTSDIRECT.*"                                                     "SPORTSDIRECT"                       Clothing
        >> label ".*UNIQLO.*"                                                           "UNIQLO"                             Clothing
        >> label ".*PRIMARK.*"                                                          "PRIMARK"                            Clothing
        >> label ".*DEBENHAMS.*"                                                        "DEBENHAMS"                          Clothing
        >> label ".*MONSOON.*"                                                          "MONSOON"                            Clothing
        >> label ".*ZARA.*"                                                             "ZARA"                               Clothing

        >> label ".*SPECSAVERS.*"                                                       "SPECSAVERS"                         HealthAndBeauty
        >> label ".*THE BODY SHOP.*"                                                    "THE BODY SHOP"                      HealthAndBeauty
        >> label ".*CRABTREE AND EVELY.*"                                               "CRABTREE AND EVELYN"                HealthAndBeauty
        >> label ".*GMC PHARMACY.*"                                                     "PHARMACY"                           HealthAndBeauty
        >> label ".*REGIS INTERNATIONA.*"                                               "SUPERCUTS"                          HealthAndBeauty
        >> label ".*SUPERDRUG.*"                                                        "SUPERDRUG"                          HealthAndBeauty
        >> label ".*BOOTS.*"                                                            "BOOTS"                              HealthAndBeauty

        >> label ".*LOON FUNG.*"                                                        "LOON FUNG"                          AsianSupermarket
        >> label ".*ASIAN MARKET.*"                                                     "ASIAN MARKET"                       AsianSupermarket

        >> label ".*NAM PO.*"                                                           "NAM PO DIM SUM"                     SweetAndSavoury
        >> label ".*PRET A MANGER.*"                                                    "PRET A MANGER"                      SweetAndSavoury
        >> label ".*POTBELLY.*"                                                         "POTBELLY SANDWICH"                  SweetAndSavoury
        >> label ".*MR SIMMS OLDE SWEE.*"                                               "MR SIMMS OLDE SWEET CHOCOLATE"      SweetAndSavoury
        >> label ".*COFFEE REPUBLIC.*"                                                  "COFFEE REPUBLIC"                    SweetAndSavoury
        >> label ".*PASTY SHOP.*"                                                       "PASTY SHOP PUFF"                    SweetAndSavoury
        >> label ".*AVID.*"                                                             "AVID CHOCOLATE"                     SweetAndSavoury
        >> label ".*(EAT BLUEWATER|EAT CANARY WHARF).*"                                 "EAT"                                SweetAndSavoury
        >> label ".*LOLAS.*"                                                            "LOLAS"                              SweetAndSavoury
        >> label ".*STICKY RICE.*"                                                      "STICKY RICE"                        SweetAndSavoury
        >> label ".*CAFFE NERO.*"                                                       "CAFFE NERO"                         SweetAndSavoury
        >> label ".*ISLINGTON SUGAR.*"                                                  "SUGAR FREE BAKERY"                  SweetAndSavoury

        >> label ".*ITUNES.COM/BILL.*"                                                  "APPLE APP STORE"                    Online
        >> label ".*AMAZON.*"                                                           "AMAZON"                             Online
        >> label ".*(GOOGLE|Non-Sterling Transaction Fee).*"                            "GOOGLE ACCOUNT (OR RELATED)"        Online
        >> label ".*PAYPAL.*"                                                           "PAYPAL"                             Online
        >> label ".*NAME-CHEAP.COM.*"                                                   "NAME-CHEAP"                         Online

        >> label ".*WASABI.*"                                                           "WASABI JAP"                         Restaurant
        >> label ".*MONGOLIAN GRILL.*"                                                  "MONGOLIAN GRILL"                    Restaurant
        >> label ".*RESTAURANTS.*"                                                      "RESTAURANTS"                        Restaurant
        >> label ".*SHAKE SHACK.*"                                                      "SHAKE SHACK"                        Restaurant
        >> label ".*NANDOS.*"                                                           "NANDOS"                             Restaurant
        >> label ".*ANGUS STEAKHOUSE.*"                                                 "ANGUS STEAKHOUSE"                   Restaurant
        >> label ".*GIRAFFE.*"                                                          "GIRAFFE"                            Restaurant
        >> label ".*CAFE ROUGE.*"                                                       "CAFE ROUGE"                         Restaurant
        >> label ".*TORTILLA.*"                                                         "TORTILLA"                           Restaurant
        >> label ".*KOFFEES & KREAM.*"                                                  "KOFFEES & KREAM"                    Restaurant
        >> label ".*BUMPKIN.*"                                                          "BUMPKIN"                            Restaurant
        >> label ".*WAGAMAMA.*"                                                         "WAGAMAMA"                           Restaurant
        >> label ".*GOLD MINE.*"                                                        "GOLD MINE"                          Restaurant
        >> label ".*THE BREAKFAST CLUB.*"                                               "THE BREAKFAST CLUB"                 Restaurant
        >> label ".*JUST EAT.*"                                                         "JUST EAT"                           Restaurant
        >> label ".*LOTUS LEAF.*"                                                       "LOTUS LEAF"                         Restaurant
        >> label ".*FRANCO MANCA .*"                                                    "FRANCO MANCA PIZZA"                 Restaurant
        >> label ".*PHO .*"                                                             "PHO VIETNAMESE"                     Restaurant
        >> label ".*RHYTHM KITCHEN.*"                                                   "RHYTHM KITCHEN"                     Restaurant
        >> label ".*ED'S EASY.*"                                                        "ED'S EASY DINER"                    Restaurant
    
        >> label ".*PET SUPPLIES.*"                                                     "PET SUPPLIES GIFT"                  Other
        >> label ".*AUDIBLE.*"                                                          "AUDIBLE"                            Other
        >> label ".*POST OFFICE.*"                                                      "POST OFFICE"                        Other
        >> label ".*DISNEY STORE.*"                                                     "DISNEY STORE"                       Other
        >> label ".*EURO LIVERPOOL.*"                                                   "EEA"                                Other
        >> label ".*THE GENTRY BARBER.*"                                                "THE GENTRY BARBER"                  Other

        >> label ".*GIFFGAFF.*"                                                         "GIFFGAFF"                           RentAndBills
        >> label ".*HYPEROPTIC.*"                                                       "HYPEROPTIC"                         RentAndBills
        >> label ".*FOXTONS.*"                                                          "FOXTONS"                            RentAndBills
        >> label ".*SPARK.*"                                                            "SPARK"                              RentAndBills
        >> label ".*(LONDON BOR NEWHAM|LONDON BOROUGH OF LONDON).*"                     "COUNCIL TAX"                        RentAndBills

        >> label ".*(LUL TICKET MACHINE|DLR).*"                                         "UNDERGROUND / DLR"                  Transport
        >> label ".*(LONDON & SOUTH EAS|GWR BURNHAM TO BURHAM|GREATER ANGLIA).*"        "RAILWAY"                            Transport
        
        >> label ".*(CINEMA|EVERYMAN|VUE).*"                                            "CINEMA"                             Leisure
        >> label ".*(NRGGYM|HARLANDS).*"                                                "NRGGYM"                             Leisure

        >> label "^[A-Z0-9]{8}\sGB[A-Z0-9]{14}\s"                                       "FUND TRANSFER (OR RELATED)"         BankTransfer
        >> label ".*SEREY PAYMENT.*"                                                    "FUND TRANSFER (OR RELATED)"         BankTransfer
