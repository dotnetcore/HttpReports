 
var global = {};

InitChart(); 

GetDayChart(); 

//GCCountChart();

GetLatelyChart(); 

function InitTimeSelect() {


    var start = FormatDateToString(new Date(new Date().setMinutes(new Date().getMinutes() - 10)));
    var end = FormatDateToString(new Date());

    $(".start").val(start);
    $(".end").val(end);   

    if (lang.LanguageFormat == "en-us") {

        laydate.render({ elem: '.start', theme: '#67c2ef', type: 'datetime', ready: ClearTimeRange(), lang: 'en' });
        laydate.render({ elem: '.end', theme: '#67c2ef', type: 'datetime', ready: ClearTimeRange(), lang: 'en' });
        laydate.render({ elem: '.day', theme: '#67c2ef', lang: 'en' });

    }


    if (lang.LanguageFormat == "zh-cn") {

        laydate.render({ elem: '.start', theme: '#67c2ef', type: 'datetime', ready: ClearTimeRange() });
        laydate.render({ elem: '.end', theme: '#67c2ef', type: 'datetime', ready: ClearTimeRange() });
        laydate.render({ elem: '.day', theme: '#67c2ef' });

    }

} 

InitTimeSelect();

 
function QueryClick(item) {

    var service = $(".service-form").find(".service").find("select").val();
    var instance = $(".service-form").find(".instance").find("select").val();  

    if (service.length == 0 || service == "ALL") {

        alertError(lang.Monitor_MustSelectNode);
        return;

    } 
   

    GetPerformanceChart();  
}

function GetPerformanceChart() {

    var service = $(".service-form").find(".service").find("select").val();
    var instance = $(".service-form").find(".instance").find("select").val();  
    var format = $(".timeFormat-form").find(".timeFormat").find("select").val();   
    var start = $(".start").val();
    var end = $(".end").val();  

    if (start.length == 0 || end.length == 0) { 
        alertError(lang.TimeNotNull)
        return;
    } 

    //Loading(global.MinuteStateTimesBar);
    //Loading(global.MinuteStateAvgBar);

    $.ajax({
        url: "/HttpReportsData/GetPerformanceChart",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify({
            service: service,
            instance: instance,
            TimeFormat: format,
            start: start,
            end: end
        }), 
        success: function (result) { 

            if (result.code != 1) {
                alertError(result.msg)
                return;
            }

            console.log(result)   

            //reset
            global.GCCountChartOption.xAxis.data = [];
            global.GCCountChartOption.series[0].data = [];
            global.GCCountChartOption.series[1].data = [];
            global.GCCountChartOption.series[2].data = [];  

            $.each(result.data, function (i, item) { 

                global.GCCountChartOption.xAxis.data.push(item.id);
                global.GCCountChartOption.series[0].data.push(item.gcGen0);
                global.GCCountChartOption.series[1].data.push(item.gcGen1); 
                global.GCCountChartOption.series[2].data.push(item.gcGen2); 

            }); 


            global.GCCountChart.setOption(global.GCCountChartOption); 


        }
    })

}

