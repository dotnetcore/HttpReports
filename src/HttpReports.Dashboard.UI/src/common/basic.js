

const basic = {
  DOMAIN:"http://127.0.0.1:5005/api",
  isEmpty: (str) => {

    if (str == null || str == undefined || str.length == 0) {
      return true;
    }
    else {
      return false;
    }

  },

  ToInt:(str) => {

    if (str == null || str == undefined || str.length == 0) {
      return 0;
    }

     try {
       
      return parseInt(str);
       
     } catch (error) {
       return 0
     } 

  },

  isMobile: (str) => {
    if (!(/^1(3|4|5|6|7|8|9)\d{9}$/.test(str))) {
      return false;
    }
    else {
      return true;
    }
  },

  isEmail: (str) => {

    if (!(/^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/.test(str))) {
      return false;
    }
    else {
      return true;
    }
  },
  getTimeSpan() {
    return Math.round(new Date().getTime() / 1000);
  },
  dateFormat(time) {
    var date = new Date(time);
    var year = date.getFullYear();
    /* 在日期格式中，月份是从0开始的，因此要加0
        * 使用三元表达式在小于10的前面加0，以达到格式统一  如 09:11:05
        * */
    var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
    var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
    var hours = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
    var minutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
    var seconds = date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds();
    // 拼接
    return year + "-" + month + "-" + day + " " + hours + ":" + minutes + ":" + seconds;
  },
  getLastTime(minutes = 30){
            var now = new Date;
            now.setMinutes (now.getMinutes () - minutes);
            return now;
  },

  addSecond(now,second){ 
        now.setSeconds(now.getSeconds() + second);
        return now;
  }, 


  numberFormat(val){

    if(val >= 1000000){

      val = val.substring(0,val.length - 4) + "w" 
      return val

    } 

    return val;

  } 

};

export { basic }
