<template>
  <div class="vue-service">
    <el-tabs style="margin-top:10px">
      <el-tab-pane :label="this.$store.state.lang.EndpointTab">
        <el-row class="box-chart" :gutter="20">
          <el-col :span="8">
            <el-card class="box-card" style="margin-top:0">
              <div id="endpoint_call_chart" style="height:300px"></div>
            </el-card>
          </el-col>

          <el-col :span="8">
            <el-card class="box-card" style="margin-top:0">
              <div id="slow_endpoint_chart" style="height:300px"></div>
            </el-card>
          </el-col>

          <el-col :span="8">
            <el-card class="box-card" style="margin-top:0">
              <div id="error_endpoint_chart" style="height:300px"></div>
            </el-card>
          </el-col>
        </el-row>
      </el-tab-pane>

      <el-tab-pane :label="this.$store.state.lang.InstanceTab">
        <el-row class="box-chart" :gutter="20">
          <el-col :span="8">
            <el-card class="box-card" style="margin-top:0">
              <div id="instance_call_chart" style="height:300px"></div>
            </el-card>
          </el-col>

          <el-col :span="8">
            <el-card class="box-card" style="margin-top:0">
              <div id="slow_instance_chart" style="height:300px"></div>
            </el-card>
          </el-col>

          <el-col :span="8">
            <el-card class="box-card" style="margin-top:0">
              <div id="error_instance_chart" style="height:300px"></div>
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
.vue-service .el-row {
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
      endpoint_call_chart: null,
      slow_endpoint_chart: null,
      error_endpoint_chart: null,
      instance_call_chart: null,
      slow_instance_chart: null,
      error_instance_chart: null,
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

       this.load_endpoint_call_chart(response.body.data);
       this.load_slow_endpoint_chart(response.body.data);
       this.load_error_endpoint_chart(response.body.data);

       this.load_instance_call_chart(response.body.data);
       this.load_slow_instance_chart(response.body.data);
       this.load_error_instance_chart(response.body.data);

       this.load_service_gc(response.body.data); 
       this.load_service_memory(response.body.data); 
       this.load_service_thread(response.body.data); 
     
    }, 
  }, 
  async mounted() {

    var response = await this.load_basic_data(); 

    this.load_endpoint_call_chart(response.body.data);
    this.load_slow_endpoint_chart(response.body.data);
    this.load_error_endpoint_chart(response.body.data);

    this.load_instance_call_chart(response.body.data);
    this.load_slow_instance_chart(response.body.data);
    this.load_error_instance_chart(response.body.data);

    this.load_service_gc(response.body.data); 
    this.load_service_memory(response.body.data); 
    this.load_service_thread(response.body.data); 
  }, 
  methods: {

    async load_basic_data() {    
     
      this.$store.commit("set_service_loading",true);  
      var data = await Vue.http.post("GetServiceBasicData", this.$store.state.query);   
      this.$store.commit("set_service_loading",false);     

      console.log(data)
      return data;

    },  

    load_endpoint_call_chart(data) {  

      var source = [];  

      data.endpoint[0].forEach((item) => {
        source.push({
          key: item.key,
          value: item.value,
        });
      });  
     
      if (this.endpoint_call_chart == null) {
        this.endpoint_call_chart = new Bar(
          document.getElementById("endpoint_call_chart"),
          {
            title: {
              visible: true,
              text: this.$store.state.lang.EndpointCall,
            },
            yAxis: {
              visible: true,
            },
            forceFit: true,
            data: source,
            xField: "value",
            yField: "key",
            label: {
              visible: true,
              adjustPosition: true,
              formatter: (v) => v,
              position: "left",
            },
            events: {
              onTitleDblClick: (e) => console.log(e),
            },
          }
        );

        this.endpoint_call_chart.render();
      } else {
        this.endpoint_call_chart.changeData(source);
      }      

    },

    load_slow_endpoint_chart(data) {

      var source = []; 

      data.endpoint[1].forEach((item) => {
        source.push({
          key: item.key,
          value: item.value,
        });
      });  
     
      if (this.slow_endpoint_chart == null) {
        this.slow_endpoint_chart = new Bar(
          document.getElementById("slow_endpoint_chart"),
          {
            title: {
              visible: true,
              text: this.$store.state.lang.SlowEndpoint,
            },
            yAxis: {
              visible: true,
            },
            forceFit: true,
            data: source,
            xField: "value",
            yField: "key",
            color: ["#9599E2"],
            label: {
              visible: true,
              adjustPosition: true,
              formatter: (v) => v,
              position: "left",
            },
            events: {
              onTitleDblClick: (e) => console.log(e),
            },
          }
        );

        this.slow_endpoint_chart.render();
      } else {
        this.slow_endpoint_chart.changeData(source);
      } 

    },

    load_error_endpoint_chart(data){
      
      var source = []; 

      data.endpoint[2].forEach((item) => {
        source.push({
          key: item.key,
          value: item.value,
        });
      });  
     
      if (this.error_endpoint_chart == null) {
        this.error_endpoint_chart = new Bar(
          document.getElementById("error_endpoint_chart"),
          {
            title: {
              visible: true,
              text: this.$store.state.lang.ErrorEndpoint,
            },
            yAxis: {
              visible: true,
            },
            forceFit: true,
            data: source,
            xField: "value",
            yField: "key",
            color: ["#FF6A88"],
            label: {
              visible: true,
              adjustPosition: true,
              formatter: (v) => v,
              position: "left",
            },
            events: {
              onTitleDblClick: (e) => console.log(e),
            },
          }
        );

        this.error_endpoint_chart.render();
      } else {
        this.error_endpoint_chart.changeData(source);
      } 

    },

    load_instance_call_chart(data){

       var source = [];  

      data.instance[0].forEach((item) => {
        source.push({
          key: item.key,
          value: item.value,
        });
      });  
     
      if (this.instance_call_chart == null) {
        this.instance_call_chart = new Bar(
          document.getElementById("instance_call_chart"),
          {
            title: {
              visible: true,
              text: this.$store.state.lang.InstanceCall,
            },
            yAxis: {
              visible: true,
            },
            forceFit: true,
            data: source,
            xField: "value",
            yField: "key",
            label: {
              visible: true,
              adjustPosition: true,
              formatter: (v) => v,
              position: "left",
            },
            events: {
              onTitleDblClick: (e) => console.log(e),
            },
          }
        );

        this.instance_call_chart.render();
      } else {
        this.instance_call_chart.changeData(source);
      }      
    },

    load_slow_instance_chart(data){

        var source = []; 

      data.instance[1].forEach((item) => {
        source.push({
          key: item.key,
          value: item.value,
        });
      });  
     
      if (this.slow_instance_chart == null) {
        this.slow_instance_chart = new Bar(
          document.getElementById("slow_instance_chart"),
          {
            title: {
              visible: true,
              text: this.$store.state.lang.SlowInstance,
            },
            yAxis: {
              visible: true,
            },
            forceFit: true,
            data: source,
            xField: "value",
            yField: "key",
            color: ["#9599E2"],
            label: {
              visible: true,
              adjustPosition: true,
              formatter: (v) => v,
              position: "left",
            },
            events: {
              onTitleDblClick: (e) => console.log(e),
            },
          }
        );

        this.slow_instance_chart.render();
      } else {
        this.slow_instance_chart.changeData(source);
      } 
    },

    load_error_instance_chart(data){
       
      var source = []; 

      data.instance[2].forEach((item) => {
        source.push({
          key: item.key,
          value: item.value,
        });
      });  
     
      if (this.error_instance_chart == null) {
        this.error_instance_chart = new Bar(
          document.getElementById("error_instance_chart"),
          {
            title: {
              visible: true,
              text: this.$store.state.lang.ErrorInstance,
            },
            yAxis: {
              visible: true,
            },
            forceFit: true,
            data: source,
            xField: "value",
            yField: "key",
            color: ["#FF6A88"],
            label: {
              visible: true,
              adjustPosition: true,
              formatter: (v) => v,
              position: "left",
            },
            events: {
              onTitleDblClick: (e) => console.log(e),
            },
          }
        );

        this.error_instance_chart.render();
      } else {
        this.error_instance_chart.changeData(source);
      } 
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