function InitChart() { 
     
    // GCChart
    global.GCCountChart = echarts.init(document.getElementById('GCCountChart'), 'macarons'); 

    global.GCCountChartOption =   {
        color: ['#af91e1'],
        tooltip: {},
        legend: {
            data: ['Gen0', 'Gen1', 'Gen2'],
            textStyle: {
                fontSize: 12,
                color: '#F1F1F3'
            }
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        title: {
            text: "GC",
            x: "left",
            y: "2%"
        },
        xAxis: {
            data: [],
            boundaryGap: true
        },
        yAxis: {},
        series: [{
            name: 'Gen0',
            type: 'line',
            smooth: true,
            symbol: 'circle',
            symbolSize: 5,
            showSymbol: false,
            lineStyle: {
                normal: {
                    width: 1
                }
            },
            areaStyle: {
                normal: {
                    color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{
                        offset: 0,
                        color: 'rgba(137, 189, 27, 0.3)'
                    }, {
                        offset: 0.8,
                        color: 'rgba(137, 189, 27, 0.1)'
                    }], false),
                    shadowColor: 'rgba(0, 0, 0, 0.3)',
                    shadowBlur: 10
                }
            },
            itemStyle: {
                normal: {
                    color: 'rgb(137,189,27)',
                    borderColor: 'rgba(137,189,2,0.27)',
                    borderWidth: 12

                }
            },
            data: []
        }, {
            name: 'Gen1',
            type: 'line',
            smooth: true,
            symbol: 'circle',
            symbolSize: 5,
            showSymbol: false,
            lineStyle: {
                normal: {
                    width: 1
                }
            },
            areaStyle: {
                normal: {
                    color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{
                        offset: 0,
                        color: 'rgba(0, 136, 212, 0.3)'
                    }, {
                        offset: 0.8,
                        color: 'rgba(0, 136, 212, 0.1)'
                    }], false),
                    shadowColor: 'rgba(0, 0, 0, 0.1)',
                    shadowBlur: 10
                }
            },
            itemStyle: {
                normal: {
                    color: 'rgb(0,136,212)',
                    borderColor: 'rgba(0,136,212,0.2)',
                    borderWidth: 12

                }
            },
            data: []
        }, {
            name: 'Gen2',
            type: 'line',
            smooth: true,
            symbol: 'circle',
            symbolSize: 5,
            showSymbol: false,
            lineStyle: {
                normal: {
                    width: 1
                }
            },
            areaStyle: {
                normal: {
                    color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{
                        offset: 0,
                        color: 'rgba(219, 50, 51, 0.3)'
                    }, {
                        offset: 0.8,
                        color: 'rgba(219, 50, 51, 0.1)'
                    }], false),
                    shadowColor: 'rgba(0, 0, 0, 0.3)',
                    shadowBlur: 10
                }
            },
            itemStyle: {
                normal: {

                    color: 'rgb(219,50,51)',
                    borderColor: 'rgba(219,50,51,0.2)',
                    borderWidth: 12
                }
            },
            data: []
        }]
    };

    global.GCCountChart.setOption(global.GCCountChartOption);   

     
    // Hour
    global.DayStateTimesBar = echarts.init(document.getElementById('DayStateTimesBar'), 'macarons');

    global.DayStateTimesBarOption = {
        tooltip: {},
        color:"#67c2ef",
        legend: {
            data: []
        },
        grid: {
            left: '7%',
            right: '3%'
        },
        title: {
            text: lang.Trend_HourTotalCount,
            x: "left",
            y: "2%"
        },
        xAxis: {
            data: []
        },
        yAxis: {},
        series: [{
            type: 'line',
            name: lang.Index_RequestCount,
            data: []
        }]
    };

    global.DayStateTimesBar.setOption(global.DayStateTimesBarOption); 
   
  
    global.DayStateAvgBar = echarts.init(document.getElementById('DayStateAvgBar'), 'macarons');

    global.DayStateAvgBarOption = {
        color: ['#af91e1'],
        tooltip: {},
        legend: {
            data: []
        },
        grid: {
            left: '7%',
            right: '3%'
        },
        title: {
            text: lang.Trend_HourAvgTime,
            x: "left",
            y: "2%"
        },
        xAxis: {
            data: []
        },
        yAxis: {},
        series: [{
            type: 'line',
            name: lang.ProcessingTime2,
            data: []
        }]
    };

    global.DayStateAvgBar.setOption(global.DayStateAvgBarOption);  

    // Day
    global.LatelyDayChart = echarts.init(document.getElementById('LatelyDayChart'), 'macarons');

    global.LatelyDayChartOption = {
        tooltip: {},
        legend: {
            data: [lang.Trend_DayTotalCount]
        },
        grid: {
            left: '7%',
            top: '20%',
            right: '3%'
        },
        title: {
            text: lang.Trend_DayTotalCount,
            x: "left",
            y: "2%",
            subtext: ""
        },
        xAxis: {
            data: []
        },
        yAxis: {},
        series: [{
            type: 'bar',
            name: lang.Index_RequestCount,
            data: []
        }]
    };

    global.LatelyDayChart.setOption(global.LatelyDayChartOption);   

} 

function GetLatelyChart() { 

    var service = $(".service-form").find(".service").find("select").val();
    var instance = $(".service-form").find(".instance").find("select").val();  

    Loading(global.LatelyDayChart); 

    $.ajax({
        url: "/HttpReportsData/GetLatelyDayChart",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            service: service,
            instance:instance
        }), 
        success: function (result) {
             
            global.LatelyDayChartOption.xAxis.data = result.data.time;
            global.LatelyDayChartOption.series[0].data = result.data.value;
            global.LatelyDayChart.setOption(global.LatelyDayChartOption);

            global.LatelyDayChart.hideLoading();

        }
    })

} 

function GetDayChart() {

    var service = $(".service-form").find(".service").find("select").val();
    var instance = $(".service-form").find(".instance").find("select").val();  

    Loading(global.DayStateTimesBar); 
    Loading(global.DayStateAvgBar); 

    $.ajax({
        url: "/HttpReportsData/GetDayStateBar",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            service: service,
            instance: instance
        }), 
        success: function (result) {

            // 24 小时请求次数
            global.DayStateTimesBar.hideLoading();
            global.DayStateTimesBarOption.xAxis.data = result.data.hours;
            global.DayStateTimesBarOption.series[0].data = result.data.timesList;
            global.DayStateTimesBar.setOption(global.DayStateTimesBarOption);

            global.DayStateTimesBar.hideLoading();


            // 24小时请求平均时长
            global.DayStateAvgBar.hideLoading();
            global.DayStateAvgBarOption.xAxis.data = result.data.hours;
            global.DayStateAvgBarOption.series[0].data = result.data.avgList;
            global.DayStateAvgBar.setOption(global.DayStateAvgBarOption);

            global.DayStateAvgBar.hideLoading();
        }
    })

}

function Loading(item) {

    item.showLoading("default", {
        text: 'loading',
        color: '#FFF',
        textColor: '#FFF',
        maskColor: 'rgba(0, 0, 0, 0.1)',
        zlevel: 0
    });
}