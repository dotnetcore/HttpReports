 

var M = {};   

window.alert = function (msg) {  

    M.dialog1 = jqueryAlert({
        'content': msg,
        'closeTime': 1500
    });    
} 

window.alert = function (msg, callback = () => { }) {   

    M.dialog1 = jqueryAlert({
        'content': msg,
        'closeTime': 1500
    });   

    setTimeout(function () {
        callback();
    }, 2000);  
   
}  
 
function Show(msg) {

    M.dialog1 = jqueryAlert({
        'content': msg,
        'closeTime': 1500 
    })  
}  

function Show(msg,fun) {

    M.dialog1 = jqueryAlert({
        'content': msg,
        'closeTime': 1500
    });

    if (fun != 0) {
        setTimeout(function () {
            fun()
        }, 1500); 
    } 
}  
 
function DelayGo(url) {

    setTimeout(function () {

        location.href = url;

    },1500);  
}

function Go(url) { 
    location.href = url;
} 


window.confirm = function (msg,fun) {

    M.dialog4 = jqueryAlert({

        'content': '<b>' + msg + '</b>',
        'modal': true,
        'top':"40%",
        'animateType': '',
        'buttons': {
            '确定': function () {

                fun();

                M.dialog4.close(); 
            },
            '关闭': function () {

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
