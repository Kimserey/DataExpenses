# DataExpenses

A data expenses manager which takes .CSV as input and categorize expenses.

## Why am I doing this?

There are a lot of expense managers already.
Those are centered around money earned versus money spent.
While it is great for tracking expenses and managing budgets, it isn't great in correcting expense behaviours in long term.
A deeper analysis must be done to discover habits and behaviours in individuals.

I believe that the best way to analyze your expenses is to do it yourself.
In order to create good analysis, one needs to be intimate with the data.
You must know the data and understand it clearly which requires a lot of time.
Fortunately, I am intimate with my own expenses therefore I am creating this application with the goal of finding patterns and behaviours which I can correct for long term savings!

## 1. Expenses breakdowns

### 1.1 Monthly breakdown

![month](https://raw.githubusercontent.com/Kimserey/DataExpenses/master/img/month.png)
![month drilldown](https://raw.githubusercontent.com/Kimserey/DataExpenses/master/img/month_dd.png)

### 1.2 Category breakdown

![category](https://raw.githubusercontent.com/Kimserey/DataExpenses/master/img/category.png)
![category drilldown](https://raw.githubusercontent.com/Kimserey/DataExpenses/master/img/category_dd.png)

### 1.3 Shop breakdown

![shop](https://raw.githubusercontent.com/Kimserey/DataExpenses/master/img/shop.png)
![shop drilldown](https://raw.githubusercontent.com/Kimserey/DataExpenses/master/img/shop_dd.png)

## 2. Analysis

### 2.1 Monitor expenses over time

View your expenses stacked over time.

![bar](https://raw.githubusercontent.com/Kimserey/DataExpenses/master/img/bar.png)

### 2.2 Monitor the evolution of your expense on a certain category with expending average

Expending average calculates the average from inception till x.

![spline](https://raw.githubusercontent.com/Kimserey/DataExpenses/master/img/spline.png)

### 2.3 Visualize amount spent against day intervals

_Experiment -_ Used to see trends with the hypotheses that one will spend less frequently after paying a higher amount.

![scatter](https://raw.githubusercontent.com/Kimserey/DataExpenses/master/img/scatter.png)

### 2.4 Monitor the expense ratio of each category

Ratio of expenses per month for each category.

![pie](https://raw.githubusercontent.com/Kimserey/DataExpenses/master/img/pie.png)

### 2.5 Drilldown to the expenses

Lookup for a particular expense through the list.

![table](https://raw.githubusercontent.com/Kimserey/DataExpenses/master/img/table.png)

## 3. Predictions

In development...

# Stack

Built with:
 - F# (of course :))
 - WebSharper
 - Deedle
 - Highcharts
 - SimpleUI - my UI framework [https://github.com/Kimserey/SimpleUI](https://github.com/Kimserey/SimpleUI)

Related:
 - [A primer on manipulating data frame with Deedle](https://kimsereyblog.blogspot.co.uk/2016/04/a-primer-on-manipulating-data-frame.html)
