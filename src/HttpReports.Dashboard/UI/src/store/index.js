import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex);  


const store = new Vuex.Store({

  state: { 
    token:"",
    lang:{

    } 
  },
  mutations: { 

  set_token(state, token) {
    state.token = token; 
  },
  set_lang(state,lang) {
    state.lang = lang; 
  }
} 
})

export default store;

