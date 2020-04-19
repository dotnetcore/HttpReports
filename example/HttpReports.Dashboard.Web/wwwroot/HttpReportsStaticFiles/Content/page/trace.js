  
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

var toptree = null;

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

    if (toptree == null) {
        toptree = k; 
    }  

    var barWidth = 300;
    if (toptree != k) {
        barWidth = parseInt( barWidth * (k.milliseconds / toptree.milliseconds));
    } 


    k.text = `<span class="service">${k.node}</span>`
        + `<span class="url">${k.url}</span>` 
        + `<span class="label label-${(k.statusCode == 200 ? "success" : "danger")} statusCode">${k.statusCode}</span>` 
        + `<span class="requestType">${k.requestType}</span>`
        + `<i onclick="bind_context('${k.id}')" class="glyphicon glyphicon-info-sign info"></i>`
        + `<span class="bar" style="width:${barWidth}px"></span>`  
        + `<span class="milliseconds">${k.milliseconds}ms</span>` 

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
            $(".context_requestType").text(info.requestType); 
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
  
     
    $(".contextBox").show();
     new mSlider({
        dom: ".contextBox",
        distance: "40%",
        direction: "left",
        callback: function () {
            $(".contextBox").hide(); 
            $(".contextBox").getNiceScroll().remove(); 
        }
    }).open();  


    $('.contextBox').niceScroll({
        cursorcolor: "#ccc",
        cursoropacitymax: 1,
        touchbehavior: false, 
        cursorwidth: "3px",
        cursorborder: "0",
        cursorborderradius: "5px",
        autohidemode: false 
    });   
}

 


 
 