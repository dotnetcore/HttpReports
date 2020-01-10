
GoTop();

laydate.render({ elem: '.start',  theme: '#67c2ef' });
laydate.render({ elem: '.end',  theme: '#67c2ef' }); 

InitTable();

ReSetTag();

function GetNodes() {

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


// 初始化Table
function InitTable() {

    var url = "/Data/GetRequestList";

    var size = localStorage.getItem("bootstarpSize") == null ? 20 : localStorage.getItem("bootstarpSize");

    console.log()

    $('#TableData').bootstrapTable({
        pageNumber: 1,
        pageSize: size,
        pageList: [10, 15, 20, 30, 50],
        url: url,
        queryParamsType: '',
        sidePagination: "server",
        pagination: true, 
        onLoadSuccess: function (data) { 
            $("#TableData").busyLoad("hide"); 
        },
        onPreBody: function (data) {

            $("#TableData").busyLoad("show", {
                color: "#000080",  
                fontawesome: "fa fa-spinner fa-spin fa-3x fa-fw",
                background: "rgba(0,0,0,0)",
            }); 
        },
        onPageChange: function (number, size) {   
            localStorage.setItem("bootstarpSize", size);
        },
        columns: [
            {
                field: 'id',
                title: '编号',
                align: 'center'
            },
            {
                field: 'node',
                title: '服务节点',
                align: 'center'
            },
            {
                field: 'route',
                title: '路径',
                align: 'center'

            },
            {
                field: 'url',
                title: '请求地址',
                align: 'center'

            },
            {
                field: 'method',
                title: '请求方法',
                align: 'center'

            },
            {
                field: 'milliseconds',
                title: '处理时间',
                align: 'center'

            },
            {
                field: 'statusCode',
                title: '状态码',
                align: 'center'

            },
            {
                field: 'ip',
                title: 'IP地址',
                align: 'center'

            },

            {
                field: 'createTime',
                title: '请求时间',
                align: 'center',
                formatter: function (value, row, index) {

                    value = value.replace('T', ' '); 
                    return value;
                }
            }
        ]
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

function RefreshTable() {   


    var url = "/Data/GetRequestList";

    var start = $(".start").val().trim();
    var end = $(".end").val().trim();
    var requestUrl = $(".url").val().trim();
    var ip = $(".ipadress").val().trim();

    var nodes = [];

    $(".node-row").find(".btn-info").each(function (i, item) {
        nodes.push($(item).text());
    });

    var node = nodes.join(",");

    url = url + `?start=${start}&end=${end}&url=${requestUrl}&ip=${ip}&node=${node}`;

    $('#TableData').bootstrapTable('refresh', { 
        url: url  
    });
}


function ReSetTag() { 

    var start = $(".start").val();
    var end = $(".end").val(); 
     
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


function QueryClick() {

    ReSetTag();

    RefreshTable();

}  
 