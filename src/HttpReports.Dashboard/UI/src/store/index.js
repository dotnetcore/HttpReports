import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex); 
import { basic } from '@/common/basic.js'

const store = new Vuex.Store({ 
  state: {
    tag: {},
    token: "", 
    lang: {},
    query:{
      service:"ALL",
      instance:"ALL",
      start:basic.dateFormat(basic.getLastTime()),
      end:basic.dateFormat(new Date()) 
    },  
    basic_loading:false, 
    detail_loading:false  
  },  
  mutations: { 
    set_token: (state, data) => state.token = data,
    set_lang: (state, data) => {
      state.lang = data;  
    },
    set_tag: (state, data) => state.tag = data,
    set_query:(state,data) => state.query = data,
    set_basic_loading:(state,data) => state.basic_loading = data,
    set_detail_loading:(state,data) => state.detail_loading = data,

  }

})

export default store;

