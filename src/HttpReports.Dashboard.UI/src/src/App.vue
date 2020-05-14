<template>
  <div id="app">
    <div>{{errorMessage}}</div>
    <div>ApiAddress: {{this.$store.state.apiAddress}}</div>
    <Loading v-if="this.$store.state.initing" id="loading" />
    <Home v-if="!this.$store.state.initing&&true" msg="Welcome to Your Vue.js App" />
  </div>
</template>

<script>
import Home from "./views/Home.vue";
import Loading from "./views/Loading.vue";
import axios from "axios";

export default {
  name: "app",
  data: () => {
    return {
      errorMessage: ""
    };
  },
  mounted: function () {
    let configUrl = window.location.href.substring(0, window.location.href.indexOf('index.html')) + "config.json";
    //let configUrl = "http://localhost:5200/dashboard/config.json"
    axios
      .get(configUrl)
      .then(response => {
        this.$store.state.apiAddress = response.data.apiAddress;
        this.$store.state.initing = false;
      })
      .catch(error => {
        this.errorMessage = error;
        this.$store.state.initing = false;
      });
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