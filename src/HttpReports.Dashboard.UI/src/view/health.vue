<template>
  <div>
    <el-card class="box-card" style="min-height:620px">
      <el-collapse @change="handleChange" style="padding-left: 10px" accordion>
        <el-collapse-item v-for="item in list" :key="item" > 

          <template slot="title">

            <span style="margin-right:16px;min-width:160px;font-weight:bolder">{{ item.title }}</span>  
            <i style="font-size:20px;color:green;margin-left:18px;margin-right:2px" class="el-icon-success"></i> 
            <span style="color:green">{{ item.passing }}</span>
 
            <i style="font-size:20px;color:#e6a23c;margin-left:18px;margin-right:2px" class="el-icon-warning"></i> 
            <span style="color:#e6a23c">{{ item.warning }}</span>

             <i style="font-size:20px;color:#f56c6c;margin-left:18px;margin-right:2px" class="el-icon-error"></i> 
            <span style="color:#f56c6c">{{ item.critical  }}</span>

         </template>

          <el-row :gutter="12" class="instances">

            <el-col :span="6" v-for="k in item.instances" :key="k"> 

              <el-card shadow="always" > 

                <span> {{ k.instance }} </span> 

                <i v-if="k.isPassing" style="font-size:20px;color:green;float:right" class="el-icon-success"></i> 

                <i v-if="k.isWarning" style="font-size:20px;color:#e6a23c;float:right" class="el-icon-warning"></i> 

                <i v-if="k.isCritical" style="font-size:20px;color:#f56c6c;float:right" class="el-icon-error"></i> 

              </el-card>

            </el-col> 

          </el-row>

        </el-collapse-item>
      </el-collapse>
    </el-card>
  </div>
</template>

<style>
.box-card {
  margin-top: 20px;
}

.instances .el-card{
  margin-top:16px; 
}

</style>

<script>
import Vue from "vue";
import { mapState } from "vuex";

export default {
  data() {
    return {
      list:[]
    };
  },
  created: function () {},
  computed: mapState({
    query: (state) => state.query,
  }),
  activated() {
    //this.$store.commit("set_index_loading_timestamp",Date.parse(new Date()));
  },
  watch: {
    async query(newVal, oldVal) {
      var response = await this.load_basic_data();
      this.load_list(response);
    },
  },
  methods: { 
    handleChange(val) {
      console.log(val);
    },
    async load_basic_data() {

      this.$store.commit("set_health_loading", true);

      var response = await Vue.http.post(
        "GetHealthCheck",
        this.$store.state.query
      );

      this.$store.commit("set_health_loading", false);

      return response;
    },

    load_list(response) { 

       this.list = []; 

        response.body.data.forEach(x => {

          var ins = [];  

          x.instances.forEach(y => {

            ins.push({
              instance:y.instance,
              isPassing:y.status == 1,
              isWarning:y.status == 2,
              isCritical:y.status == 3
            });
            
          }); 

           this.list.push({ 
              title: x.serviceInfo.service,
              passing: x.serviceInfo.passing,
              warning: x.serviceInfo.warning,
              critical: x.serviceInfo.critical, 
              instances: ins
           });
          
        }); 

    },
  },
  async mounted() {
    var response = await this.load_basic_data();
    this.load_list(response);
  },
};
</script>
