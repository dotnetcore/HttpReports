 

function startJob(Id) {

    confirm("确认要开启任务吗?", function () {

        $.ajax({

            url: "/HttpReportsData/ChangeJobState/" + Id,
            success: function (result) {

                alertOk("修改成功",1200,function () {

                    location.reload();

                });
            }
        })

    });

}

function stopJob(Id) {

    confirm("确认要停止任务吗?", function () {

        $.ajax({

            url: "/HttpReportsData/ChangeJobState/" + Id,
            success: function (result) {

                alertOk("修改成功",1200, function () {

                    location.reload();

                });
            }
        })


    });

}

function delJob(Id) {

    confirm("确认要删除任务吗?", function () {

        $.ajax({

            url: "/HttpReportsData/DeleteJob/" + Id,
            success: function (result) {

                alertOk("删除成功",1200, function () {

                    location.reload();

                });
            }
        })

    });
}


