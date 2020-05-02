 
var global = {};

InitChart(); 

GetDayChart(); 

GetMinuteChart();

GetLatelyChart(); 

function QueryClick(item) {

    GetDayChart();

    GetLatelyChart();

    GetMinuteChart();
}

function GetMinuteChart() {

    var node = [];

    $(".node-row").find(".btn-info").each(function (i, item) {
        node.push($(item).text());
    });

    Loading(global.MinuteStateTimesBar);
    Loading(global.MinuteStateAvgBar);

    $.ajax({
        url: "/HttpReportsData/GetMinuteStateBar",
        type: "POST",
        data: {
            node: node.join(",")
        },
        success: function (result) {

         
            global.MinuteStateTimesBar.hideLoading();
            global.MinuteStateTimesBarOption.xAxis.data = result.data.time;
            global.MinuteStateTimesBarOption.series[0].data = result.data.timesList;
            global.MinuteStateTimesBar.setOption(global.MinuteStateTimesBarOption);

            global.MinuteStateTimesBar.hideLoading();


           
            global.MinuteStateAvgBar.hideLoading();
            global.MinuteStateAvgBarOption.xAxis.data = result.data.time;
            global.MinuteStateAvgBarOption.series[0].data = result.data.avgList;
            global.MinuteStateAvgBar.setOption(global.MinuteStateAvgBarOption);

            global.MinuteStateAvgBar.hideLoading();
        }
    })

}

function InitChart() { 
     
    // Minute
    global.MinuteStateTimesBar = echarts.init(document.getElementById('MinuteStateTimesBar'), 'macarons');

    global.MinuteStateTimesBarOption = {
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
            text: lang.Trend_MinuteTotalCount,
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

    global.MinuteStateTimesBar.setOption(global.MinuteStateTimesBarOption); 
  
    global.MinuteStateAvgBar = echarts.init(document.getElementById('MinuteStateAvgBar'), 'macarons');

    global.MinuteStateAvgBarOption =  {
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
            text: lang.Trend_MinuteAvgTime,
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

    global.MinuteStateAvgBar.setOption(global.MinuteStateAvgBarOption);   

     
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
    var node = []; 

    $(".node-row").find(".btn-info").each(function (i, item) {
        node.push($(item).text());
    });

    Loading(global.LatelyDayChart); 

    $.ajax({
        url: "/HttpReportsData/GetLatelyDayChart",
        type: "POST",
        data: { 
            node: node.join(",")
        },
        success: function (result) {
             
            global.LatelyDayChartOption.xAxis.data = result.data.time;
            global.LatelyDayChartOption.series[0].data = result.data.value;
            global.LatelyDayChart.setOption(global.LatelyDayChartOption);

            global.LatelyDayChart.hideLoading();

        }
    })

} 

function GetDayChart() {

    var node = []; 

    $(".node-row").find(".btn-info").each(function (i, item) {
        node.push($(item).text());
    });

    Loading(global.DayStateTimesBar); 
    Loading(global.DayStateAvgBar); 

    $.ajax({
        url: "/HttpReportsData/GetDayStateBar",
        type: "POST",
        data: { 
            node: node.join(",")
        },
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