import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex); 

const mutations = {

  updateUserName(state, str) {
    state.username = str;
    return str;
  },

  updateToken(state, token) {
    state.token.token = token.token;
    state.token.express = token.express; 
  },

  updateUser(state, user) {
    state.user.username = user.username;
    state.user.mobile = user.mobile;
  } 
} 


const store = new Vuex.Store({

  state: { 
    token: { token: "", express: "" },
    user: { username:"",mobile:"" }
  },
  mutations
})

export default store;

