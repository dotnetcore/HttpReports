 
LoadJob();

function Save() {

    var Id = $(".id").val();

    var title = $(".title").val().trim();

    var rate = $(".rate").val().trim();

    var email = $(".email").val().trim();

    var mobiles = $(".mobiles").val().trim();

    var status = $(".status").val().trim(); 

    var nodes = [];

    $(".node-row").find(".btn-info").each(function (i, item) {
        nodes.push($(item).text());
    });

    var node = nodes.join(",");

    var RtStatus = $(".RtStatus").val().trim();

    var RtTime = $(".RtTime").val().trim();

    var RtRate = $(".RtRate").val().trim();

    var HttpStatus = $(".HttpStatus").val().trim();

    var HttpCodes = $(".HttpCodes").val().trim();

    var HttpRate = $(".HttpRate").val().trim();

    var IPStatus = $(".IPStatus").val().trim();

    var IPWhiteList = $(".IPWhiteList").val().trim();

    var IPRate = $(".IPRate").val().trim();   

    var RequestStatus = $(".RequestStatus").val().trim();

    var RequestCount = $(".RequestCount").val().trim(); 
    

    $.ajax({

        url: "/Data/EditJob",
        type: "POST",
        data: {
            Id, title, rate, email, status, node, RtStatus, RtTime, RtRate, HttpStatus, HttpCodes, HttpRate, IPStatus, IPWhiteList, IPRate, RequestStatus, RequestCount, mobiles
        },
        success: function (result) {

            if (result.code == 1) {

                alert("保存成功", function () {
                    location.href = "/Home/Monitor";
                }); 

            }
            else {
                alert(result.msg)
            } 

        } 
    });  

} 

function LoadJob() {

    var id = $(".id").val();

    if (id == 0) {
        return;
    }

    $.ajax({
        url: "/Data/GetJob/" + id,
        success: function (result) { 

            if (result.data.id == 0) {
                return;
            } 

            console.log(result)

            $(".title").val(result.data.title);
            $(".rate").val(result.data.rate)  
            $(".email").val(result.data.email)   
            $(".mobiles").val(result.data.mobiles); 
            $(".status").val(result.data.status);    

            var server = result.data.node.split(","); 

            $(".node-row").find("button").each(function (i, item) {

                var k = $(item).text().trim();

                if (server.indexOf(k) > -1) { 

                    if (!$(item).hasClass("btn-info")) {

                        $(item).removeClass("btn-default")

                        $(item).addClass("btn-info")

                    }

                } 
                else {

                    if ($(item).hasClass("btn-info")) {

                        $(item).removeClass("btn-info");

                        $(item).addClass("btn-default");

                    }

                } 
            }); 

            $(".RtStatus").val(result.data.rtStatus);

            $(".RtTime").val(result.data.rtTime);

            $(".RtRate").val(result.data.rtRate+"%"); 

            $(".HttpStatus").val(result.data.httpStatus);

            $(".HttpCodes").val(result.data.httpCodes);

            $(".HttpRate").val(result.data.httpRate+"%");

            $(".IPStatus").val(result.data.ipStatus);

            $(".IPWhiteList").val(result.data.ipWhiteList);

            $(".IPRate").val(result.data.ipRate+"%");

            $(".RequestStatus").val(result.data.requestStatus);

            $(".RequestCount").val(result.data.requestCount);  


        } 

    });  

}
