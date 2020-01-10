


var httpreports = {};  
httpreports.chart_theme = "macarons";

initTheme(); 

$(function () {  

    $('[data-toggle="tooltip"]').tooltip();

    tippy('.serviceTip', {
        content: "<div class='tipbox'>服务节点是WebAPI请求的服务节点，点击选中和取消节点</div>",
        arrow: true,
        size: "large",
        inertia: true,
        placement: "right"
    })  

}); 


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
        url: "/Data/GetTimeRange?Tag=" + tag,
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

    if ($(".theme").attr("href").indexOf(key) < 0) { 
        $(".theme").attr("href", "/Content/css/theme/" + key + ".css");
    }  

} 

function initTheme() { 

    var current = localStorage.getItem("httpreports.theme");

    if (current == null || current == "" || current == undefined) {
       
        $("#theme_dark").remove();
        httpreports.theme = "light";
    }
    else {

        httpreports.chart_theme = current == "light" ? "macarons" : "dark";

        if (current == "light") { $("#theme_dark").remove(); } 
        if (current == "dark") { $("#theme_light").remove(); }  

    }  
}

