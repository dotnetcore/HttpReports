 
var httpreports = {};  
httpreports.chart_theme = "macarons";
httpreports.theme = "light";
httpreports.index_chart_color = "#333333"; 

initTheme();   

function InitTimeRange() {

    var btn = $(".timeSelect").find(".btn").eq(0);

    timeChange(btn, 15);
} 


function timeChange(k, minute) {  

    var start = FormatDateToString(new Date(new Date().setMinutes(new Date().getMinutes() - minute)));
    var end = FormatDateToString(new Date());  

    $(".start").val(start);
    $(".end").val(end); 

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

}  

function FormatDateToString(d) {

    var dateStr = PrefixInteger((d.getFullYear()),4) + "-" +
        PrefixInteger((d.getMonth() + 1),2) + "-" +
        PrefixInteger((d.getDate()),2) + " " +
        PrefixInteger((d.getHours()),2) + ":" +
        PrefixInteger((d.getMinutes()),2) + ":" +
        PrefixInteger((d.getSeconds()),2); 

    return dateStr;
}

function PrefixInteger(num,length) {
    return (Array(length).join('0') + num).slice(-length);
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

function ClearTimeRange() {

    $(".timeSelect").find(".btn").each(function (k, item) {

        if ($(item).hasClass("btn-info")) {

            $(item).removeClass("btn-info");

            $(item).addClass("btn-default");
        }

    });

}


function ChangeTheme(item) { 

    var key = $(item).attr("data-key");

    localStorage.setItem("httpreports.theme", key); 

    location.reload(); 

} 



function ChangeLanguage(item) {

    var language = $(item).attr("data-key");

    $.ajax({
        url: "/HttpReportsData/ChangeLanguage",
        data: { language: language },
        success: function (result) {


            location.reload();
        } 
    })  
}



function initTheme() { 

    var current = localStorage.getItem("httpreports.theme");

    if (current == null || current == "" || current == undefined) {
       
        $("#theme_light").remove();

        httpreports.theme = "dark"; 
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

function show_password_modal() {   

    var username = $.cookie('HttpReports.Login.User');

    if (username == undefined || username == "") {
        location.href = "/HttpReports/UserLogin";
        return;
    }   

    $(".update_username").val(username); 

    $(".userModal").modal("show");  

}

function update_password() {  

    var username = $.cookie('HttpReports.Login.User');  
    var newUserName = $(".update_username").val().trim();
    var oldPwd = $(".update_oldpwd").val().trim();
    var newPwd = $(".update_newpwd").val().trim(); 

    if (newUserName.length == 0 || oldPwd.length == 0 || newPwd.length == 0) {
        alertError(lang.User_NotNull);
        return;  
    }  

    if (oldPwd == newPwd) {
        alertError(lang.User_OldNewPass);
        return;  
    } 

    $.ajax({

        url: "/HttpReportsData/UpdateAccountInfo",
        type: "POST",
        data: {
            username, newUserName, oldPwd, newPwd
        },
        success: function (result) {

            if (result.code == 1) {

                alertOk(lang.User_UpdateSuccess, 800, function () { 
                    location.href = "/HttpReports/UserLogout"; 
                }); 
            }
            else {
                alertError(result.msg);
            } 
        }

    })



}

