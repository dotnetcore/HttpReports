  
function delMonitorRule(id) {

    confirm("确定要删除这个监控规则吗?", function () {

        $.ajax({
            url: "/Data/DeleteMonitorRule/" + id,
            success: function (result) {

                alert("删除成功", function () {

                    location.reload();

                });

            } 
        }); 

    }); 
}
