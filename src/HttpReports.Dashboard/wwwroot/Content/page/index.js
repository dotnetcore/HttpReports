  
GoTop();  

function InitPage() {

    laydate.render({ elem: '.start', theme: '#67c2ef' });
    laydate.render({ elem: '.end', theme: '#67c2ef' });
    laydate.render({ elem: '.day', theme: '#67c2ef' });   
  
}  

InitPage();


var global = {};

global.top = localStorage.getItem("TopCount") == null ? 10 : localStorage.getItem("TopCount");

InitChart();

getTopCount(); 

QueryClick();

 

function getTopCount() {

    var top = localStorage.getItem("TopCount");

    if (top == null || top == "") {
        return;
    }

    $(".topCount").val(top);
}

function changeTopCount(item) {

    localStorage.setItem("TopCount", $(item).val());

    location.reload();

}

//初始化百度Echart
function InitChart() { 

    // 状态码
    global.StatusCodePie = echarts.init(document.getElementById('StatusCodePie'),httpreports.chart_theme);

    global.StatusCodePieOption = {
        title: {
            text: '请求状态码',
            subtext: "",
            x: "left",
            y: "2%"
        },
        legend: {
            orient: 'vertical',
            left: 'right',
            data: []
        },
        tooltip: {
            trigger: 'item',
            formatter: "{a} <br />{b} : {c} ({d}%)"
        },
        series: [
            {
                name: '状态码',
                type: 'pie',
                radius: '55%',
                center: ['50%', '60%'],
                data: [],
                itemStyle: {
                    emphasis: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                }
            }
        ]
    };

    global.StatusCodePie.setOption(global.StatusCodePieOption);

    global.StatusCodePie.showLoading("default", {
        text: '',
        color: '#FFF',
        textColor: '#FFF',
        maskColor: 'rgba(0, 0, 0, 0.01)',
        zlevel: 0
    });



    // 处理时间
    global.ResponseTimePie = echarts.init(document.getElementById('ResponseTimePie'), 'macarons');

    global.ResponseTimePieOption = {
        title: {
            text: '请求处理时间(ms)',
            x: "left",
            y: "2%"
        },
        legend: {
            orient: 'vertical',
            left: 'right',
            data: []
        },
        tooltip: {
            trigger: 'item',
            formatter: "{a} <br />{b} ms : {c}次 <br /> {d}%"
        },
        series: [
            {
                name: '处理时间',
                type: 'pie',
                radius: '55%',
                center: ['50%', '60%'],
                data: [],
                itemStyle: {
                    emphasis: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                }
            }
        ]
    };

    global.ResponseTimePie.setOption(global.ResponseTimePieOption);

    global.ResponseTimePie.showLoading("default", {
        text: '',
        color: '#FFF',
        textColor: '#FFF',
        maskColor: 'rgba(0, 0, 0, 0.01)',
        zlevel: 0
    });

   



    global.MostRequestChart = echarts.init(document.getElementById('MostRequestChart'), 'macarons');

    global.MostRequestChartOption = {
        title: {
            text: '最多请求 TOP' + global.top,
            x: "left",
            y: "2%"
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        grid: {
            left: '3%',
            right: '3%',
            top: '15%',
            bottom: '5%',
            containLabel: true
        },
        xAxis: {
            type: 'value',
            position: "top",
            boundaryGap: [0, 0.01]
        },
        yAxis: {
            type: 'category',
            data: [],
            axisTick: { show: false },
            xisLine: { show: false },
            axisLabel: {
                textStyle: {
                    fontSize: 12,
                    align: "right",
                    fontWeight: "bolder"
                }
            },
        },
        series: [
            {
                name: '请求次数',
                type: 'bar',
                barWidth: 12,
                itemStyle: {
                    color: "#87CEFA"
                },
                data: []
            }
        ]
    };

    global.MostRequestChart.setOption(global.MostRequestChartOption);



    global.Code500RequestChart = echarts.init(document.getElementById('Code500RequestChart'), 'macarons');

    global.Code500RequestChartOption = {
        title: {
            text: '请求错误率 TOP' + global.top,
            x: "left",
            y: "2%"
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        grid: {
            left: '3%',
            right: '3%',
            top: '15%',
            bottom: '5%',
            containLabel: true
        },
        xAxis: {
            type: 'value',
            position: "top",
            boundaryGap: [0, 0.01]
        },
        yAxis: {
            type: 'category',
            data: [],
            axisTick: { show: false },
            xisLine: { show: false },

            axisLabel: {
                textStyle: {
                    fontSize: 12,
                    align: "right",
                    fontWeight: "bolder"
                }
            },
        },
        series: [
            {
                name: '请求次数',
                type: 'bar',
                barWidth: 12,
                itemStyle: {
                    color: "#F08080"
                },
                data: []
            }
        ]
    };

    global.Code500RequestChart.setOption(global.Code500RequestChartOption);




    global.FastARTChart = echarts.init(document.getElementById('FastARTChart'), 'macarons');

    global.FastARTChartOption = {
        title: {
            text: '平均处理时间最快 TOP' + global.top,
            x: "left",
            y: "2%"
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        grid: {
            left: '3%',
            right: '3%',
            top: '15%',
            bottom: '5%',
            containLabel: true
        },
        xAxis: {
            type: 'value',
            position: "top",
            boundaryGap: [0, 0.01]
        },
        yAxis: {
            type: 'category',
            data: [],
            axisTick: { show: false },
            xisLine: { show: false },

            axisLabel: {
                textStyle: {
                    fontSize: 12,
                    align: "right",
                    fontWeight: "bolder"
                }
            },
        },
        series: [
            {
                name: '平均处理时间',
                type: 'bar',
                barWidth: 12,
                itemStyle: {

                },
                data: []
            }
        ]
    };

    global.FastARTChart.setOption(global.FastARTChartOption);


    global.SlowARTChart = echarts.init(document.getElementById('SlowARTChart'), 'macarons');

    global.SlowARTChartOption = {
        title: {
            text: '平均处理时间最慢 TOP' + global.top,
            x: "left",
            y: "2%"
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        grid: {
            left: '3%',
            right: '3%',
            top: '15%',
            bottom: '5%',
            containLabel: true
        },
        xAxis: {
            type: 'value',
            position: "top",
            boundaryGap: [0, 0.01]
        },
        yAxis: {
            type: 'category',
            data: [],
            axisTick: { show: false },
            xisLine: { show: false },

            axisLabel: {
                textStyle: {
                    fontSize: 12,
                    align: "right",
                    fontWeight: "bolder"
                }
            },
        },
        series: [
            {
                name: '平均处理时间',
                type: 'bar',
                barWidth: 12,
                itemStyle: {
                    color: "#af91e1"
                },
                data: []
            }
        ]
    };

    global.SlowARTChart.setOption(global.SlowARTChartOption);  
   
}

//Ajax获取页面数据
function GetData() {
    $.ajax({
        url: "/Data/GetNodes",
        success: function (result) {

            $(".node-row").html("");

            $.each(result.data, function (i, item) {

                $(".node-row").append(' <button onclick="check_node(this)" style="width:120px;margin-left:20px;" class="btn btn-info">' + item + '</button>');

            });
        }
    })
}  
 

function GetTOPRequestChart() {

    var start = $(".start").val();
    var end = $(".end").val();

    var node = [];

    $(".node-row").find(".btn-info").each(function (i, item) {
        node.push($(item).text());
    });


    $.ajax({
        url: "/Data/GetTopRequest",
        type: "POST",
        data: {
            start: start,
            end: end,
            node: node.join(","),
            top: global.top
        },
        success: function (result) {


            // 最多 TOP15
            global.MostRequestChartOption.yAxis.data = [];
            global.MostRequestChartOption.series[0].data = [];

            $.each(result.data.most, function (i, item) {
                global.MostRequestChartOption.yAxis.data.push(item.url + "    ");
                global.MostRequestChartOption.series[0].data.push(item.total);
            });

            global.MostRequestChartOption.yAxis.data.reverse();
            global.MostRequestChartOption.series[0].data.reverse();

            global.MostRequestChart.setOption(global.MostRequestChartOption);
        }
    });
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


function GetIndexChartA() { 

    var start = $(".start").val();
    var end = $(".end").val();

    var node = [];

    $(".node-row").find(".btn-info").each(function (i, item) {
        node.push($(item).text());
    });


    Loading(global.StatusCodePie);
    Loading(global.ResponseTimePie);
    Loading(global.MostRequestChart);
    Loading(global.Code500RequestChart); 
    Loading(global.FastARTChart);
    Loading(global.SlowARTChart); 

    $.ajax({
        url: "/Data/GetIndexChartA",
        type: "POST",
        data: {
            start: start,
            end: end,
            node: node.join(","), 
            top: global.top
        },
        success: function (result) { 

            //=========================================
            global.StatusCodePie.hideLoading();

            global.StatusCodePieOption.series[0].data = result.data.statusCode;

            global.StatusCodePieOption.legend.data = [];

            $.each(result.data.statusCode, function (i, item) {
                global.StatusCodePieOption.legend.data.push(item.name);
            });

            global.StatusCodePie.setOption(global.StatusCodePieOption);


            //========================================= 
            global.ResponseTimePie.hideLoading();

            global.ResponseTimePieOption.series[0].data = result.data.responseTime;

            global.ResponseTimePieOption.legend.data = [];

            $.each(result.data.responseTime, function (i, item) {
                global.ResponseTimePieOption.legend.data.push(item.name);
            });

            global.ResponseTimePie.setOption(global.ResponseTimePieOption);  

            // 最多 TOP15 ================================  

            global.MostRequestChart.hideLoading();

            global.MostRequestChartOption.yAxis.data = [];
            global.MostRequestChartOption.series[0].data = [];

            $.each(result.data.topRequest, function (i, item) {
                global.MostRequestChartOption.yAxis.data.push(item.url + "    ");
                global.MostRequestChartOption.series[0].data.push(item.total);
            });

            global.MostRequestChartOption.yAxis.data.reverse();
            global.MostRequestChartOption.series[0].data.reverse();

            global.MostRequestChart.setOption(global.MostRequestChartOption);  


            // 错误率最高 TOP15 =========================== 
            global.Code500RequestChart.hideLoading();
            global.Code500RequestChartOption.yAxis.data = [];
            global.Code500RequestChartOption.series[0].data = [];

            $.each(result.data.topError500, function (i, item) {
                global.Code500RequestChartOption.yAxis.data.push(item.url + "    ");
                global.Code500RequestChartOption.series[0].data.push(item.total);
            });

            global.Code500RequestChartOption.yAxis.data.reverse();
            global.Code500RequestChartOption.series[0].data.reverse();

            global.Code500RequestChart.setOption(global.Code500RequestChartOption);   


            
            // 最快平均处理 TOP15 ============================
            global.FastARTChartOption.yAxis.data = [];
            global.FastARTChartOption.series[0].data = [];

            $.each(result.data.art.fast, function (i, item) {
                global.FastARTChartOption.yAxis.data.push(item.name + "    ");
                global.FastARTChartOption.series[0].data.push(item.value);
            });

            global.FastARTChartOption.yAxis.data.reverse();
            global.FastARTChartOption.series[0].data.reverse();

            global.FastARTChart.setOption(global.FastARTChartOption);

            global.FastARTChart.hideLoading();


            // 最慢平均处理 TOP15 ============================
            global.SlowARTChartOption.yAxis.data = [];
            global.SlowARTChartOption.series[0].data = [];

            $.each(result.data.art.slow, function (i, item) {
                global.SlowARTChartOption.yAxis.data.push(item.name + "    ");
                global.SlowARTChartOption.series[0].data.push(item.value);
            });

            global.SlowARTChartOption.yAxis.data.reverse();
            global.SlowARTChartOption.series[0].data.reverse();

            global.SlowARTChart.setOption(global.SlowARTChartOption); 

            global.SlowARTChart.hideLoading(); 

        }
    })  

}



function GetTopCode500Chart() {

    var start = $(".start").val();
    var end = $(".end").val();

    var node = [];

    $(".node-row").find(".btn-info").each(function (i, item) {
        node.push($(item).text());
    });


    $.ajax({
        url: "/Data/GetTopCode500",
        type: "POST",
        data: {
            start: start,
            end: end,
            node: node.join(","),
            top: global.top
        },
        success: function (result) {

            // 错误率最高 TOP15
            global.Code500RequestChartOption.yAxis.data = [];
            global.Code500RequestChartOption.series[0].data = [];

            $.each(result.data, function (i, item) {
                global.Code500RequestChartOption.yAxis.data.push(item.url + "    ");
                global.Code500RequestChartOption.series[0].data.push(item.total);
            });

            global.Code500RequestChartOption.yAxis.data.reverse();
            global.Code500RequestChartOption.series[0].data.reverse();

            global.Code500RequestChart.setOption(global.Code500RequestChartOption);
        }
    });




}  

// 获取首页请求状态码图表
function GetStatusCodePie() {

    var start = $(".start").val();
    var end = $(".end").val();

    var node = [];

    $(".node-row").find(".btn-info").each(function (i, item) {
        node.push($(item).text());
    });

    global.StatusCodePie.showLoading("default", {
        text: '',
        color: '#FFF',
        textColor: '#FFF',
        maskColor: 'rgba(0, 0, 0, 0.1)',
        zlevel: 0
    });

    $.ajax({
        url: "/Data/GetStatusCodePie",
        type: "POST",
        data: {
            start: start,
            end: end,
            node: node.join(",")
        },
        success: function (result) {

            global.StatusCodePie.hideLoading();

            global.StatusCodePieOption.series[0].data = result.data;

            global.StatusCodePieOption.legend.data = [];

            $.each(result.data, function (i, item) {
                global.StatusCodePieOption.legend.data.push(item.name);
            });

            global.StatusCodePie.setOption(global.StatusCodePieOption);
        }
    })

}

// 获取首页请求平均时间图表
function GetResponseTimePie() {

    var start = $(".start").val();
    var end = $(".end").val();

    var node = [];

    $(".node-row").find(".btn-info").each(function (i, item) {
        node.push($(item).text());
    });

    global.ResponseTimePie.showLoading("default", {
        text: '',
        color: '#FFF',
        textColor: '#FFF',
        maskColor: 'rgba(0, 0, 0, 0.01)',
        zlevel: 0
    });

    $.ajax({
        url: "/Data/GetResponseTimePie",
        type: "POST",
        data: {
            start: start,
            end: end,
            node: node.join(",")
        },
        success: function (result) {

            global.ResponseTimePie.hideLoading();

            global.ResponseTimePieOption.series[0].data = result.data;

            global.ResponseTimePieOption.legend.data = [];

            $.each(result.data, function (i, item) {
                global.ResponseTimePieOption.legend.data.push(item.name);
            });

            global.ResponseTimePie.setOption(global.ResponseTimePieOption);

        }
    })
}


// 获取首页面板数据
function GetBoardData() {   

    $(".board-row").busyLoad("show", {
        color: "#2baae8",
        fontawesome: "fa fa-spinner fa-spin fa-3x fa-fw",
        background: "rgba(0,0,0,0)",
    });

    var start = $(".start").val();
    var end = $(".end").val();

    var node = [];

    $(".node-row").find(".btn-info").each(function (i, item) {
        node.push($(item).text());
    });

    $.ajax({
        url: "/Data/GetIndexData",
        type: "POST",
        data: {
            start: start,
            end: end,
            node: node.join(",")
        },
        success: function (result) {   


            $(".board-row").busyLoad("hide");

            $(".board-row").find("span").eq(0).text(result.data.total);
            $(".board-row").find("span").eq(1).text(result.data.art);
            $(".board-row").find("span").eq(2).text(result.data.code404);
            $(".board-row").find("span").eq(3).text(result.data.code500);
            $(".board-row").find("span").eq(4).text(result.data.errorPercent);
            $(".board-row").find("span").eq(5).text(result.data.apiCount); 
        }
    });
}


function GetARTChart() {

    var start = $(".start").val();
    var end = $(".end").val();

    var node = [];

    $(".node-row").find(".btn-info").each(function (i, item) {
        node.push($(item).text());
    });


    $.ajax({
        url: "/Data/GetTOPART",
        type: "POST",
        data: {
            start: start,
            end: end,
            node: node.join(","),
            top: global.top
        }, 
        success: function (result) {

            // 最快平均处理 TOP15
            global.FastARTChartOption.yAxis.data = [];
            global.FastARTChartOption.series[0].data = [];

            $.each(result.data.fast, function (i, item) {
                global.FastARTChartOption.yAxis.data.push(item.name + "    ");
                global.FastARTChartOption.series[0].data.push(item.value);
            });

            global.FastARTChartOption.yAxis.data.reverse();
            global.FastARTChartOption.series[0].data.reverse();

            global.FastARTChart.setOption(global.FastARTChartOption);


            // 最慢平均处理 TOP15
            global.SlowARTChartOption.yAxis.data = [];
            global.SlowARTChartOption.series[0].data = [];

            $.each(result.data.slow, function (i, item) {
                global.SlowARTChartOption.yAxis.data.push(item.name + "    ");
                global.SlowARTChartOption.series[0].data.push(item.value);
            });

            global.SlowARTChartOption.yAxis.data.reverse();
            global.SlowARTChartOption.series[0].data.reverse();

            global.SlowARTChart.setOption(global.SlowARTChartOption);

        }
    });




}


// 查询按钮点击
function QueryClick() {   

    ReSetTag();

    GetBoardData();  

    GetIndexChartA();   

    //ReSetTag();  
    ////GetStatusCodePie();
    ////GetResponseTimePie();

    //GetBoardData();  

    //GetARTChart(); 

    ////GetTopCode500Chart();
    ////GetTOPRequestChart();

    ////GetLatelyChart(); 

}

function ReSetTag() {  


    var start = $(".start").val();
    var end = $(".end").val();

    var tag = $(".board-row").find("p").find("b");

    if (start.length == 0 && end.length == 0) { 
        tag.text('今天');
    }
    else {
        tag.text((start.length > 0 ? start.substr(0, 10) : "null") + " - " + (end.length > 0 ? end.substr(0, 10) : "null"));
    } 

    var tagId = $(".timeSelect").find(".btn-info").attr("data-id");   

    var tagValue = tagId == undefined ? 0 : tagId;   

    $.ajax({
        url: "/Data/GetTimeTag",
        type: "POST",
        data: {
            start: start,
            end: end,
            tagValue: tagValue
        },
        success: function (result) {  

            if (result.data == -1) {
                return;
            }   

            $(".timeSelect").find("button").each(function (i, item) { 

                var tag = $(item).attr("data-id"); 

                if (tag == result.data) {

                    if ($(item).hasClass("btn-default")) {  

                        $(item).removeClass("btn-default");
                        $(item).addClass("btn-info");
                    }   
                }
                else {

                    if ($(item).hasClass("btn-info")) {

                        $(item).removeClass("btn-info");
                        $(item).addClass("btn-default");
                    }  
                }  

            }); 

        }     
    });  
}




//全选
function select_all(item) {

    $(item).parent().next().find("button").each(function (i, k) {

        if ($(k).hasClass("btn-default")) {
            $(k).removeClass("btn-default");
            $(k).addClass("btn-info");
        }
    });
}

//反选
function select_reverse(item) {

    $(item).parent().next().find("button").each(function (i, k) {

        if ($(k).hasClass("btn-info")) {
            $(k).removeClass("btn-info");
            $(k).addClass("btn-default");
        }
        else {
            $(k).removeClass("btn-default");
            $(k).addClass("btn-info");
        }

    });
}  




 