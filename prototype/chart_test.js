/*
 For testing purposes and evaluate if the chart brings value
*/

$.getJSON('http://172.16.81.128:9600/api/monthlyexpandingsum', 
    function (data) {
        function makeChart(chartData) {
            return {
                chart: {            
                    type: 'spline'
                },
                title: {
                    text: chartData.Category
                },
                tooltip: {
                    headerFormat: '<span style="color:{point.color}">\u25CF</span>{series.name}:<br />',
                    pointFormat: '<span>{point.date}: </span><b>{point.y} GBP</b><br/>'
                },
                plotOptions: {
                    spline: {
                        marker: {
                            enabled: false
                        }
                    }
                },
                series: 
                    chartData
                        .ExpandedSums
                        .map(function(sum) {
                            return {
                                name: sum.MonthReadable + " " + sum.Year,
                                data: 
                                    sum.Values
                                       .map(function (value) {
                                           return {
                                               date: new Date(value.Date).toDateString(),
                                               y: value.Value
                                           };
                                       })
                            };
                        })
            };
        }

        data.forEach(function(data) {
            $('<div style="width:50%; float:left;">')
                .highcharts(makeChart(data))
                .appendTo($('#expandedsum'));
        });
    }
);


$.getJSON('http://172.16.81.128:9600/api/expenses',
    function(data) {
        $('#expenses').highcharts({
            chart: {
                type: 'column',
                zoomType: 'xy'
            },
            title: {
                text: data.Title
            },
            xAxis: {
                categories: data.Labels
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Total amount'
                }
            },
            legend: {
                align: 'right',
                x: -30,
                verticalAlign: 'top',
                y: 25,
                floating: true,
                borderColor: '#CCC',
                borderWidth: 1,
                shadow: false
            },
            tooltip: {
                pointFormat: '{series.name}: {point.y} GBP<br/>Total: {point.stackTotal} GBP'
            },
            plotOptions: {
                column: {
                    stacking: 'normal'
                }
            },
            series: 
                data.DataSeriesList
                    .map(function(series) {
                        return {
                            name: series.Title,
                            data: series.Values
                        };
                    })
        });
    }
);

$.getJSON('http://172.16.81.128:9600/api/supermarket',
    function(data) {
        $('#supermarket').highcharts({
            chart: {
                type: "spline",
                zoomType: 'xy'
            },
            title: {
                text: data.Title
            },
            xAxis: {
                categories: data.Labels
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Amount'
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }]
            },
            series: data.DataSeriesList.map(function(series) {
                return {
                    name: series.Title,
                    data: series.Values.map(function(expense) { 
                        return Math.abs(expense.Amount); 
                    })
                };    
            })
        });
    }
);

$.getJSON('http://172.16.81.128:9600/api/smoothsupermarket',
    function(data) {
        $('#smoothsupermarket').highcharts({
            chart: {
                type: 'spline',
                zoomType: 'xy'
            },
            title: {
                text: data.Title
            },
            xAxis: {
                categories: data.Labels
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Amount'
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }]
            },
            series: data.DataSeriesList.map(function(series) {
                return {
                    name: series.Title,
                    data: series.Values.map(function(expense) { 
                        return Math.abs(expense.Amount); 
                    })
                };    
            })
        });
    }
);

$.getJSON('http://172.16.81.128:9600/api/dayspan',
    function(data) {
        $('#dayspan').highcharts({
            chart: {
                type: 'scatter',
                zoomType: 'xy'
            },
            title: {
                text: 'Day span with amount spent - Supermarket'
            },
            xAxis: {
                title: {
                    text: 'Amount'
                }
            },
            yAxis: {
                title: {
                    text: 'Day span'
                }
            },
            plotOptions: {
                scatter: {
                    marker: {
                        radius: 5,
                        states: {
                            hover: {
                                enabled: true,
                                lineColor: 'rgb(100,100,100)'
                            }
                        }
                    },
                    states: {
                        hover: {
                            marker: {
                                enabled: false
                            }
                        }
                    },
                    tooltip: {
                        pointFormat: '{point.x} GBP, {point.y} days'
                    }
                }
            },
            series: [{
                name: 'Expenses',
                color: 'rgba(223, 83, 83, .5)',
                data: data
            }]
        });
    }
);

$.getJSON('http://172.16.81.128:9600/api/binaryexpenses', 
    function (data) {
        $('#binaryexpenses').highcharts({
            title: {
                text: 'Supermarket - Expense'
            },
            plotOptions: {
                line: {
                    marker: {
                        enabled: false
                    }
                }
            },
            xAxis: {
                categories: data.map(function(x) { return (new Date(x[0])).toDateString(); })
            },
            yAxis: {
              min: 0,
              max: 1
            },
            series: [{
                name: 'Supermarket expense',
                data: data.map(function(x) { return x[2]; }),
                step: true
            }]
        });
    }
);
$.getJSON('http://172.16.81.128:9600/api/expanding', 
    function (data) {
        $('#expandingmean').highcharts({
            chart: {
                type: 'spline',
                zoomType: 'xy'
            },
            title: {
                text: 'Supermarket - Expanding mean'
            },
            xAxis: {
                categories: data.map(function(d) { return new Date(d[0]).toDateString(); })
            },
            yAxis: {
                title: {
                    text: 'Amount'
                }
            },
            series: [{
                name: 'Supermarket',
                data: data.map(function(d) { return d[1]; })
            }]
        });
    }
);

$.getJSON('http://172.16.81.128:9600/api/ratio', 
    function (data) {
        function makeChart(chartData) {
            return {
                chart: {            
                    type: 'pie'
                },
                title: {
                    text: chartData[0]
                },
                tooltip: {
                    pointFormat: '<b>{point.percentage:.4f}%</b>'
                },
                plotOptions: {
                    pie: {
                        dataLabels: {
                            enabled: true,
                            format: '<b>{point.name}</b>: {point.percentage:.2f} %'
                        }
                    }
                },
                series: [{
                    data: chartData[1]
                            .map(function(category) {
                                return {
                                    name: category[0],
                                    y: category[1]
                                };
                            })
                }]
            };
        }

        data.forEach(function(data) {
            $('<div style="width:50%; float:left;">')
                .highcharts(makeChart(data))
                .appendTo($('#ratios'));
        });
    }
);

$.getJSON('http://172.16.81.128:9600/api/transactions',
    function(data) {
        $('#transactions').highcharts({
            chart: {
                type: 'scatter',
                zoomType: 'xy'
            },
            title: {
                text: 'Transactions versus amount spent for each month'
            },
            xAxis: {
                title: {
                    text: 'Number of transactions'
                }
            },
            yAxis: {
                title: {
                    text: 'Total amount spent'
                }
            },
            plotOptions: {
                scatter: {
                    marker: {
                        radius: 5,
                        states: {
                            hover: {
                                enabled: true,
                                lineColor: 'rgb(100,100,100)'
                            }
                        }
                    },
                    states: {
                        hover: {
                            marker: {
                                enabled: false
                            }
                        }
                    },
                    tooltip: {
                        pointFormat: '{point.x} transactions, {point.y} GBP'
                    }
                }
            },
            series: [{
                name: 'Expenses',
                color: 'rgba(223, 83, 83, .5)',
                data: data
            }]
        });
    }
);