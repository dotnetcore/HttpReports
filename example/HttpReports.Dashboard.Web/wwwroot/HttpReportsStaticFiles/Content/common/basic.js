  
var M = {};   

window.alertOk = function (msg,time = 2000, callback = () => { }) {

    if (typeof (time) == "function") {
        callback = time;
        time = 2000;
    }

    $.message({
        time: time,
        message: msg,
        type: 'success'
    });

    setTimeout(function () {
        callback();
    }, time);
}


window.alertInfo = function (msg, time = 200, callback = () => { }) {

    if (typeof (time) == "function") {
        callback = time;
        time = 2000;
    }

    $.message({
        time: time,
        message: msg,
        type: 'info'
    });

    setTimeout(function () {
        callback();
    }, time);
}

window.alertError = function (msg, time = 2000, callback = () => { }) {

    if (typeof (time) == "function") { 
        callback = time;
        time = 2000;
    } 

    $.message({
        time: time,
        message: msg,
        type: 'error'
    });

    setTimeout(function () {
        callback();
    }, time);
}

window.alertWarn = function (msg, time = 2000, callback = () => { }) {

    if (typeof (time) == "function") {
        callback = time;
        time = 2000;
    }

    $.message({
        time: time,
        message: msg,
        type: 'warning'
    });

    setTimeout(function () {
        callback();
    }, time);

} 


window.alert = function (msg, callback = () => { }) {    

    M.dialog1 = jqueryAlert({
        'content': msg,
        'closeTime': 1500
    });   

    setTimeout(function () {
        callback();
    }, 1500);  
   
}  
 
 
function DelayGo(url) {

    setTimeout(function () {

        location.href = url;

    },1500);  
}

function Go(url) { 
    location.href = url;
} 


window.confirm = function (msg, fun) {

    var ok = lang.Button_OK;
    var cancel = lang.Button_Cancel;


    M.dialog4 = jqueryAlert({ 


        'content': '<b>' + msg + '</b>',
        'modal': true,
        'top':"40%",
        'animateType': '',
        'buttons': {
            ok: function () {

                fun();

                M.dialog4.close(); 
            },
            cancel: function () {

                M.dialog4.close();   
            }
        }
    })  
}  

function HttpPost(url, data) { 

    return $.ajax({
        type:"POST",
        url: url,
        async: false,
        data: data
    }).responseJSON;  

}

function HttpGet(url,data) {

    return $.ajax({
        type:"GET",
        url: url,
        async: false,
        data: data
    }).responseJSON;

}


function GoTop() {

    $.goup({
        trigger: 100,
        bottomOffset:60,
        locationOffset:16,
        title: '',
        titleAsText: true
    }); 

}

function goback() {

    window.history.back();

}
