

const basic = {
  isEmpty: (str) => {

    if (str == null || str == undefined || str.length == 0) {
      return true;
    }
    else {
      return false;
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
  }

};

export { basic }
