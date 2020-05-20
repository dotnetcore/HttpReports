import Vue from "vue";
import Vuex from "vuex";

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    initing: true,
    apiAddress: "",
    localize: {
      name: "Hello localize"
    }
  },
  getters: {
    apiFullAddress: state => path => {
      return state.apiAddress + path;
    }
  },
  mutations: {},
  actions: {},
  modules: {}
});