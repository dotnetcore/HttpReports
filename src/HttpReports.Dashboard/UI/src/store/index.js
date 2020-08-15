import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex);


const store = new Vuex.Store({ 
  state: {
    tag: {},
    token: "", 
    lang: {},
    query:{}
  },  
  mutations: { 
    set_token: (state, data) => state.token = data,
    set_lang: (state, data) => {
      state.lang = data;  
    },
    set_tag: (state, data) => state.tag = data,
    set_query:(state,data) => state.query = data,
  }

})

export default store;

