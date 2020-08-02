import Vue from 'vue'
import App from './App'
import router from './router'
import store from './store'

import ElementUI from 'element-ui';
import 'element-ui/lib/theme-chalk/index.css';
import './assets/css/web.css';
import animate from 'animate.css'
import VueResource from 'vue-resource' 
import 'font-awesome/css/font-awesome.min.css'


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

    var configUrl = window.location.href.substring(0, window.location.href.indexOf('index.html')) + "config.json";   
   

    setHttpFilter();  

    loadLanguage(); 

    loadState();  

  }
})

function loadState() {

  var token = store.state.token;  

  if (basic.isEmpty(token)) {

    token = localStorage.getItem("token");  

    if (basic.isEmpty(token)) {   

        router.push({ path: '/user/login' })  
    }
    else {
      store.commit('set_token',token);
    }
  }

} 


function loadLanguage() {     
 
  Vue.http.get("GetLanguage").then(response =>{
 
    var lang = response.body.data;  

    Vue.http.get(`/static/lang/${lang}.json`).then(res =>{   
      
      store.commit('set_lang',res.body);

    });
     

  }); 
  
}

 

function setHttpFilter() {   

  Vue.http.options.root = "http://localhost:5010/HttpReportsData";  

  // VueResource 请求拦截器
  Vue.http.interceptors.push((request, next) => {

    var token = store.state.token; 

    if (!basic.isEmpty(token)) {
      
      request.headers.set('HttpReports.Auth.Token',token); 

    }  

    next(function (response) { 

      if (response.status != 200) {

        if (response.status == 401) { 

          router.push({ path: '/user/login' })  
          return;
        } 

        this.$message.error('error');
        return;

      }

      return response;

    });

    
  })

}

router.beforeEach((to, from, next) => { 
 
  if(to.path != '/user/login'){
    loadState(); 
  } 

  next();

});


