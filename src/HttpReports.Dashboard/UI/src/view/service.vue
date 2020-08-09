<template>
  <div>
    <el-tabs style="margin-top:10px">
      <el-tab-pane label="接口数据">
        <el-row class="box-chart" :gutter="20">
          <el-col :span="8">
            <el-card class="box-card" style="margin-top:0">
              <div id="service-call" style="height:300px"></div>
            </el-card>
          </el-col>

          <el-col :span="8">
            <el-card class="box-card" style="margin-top:0">
              <div id="slow-service" style="height:300px"></div>
            </el-card>
          </el-col>

          <el-col :span="8">
            <el-card class="box-card" style="margin-top:0">
              <div id="error-service" style="height:300px"></div>
            </el-card>
          </el-col>
        </el-row>
      </el-tab-pane>

      <el-tab-pane label="实例数据">
        <el-row class="box-chart" :gutter="20">
          <el-col :span="8">
            <el-card class="box-card" style="margin-top:0">
              <div id="service-call1" style="height:300px"></div>
            </el-card>
          </el-col>

          <el-col :span="8">
            <el-card class="box-card" style="margin-top:0">
              <div id="slow-service1" style="height:300px"></div>
            </el-card>
          </el-col>

          <el-col :span="8">
            <el-card class="box-card" style="margin-top:0">
              <div id="error-service1" style="height:300px"></div>
            </el-card>
          </el-col>
        </el-row>
      </el-tab-pane>
    </el-tabs>

    <el-row class="box-chart" :gutter="20">
      <el-col :span="8">
        <el-card class="box-card" style="margin-top:0">
          <div id="service-gc" style="height:280px"></div>
        </el-card>
      </el-col>

      <el-col :span="8">
        <el-card class="box-card" style="margin-top:0">
          <div id="service-memory" style="height:280px"></div>
        </el-card>
      </el-col>

      <el-col :span="8">
        <el-card class="box-card" style="margin-top:0">
          <div id="service-thread" style="height:280px"></div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<style>
.el-row {
  margin-top: 20px;
}

.el-tabs__header {
  margin: 0;
}

.box-chart .el-card__body {
  padding: 4px;
}
</style>

<script>
import { Line } from "@antv/g2plot";
import { Bar } from "@antv/g2plot";
import { Chart } from "@antv/g2";
import { Heatmap } from "@antv/g2plot";

