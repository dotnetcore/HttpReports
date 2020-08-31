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
import { mapState } from "vuex";
import Vue from "vue";

export default {
  data() {
    return {
      isCollapse: false,
      userName: "",
      service_gc_chart:null,
      service_memory_chart:null,
      service_thread_chart:null
    };
  }, 
computed: mapState({
    query: (state) => state.query 
  }),
  watch: {  
    async query(newVal, oldVal) {  

       var response = await this.load_basic_data();    
       this.load_service_gc(response.body.data); 
       this.load_service_memory(response.body.data); 
       this.load_service_thread(response.body.data); 
     
    }, 
  }, 
  async mounted() {

    var response = await this.load_basic_data(); 

    this.init_service_call1();
    this.init_slow_service1();
    this.init_error_service1();

    this.init_service_call();
    this.init_slow_service();
    this.init_error_service();

    this.load_service_gc(response.body.data); 
    this.load_service_memory(response.body.data); 
    this.load_service_thread(response.body.data); 
  }, 
  methods: {

    async load_basic_data() {    
     
      this.$store.commit("set_service_loading",true);  
      var data = await Vue.http.post("GetServiceBasicData", this.$store.state.query);   
      this.$store.commit("set_service_loading",false);   
      return data;

    }, 


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

    load_service_gc(data){ 
      
      var source = [];  

      data.app.forEach((item) => { 

        source.push({
          key: "GcGen0",
          time: item.timeField,
          value: item.gcGen0,
        });

        source.push({
          key: "GcGen1",
          time: item.timeField,
          value: item.gcGen1,
        });

        source.push({
          key: "GcGen2",
          time: item.timeField,
          value: item.gcGen2,
        }); 

      });  

      if (this.service_gc_chart == null) {
        this.service_gc_chart = new Line(
          document.getElementById("service-gc"),
          {
            title: {
              visible: true,
              text: "GC",
            },

            forceFit: true,
            data: source,
            xField: "time",
            yField: "value",
            seriesField: "key",
            xAxis: {
              type: "dateTime",
              label: {
                visible: true,
                autoHide: true,
                autoRotate: false,
              },
            },
            animation: {
              appear: {
                animation: "clipingWithData",
              },
            },
            yAxis: {
              formatter: (v) => v,
            },
            legend: {
              visible: false,
            },
            label: {
              visible: true,
              type: "line",
            },
            smooth: true,
          }
        );

        this.service_gc_chart.render();
      } else {
        this.service_gc_chart.changeData(source);
      }

    },

    load_service_memory(data) {
 
      var source = [];   

      data.app.forEach((item) => { 

        source.push({
          key: "HeapMemory",
          time: item.timeField,
          value: item.heapMemory,
        });

        source.push({
          key: "ProcessMemory",
          time: item.timeField,
          value: item.processMemory,
        }); 

      });  

      if (this.service_memory_chart == null) {
        this.service_memory_chart = new Line(
          document.getElementById("service-memory"),
          {
            title: {
              visible: true,
              text: "Memory",
            },

            forceFit: true,
            data: source,
            xField: "time",
            yField: "value",
            seriesField: "key",
            xAxis: {
              type: "dateTime",
              label: {
                visible: true,
                autoHide: true,
                autoRotate: false,
              },
            },
            animation: {
              appear: {
                animation: "clipingWithData",
              },
            },
            yAxis: {
              formatter: (v) => v,
            },
            legend: {
              visible: false,
            },
            label: {
              visible: true,
              type: "line",
            },
            smooth: true,
          }
        );

        this.service_memory_chart.render();
      } else {
        this.service_memory_chart.changeData(source);
      } 
 
    },

    load_service_thread(data){ 
      
      var source = [];   

      data.app.forEach((item) => { 

        source.push({
          key: "Thread",
          time: item.timeField,
          value: item.threadCount,
        });
 

      });  

      if (this.service_thread_chart == null) {
        this.service_thread_chart = new Line(
          document.getElementById("service-thread"),
          {
            title: {
              visible: true,
              text: "Thread",
            },

            forceFit: true,
            data: source,
            xField: "time",
            yField: "value",
            seriesField: "key",
            xAxis: {
              type: "dateTime",
              label: {
                visible: true,
                autoHide: true,
                autoRotate: false,
              },
            },
            animation: {
              appear: {
                animation: "clipingWithData",
              },
            },
            yAxis: {
              formatter: (v) => v,
            },
            legend: {
              visible: false,
            },
            label: {
              visible: true,
              type: "line",
            },
            smooth: true,
          }
        );

        this.service_thread_chart.render();
      } else {
        this.service_thread_chart.changeData(source);
      }  

    } 

  },
};
</script>
