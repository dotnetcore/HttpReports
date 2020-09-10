import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex); 
import { basic } from '@/common/basic.js'
import { data } from 'autoprefixer';

const store = new Vuex.Store({ 
  state: {
    tag: {},
    token: "", 
    lang: {},
    route:"/",
    query:{
      service:"",
      instance:"",
      start:basic.dateFormat(basic.getLastTime()),
      end:basic.dateFormat(new Date()) 
    },  
    basic_loading:false, 
    service_loading:false,  
    detail_loading:false,
    topology_loading:false
  },  
  mutations: { 
    set_token: (state, data) => state.token = data,
    set_lang: (state, data) => {
      state.lang = data;  
    },
    set_tag: (state, data) => state.tag = data,
    set_query(state,data) {  
      
      if (data != null) {  
        if (data.service == "ALL") {  
           data.service = "";
        }

        if (data.instance == "ALL") {
          data.instance = "";
       } 
       
     } 

      state.query = data;

    },
    set_basic_loading:(state,data) => state.basic_loading = data,
    set_detail_loading:(state,data) => state.detail_loading = data,
    set_topology_loading:(state,data) => state.topology_loading = data,
    set_service_loading:(state,data) => state.service_loading = data,
    set_detail_loading:(state,data) => state.detail_loading = data,
    set_route:(state,data) => state.route = data, 

  }

})

export default store;

