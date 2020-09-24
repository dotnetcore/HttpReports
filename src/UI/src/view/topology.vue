<template>
  <div>
    <el-card class="box-card">
      <h3 style="font-size:17px;font-weight: 500;margin-left:30px">{{ $t('ServiceRelationship') }}</h3>
      <div style="height:620px;width:100%" id="main-chart"></div>
    </el-card>
  </div>
</template>

<style>
.box-card {
  margin-top: 20px;
}
</style>

<script>
import G6 from "@antv/g6";
import { mapState } from "vuex";
import Vue from "vue";

export default {
  data() {
    return {
      topology_chart: null,
    };
  },
  created: function () {},
  computed: mapState({
    query: (state) => state.query,
  }),
  activated(){   
    this.$store.commit("set_index_loading_timestamp",Date.parse(new Date()));  
  },
  watch: {
    async query(newVal, oldVal) {
      var response = await this.load_basic_data(); 
      this.load_tololpgy(response);
    },
  },
  methods: {
    async load_basic_data() {
      this.$store.commit("set_topology_loading", true);
      var response = await Vue.http.post(
        "GetTopologyData",
        this.$store.state.query
      );
      this.$store.commit("set_topology_loading", false); 

      return response;
    },
    refreshDragedNodePosition(e) {
      const model = e.item.get("model");
      model.fx = e.x;
      model.fy = e.y;
    },
    load_tololpgy(response) {
      var source = {
        nodes: [],
        edges: [],
      };

      response.body.nodes.forEach((x) => {
        source.nodes.push({
          id: x,
          label: x,
        });
      });

      response.body.edges.forEach((x) => {
        source.edges.push({
          source: x.key,
          target: x.stringValue,
        });
      });

      if (this.topology_chart != null) {
        this.topology_chart.changeData(source);
        return;
      }

      this.topology_chart = new G6.Graph({
        container: "main-chart",
        width: document.getElementById("main-chart").scrollWidth,
        height: document.getElementById("main-chart").scrollHeight,
        layout: {
          type: "force",
          preventOverlap: true,
          nodeSize: 100,
          linkDistance: 120,
        },
        modes: {
          default: ["drag-canvas", "zoom-canvas", "drag-node"],
        },
        defaultNode: {
          size: 50,
          color: "#5B8FF9",
          style: {
            lineWidth: 2,
            fill: "#C6E5FF",
          },
          labelCfg: {
            position: "bottom",
          },
        },
        defaultEdge: {
          size: 1,
          color: "#e2e2e2",
          style: {
            startArrow: true,
          },
        },
      });

      this.topology_chart.data(source);
      this.topology_chart.render();

      this.topology_chart.on("node:dragstart", (e) => {
        this.topology_chart.layout();
        this.refreshDragedNodePosition(e);
      });
      this.topology_chart.on("node:drag", (e) => {
        this.refreshDragedNodePosition(e);
      });


    },
  },
  async mounted() {

    var response = await this.load_basic_data(); 
    this.load_tololpgy(response); 

  }
};
</script>
