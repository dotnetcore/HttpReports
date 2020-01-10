
laydate.render({ elem: '.month', type: 'month' });
laydate.render({ elem: '.year', type: 'year' });
laydate.render({ elem: '.day' });

function QueryClick(item) {

    GetDayChart();

    GetLatelyChart();

    GetLatelyMonthChart();

}


var global = {};

InitChart(); 

GetDayChart(); 

GetLatelyChart(); 

GetLatelyMonthChart();

function InitChart() { 

    // 24小时请求次数
    global.DayStateTimesBar = echarts.init(document.getElementById('DayStateTimesBar'), 'macarons');

    global.DayStateTimesBarOption = {
        tooltip: {},
        color:"#67c2ef",
        legend: {
            data: ['请求次数']
        },
        grid: {
            left: '7%',
            right: '3%'
        },
        title: {
            text: '每小时请求次数 ',
            x: "left",
            y: "2%"
        },
        xAxis: {
            data: []
        },
        yAxis: {},
        series: [{
            type: 'line',
            name: "请求次数",
            data: []
        }]
    };

    global.DayStateTimesBar.setOption(global.DayStateTimesBarOption); 
   
    // 24小时平均处理时间 ms
    global.DayStateAvgBar = echarts.init(document.getElementById('DayStateAvgBar'), 'macarons');

    global.DayStateAvgBarOption = {
        color: ['#af91e1'],
        tooltip: {},
        legend: {
            data: ['处理时间']
        },
        grid: {
            left: '7%',
            right: '3%'
        },
        title: {
            text: '每小时平均处理时间  ms',
            x: "left",
            y: "2%"
        },
        xAxis: {
            data: []
        },
        yAxis: {},
        series: [{
            type: 'line',
            name: "处理时间",
            data: []
        }]
    };

    global.DayStateAvgBar.setOption(global.DayStateAvgBarOption); 


    // 每天请求数量
    global.LatelyDayChart = echarts.init(document.getElementById('LatelyDayChart'), 'macarons');

    global.LatelyDayChartOption = {
        tooltip: {},
        legend: {
            data: ['请求次数（天）']
        },
        grid: {
            left: '7%',
            top: '20%',
            right: '3%'
        },
        title: {
            text: '请求次数（天） ',
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
            name: "请求次数",
            data: []
        }]
    };

    global.LatelyDayChart.setOption(global.LatelyDayChartOption);  

    // 每月请求数量
    global.LatelyMonthChart = echarts.init(document.getElementById('LatelyMonthChart'), 'macarons');

    global.LatelyMonthChartOption = {
        color:"#af91e1",
        tooltip: {},
        legend: {
            data: ['请求次数（月）']
        },
        grid: {
            left: '7%',
            top: '20%',
            right: '3%'
        },
        title: {
            text: '请求次数（月） ',
            x: "left",
            y: "2%",
            subtext: ""
        },
        xAxis: {
            data: []
        },
        yAxis: {},
        series: [{
            type: 'line',
            name: "请求次数",
            data: []
        }]
    };

    global.LatelyMonthChart.setOption(global.LatelyMonthChartOption);   

} 

function GetLatelyChart() {

    var month = $(".month").val();

    var node = []; 

    $(".node-row").find(".btn-info").each(function (i, item) {
        node.push($(item).text());
    });

    Loading(global.LatelyDayChart); 

    $.ajax({
        url: "/Data/GetLatelyDayChart",
        type: "POST",
        data: {
            month: month,
            node: node.join(",")
        },
        success: function (result) {

            global.LatelyDayChartOption.title.subtext = result.data.range;
            global.LatelyDayChartOption.xAxis.data = result.data.time;
            global.LatelyDayChartOption.series[0].data = result.data.value;
            global.LatelyDayChart.setOption(global.LatelyDayChartOption);

            global.LatelyDayChart.hideLoading();

        }
    })

}

function GetLatelyMonthChart() {

    var year = $(".year").val();

    var node = [];

    $(".node-row").find(".btn-info").each(function (i, item) {
        node.push($(item).text());
    });

    Loading(global.LatelyMonthChart);

    $.ajax({
        url: "/Data/GetMonthDataByYear",
        type: "POST",
        data: {
            year: year,
            node: node.join(",")
        },
        success: function (result) {   

            global.LatelyMonthChartOption.title.subtext = result.data.range;
            global.LatelyMonthChartOption.xAxis.data = result.data.time;
            global.LatelyMonthChartOption.series[0].data = result.data.value;
            global.LatelyMonthChart.setOption(global.LatelyMonthChartOption);

            global.LatelyMonthChart.hideLoading(); 
        }
    })

} 

function GetDayChart() {

    var node = [];

    var day = $(".day").val();

    $(".node-row").find(".btn-info").each(function (i, item) {
        node.push($(item).text());
    });

    Loading(global.DayStateTimesBar); 
    Loading(global.DayStateAvgBar); 

    $.ajax({
        url: "/Data/GetDayStateBar",
        type: "POST",
        data: {
            day: day,
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