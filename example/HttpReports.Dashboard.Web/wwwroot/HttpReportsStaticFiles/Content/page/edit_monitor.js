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


BindMonitorJob();

function saveMonitorJob() {

    var id = $(".id").val();
    var title = $(".title").val().trim();
    var description = $(".description").val().trim();
    var emails = $(".email").val().trim();
    var webhook = $(".webhook").val().trim();
    var mobiles = $(".mobiles").val().trim();
    var interval = $(".interval").val().trim();
    var status = $('.checkbox').eq(0).bootstrapSwitch('state') ? 1 : 0;

    var nodeList = [];

    $(".node-row").find(".btn-info").each(function (i, item) {
        nodeList.push($(item).text());
    });

    var nodes = nodeList.join(",");

    var responseTimeOutMonitor = null;
    var errorResponseMonitor = null;
    var IPMonitor = null;
    var requestCountMonitor = null;

    if ($('.checkbox').eq(1).bootstrapSwitch('state')) {

        responseTimeOutMonitor = {
            Status:1, 
            TimeOutMs: $(".RtTime").val().trim(),
            Percentage: $(".RtPercent").val().trim()

        }

    }   

    if ($('.checkbox').eq(2).bootstrapSwitch('state')) {

        errorResponseMonitor = {
            Status: 1, 
            HttpCodeStatus: $(".HttpCodes").val().trim(),
            Percentage: $(".HttpPercent").val().trim() 
        }

    }  

    if ($('.checkbox').eq(3).bootstrapSwitch('state')) {

        IPMonitor = {
            Status: 1, 
            WhileList: $(".IPWhiteList").val().trim(),
            Percentage: $(".IPPercent").val().trim()

        }
    }  

    if ($('.checkbox').eq(4).bootstrapSwitch('state')) {

        requestCountMonitor = {
            Status: 1, 
            Max: $(".RequestCount").val().trim() 
        }
    }  


    $.ajax({
        url: "/HttpReportsData/EditMonitor",
        type: "POST",
        data: {
            id, title, description, emails, webhook, mobiles, nodes, interval, status, responseTimeOutMonitor, errorResponseMonitor, IPMonitor, requestCountMonitor
        },
        success: function (result) {

            if (result.code == 1) {

                alertOk(lang.Save_Success, 1200, function () {

                    location.href = "/HttpReports/Monitor";

                });
            }
            else { 
                alertError(result.msg)
            }
        }

    });  

}

function BindMonitorJob() {

    var id = $(".id").val();

    if (id == "0" || id == "") {
        return;
    }

    $.ajax({
        url: "/HttpReportsData/GetMonitor/" + id,
        success: function (result) {

            var job = result.data; 

            $(".title").val(job.title);
            $(".description").val(job.description);
            $(".email").val(job.emails);
            $(".webhook").val(job.webHook);
            $(".mobiles").val(job.mobiles);
            $(".interval").val(job.interval);
            $('.checkbox').eq(0).bootstrapSwitch('state', job.status > 0); 

            if (job.responseTimeOutMonitor != null) {

                $('.checkbox').eq(1).bootstrapSwitch('state', job.responseTimeOutMonitor.status > 0);
                $(".RtTime").val(job.responseTimeOutMonitor.timeOutMs);
                $(".RtPercent").val(job.responseTimeOutMonitor.percentage);   
            } 

            if (job.errorResponseMonitor != null) {

                $('.checkbox').eq(2).bootstrapSwitch('state', job.errorResponseMonitor.status > 0);
                $(".HttpCodes").val(job.errorResponseMonitor.httpCodeStatus);
                $(".HttpPercent").val(job.errorResponseMonitor.percentage); 
            } 

            if (job.ipMonitor != null) {

                $('.checkbox').eq(3).bootstrapSwitch('state', job.ipMonitor.status > 0);
                $(".IPWhiteList").val(job.ipMonitor.whileList);
                $(".IPPercent").val(job.ipMonitor.percentage); 
            }  

            if (job.requestCountMonitor != null) {

                $('.checkbox').eq(4).bootstrapSwitch('state', job.requestCountMonitor.status > 0); 
                $(".RequestCount").val(job.requestCountMonitor.max);

            } 
        } 
    }); 

}

 