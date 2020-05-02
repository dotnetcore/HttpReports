 

function startJob(Id) {

    confirm(lang.Monitor_ConfirmStart, function () {

        $.ajax({

            url: "/HttpReportsData/ChangeJobState/" + Id,
            success: function (result) {

                alertOk(lang.Monitor_UpdateSuccess,1200,function () {

                    location.reload();

                });
            }
        })

    });

}

function stopJob(Id) {

    confirm(lang.Monitor_ConfirmStop, function () {

        $.ajax({

            url: "/HttpReportsData/ChangeJobState/" + Id,
            success: function (result) {

                alertOk(lang.Monitor_UpdateSuccess,1200, function () {

                    location.reload();

                });
            }
        })


    });

}

function delJob(Id) {

    confirm(lang.Monitor_ConfirmDelete, function () {

        $.ajax({

            url: "/HttpReportsData/DeleteJob/" + Id,
            success: function (result) {

                alertOk(lang.Monitor_DeleteSuccess,1200, function () {

                    location.reload();

                });
            }
        })

    });
}


