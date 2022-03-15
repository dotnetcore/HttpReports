
<style>
@media screen and (min-width: 1410px) {
  .detailDrawer .el-drawer {
    width: 30% !important;
  }
}

@media screen and (max-width: 1410px) {
  .detailDrawer .el-drawer {
    width: 40% !important;
  }
}

.detailDrawer .el-drawer {
  padding: 20px 20px;
  overflow: auto;
}

.traceDrawer .el-drawer {
  padding: 20px 30px;
}

.custom-tree-node { 
    font-size: 14px;
    padding-right: 8px;
    width: 100%; ;
}

.custom-tree-node:hover{

background-color: #dadada;

}

.traceDrawer .el-tree-node__content{
 
   height: 30px;
  
}

.traceDrawer .el-tag--mini{
    
    height: 20px;
    line-height: 16px;

}

.trace-bar{    
  border-bottom: 2px solid #409eff; 
  font-size: 12px;
  text-indent: -50px;
  height: 14px;
  line-height: 28px; 
  text-align: left;
  display: inline-block;
}


@media screen and (max-width: 1375px){
  .trace-bar-span{ 
    width: 560px; 
  }
}

@media screen and (min-width: 1375px){
  .trace-bar-span{ 
    width: 860px; 
  }
}

.trace-bar-span{  
  float: right;
}

.endpoint_item{

  font-size:12px;

}

