<template>
  <div id="app">
    <div>{{errorMessage}}</div>
    <div>ApiAddress: {{this.$store.state.apiAddress}}</div>
    <Loading v-if="this.$store.state.initing" id="loading" />
    <Home v-if="!this.$store.state.initing" />
  </div>
</template>

<script>
import Home from "./views/Home.vue";
import Loading from "./views/Loading.vue";
import axios from "axios";
import api from "./api";
import store from "./store/index";

export default {
  name: "app",
  data: () => {
    return {
      errorMessage: ""
    };
  },
  mounted: async function () {
    try    {
      let configUrl = window.location.href.substring(0, window.location.href.indexOf('index.html')) + "config.json";
      //let configUrl = "http://localhost:5200/dashboard/config.json"

      let response = await axios.get(configUrl);
      store.state.apiAddress = response.data.apiAddress;

      response = await api.getLocalizeLanguage(null);
      store.state.localize = response.data.data;
    } catch (error)    {
      this.errorMessage = error;
    }
    store.state.initing = false;
  },
  components: {
    Home,
    Loading
  }
};

</script>

<style>
#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
</style>