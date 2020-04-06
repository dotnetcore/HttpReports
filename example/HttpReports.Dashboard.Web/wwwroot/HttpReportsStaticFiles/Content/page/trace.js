  
InitTree();   
 

function InitTree() { 

    var id = $(".trace_id").val();  

    $.ajax({

        url: "/HttpReportsData/GetTraceList/" + id,
        type: "POST",
        success: function (result) {

            var tree = result.data;  

            BuildTree(tree); 

        }

    });  
}   

function BuildTree(tree) {  

    var newTree = ParseTree(tree);  

    $('.request-tree').treeview({
        data: newTree,
        levels: 9999,
        showTags: true
    });  
}

function ParseTree(tree) {   

    var k = $.isArray(tree) ? tree[0] : tree; 

    k = AppendTree(k); 

    if (k.nodes != null && k.nodes != undefined && k.nodes.length > 0) {

        $.each(k.nodes, function (index, item) {

            ParseTree(item);

        });
    }
    else {
        delete k["nodes"];
    }   
   

    return tree;  
} 

function AppendTree(k) {  

    k.text = `<span class="service">${k.node}<span>` + `<span class="url">${k.url}<span>` + `<i onclick="bind_context('${k.id}')" class="glyphicon glyphicon-info-sign info"></i>`

    return k;
}   

function bind_context(Id) { 

    $.ajax({
        url: "/HttpReportsData/GetRequestInfoDetail/" + Id,
        success: function (result) {   

            var info = result.data.info;
            var detail = result.data.detail;  

            $(".context_requestId").text(info.id);
            $(".context_node").text(info.node);
            $(".context_route").text(info.route);
            $(".context_url").text(info.url);
            $(".context_method").text(info.method);
            $(".context_milliseconds").text(info.milliseconds);
            $(".context_statusCode").text(info.statusCode);
            $(".context_ip").text(info.ip);
            $(".context_port").text(info.port);
            $(".context_localIp").text(info.localIP);
            $(".context_localPort").text(info.localPort);
            $(".context_createTime").text(info.createTime);   

            $(".context_queryString").text(detail.queryString);
            $(".context_header").text(detail.header);
            $(".context_cookie").text(detail.cookie);
            $(".context_requestBody").text(detail.requestBody);
            $(".context_responseBody").text(detail.responseBody);
            $(".context_error").text(detail.errorMessage);
            $(".context_errorStack").text(detail.errorStack);

          
            show_modal(); 
        } 
    }); 

}

function show_modal() { 

    //$('.contextBox').niceScroll({
    //    cursorcolor: "#ccc",//#CC0071 光标颜色
    //    cursoropacitymax: 1, //改变不透明度非常光标处于活动状态（scrollabar“可见”状态），范围从1到0
    //    touchbehavior: false, //使光标拖动滚动像在台式电脑触摸设备
    //    cursorwidth: "5px", //像素光标的宽度
    //    cursorborder: "0", // 游标边框css定义
    //    cursorborderradius: "5px",//以像素为光标边界半径
    //    autohidemode: false //是否隐藏滚动条
    //}); 

 
    $(document.body).css({  "overflow-y": "hidden"  });
     
    $(".contextBox").show();
    new mSlider({
        dom: ".contextBox",
        distance: "40%",
        direction: "right",
        callback: function () {
            $(".contextBox").hide(); 
            $(document.body).css({  "overflow-y": "auto" });
        }
    }).open(); 

}

 


 
 