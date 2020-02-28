
var httpreports = {};  
httpreports.chart_theme = "macarons";
httpreports.theme = "light";
httpreports.index_chart_color = "#333333";

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
        alertError("不能为空");
        return;  
    }  

    if (oldPwd == newPwd) {
        alertError("新旧密码不能一样");
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

                alertOk("修改成功",800,function () { 
                    location.href = "/HttpReports/UserLogout"; 
                }); 
            }
            else {
                alertError(result.msg);
            } 
        }

    })



}

