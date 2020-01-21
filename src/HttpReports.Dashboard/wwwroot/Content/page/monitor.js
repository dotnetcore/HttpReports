function selectRule(item) {

    var id = $(item).attr("data-id"); 

    $(".resetId").val(id);

    if (id > 0) {

        var nodes = $(item).parent().prev().text().trim().split(',');  

        $(".node-row").find(".btn").each(function (i, k) {  
         
            if ($.inArray($(k).text().trim(),nodes) >= 0) {

                if ($(k).hasClass("btn-default")) {
                    $(k).removeClass("btn-default");
                    $(k).addClass("btn-info");
                }   

            }
            else {

                if ($(k).hasClass("btn-info")) {
                    $(k).removeClass("btn-info");
                    $(k).addClass("btn-default");
                }  
            } 

        });  

        $(".select_rule_button").each(function (i, k) {

            if ($(k).attr("data-id") == id) {

                if ($(k).hasClass("btn-default")) {
                    $(k).removeClass("btn-default");
                    $(k).addClass("btn-info");
                }   

            } 
            else {

                if ($(k).hasClass("btn-info")) {
                    $(k).removeClass("btn-info");
                    $(k).addClass("btn-default");
                }  
            }

        }); 

        

    }
    else {

        $(".node-row").find(".btn").each(function (i, k) {  

            if ($(k).hasClass("btn-info")) {
                $(k).removeClass("btn-info");
                $(k).addClass("btn-default");
            } 

        });    

        $(".select_rule_button").each(function (i, k) { 

            if ($(k).hasClass("btn-info")) {
                $(k).removeClass("btn-info");
                $(k).addClass("btn-default");
            } 

        }); 


    }

    $('#SelectRuleModal').modal('show'); 
}

function removeRule(item) {

    confirm("确定要移除监控规则吗?", function () {

        var id = $(item).attr("data-id");

        $.ajax({
            url: "/Data/BindMonitorRuleAndNode",
            type: "POST",
            data: {
                ruleId: id,
                nodes: ""
            },
            success: function (result) {

                alert("移除成功", function () {

                    location.reload();

                });
            }

        }); 

    }); 
  
    
}

function Save_Select_Rule() {

    var resetId = $(".resetId").val();

    var nodes = [];

    $(".node-row").find(".btn-info").each(function (i, k) {
        nodes.push($(k).text());
    });

    console.log(nodes);

    if (nodes.length == 0) {

        alert("请至少一个服务节点");
        return

    }

    var ruleId = 0;

    $(".select_rule_button").each(function (i,k) {

        if ($(k).hasClass("btn-info")) {
            ruleId = $(k).attr("data-id");
        } 
    });

    if (ruleId == 0) {

        alert("请选择一项监控规则");
        return; 
    }  

    $.ajax({
        url: "/Data/BindMonitorRuleAndNode",
        type: "POST",
        data: {
            ruleId: ruleId,
            nodes: nodes.join(','),
            reset: resetId > 0
        },
        success: function (result) {

            alert("保存成功", function () {

                location.reload();

            }); 
        } 

    }); 

}  

function select_rule(item) {

    var now = $(item).attr("data-id");

    $(".select_rule_button").each(function (i, k) {

        if ($(k).attr("data-id") != now) {

            if ($(k).hasClass("btn-info")) {
                $(k).removeClass("btn-info");
                $(k).addClass("btn-default");
            } 
        }   
    }); 


    if ($(item).hasClass("btn-info")) {
        $(item).removeClass("btn-info");
        $(item).addClass("btn-default");
    }
    else {
        $(item).removeClass("btn-default");
        $(item).addClass("btn-info");
    } 


}