</style> 


  <template>
  <div>
    <el-row>
      <el-col :span="24">
        <el-card class="box-card">

          <el-form :inline="true" :model="requestQuery" class="demo-form-inline">  

          <el-form-item :label="this.$store.state.lang.Request_Request_Route">
           <el-select
              size="small"
              style="width:300px" 
              v-model="requestQuery.route"
              filterable
              @change="load">
              <el-option
               class="endpoint_item"
                v-for="item in routes"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              ></el-option>
            </el-select>
            </el-form-item>   
         

            <el-form-item :label="this.$store.state.lang.RequestId">
              <el-input size="small" v-model="requestQuery.requestId"></el-input>
            </el-form-item> 
           

            <el-form-item :label="this.$store.state.lang.StatusCode">
              <el-input style="width:100px;margin-right:6px"  size="small" v-model="requestQuery.statusCode"></el-input>
            </el-form-item>


             <el-form-item :label="this.$store.state.lang.Request_Type">
           <el-select
              size="small"
              style="width:100px" 
              v-model="requestQuery.method"
              filterable
              @change="load">
              <el-option
               class="endpoint_item"
                v-for="item in ['ALL','GET','POST','PUT','DELETE']"
                :key="item"
                :label="item"
                :value="item == 'ALL' ? '' : item"
              ></el-option>
            </el-select>
            </el-form-item>    
           
            <el-form-item :label="this.$store.state.lang.MinMs">
              <el-input-number :precision="0" :min="0" :step="500" size="small" v-model="requestQuery.minMs"></el-input-number>
            </el-form-item>

            <el-form-item v-show="false" :label="this.$store.state.lang.RequestBody">
              <el-input size="mini" v-model="requestQuery.request"></el-input>
            </el-form-item>

            <el-form-item v-show="false" :label="this.$store.state.lang.ResponseBody">
              <el-input size="mini" v-model="requestQuery.response"></el-input>
            </el-form-item>
          </el-form>

          <el-table :border="true" :data="requestData" stripe height="580" size="small">
            <el-table-column prop="id" :label="this.$i18n.t('RequestId')"></el-table-column>
            <el-table-column prop="service" :label="this.$i18n.t('ServiceTag')"></el-table-column>
            <el-table-column prop="instance" :label="this.$i18n.t('InstanceTag')"></el-table-column>
            <el-table-column prop="route" :label="this.$i18n.t('Request_RequestUrl')"></el-table-column>
            <el-table-column prop="method" width="80" :label="this.$i18n.t('Request_Type')"></el-table-column>
            <el-table-column prop="milliseconds" width="80" :label="this.$i18n.t('Request_Time')"></el-table-column>
            <el-table-column prop="statusCode" width="80" :label="this.$i18n.t('StatusCode')"></el-table-column>
            <el-table-column prop="loginUser" :label="this.$i18n.t('LoginInfo')"></el-table-column>
            <el-table-column prop="remoteIP" :label="this.$i18n.t('RemoteIP')"></el-table-column>
            <el-table-column prop="createTime" :label="this.$i18n.t('Request_CreateTime')"></el-table-column>
            <el-table-column prop="id" :label="this.$i18n.t('Monitor_Operation')" width="160">
              <template slot-scope="scope">
                <el-button
                  style="margin-left:4px;padding:4px 8px" 
                  type="primary"
                  icon="el-icon-more-outline" 
                  size="mini"
                  @click="load_detail(scope.row.id)"
                >{{ scope._self.$i18n.t('Detail') }}</el-button>
                <el-button
                  style="margin-left:4px;padding:4px 8px" 
                  type="danger"
                  icon="el-icon-s-promotion" 
                  size="mini"
                  @click="load_trace(scope.row.id)"
                >{{ scope._self.$i18n.t('Request_Trace') }}</el-button>
              </template>
            </el-table-column>
          </el-table>

          <el-pagination
            style="margin-top:14px"
            @size-change="this.pageSizeChange"
            @current-change="this.pageCurrentChange"
            :current-page="this.requestPage.pageNumber"
            :page-sizes="[10, 20, 50, 100]"
            :page-size="this.requestPage.pageSize"
            layout="total, sizes, prev, pager, next, jumper"
            :total="this.requestPage.total"
          ></el-pagination>

          <el-drawer
            class="detailDrawer"
            direction="ltr"
            title="detailDrawer"
            :visible.sync="detailDrawer"
            :with-header="false"
          >
            <h4>{{ this.$store.state.lang.Request_BasicInfo }}</h4>

            <el-form  :inline="true" ref="form" :model="info" label-width="80px" size="mini">
              <el-form-item :label="this.$store.state.lang.RequestId">
                <el-input style="width:150px" v-model="info.id"></el-input>
              </el-form-item>

              <el-form-item :label="this.$store.state.lang.ParentId">
                <el-input style="width:150px" v-model="info.parentId"></el-input>
              </el-form-item>

              <el-form-item :label="this.$store.state.lang.ServiceTag">
                <el-input style="width:150px" v-model="info.service"></el-input>
              </el-form-item>

               <el-form-item :label="this.$store.state.lang.ParentService">
                <el-input style="width:150px" v-model="info.parentService"></el-input>
              </el-form-item> 

              <el-form-item :label="this.$store.state.lang.InstanceTag">
                <el-input style="width:150px" v-model="info.instance"></el-input>
              </el-form-item>   
            
              <el-form-item :label="this.$store.state.lang.Request_Type">
                <el-input style="width:150px" v-model="info.method"></el-input>
              </el-form-item>

              <el-form-item :label="this.$store.state.lang.Request_Time">
                <el-input style="width:150px" v-model="info.milliseconds"></el-input>
              </el-form-item>

              <el-form-item :label="this.$store.state.lang.StatusCode">
                <el-input style="width:150px" v-model="info.statusCode"></el-input>
              </el-form-item>

              <el-form-item :label="this.$store.state.lang.LoginInfo">
                <el-input style="width:150px" v-model="info.loginUser"></el-input>
              </el-form-item>

              <el-form-item :label="this.$store.state.lang.RemoteIP">
                <el-input style="width:150px" v-model="info.remoteIP"></el-input>
              </el-form-item> 

              <el-form-item :label="this.$store.state.lang.Request_Request_Route">
                <el-input style="width:396px" v-model="info.route"></el-input>
              </el-form-item>
              
              <el-form-item :label="this.$store.state.lang.Request_RequestUrl">
                <el-input style="width:396px" v-model="info.url"></el-input>
              </el-form-item>

              <el-form-item :label="this.$store.state.lang.Request_CreateTime">
                <el-input style="width:396px" v-model="info.createTime"></el-input>
              </el-form-item>
            </el-form>

            <h4>{{ this.$store.state.lang.Request_DetailInfo }}</h4>

            <el-form :inline="true" ref="form" :model="detail" label-width="100px" size="mini">  

              <el-form-item :label="this.$store.state.lang.QueryString">
                <el-input type="textarea" style="width:380px;" v-model="detail.queryString"></el-input>
              </el-form-item>

              <el-form-item :label="this.$store.state.lang.QueryHeader">
                <el-input type="textarea" style="width:380px;" v-model="detail.header"></el-input>
              </el-form-item>

              <el-form-item label="Cookie">
                <el-input type="textarea" style="width:380px;" v-model="detail.cookie"></el-input>
              </el-form-item>

              <el-form-item :label="this.$store.state.lang.RequestBody">
                <el-input type="textarea" autosize=true style="width:380px;" v-model="detail.requestBody"></el-input>
              </el-form-item>

              <el-form-item :label="this.$store.state.lang.ResponseBody">
                <el-input type="textarea" autosize=true style="width:380px;" v-model="detail.responseBody"></el-input>
              </el-form-item>

              <el-form-item :label="this.$store.state.lang.ErrorMessage">
                <el-input type="textarea" style="width:380px;" v-model="detail.errorMessage"></el-input>
              </el-form-item>

              <el-form-item :label="this.$store.state.lang.ErrorStack">
                <el-input type="textarea" style="width:380px;" v-model="detail.errorStack"></el-input>
              </el-form-item>
            </el-form>
          </el-drawer>

          <el-drawer
            class="traceDrawer"
            direction="ttb"
            size="40%"
            title="detailTrace"
            :visible.sync="traceDrawer"
            :with-header="false"
          >
            <h4>Trace</h4>

            <el-tree
              :default-expand-all="true"
              :data="trace_tree_data"
              :props="defaultProps"
              :expand-on-click-node="false"
              @node-click="handleNodeClick"
            >
              <span class="custom-tree-node" slot-scope="{ data }">

                <el-tag type="primary" effect="dark" size="mini">{{ data.info.service }}</el-tag> 
                <span style="font-size:12px;margin-left:12px">{{ data.info.url }}</span>
                <span style="font-size:12px;margin-left:12px">{{ data.info.method }}</span>
                <span style="font-size:12px;margin-left:12px">{{ data.info.statusCode }}</span>  
                <el-button style="font-size:12px;margin-left:12px" type="text" size="mini" @click="load_detail(data.info.id)">{{ data.title }}</el-button> 

                <span class="trace-bar-span"> 

                  <span class="trace-bar" :style="'width:' + data.width +'px;margin-left:' + data.marginLeft +'px;'">{{ data.info.milliseconds + "ms" }}</span>

                </span>    

              </span>  

            </el-tree>
          </el-drawer>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>

