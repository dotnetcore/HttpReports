 

function startJob(Id) {

    confirm("确认要开启任务吗?", function () {

        $.ajax({

            url: "/Data/ChangeJobState/" + Id,
            success: function (result) {

                alert("修改成功", function () {

                    location.reload();

                });
            }
        })

    });

}

function stopJob(Id) {

    confirm("确认要停止任务吗?", function () {

        $.ajax({

            url: "/Data/ChangeJobState/" + Id,
            success: function (result) {

                alert("修改成功", function () {

                    location.reload();

                });
            }
        })


    });

}

function delJob(Id) {

    confirm("确认要删除任务吗?", function () {

        $.ajax({

            url: "/Data/DeleteJob/" + Id,
            success: function (result) {

                alert("删除成功", function () {

                    location.reload();

                });
            }
        })

    });
}


