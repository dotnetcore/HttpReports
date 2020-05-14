<template>
  <div id="overview">
    <h1>Overview</h1>
    <span>还没开始写</span>
    <h2>Api返回数据</h2>
    <div>{{res.data}}</div>
  </div>
</template>
<script>
import api from "../api";

export default {
  data: () => {
    return {
      res: {}
    };
  },
  mounted: function () {
    loadData().then((res) => {
      this.res = res.data;
    });
  }
}
async function loadData () {
  return await api.getIndexChartData()
}
</script>