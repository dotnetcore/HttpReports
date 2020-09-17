

<style>
.center-chart .el-card__body {
  padding: 4px;
}

.chart-bottom .el-card__body {
  padding: 4px;
}

.tag .box-card {
  margin-top: 0;
}

.el-carousel__item h3 {
  color: #475669;
  font-size: 18px;
  opacity: 0.75;
  line-height: 300px;
  margin: 0;
}

.el-carousel__item:nth-child(2n) {
  background-color: #99a9bf;
}

.el-carousel__item:nth-child(2n + 1) {
  background-color: #d3dce6;
}

.vue-basic .el-row {
  margin-top: 20px;
  margin-bottom: 20px;
}

.el-row:last-child {
  margin-bottom: 0;
}

.el-col {
  border-radius: 4px;
}

.bg-purple-dark {
  background: #99a9bf;
}

.bg-purple {
  background: #d3dce6;
}

.bg-purple-light {
  background: #e5e9f2;
}

.grid-content {
  border-radius: 4px;
  min-height: 36px;
  min-height: 220px;
}

.row-bg {
  padding: 10px 0;
  background-color: #f9fafc;
}
</style> 

 
<template>
  <div class="vue-basic">
    <el-row :gutter="20" class="tag">
      <el-col :span="6">
        <el-card class="box-card">
          <i class="el-icon-s-promotion icon-rounded pull-left bgc1"></i>

          <div class="stats">
            <h5>
              <strong>{{ this.basic_data.total }}</strong>
            </h5>
            <span>{{ this.$store.state.lang.Index_RequestCount }}</span>
          </div>
        </el-card>
      </el-col>

      <el-col :span="6">
        <el-card class="box-card">
          <i class="el-icon-close icon-rounded pull-left bgc2"></i>

          <div class="stats">
            <h5>
              <strong>{{ this.basic_data.serverError }}</strong>
            </h5>
            <span>{{ this.$store.state.lang.Index_Errors }}</span>
          </div>
        </el-card>
      </el-col>

      <el-col :span="6">
        <el-card class="box-card">
          <i class="el-icon-star-on icon-rounded pull-left bgc3"></i>

          <div class="stats">
            <h5>
              <strong>{{ this.basic_data.service }}</strong>
            </h5>
            <span>{{ this.$store.state.lang.ServiceTag }}</span>
          </div>
        </el-card>
      </el-col>

      <el-col :span="6">
        <el-card class="box-card">
          <i class="el-icon-menu icon-rounded pull-left bgc4"></i>

          <div class="stats">
            <h5>
              <strong>{{ this.basic_data.instance }}</strong>
            </h5>
            <span>{{ this.$store.state.lang.InstanceTag }}</span>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-row class="center-chart" :gutter="20">
      <el-col :span="8">
        <el-card class="box-card" style="margin-top:0">
          <div id="service-call" style="height:280px"></div>
        </el-card>
      </el-col>

      <el-col :span="8">
        <el-card class="box-card" style="margin-top:0">
          <div id="slow-service" style="height:280px"></div>
        </el-card>
      </el-col>

      <el-col :span="8">
        <el-card class="box-card" style="margin-top:0">
          <div id="error-service" style="height:280px"></div>
        </el-card>
      </el-col>
    </el-row>

    <el-row class="chart-bottom" :gutter="20">
      <el-col :span="12">
        <el-card class="box-card" style="margin-top:0">
          <div id="service-call-line" style="height:280px"></div>
        </el-card>
      </el-col>

      <el-col :span="12">
        <el-card class="box-card" style="margin-top:0">
          <div id="service-call-heap" style="height:280px"></div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>  

<script>
import { Line } from "@antv/g2plot";
import { Bar } from "@antv/g2plot";
import { Chart } from "@antv/g2";
import { Heatmap } from "@antv/g2plot";
import Index from "./index";
import { mapState } from "vuex";
import Vue from "vue";