export default {
  data() {
    return {
      isCollapse: false,
      userName: "",
    };
  },
  created: () => {},
  mounted: () => {
    this.a.methods.init_service_call1();
    this.a.methods.init_slow_service1();
    this.a.methods.init_error_service1();

    this.a.methods.init_service_call();
    this.a.methods.init_slow_service();
    this.a.methods.init_error_service();

    this.a.methods.init_service_gc(); 
    this.a.methods.init_service_memory(); 
    this.a.methods.init_service_thread();
  },
  methods: {
    init_service_call1: () => {
      const data = [
        { 地区: "UserService", 销售额: 4684506.442 },
        { 地区: "OrderService", 销售额: 4137415.0929999948 },
        { 地区: "Payment", 销售额: 2681567.469000001 },
        { 地区: "Log", 销售额: 2447301.017000004 },
        { 地区: "DataService", 销售额: 1303124.508000002 },
        { 地区: "DataService2", 销售额: 1303124.508000002 },
      ];

      const barPlot = new Bar(document.getElementById("service-call1"), {
        title: {
          visible: true,
          text: "实例负载",
        },
        yAxis: {
          visible: true,
        },
        forceFit: true,
        data: data,
        xField: "销售额",
        yField: "地区",
        color: ["#80D0C7"],
        label: {
          visible: true,
          adjustPosition: true,
          formatter: (v) => Math.round(v / 10000),
          position: "left",
        },
        events: {
          onTitleDblClick: (e) => console.log(e),
        },
      });

      barPlot.render();
    },

    init_slow_service1: () => {
      const data = [
        { 地区: "UserService", 销售额: 4684506.442 },
        { 地区: "OrderService", 销售额: 4137415.0929999948 },
        { 地区: "Payment", 销售额: 2681567.469000001 },
        { 地区: "Log", 销售额: 2447301.017000004 },
        { 地区: "DataService", 销售额: 1303124.508000002 },
        { 地区: "DataService2", 销售额: 1303124.508000002 },
      ];

      const barPlot = new Bar(document.getElementById("slow-service1"), {
        title: {
          visible: true,
          text: "慢实例",
        },
        yAxis: {
          visible: true,
        },
        forceFit: true,
        data,
        xField: "销售额",
        yField: "地区",
        color: ["#FFE32C"],
        label: {
          visible: true,
          adjustPosition: true,
          formatter: (v) => Math.round(v / 10000),
          position: "left",
        },
        events: {
          onTitleDblClick: (e) => console.log(e),
        },
      });

      barPlot.render();
    },

    init_error_service1: () => {
      const data = [
        { 地区: "UserService", 销售额: 4684506.442 },
        { 地区: "OrderService", 销售额: 4137415.0929999948 },
        { 地区: "Payment", 销售额: 2681567.469000001 },
        { 地区: "Log", 销售额: 2447301.017000004 },
        { 地区: "DataService", 销售额: 1303124.508000002 },
        { 地区: "192.168.1.1:8888", 销售额: 1303124.508000002 },
      ];

      const barPlot = new Bar(document.getElementById("error-service1"), {
        title: {
          visible: true,
          text: "实例错误排行",
        },
        yAxis: {
          visible: true,
        },
        forceFit: true,
        data,
        color: ["#FF6A88"],
        xField: "销售额",
        yField: "地区",
        label: {
          visible: true,
          adjustPosition: true,
          formatter: (v) => Math.round(v / 10000),
          position: "left",
        },
        events: {
          onTitleDblClick: (e) => console.log(e),
        },
      });

      barPlot.render();
    },

    init_service_call: () => {
      const data = [
        { 地区: "UserService", 销售额: 4684506.442 },
        { 地区: "OrderService", 销售额: 4137415.0929999948 },
        { 地区: "Payment", 销售额: 2681567.469000001 },
        { 地区: "Log", 销售额: 2447301.017000004 },
        { 地区: "DataService", 销售额: 1303124.508000002 },
        { 地区: "DataService2", 销售额: 1303124.508000002 },
      ];

      const barPlot = new Bar(document.getElementById("service-call"), {
        title: {
          visible: true,
          text: "接口调用排行",
        },
        yAxis: {
          visible: false,
        },
        forceFit: true,
        data: data,
        xField: "销售额",
        yField: "地区",
        label: {
          visible: true,
          adjustPosition: true,
          formatter: (v) => Math.round(v / 10000),
          position: "left",
        },
        events: {
          onTitleDblClick: (e) => console.log(e),
        },
      });

      barPlot.render();
    },

    init_slow_service: () => {
      const data = [
        { 地区: "UserService", 销售额: 4684506.442 },
        { 地区: "OrderService", 销售额: 4137415.0929999948 },
        { 地区: "Payment", 销售额: 2681567.469000001 },
        { 地区: "Log", 销售额: 2447301.017000004 },
        { 地区: "DataService", 销售额: 1303124.508000002 },
        { 地区: "DataService2", 销售额: 1303124.508000002 },
      ];

      const barPlot = new Bar(document.getElementById("slow-service"), {
        title: {
          visible: true,
          text: "慢接口",
        },
        yAxis: {
          visible: false,
        },
        forceFit: true,
        data,
        xField: "销售额",
        yField: "地区",
        color: ["#9599E2"],
        label: {
          visible: true,
          adjustPosition: true,
          formatter: (v) => Math.round(v / 10000),
          position: "left",
        },
        events: {
          onTitleDblClick: (e) => console.log(e),
        },
      });

      barPlot.render();
    },

    init_error_service: () => {
      const data = [
        { 地区: "UserService", 销售额: 4684506.442 },
        { 地区: "OrderService", 销售额: 4137415.0929999948 },
        { 地区: "Payment", 销售额: 2681567.469000001 },
        { 地区: "Log", 销售额: 2447301.017000004 },
        { 地区: "DataService", 销售额: 1303124.508000002 },
        { 地区: "192.168.1.1:8888", 销售额: 1303124.508000002 },
      ];

      const barPlot = new Bar(document.getElementById("error-service"), {
        title: {
          visible: true,
          text: "错误接口排行",
        },
        yAxis: {
          visible: false,
        },
        forceFit: true,
        data,
        color: ["#FF6A88"],
        xField: "销售额",
        yField: "地区",
        label: {
          visible: true,
          adjustPosition: true,
          formatter: (v) => Math.round(v / 10000),
          position: "left",
        },
        events: {
          onTitleDblClick: (e) => console.log(e),
        },
      });

      barPlot.render();
    },

    init_service_gc: () => {
      const data = [
        { year: "1991", value: 3 },
        { year: "1992", value: 4 },
        { year: "1993", value: 3.5 },
        { year: "1994", value: 5 },
        { year: "1995", value: 4.9 },
        { year: "1996", value: 6 },
        { year: "1997", value: 7 },
        { year: "1998", value: 9 },
        { year: "1999", value: 13 },
      ];

      const linePlot = new Line(document.getElementById("service-gc"), {
        title: {
          visible: true,
          text: "GC",
        },
        forceFit: true,
        padding: "auto",
        data,
        xField: "year",
        yField: "value",
        point: {
          visible: true,
        },
        label: {
          visible: true,
          type: "point",
        },
      });

      linePlot.render();
    },

    init_service_memory: () => {
      const data = [
        { year: "1991", value: 3 },
        { year: "1992", value: 4 },
        { year: "1993", value: 3.5 },
        { year: "1994", value: 5 },
        { year: "1995", value: 4.9 },
        { year: "1996", value: 6 },
        { year: "1997", value: 7 },
        { year: "1998", value: 9 },
        { year: "1999", value: 13 },
      ];

      const linePlot = new Line(document.getElementById("service-memory"), {
        title: {
          visible: true,
          text: "Memory",
        },
        forceFit: true,
        padding: "auto",
        data,
        xField: "year",
        yField: "value",
        point: {
          visible: true,
        },
        label: {
          visible: true,
          type: "point",
        },
      });

      linePlot.render();
    },

    init_service_thread: () => {
      const data = [
        { year: "1991", value: 3 },
        { year: "1992", value: 4 },
        { year: "1993", value: 3.5 },
        { year: "1994", value: 5 },
        { year: "1995", value: 4.9 },
        { year: "1996", value: 6 },
        { year: "1997", value: 7 },
        { year: "1998", value: 9 },
        { year: "1999", value: 13 },
      ];

      const linePlot = new Line(document.getElementById("service-thread"), {
        title: {
          visible: true,
          text: "Thread",
        },
        forceFit: true,
        padding: "auto",
        data,
        xField: "year",
        yField: "value",
        point: {
          visible: true,
        },
        label: {
          visible: true,
          type: "point",
        },
      });

      linePlot.render();
    } 

  },
};
</script>
