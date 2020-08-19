

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

.el-row {
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
  <div>
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
          <i class="el-icon-message-solid icon-rounded pull-left bgc2"></i>

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
          <div id="service-call-line" style="height:270px"></div>
        </el-card>
      </el-col>

      <el-col :span="12">
        <el-card class="box-card" style="margin-top:0">
          <div id="service-call-heap" style="height:270px"></div>
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
    };
  },
  created: () => {},
  computed: mapState({
    query: (state) => state.query,
  }),
  watch: {
    async query(newVal, oldVal) {
      var response = await this.load_basic_data();
      this.basic_data = response.body.data;

      this.load_service_call(response);
      this.load_slow_service(response);
      this.load_error_service(response);

    },
  },
  async mounted() {
    var response = await this.load_basic_data();
    this.basic_data = response.body.data;

    this.load_service_call(response);
    this.load_slow_service(response);
    this.load_error_service(response);
    this.init_service_call_line();
    this.init_service_call_heap();
  },
  methods: {
    async load_basic_data() {
      return await Vue.http.post("GetIndexBasicData", this.$store.state.query);
    },

    load_service_call(response) {
      var source = [];

      response.data.data.top[0].forEach((item) => {
        source.push({
          key: item.item1,
          value: item.item2,
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

        this.service_call_chart.render();
      } else {
        this.service_call_chart.changeData(source);
      }
    },

    load_slow_service(response) {

      var source = [];

      response.data.data.top[1].forEach((item) => {
        source.push({
          key: item.item1,
          value: item.item2,
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

        this.slow_service_chart.render();

      } else {
        this.slow_service_chart.changeData(source);
      }
    },

    load_error_service(response) {

       var source = [];

      response.data.data.top[2].forEach((item) => {
        source.push({
          key: item.item1,
          ms: item.item2,
        });
      });

      if (this.error_service_chart == null) {

        this.error_service_chart = new Bar(
          document.getElementById("error-service"),
          {
            title: {
              visible: true,
              text: this.$store.state.lang.SlowService,
            },
            yAxis: {
              visible: true,
            },
            forceFit: true,
            data: source,
            xField: "ms",
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

        this.error_service_chart.render();

      } else {
        this.error_service_chart.changeData(source);
      } 
      
    },

    init_service_call_heap() {
      const data = [
        { name: "hot dog", value: 70, country: "AD" },
        { name: "burger", value: 54, country: "AD" },
        { name: "sandwich", value: 49, country: "AD" },
        { name: "kebab", value: 4, country: "AD" },
        { name: "fries", value: 11, country: "AD" },
        { name: "donut", value: 68, country: "AD" },
        { name: "junk", value: 49, country: "AD" },
        { name: "sushi", value: 47, country: "AD" },
        { name: "ramen", value: 64, country: "AD" },
        { name: "curry", value: 51, country: "AD" },
        { name: "udon", value: 6, country: "AD" },
        { name: "hot dog", value: 3, country: "AI" },
        { name: "burger", value: 6, country: "AI" },
        { name: "sandwich", value: 34, country: "AI" },
        { name: "kebab", value: 72, country: "AI" },
        { name: "fries", value: 21, country: "AI" },
        { name: "donut", value: 30, country: "AI" },
        { name: "junk", value: 99, country: "AI" },
        { name: "sushi", value: 40, country: "AI" },
        { name: "ramen", value: 1, country: "AI" },
        { name: "curry", value: 70, country: "AI" },
        { name: "udon", value: 58, country: "AI" },
        { name: "hot dog", value: 83, country: "AO" },
        { name: "burger", value: 6, country: "AO" },
        { name: "sandwich", value: 17, country: "AO" },
        { name: "kebab", value: 40, country: "AO" },
        { name: "fries", value: 61, country: "AO" },
        { name: "donut", value: 72, country: "AO" },
        { name: "junk", value: 61, country: "AO" },
        { name: "sushi", value: 50, country: "AO" },
        { name: "ramen", value: 77, country: "AO" },
        { name: "curry", value: 97, country: "AO" },
        { name: "udon", value: 17, country: "AO" },
        { name: "hot dog", value: 15, country: "AQ" },
        { name: "burger", value: 34, country: "AQ" },
        { name: "sandwich", value: 26, country: "AQ" },
        { name: "kebab", value: 80, country: "AQ" },
        { name: "fries", value: 100, country: "AQ" },
        { name: "donut", value: 97, country: "AQ" },
        { name: "junk", value: 34, country: "AQ" },
        { name: "sushi", value: 81, country: "AQ" },
        { name: "ramen", value: 25, country: "AQ" },
        { name: "curry", value: 100, country: "AQ" },
        { name: "udon", value: 56, country: "AQ" },
        { name: "hot dog", value: 15, country: "AQ1" },
        { name: "burger", value: 34, country: "AQ1" },
        { name: "sandwich", value: 26, country: "AQ1" },
        { name: "kebab", value: 80, country: "AQ1" },
        { name: "fries", value: 100, country: "AQ1" },
        { name: "donut", value: 97, country: "AQ1" },
        { name: "junk", value: 34, country: "AQ1" },
        { name: "sushi", value: 81, country: "AQ1" },
        { name: "ramen", value: 25, country: "AQ1" },
        { name: "curry", value: 100, country: "AQ1" },
        { name: "udon", value: 56, country: "AQ1" },
        { name: "hot dog", value: 15, country: "AQ2" },
        { name: "burger", value: 34, country: "AQ2" },
        { name: "sandwich", value: 26, country: "AQ2" },
        { name: "kebab", value: 80, country: "AQ2" },
        { name: "fries", value: 100, country: "AQ2" },
        { name: "donut", value: 97, country: "AQ2" },
        { name: "junk", value: 34, country: "AQ2" },
        { name: "sushi", value: 81, country: "AQ2" },
        { name: "ramen", value: 25, country: "AQ2" },
        { name: "curry", value: 100, country: "AQ2" },
        { name: "udon", value: 56, country: "AQ2" },
        { name: "hot dog", value: 15, country: "AQ3" },
        { name: "burger", value: 34, country: "AQ3" },
        { name: "sandwich", value: 26, country: "AQ3" },
        { name: "kebab", value: 80, country: "AQ3" },
        { name: "fries", value: 100, country: "AQ3" },
        { name: "donut", value: 97, country: "AQ3" },
        { name: "junk", value: 34, country: "AQ3" },
        { name: "sushi", value: 81, country: "AQ3" },
        { name: "ramen", value: 25, country: "AQ3" },
        { name: "curry", value: 160, country: "AQ3" },
        { name: "udon", value: 56, country: "AQ3" },
      ];

      var heatmapPlot = new Heatmap(
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
          shapeType: "rect",
          data,
          xField: "name",
          yField: "country",
          colorField: "value",
          color: ["#F3F3FF", "#9599E2"],
        }
      );

      heatmapPlot.render();
    },
    init_service_call_line() {
      var data = [
        {
          name: "China",
          year: "2000",
          gdp: 1211346869605.24,
        },
        {
          name: "China",
          year: "2001",
          gdp: 1339395718865.3,
        },
        {
          name: "China",
          year: "2002",
          gdp: 1470550015081.55,
        },
        {
          name: "China",
          year: "2003",
          gdp: 1660287965662.68,
        },
        {
          name: "China",
          year: "2004",
          gdp: 1955347004963.27,
        },
        {
          name: "China",
          year: "2005",
          gdp: 2285965892360.54,
        },
        {
          name: "China",
          year: "2006",
          gdp: 2752131773355.16,
        },
        {
          name: "China",
          year: "2007",
          gdp: 3550342425238.25,
        },
        {
          name: "China",
          year: "2008",
          gdp: 4594306848763.08,
        },
        {
          name: "China",
          year: "2009",
          gdp: 5101702432883.45,
        },
        {
          name: "China",
          year: "2010",
          gdp: 6087164527421.24,
        },
        {
          name: "China",
          year: "2011",
          gdp: 7551500425597.77,
        },
        {
          name: "China",
          year: "2012",
          gdp: 8532230724141.76,
        },
        {
          name: "China",
          year: "2013",
          gdp: 9570405758739.79,
        },
        {
          name: "China",
          year: "2014",
          gdp: 10438529153237.6,
        },
        {
          name: "China",
          year: "2015",
          gdp: 11015542352468.9,
        },
        {
          name: "China",
          year: "2016",
          gdp: 11137945669350.6,
        },
        {
          name: "China",
          year: "2017",
          gdp: 12143491448186.1,
        },
        {
          name: "China",
          year: "2018",
          gdp: 13608151864637.9,
        },
        {
          name: "United States",
          year: "2000",
          gdp: 10252345464000,
        },
        {
          name: "United States",
          year: "2001",
          gdp: 10581821399000,
        },
        {
          name: "United States",
          year: "2002",
          gdp: 10936419054000,
        },
        {
          name: "United States",
          year: "2003",
          gdp: 11458243878000,
        },
        {
          name: "United States",
          year: "2004",
          gdp: 12213729147000,
        },
        {
          name: "United States",
          year: "2005",
          gdp: 13036640229000,
        },
        {
          name: "United States",
          year: "2006",
          gdp: 13814611414000,
        },
        {
          name: "United States",
          year: "2007",
          gdp: 14451858650000,
        },
        {
          name: "United States",
          year: "2008",
          gdp: 14712844084000,
        },
        {
          name: "United States",
          year: "2009",
          gdp: 14448933025000,
        },
        {
          name: "United States",
          year: "2010",
          gdp: 14992052727000,
        },
        {
          name: "United States",
          year: "2011",
          gdp: 15542581104000,
        },
        {
          name: "United States",
          year: "2012",
          gdp: 16197007349000,
        },
        {
          name: "United States",
          year: "2013",
          gdp: 16784849190000,
        },
        {
          name: "United States",
          year: "2014",
          gdp: 17521746534000,
        },
        {
          name: "United States",
          year: "2015",
          gdp: 18219297584000,
        },
        {
          name: "United States",
          year: "2016",
          gdp: 18707188235000,
        },
        {
          name: "United States",
          year: "2017",
          gdp: 19485393853000,
        },
        {
          name: "United States",
          year: "2018",
          gdp: 20544343456936.5,
        },
        {
          name: "United Kingdom",
          year: "2000",
          gdp: 1657816613708.58,
        },
        {
          name: "United Kingdom",
          year: "2001",
          gdp: 1640246149417.01,
        },
        {
          name: "United Kingdom",
          year: "2002",
          gdp: 1784473920863.31,
        },
        {
          name: "United Kingdom",
          year: "2003",
          gdp: 2053018775510.2,
        },
        {
          name: "United Kingdom",
          year: "2004",
          gdp: 2416931526913.22,
        },
        {
          name: "United Kingdom",
          year: "2005",
          gdp: 2538680000000,
        },
        {
          name: "United Kingdom",
          year: "2006",
          gdp: 2713749770009.2,
        },
        {
          name: "United Kingdom",
          year: "2007",
          gdp: 3100882352941.18,
        },
        {
          name: "United Kingdom",
          year: "2008",
          gdp: 2922667279411.76,
        },
        {
          name: "United Kingdom",
          year: "2009",
          gdp: 2410909799034.12,
        },
        {
          name: "United Kingdom",
          year: "2010",
          gdp: 2475244321361.11,
        },
        {
          name: "United Kingdom",
          year: "2011",
          gdp: 2659310054646.23,
        },
        {
          name: "United Kingdom",
          year: "2012",
          gdp: 2704887678386.72,
        },
        {
          name: "United Kingdom",
          year: "2013",
          gdp: 2786022872706.81,
        },
        {
          name: "United Kingdom",
          year: "2014",
          gdp: 3063803240208.01,
        },
        {
          name: "United Kingdom",
          year: "2015",
          gdp: 2928591002002.51,
        },
        {
          name: "United Kingdom",
          year: "2016",
          gdp: 2694283209613.29,
        },
        {
          name: "United Kingdom",
          year: "2017",
          gdp: 2666229179958.01,
        },
        {
          name: "United Kingdom",
          year: "2018",
          gdp: 2855296731521.96,
        },
        {
          name: "Russian",
          year: "2000",
          gdp: 259710142196.94,
        },
        {
          name: "Russian",
          year: "2001",
          gdp: 306602070620.5,
        },
        {
          name: "Russian",
          year: "2002",
          gdp: 345470494417.86,
        },
        {
          name: "Russian",
          year: "2003",
          gdp: 430347770731.79,
        },
        {
          name: "Russian",
          year: "2004",
          gdp: 591016690742.8,
        },
        {
          name: "Russian",
          year: "2005",
          gdp: 764017107992.39,
        },
        {
          name: "Russian",
          year: "2006",
          gdp: 989930542278.7,
        },
        {
          name: "Russian",
          year: "2007",
          gdp: 1299705764823.62,
        },
        {
          name: "Russian",
          year: "2008",
          gdp: 1660846387624.78,
        },
        {
          name: "Russian",
          year: "2009",
          gdp: 1222644282201.86,
        },
        {
          name: "Russian",
          year: "2010",
          gdp: 1524917468442.01,
        },
        {
          name: "Russian",
          year: "2011",
          gdp: 2051661732059.78,
        },
        {
          name: "Russian",
          year: "2012",
          gdp: 2210256976945.38,
        },
        {
          name: "Russian",
          year: "2013",
          gdp: 2297128039058.21,
        },
        {
          name: "Russian",
          year: "2014",
          gdp: 2059984158438.46,
        },
        {
          name: "Russian",
          year: "2015",
          gdp: 1363594369577.82,
        },
        {
          name: "Russian",
          year: "2016",
          gdp: 1282723881134.01,
        },
        {
          name: "Russian",
          year: "2017",
          gdp: 1578624060588.26,
        },
        {
          name: "Russian",
          year: "2018",
          gdp: 1657554647149.87,
        },
        {
          name: "Japan",
          year: "2000",
          gdp: 4887519660744.86,
        },
        {
          name: "Japan",
          year: "2001",
          gdp: 4303544259842.72,
        },
        {
          name: "Japan",
          year: "2002",
          gdp: 4115116279069.77,
        },
        {
          name: "Japan",
          year: "2003",
          gdp: 4445658071221.86,
        },
        {
          name: "Japan",
          year: "2004",
          gdp: 4815148854362.11,
        },
        {
          name: "Japan",
          year: "2005",
          gdp: 4755410630912.14,
        },
        {
          name: "Japan",
          year: "2006",
          gdp: 4530377224970.4,
        },
        {
          name: "Japan",
          year: "2007",
          gdp: 4515264514430.57,
        },
        {
          name: "Japan",
          year: "2008",
          gdp: 5037908465114.48,
        },
        {
          name: "Japan",
          year: "2009",
          gdp: 5231382674593.7,
        },
        {
          name: "Japan",
          year: "2010",
          gdp: 5700098114744.41,
        },
        {
          name: "Japan",
          year: "2011",
          gdp: 6157459594823.72,
        },
        {
          name: "Japan",
          year: "2012",
          gdp: 6203213121334.12,
        },
        {
          name: "Japan",
          year: "2013",
          gdp: 5155717056270.83,
        },
        {
          name: "Japan",
          year: "2014",
          gdp: 4850413536037.84,
        },
        {
          name: "Japan",
          year: "2015",
          gdp: 4389475622588.97,
        },
        {
          name: "Japan",
          year: "2016",
          gdp: 4926667087367.51,
        },
        {
          name: "Japan",
          year: "2017",
          gdp: 4859950558538.97,
        },
        {
          name: "Japan",
          year: "2018",
          gdp: 4971323079771.87,
        },
      ];

      const linePlot = new Line(document.getElementById("service-call-line"), {
        title: {
          visible: true,
          text: this.$store.state.lang.ServiceLoad,
        },
        padding: [20, 100, 30, 80],
        forceFit: true,
        data,
        xField: "year",
        yField: "gdp",
        seriesField: "name",
        xAxis: {
          type: "dateTime",
          label: {
            visible: true,
            autoHide: true,
          },
        },
        animation: {
          appear: {
            animation: "clipingWithData",
          },
        },
        yAxis: {
          formatter: (v) => `${(v / 10e8).toFixed(1)} B`,
        },
        legend: {
          visible: false,
        },
        label: {
          visible: true,
          type: "line",
        },
        smooth: true,
      });

      linePlot.render();
    },
  },
};
</script>