import { mapState } from "vuex";
import Vue from "vue";
import ComboViewLayer from "@antv/g2plot/lib/combo/base";

export default {
  data() {
    return { 
      routes:[], 
      trace_tree_data: [],
      trace_tree_top:{ }, 
      top_width: document.body.clientWidth > 1375 ? 860 : 560,
      defaultProps: {
        children: "children",
        label: "label",
      },
      selectId: "",
      detailDrawer: false,
      traceDrawer: false,
      requestQuery: {
        requestId: "",
        route: "",
        statusCode: "",
        method:"",
        request: "",
        response: "",
        minMs: 0
      },
      requestPage: {
        total: 0,
        pageNumber: 1,
        pageSize: 20,
      },
      requestData: [],
      info: {},
      detail: {},
    };
  },
  methods: {
    onSubmit() {},
    handleNodeClick(data) { 
    },
    format(percentage) {  
       return `${percentage}%`;
    }, 
    pageSizeChange(val) {
      this.requestPage.pageSize = val;
      this.load();
    },
    pageCurrentChange(val) {
      this.requestPage.pageNumber = val;
      this.load();
    },
    cutTime(time) {

      time = time.replace("T", " ").replace("."," "); 
      time = time.substr(0, 23);

      if(time.length == 22) { time = time + "0" }
      if(time.length == 21) { time = time + "00" } 
      if(time.length == 20) { time = time + "000" } 
      if(time.length == 19) { time = time + " 000" } 
      return time;

    },
    getTimeSpan(time){  

      var date = this.cutTime(time);
      var year = date.substr(0,4)
      var month = date.substr(5,2);
      var day = date.substr(8,2);
      var hour = date.substr(11,2);
      var minute = date.substr(14,2);
      var second = date.substr(17,2);
      var millisecond = date.substr(20,3);

      var newDate = new Date(year,month,day,hour,minute,second,millisecond);

      var timespan = newDate.getTime(); 

      return timespan;

    },  
    parseNode(nodes) {

      if (nodes == null) return null;

      var list = []; 

      var top = this.trace_tree_top;   
      
      this.getTimeSpan(top.createTime); 

      nodes.forEach((x) => { 

        list.push({
          info: x.info, 
          width:parseInt(this.top_width * (x.info.milliseconds / top.milliseconds)),
          marginLeft: parseInt(((this.getTimeSpan(x.info.createTime) - this.getTimeSpan(top.createTime)) / top.milliseconds) * this.top_width),
          title:this.$store.state.lang.Detail, 
          label:"",
          children: this.parseNode(x.nodes),
        });
      }); 

      return list;

    }, 
    async load_endpoints(){

      var response = await Vue.http.post("GetEndpoints", {
        service: this.$store.state.query.service,
        instance: this.$store.state.query.instance 
      });     

      this.routes = []; 

      this.routes.push({
          value:"",
          label:"ALL"
      });  

      response.body.data.forEach((x) => {
        
        this.routes.push({
          value:x,
          label:x
        }); 

      }); 

    }, 
    async load() { 

      var response = await this.load_basic_data();
      this.load_table(response);  

    },
    async load_basic_data() {
      this.$store.commit("set_detail_loading", true);
      var response = await Vue.http.post("GetDetailData", {
        service: this.$store.state.query.service,
        instance: this.$store.state.query.instance,
        start: this.$store.state.query.start,
        end: this.$store.state.query.end,
        requestId: this.requestQuery.requestId,
        route: this.requestQuery.route,
        requestBody: this.requestQuery.request,
        responseBody: this.requestQuery.response,
        method:this.requestQuery.method,
        statusCode:this.requestQuery.statusCode == "" ? 0 : Number(this.requestQuery.statusCode),
        pageNumber: this.requestPage.pageNumber,
        pageSize: this.requestPage.pageSize,
        minMs: this.requestQuery.minMs == "" ? 0 : Number(this.requestQuery.minMs)
      });  
    

      this.$store.commit("set_detail_loading", false);
      return response;
    },
    async load_detail(id) {
      var response = await Vue.http.post("GetRequestInfoDetail", { id });
      this.detailDrawer = true;
      this.info = response.body.data.info;
      if (this.info != null) {
        this.info.createTime = this.cutTime(this.info.createTime);
      }   

      this.detail = response.body.data.detail;  

      if(this.detail.header.length > 0){  
         this.detail.header =  JSON.stringify(JSON.parse(this.detail.header),null,4);
      }
      if(this.detail.cookie.length > 0){  
         this.detail.cookie =  JSON.stringify(JSON.parse(this.detail.cookie),null,4);
      } 
      if(this.detail.requestBody.length > 0){  
         this.detail.requestBody =  JSON.stringify(JSON.parse(this.detail.requestBody),null,4);
      }
      if(this.detail.responseBody.length > 0){  
         this.detail.responseBody =  JSON.stringify(JSON.parse(this.detail.responseBody),null,4);
      }

      if (this.detail != null) {
        this.detail.createTime = this.cutTime(this.detail.createTime);
      }
   
    },
    activated(){   
      //this.$store.commit("set_index_loading_timestamp",Date.parse(new Date()));  
   }, 
    async load_trace(id) {

      var response = await Vue.http.post("GetTraceList", { id }); 

      var tree = response.body.data; 

      this.traceDrawer = true;

      this.trace_tree_data = [];

      var that = this; 

      this.trace_tree_top = tree.info;

      this.trace_tree_data.push({
        info: tree.info, 
        width:this.top_width,
        marginLeft:0,
        title:this.$store.state.lang.Detail, 
        label:"",
        children: this.parseNode(tree.nodes), 
      });

    },
    load_table(response) {

      this.requestData = [];

      this.requestPage.total = response.body.data.total; 

      response.body.data.list.forEach((x) => {
        x.createTime = this.cutTime(x.createTime);
        this.requestData.push(x);
      });
    },
  },
  async mounted() { 

    this.load_endpoints();

    this.load();  

  },
  computed: mapState({
    query: (state) => state.query,
  }),
  watch: {
    async query(newVal, oldVal) {
      this.load();
    },
  },
};
</script>


