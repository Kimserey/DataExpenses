namespace London.Web

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Templating

[<JavaScript>]
module ExpensesPerMonth =
    open London.Web.Rpcs.ExpensesPerMonth

    type private Table = Template<"Templates/tpl.expenses-per-month.html">

    let page() =
        View.Const ()
        |> View.MapAsync Rpcs.ExpensesPerMonth.get
        |> Doc.BindView (fun expenses ->
            expenses
            |> Map.toList
            |> List.sortByDescending(fun ((Month (name, m), Year y), _) -> m + y * 100)
            |> List.map (fun ((Month (name, m), Year y), v) -> 
                Table.Doc(name + " " + string y, v |> List.map (fun expense -> Table.row.Doc(expense.Date.ToShortDateString(), expense.Label, string expense.Amount, expense.Category))))
            |> Doc.Concat)
