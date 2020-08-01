import Vue from 'vue'
import App from './App'
import router from './router'
import store from './store'

import ElementUI from 'element-ui';
import 'element-ui/lib/theme-chalk/index.css';
import './assets/css/web.css';
import animate from 'animate.css'
import VueResource from 'vue-resource'


// 引入自定义js库
import { basic } from '@/common/basic.js'
Vue.prototype.basic = basic


Vue.use(ElementUI);
Vue.use(animate)
Vue.use(VueResource)

Vue.config.productionTip = false


new Vue({
  el: '#app',
  store,
  router,
  render: h => h(App),
  created: function () {

    console.log("3333")

    loadState();

    checkLoginState();

    setHttpFilter();

  }
})

function loadState() {

  var token = store.state.token.token;
  var express = store.state.token.express;

  if (basic.isEmpty(token)) {

    token = localStorage.getItem("token");
    express = localStorage.getItem("express");
    var now = basic.getTimeSpan();

    if (basic.isEmpty(token)) {

      // 不BB 跳到登录页
      if (router.history.current.name != "login2" && router.history.current.name != "login") {

        router.push({ path: '/user/login2' })

      }
    }
    else {
      store.commit('updateToken', { token: token, express: express });
    }
  }

}

function checkLoginState() {

  var token = store.state.token.token;
  var express = store.state.token.express;
  var now = basic.getTimeSpan();

  if (basic.isEmpty(token) && basic.isEmpty(express)) {

    if (now > express) {

      // 不BB 跳到登录页
      if (router.history.current.name != "login2" && router.history.current.name != "login") {
        router.push({ path: '/user/login2' })
      }

    }

  }
}

function setHttpFilter() {

  // 设置根请求
  Vue.http.options.root = "http://localhost:1234/";

  // VueResource 请求拦截器
  Vue.http.interceptors.push((request, next) => {

    var token = store.state.token.token;

    if (request.url != "token/CheckLogin") {
      request.headers.set('Authorization', 'Bearer ' + token);
    }

    next(function (response) {

      if (response.status != 200) {
        this.$message.error('出错了,请重试！');
        return;
      }
      return response;

    });
  })

}

router.beforeEach((to, from, next) => {

  //loadState();

  //checkLoginState();

  //next();

});


