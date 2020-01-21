

loadMonitorRule();

$(".checkbox").bootstrapSwitch({ 

    onSwitchChange: function (event, state) { 

        return;

        if (state == true) {  

            $(event.target).parents(".col-sm-12").next().slideToggle()

            $(event.target).parents(".col-sm-12").next().next().slideToggle() 


        } else {

            $(event.target).parents(".col-sm-12").next().slideToggle()

            $(event.target).parents(".col-sm-12").next().next().slideToggle() 
        }
    }

});

function saveMonitorRule() {

    var id = $(".ruleId").val();
    var title = $(".title").val().trim();
    var description = $(".description").val().trim();
    var email = $(".email").val().trim();

    var responseTimeOutMonitor = null;
    var errorResponseMonitor = null;
    var remoteAddressRequestTimesMonitor = null;
    var requestTimesMonitor = null;

    if ($('.checkbox').eq(0).bootstrapSwitch('state')) {

        responseTimeOutMonitor = {
            Id: $('.checkbox').eq(0).attr("data-id"),
            Interval : $(".rate").eq(0).val(),
            TimeoutThreshold : $(".RtTime").val().trim(),
            Percentage: $(".RtPercent").val().trim()

        }

    }  

    if ($('.checkbox').eq(1).bootstrapSwitch('state')) {

        errorResponseMonitor = {
            Id: $('.checkbox').eq(1).attr("data-id"),
            Interval: $(".rate").eq(1).val(),
            StatusCodes:$(".HttpCodes").val().trim(),
            Percentage: $(".HttpPercent").val().trim()

        }

    } 

    if ($('.checkbox').eq(2).bootstrapSwitch('state')) {

        remoteAddressRequestTimesMonitor = {
            Id: $('.checkbox').eq(2).attr("data-id"),
            Interval: $(".rate").eq(2).val(),
            WhileList: $(".IPWhiteList").val().trim(),
            Percentage: $(".IPPercent").val().trim()

        } 
    }  

    if ($('.checkbox').eq(3).bootstrapSwitch('state')) {

        requestTimesMonitor = {
            Id: $('.checkbox').eq(3).attr("data-id"),
            Interval: $(".rate").eq(3).val(),
            WarningThreshold: $(".RequestCount").val().trim() 
            
        }
    }  

    $.ajax({
        url: "/Data/EditMonitorRule",
        type:"POST",
        data: {
            id, title, description, email, responseTimeOutMonitor, errorResponseMonitor, remoteAddressRequestTimesMonitor, requestTimesMonitor
        },
        success: function (result) {

            if (result.code == 1) {
                alert("保存成功", function () {

                    location.href = "/Home/MonitorRule";

                });
            } 
            else {
                alert(result.msg)
            } 
        }
    });

}

function loadMonitorRule() {

    var id = $(".ruleId").val();

    if (id == 0) return 

    $.ajax({

        url: "/Data/GetMonitorRuleById/" + id,
        success: function (result) {

            console.log(result)

            if (result.code != 1) {
                alert("获取规则失败,请重试");
            } 


            // 绑定到表单 
            var k = result.data; 

            $(".ruleId").val(k.id);
            $(".title").val(k.title);
            $(".description").val(k.description)
            $(".email").val(k.email);

            if (k.responseTimeOutMonitor != null) {

                $('.checkbox').eq(0).bootstrapSwitch('state', true);  
                $('.checkbox').eq(0).attr("data-id", k.responseTimeOutMonitor.id); 
                $(".rate").eq(0).val(k.responseTimeOutMonitor.interval);
                $(".RtTime").val(k.responseTimeOutMonitor.timeoutThreshold);
                $(".RtPercent").val(k.responseTimeOutMonitor.percentage);  

            }  

            if (k.errorResponseMonitor != null) {

                $('.checkbox').eq(1).bootstrapSwitch('state', true);
                $('.checkbox').eq(1).attr("data-id", k.errorResponseMonitor.id);
                $(".rate").eq(1).val(k.errorResponseMonitor.interval);
                $(".HttpCodes").val(k.errorResponseMonitor.statusCodes);
                $(".HttpPercent").val(k.errorResponseMonitor.percentage); 
            } 

            if (k.remoteAddressRequestTimesMonitor != null) {

                $('.checkbox').eq(2).bootstrapSwitch('state', true);
                $('.checkbox').eq(2).attr("data-id", k.remoteAddressRequestTimesMonitor.id);
                $(".rate").eq(2).val(k.remoteAddressRequestTimesMonitor.interval); 
                $(".IPWhiteList").val(k.remoteAddressRequestTimesMonitor.whileList);
                $(".IPPercent").val(k.remoteAddressRequestTimesMonitor.percentage); 
            } 

            if (k.requestTimesMonitor != null) {

                $('.checkbox').eq(3).bootstrapSwitch('state', true);
                $('.checkbox').eq(3).attr("data-id", k.requestTimesMonitor.id);
                $(".rate").eq(3).val(k.requestTimesMonitor.interval);
                $(".RequestCount").val(k.requestTimesMonitor.warningThreshold) 
            }  
        } 

    }); 

}

