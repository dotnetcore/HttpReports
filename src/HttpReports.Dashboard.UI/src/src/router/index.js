import Vue from "vue";
import VueRouter from "vue-router";
import Overview from "../views/Overview.vue";
import Trend from "../views/Trend.vue";
import Request from "../views/Request.vue";
import Monitor from "../views/Monitor.vue";
import About from "../views/About.vue";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Overview",
    component: Overview
  },
  {
    path: "/Trend",
    name: "Trend",
    component: Trend
  },
  {
    path: "/Request",
    name: "Request",
    component: Request
  },
  {
    path: "/Monitor",
    name: "Monitor",
    component: Monitor
  },
  {
    path: "/About",
    name: "About",
    component: About
  }
];

const router = new VueRouter({
  routes
});

export default router;
