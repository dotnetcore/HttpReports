
var httpreports = {};  
httpreports.chart_theme = "macarons";
httpreports.theme = "light";

initTheme();  

function timeChange(k) {

    $(k).parent().find("button").each(function (i, item) {

        if ($(item).hasClass("btn-info")) {

            $(item).removeClass("btn-info");

            $(item).addClass("btn-default");
        }
    });


    if (!$(k).hasClass("btn-info")) {

        $(k).removeClass("btn-default");

        $(k).addClass("btn-info");

    }

    var tag = $(k).attr("data-id");

    $.ajax({
        url: "/HttpReportsData/GetTimeRange?Tag=" + tag,
        success: function (result) {

            $(".start").val(result.data.start);

            $(".end").val(result.data.end);

            QueryClick();

        }
    });

} 


//选择服务节点
function check_node(item) {

    if ($(item).hasClass("btn-info")) {
        $(item).removeClass("btn-info");
        $(item).addClass("btn-default");
    }
    else {
        $(item).removeClass("btn-default");
        $(item).addClass("btn-info");
    }
}


function ChangeTheme(item) { 

    var key = $(item).attr("data-key");

    localStorage.setItem("httpreports.theme", key);  


    location.reload(); 

} 

function initTheme() { 

    var current = localStorage.getItem("httpreports.theme");

    if (current == null || current == "" || current == undefined) {
       
        $("#theme_dark").remove();

        httpreports.theme = "light"; 
    }
    else { 

        httpreports.theme = current;
 
        httpreports.chart_theme = current == "light" ? "macarons" : "dark";

        if (current == "light") { $("#theme_dark").remove(); } 
        if (current == "dark") { $("#theme_light").remove(); }  

    }    

    if (httpreports.theme == "light") {

        httpreports.index_chart_color = "#333333";
        httpreports.index_chart_backgroundbar = "#99CCFF";
    }
    else {  
        httpreports.index_chart_color = "#FFFFFF";  
        httpreports.index_chart_backgroundbar = "#336699";

    }   

} 