export default {
  data() {
    return {
      basic_data: {
        total: 0,
        serverError: 0,
        service: 0,
        instance: 0,
      }, 
      service_call_chart: null,
      slow_service_chart: null,
      error_service_chart: null,
      service_call_line_chart: null,
      service_heatmap_chart: null,
    };
  }, 
  computed: mapState({
    query: (state) => state.query 
  }),
  watch: { 

    async query(newVal, oldVal) {  
       
      var response = await this.load_basic_data();  

      this.load_board_data(response);
      this.load_service_call(response);
      this.load_slow_service(response);
      this.load_error_service(response);
      this.load_service_call_line(response);
      this.load_service_call_heap(response);

    },
  },
  async mounted() {      
      
    var response = await this.load_basic_data();    
      
    this.load_board_data(response);
    this.load_service_call(response);
    this.load_slow_service(response);
    this.load_error_service(response);
    this.load_service_call_line(response);
    this.load_service_call_heap(response);
    
  },

activated(){   
  this.$store.commit("set_basic_loading",false);  
}, 
deactivated(){
 
},

  methods: {
    async load_basic_data() {      

      this.$store.commit("set_basic_loading",true);  
      var response = await Vue.http.post("GetIndexBasicData", this.$store.state.query);  
      this.$store.commit("set_basic_loading",false);    
      return response;

    }, 
    load_board_data(response){

      var data = response.body.data;

      this.basic_data.total = data.total;
      this.basic_data.serverError = data.serverError;
      this.basic_data.service = data.service;
      this.basic_data.instance = data.instance; 

    }, 
    load_service_call(response) {

      var source = []; 

      response.body.data.top[0].forEach((item) => {
        source.push({
          key: item.key,
          value: item.value,
        });
      }); 

     
      if (this.service_call_chart == null) {
        this.service_call_chart = new Bar(
          document.getElementById("service-call"),
          {
            title: {
              visible: true,
              text: this.$store.state.lang.ServiceLoad,
            },
            yAxis: {
              visible: true,
            },
            //barSize:2,
            forceFit: true,
            data: source,
            xField: "value",
            yField: "key",
            label: {
              visible: true, 
              adjustPosition: true,
              formatter: (v) => v,
              position: "right",
            },
            events: {
              onTitleDblClick: (e) => console.log(e),
            },
          }
        );

        this.service_call_chart.render();
      } else {
        this.service_call_chart.changeData(source);
      }
    },

    load_slow_service(response) {
      var source = [];

      response.body.data.top[1].forEach((item) => {
        source.push({
          key: item.key,
          value: item.value,
        });
      });

      if (this.slow_service_chart == null) {
        this.slow_service_chart = new Bar(
          document.getElementById("slow-service"),
          {
            title: {
              visible: true,
              text: this.$store.state.lang.SlowService,
            },
            yAxis: {
              visible: true,
            },
            //barSize:2,
            forceFit: true,
            data: source,
            xField: "value",
            yField: "key",
            color: ["#9599E2"],
            label: {
              visible: true,
              adjustPosition: true,
              formatter: (v) => v,
              position: "right",
            },
            events: {
              onTitleDblClick: (e) => console.log(e),
            },
          }
        );

        this.slow_service_chart.render();
      } else {
        this.slow_service_chart.changeData(source);
      }
    },

    load_error_service(response) {

      var source = []; 
      response.body.data.top[2].forEach((item) => {
        source.push({
          key: item.key,
          ms: item.value,
        });
      });

      if (this.error_service_chart == null) {
        this.error_service_chart = new Bar(
          document.getElementById("error-service"),
          {
            title: {
              visible: true,
              text: this.$store.state.lang.ErrorService,
            },
            yAxis: {
              visible: true,
            },
            //barSize:2,
            forceFit: true,
            data: source,
            xField: "ms",
            yField: "key",
            color: ["#FF6A88"],
            label: {
              visible: true,
              adjustPosition: true,
              formatter: (v) => v,
              position: "right",
            },
            events: {
              onTitleDblClick: (e) => console.log(e),
            },
          }
        );

        this.error_service_chart.render();
      } else {
        this.error_service_chart.changeData(source);
      }
    },

    load_service_call_line(response) {
      
      var source = [];

      response.body.data.trend.forEach((item) => {
        source.push({
          service: item.keyField,
          time: item.timeField,
          value: item.valueField,
        });
      });

      if (this.service_call_line_chart == null) {
        this.service_call_line_chart = new Line(
          document.getElementById("service-call-line"),
          {
            title: {
              visible: true,
              text: this.$store.state.lang.ServiceLoad,
            },

            forceFit: true,
            data: source,
            xField: "time",
            yField: "value",
            seriesField: "service",
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

        this.service_call_line_chart.render();
      } else {
        this.service_call_line_chart.changeData(source);
      }
    },

    load_service_call_heap(response) {

      var source = []; 

      response.body.data.heatMap.forEach((item) => {
        source.push({
          time: item.timeField,
          span: item.keyField,
          value: item.valueField,
        });
      });

      if (this.service_heatMap_chart == null) {
        this.service_heatMap_chart = new Heatmap(
          document.getElementById("service-call-heap"),
          {
            title: {
              visible: true,
              text: this.$store.state.lang.Index_Heatmap,
            },
            legend: {
              visible: false,
            },
            label: {
              visible: false,
            },
            xAxis: {
              type: "dateTime",
              label: {
                visible: true,
                autoHide: true,
                autoRotate: false,
              },
            },
            shapeType: "rect",
            data:source,
            xField: "time",
            yField: "span",
            colorField: "value",
            color: ["#F3F3FF", "#9599E2"],
          }
        );
 
  
        this.service_heatMap_chart.render();
      } else { 
        this.service_heatMap_chart.changeData(source);
      } 
    },
  },
};
</script>



