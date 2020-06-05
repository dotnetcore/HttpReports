
var global = {};

InitTimeSelect();

InitChart();

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


$(function () {

    $(".timeFormat-form").find(".timeFormat").on('loaded.bs.select', (e, clickedIndex, isSelected, previousValue) => {

        GetPerformanceChart();

    });

});


function QueryClick(item) {

    GetPerformanceChart();
}

function GetPerformanceChart() {

    var service = $(".service-form").find(".service").find("select").val();
    var instance = $(".service-form").find(".instance").find("select").val();
    var format = $(".timeFormat-form").find(".timeFormat").find("select").val();
    var start = $(".start").val();
    var end = $(".end").val();

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

      
            //reset
            global.GCCountChartOption.xAxis.data = [];
            global.GCCountChartOption.series[0].data = [];
            global.GCCountChartOption.series[1].data = [];
            global.GCCountChartOption.series[2].data = [];

            global.HeapMemoryChartOption.xAxis.data = [];
            global.HeapMemoryChartOption.series[0].data = []; 

            global.ProcessCPUChartOption.xAxis.data = [];
            global.ProcessCPUChartOption.series[0].data = [];  

            global.ProcessMemoryChartOption.xAxis.data = [];
            global.ProcessMemoryChartOption.series[0].data = []; 

            global.ThreadCountChartOption.xAxis.data = [];
            global.ThreadCountChartOption.series[0].data = [];  

            $.each(result.data, function (i, item) {

                global.GCCountChartOption.xAxis.data.push(item.id);
                global.GCCountChartOption.series[0].data.push(item.gcGen0);
                global.GCCountChartOption.series[1].data.push(item.gcGen1);
                global.GCCountChartOption.series[2].data.push(item.gcGen2);


                global.HeapMemoryChartOption.xAxis.data.push(item.id)
                global.HeapMemoryChartOption.series[0].data.push(item.heapMemory); 

                global.ProcessCPUChartOption.xAxis.data.push(item.id)
                global.ProcessCPUChartOption.series[0].data.push(item.processCPU); 

                global.ProcessMemoryChartOption.xAxis.data.push(item.id)
                global.ProcessMemoryChartOption.series[0].data.push(item.processMemory); 

                global.ThreadCountChartOption.xAxis.data.push(item.id)
                global.ThreadCountChartOption.series[0].data.push(item.threadCount); 

            }); 

            global.GCCountChart.setOption(global.GCCountChartOption); 
            global.HeapMemoryChart.setOption(global.HeapMemoryChartOption);  
            global.ProcessCPUChart.setOption(global.ProcessCPUChartOption);  
            global.ProcessMemoryChart.setOption(global.ProcessMemoryChartOption);  
            global.ThreadCountChart.setOption(global.ThreadCountChartOption);  

        }
    })

}

function InitChart() {

    // GCChart
    global.GCCountChart = echarts.init(document.getElementById('GCCountChart'), 'macarons');   

    global.GCCountChartOption = {
        color: ['#af91e1'],
        tooltip: {},
        legend: {
            data: ['Gen0', 'Gen1', 'Gen2'],
            textStyle: {
                fontSize: 12,
                color: httpreports.index_chart_color
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
            boundaryGap: false
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
     

    //HeapMemory
    global.HeapMemoryChart = echarts.init(document.getElementById('HeapMemoryChart'), 'macarons');  
      
    global.HeapMemoryChartOption = {
        tooltip: {},
        color: "#67c2ef",
        legend: {
            data: []
        },
        grid: {
            left: '7%',
            right: '3%'
        },
        title: {
            text: lang.HeapMemory,
            x: "left",
            y: "2%"
        },
        xAxis: {
            data: []
        },
        yAxis: {},
        series: [{
            type: 'line',
            name: lang.HeapMemory,
            data: []
        }]
    };

    global.HeapMemoryChart.setOption(global.HeapMemoryChartOption); 


    //ProcessCPUChart
    global.ProcessCPUChart = echarts.init(document.getElementById('ProcessCPUChart'), 'macarons');

    global.ProcessCPUChartOption = {
        tooltip: {},
        color: "#67c2ef",
        legend: {
            data: []
        },
        grid: {
            left: '7%',
            right: '3%'
        },
        title: {
            text: lang.ProcessCPU,
            x: "left",
            y: "2%"
        },
        xAxis: {
            data: []
        },
        yAxis: {},
        series: [{
            type: 'line',
            name: lang.ProcessCPU,
            data: []
        }]
    };

    global.ProcessCPUChart.setOption(global.ProcessCPUChartOption);   


    //ProcessMemoryChart
    global.ProcessMemoryChart = echarts.init(document.getElementById('ProcessMemoryChart'), 'macarons');

    global.ProcessMemoryChartOption = {
        tooltip: {},
        color: "#67c2ef",
        legend: {
            data: []
        },
        grid: {
            left: '7%',
            right: '3%'
        },
        title: {
            text: lang.ProcessMemory,
            x: "left",
            y: "2%"
        },
        xAxis: {
            data: []
        },
        yAxis: {},
        series: [{
            type: 'line',
            name: lang.ProcessMemory,
            data: []
        }]
    };

    global.ProcessMemoryChart.setOption(global.ProcessMemoryChartOption);   


    //ThreadCountChart
    global.ThreadCountChart = echarts.init(document.getElementById('ThreadCountChart'), 'macarons');

    global.ThreadCountChartOption = {
        tooltip: {},
        color: "#67c2ef",
        legend: {
            data: []
        },
        grid: {
            left: '7%',
            right: '3%'
        },
        title: {
            text: lang.ThreadCount,
            x: "left",
            y: "2%"
        },
        xAxis: {
            data: []
        },
        yAxis: {},
        series: [{
            type: 'line',
            name: lang.ThreadCount,
            data: []
        }]
    };

    global.ThreadCountChart.setOption(global.ThreadCountChartOption);    

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
            instance: instance
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