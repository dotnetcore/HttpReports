<template>
  <div>
    <el-tabs style="margin-top:10px" tab-position="top">
      <el-tab-pane :label="this.$store.state.lang.MonitorTask">
        <el-card class="box-card">
          <el-row>
            <el-button
              size="mini"
              @click="this.showdialog"
              icon="el-icon-circle-plus-outline"
              type="primary"
            >{{ this.$store.state.lang.AddMonitorTask }}</el-button>
          </el-row>

          <el-table :data="tableData" style="width:100%;font-size:12px">
            <el-table-column prop="title" :label="this.$i18n.t('Monitor_Title')"></el-table-column>
            <el-table-column prop="service" :label="this.$i18n.t('ServiceTag')"></el-table-column>
            <el-table-column prop="instance" :label="this.$i18n.t('InstanceTag')"></el-table-column>
            <el-table-column prop="cronLike" :label="this.$i18n.t('ExecuteInterval')"></el-table-column>
            <el-table-column prop="startTime" :label="this.$i18n.t('ExecuteTime')"></el-table-column>
            <el-table-column prop="status" :label="this.$i18n.t('Monitor_State')">

              <template slot-scope="scope">
              <el-tag size="mini" effect="plain" :type="scope.row.status === 1 ? 'primary' : 'danger'">{{ scope.row.status == 1 ? scope._self.$i18n.t('Monitor_Runing'):scope._self.$i18n.t('Monitor_Stoping')}}</el-tag>
             </template> 
              
            </el-table-column>
            <el-table-column prop="createTime" :label="this.$i18n.t('Request_CreateTime')"></el-table-column>
            <el-table-column prop="id" :label="this.$i18n.t('Monitor_Operation')"> 
              <template slot-scope="scope">

                <el-button
                  style="margin-left:4px;margin-right:4px;padding:4px 8px;font-size:12px" 
                  type="primary"
                  icon="el-icon-edit"
                  size="mini"
                  @click="editJob(scope.row.id)"
                >{{ scope._self.$i18n.t('Monitor_Edit') }}</el-button>

                <el-button 
                  style="margin-left:4px;margin-right:4px;padding:4px 8px;font-size:12px"
                  type="danger"
                  icon="el-icon-delete" 
                  size="mini"
                  @click="deleteJob(scope.row.id)"
                >{{ scope._self.$i18n.t('Monitor_Delete') }}</el-button> 

              </template>

            </el-table-column>
          </el-table> 
        </el-card>
      </el-tab-pane>

      <el-tab-pane :label="this.$store.state.lang.AlarmInfo">
        <el-card class="box-card">
          <div class="block">
            <el-timeline :reverse="true">
              <el-timeline-item
                v-for="(activity, index) in activities"
                :key="index"
                :timestamp="activity.timestamp"
              >{{activity.content}}</el-timeline-item>
            </el-timeline>
          </div>
        </el-card>
      </el-tab-pane>
    </el-tabs>

    <el-dialog top="5%" :visible.sync="dialogFormVisible">
      <el-form size="small" ref="monitor" :rules="rules" :model="monitor" label-width="100px">
        <el-form-item :label="this.$store.state.lang.Monitor_Title" prop="title">
          <el-input v-model="monitor.title"></el-input>
        </el-form-item>

        <el-form-item :label="this.$store.state.lang.Monitor_Description">
          <el-input type="textarea" v-model="monitor.description"></el-input>
        </el-form-item>

        <el-form-item :label="this.$store.state.lang.Monitor_InstanceName">
          <el-select v-model="monitor.service" filterable @change="alarm_serviceChange">
            <el-option
              v-for="item in this.tag.service"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            ></el-option>
          </el-select>

          <el-select v-model="monitor.instance" style="margin-left:12px">
            <el-option
              v-for="item in this.tag.instance"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            ></el-option>
          </el-select>
        </el-form-item>

        <el-form-item :label="this.$store.state.lang.Monitor_Email" prop="emails">
          <el-input v-model="monitor.emails"></el-input>
        </el-form-item>

        <el-form-item label="webhook" prop="webhook">
          <el-input v-model="monitor.webhook"></el-input>
        </el-form-item>

        <el-form-item :label="this.$store.state.lang.ExecuteTime">
          <el-time-select
            v-model="monitor.startTime"
            :picker-options="{
            start: '00:00',
            step: '00:30',
            end: '23:00' }"
          ></el-time-select>
          <el-time-select
            v-model="monitor.endTime"
            :picker-options="{  start: '00:00',
            step: '00:30',
            end: '23:00'  }"
          ></el-time-select>
        </el-form-item>

        <el-form-item :label="this.$store.state.lang.ExecuteInterval">
          <el-select v-model="monitor.interval">
            <el-option :label="this.$store.state.lang.Monitor_Time1Min" value="0 0/1 * * * ?"></el-option>
            <el-option :label="this.$store.state.lang.Monitor_Time3Min" value="0 0/3 * * * ?"></el-option>
            <el-option :label="this.$store.state.lang.Monitor_Time5Min" value="0 0/5 * * * ?"></el-option>
            <el-option :label="this.$store.state.lang.Monitor_Time10Min" value="0 0/10 * * * ?"></el-option>
            <el-option :label="this.$store.state.lang.Monitor_Time30Min" value="0 0/30 * * * ?"></el-option>
            <el-option :label="this.$store.state.lang.Monitor_Time1Hour" value="0 0 0/1 * * ?"></el-option>
          </el-select>
        </el-form-item>

        <el-form-item :label="this.$store.state.lang.Switch">
          <el-switch v-model="monitor.enabled"></el-switch>
        </el-form-item>

        <el-tabs type="border-card">
          <el-tab-pane :label="this.$store.state.lang.Monitor_Type_Timeout">
            <el-switch
              :active-text="this.$store.state.lang.Switch"
              v-model="monitor.responseTimeMonitor.enabled"
            ></el-switch>
            <span
              style="margin-left:16px;margin-right:8px;"
            >{{ this.$store.state.lang.Monitor_Timeout }}</span>
            <el-input-number
              :max="10000"
              :step="100"
              size="small"
              v-model="monitor.responseTimeMonitor.timeout"
            ></el-input-number>
            <span
              style="margin-left:16px;margin-right:8px;"
            >{{ this.$store.state.lang.Monitor_Timeout_Percentage }}</span>
            <el-input-number
              :precision="0"
              :max="100"
              :min="0"
              :step="1"
              size="small"
              v-model="monitor.responseTimeMonitor.percentage"
            ></el-input-number>
          </el-tab-pane>
          <el-tab-pane :label="this.$store.state.lang.Monitor_Type_RequestError">
            <el-switch
              :active-text="this.$store.state.lang.Switch"
              v-model="monitor.errorMonitor.enabled"
            ></el-switch>
            <span
              style="margin-left:16px;margin-right:8px;font-size:12px"
            >{{ this.$store.state.lang.Monitor_Timeout_Percentage }}</span>
            <el-input-number
              :precision="0"
              :max="100"
              :min="0"
              :step="1"
              size="small"
              v-model="monitor.errorMonitor.percentage"
            ></el-input-number>
          </el-tab-pane>
          <el-tab-pane :label="this.$store.state.lang.Monitor_Type_RequestCount">
            <el-switch
              :active-text="this.$store.state.lang.Switch"
              v-model="monitor.callMonitor.enabled"
            ></el-switch>
            <span
              style="margin-left:16px;margin-right:8px;font-size:12px"
            >{{ this.$store.state.lang.Min }}</span>
            <el-input-number
              style="width:100px"
              :controls="false"
              :min="0"
              size="small"
              v-model="monitor.callMonitor.min"
            ></el-input-number>

            <span
              style="margin-left:16px;margin-right:8px;font-size:12px"
            >{{ this.$store.state.lang.Max }}</span>
            <el-input-number
              style="width:100px"
              :controls="false"
              :max="100000000"
              :min="0"
              size="small"
              v-model="monitor.callMonitor.max"
            ></el-input-number>
          </el-tab-pane>
        </el-tabs>
      </el-form>

      <div slot="footer" class="dialog-footer">
        <el-button
          size="small"
          @click="dialogFormVisible = false"
        >{{ this.$store.state.lang.Button_Cancel }}</el-button>
        <el-button
          size="small"
          type="primary"
          @click="submitForm('monitor')"
        >{{ this.$store.state.lang.Button_OK }}</el-button>
      </div>
    </el-dialog>
  </div>
</template> 

<style>

.el-timeline-item{

  padding-top: 16px;

}

.el-tabs__header {
  margin: 0;
}
</style>

<script>
import { mapState } from "vuex";
import Vue from "vue";

export default {
  data() {
    return {
      dialogFormVisible: false,
      tag: {
        service: [],
        instance: [],
      },
      rules: {
        title: [
          {
            required: true,
            message: this.$i18n.t("Monitor_TitleNotNull"),
            trigger: "blur",
          },
          {
            min: 3,
            max: 20,
            message: this.$i18n.t("Monitor_Title_Range"),
            trigger: "blur",
          },
        ],
        emails: [
          {
            required: true,
            message: this.$i18n.t("Monitor_EmailNotNull"),
            trigger: "blur",
          },
          {
            min: 5,
            max: 200,
            message: this.$i18n.t("Monitor_Email_Rule"),
            trigger: "blur",
          },
        ],
        webhook: [
          {
            min: 5,
            max: 100,
            message: this.$i18n.t("Monitor_WebHook_Rule"),
            trigger: "blur",
          },
        ],
      },
      monitor: {
        id: "",
        title: "",
        description: "",
        service: "",
        instance: "",
        emails: "",
        webhook: "",
        startTime: "",
        endTime: "",
        enabled: true,
        interval: "0 0/5 * * * ?",
        responseTimeMonitor: {
          enabled: false,
          timeout: 0,
          percentage: 0,
        },
        errorMonitor: {
          enabled: false,
          percentage: 0,
        },
        callMonitor: {
          enabled: false,
          min: 0,
          max: 0,
        },
      },
      tableData: [],
      activities: [],
    };
  },
  methods: {

    async editJob(id){   
      
       this.showdialog();

        var response = await Vue.http.post("GetMonitorJob", {id});  
        var job = response.body.data;  
        
        this.monitor.id = job.id; 
        this.monitor.title = job.title;
        this.monitor.emails = job.emails;
        this.monitor.enabled = job.status > 0; 
        this.monitor.startTime = job.startTime;
        this.monitor.endTime = job.endTime;
        this.monitor.description = job.description;
        this.monitor.interval = this.parseCronlike(job.cronLike); 
        this.monitor.webhook = job.webHook; 
        this.monitor.service = job.service == "" ? "ALL" : job.service;  

        this.alarm_serviceChange(this.monitor.service); 
        this.monitor.instance = job.instance == "" ? "ALL" : job.instance;

        var payload = JSON.parse(job.payload); 

        this.monitor.responseTimeMonitor.enabled = payload.responseTimeMonitor.status > 0;
        this.monitor.responseTimeMonitor.timeout = payload.responseTimeMonitor.timeout;
        this.monitor.responseTimeMonitor.percentage = payload.responseTimeMonitor.percentage; 

        this.monitor.errorMonitor.enabled = payload.errorMonitor.status > 0;
        this.monitor.errorMonitor.percentage = payload.errorMonitor.percentage; 


        this.monitor.callMonitor.enabled = payload.callMonitor.status > 0;
        this.monitor.callMonitor.min = payload.callMonitor.min; 
        this.monitor.callMonitor.max = payload.callMonitor.max; 
        
    },
    async deleteJob(id){ 

       this.$confirm( this.$i18n.t("Monitor_ConfirmDelete"), '', {
          confirmButtonText: this.$i18n.t("Button_OK"),
          cancelButtonText: this.$i18n.t("Button_Cancel"),
          type: 'warning'
        }).then(async () => {  

           await Vue.http.post("DeleteJob", {id});  

           await this.loadMonitorJob();

           this.$message({
            type: 'success',
            message: this.$i18n.t("Monitor_DeleteSuccess") 
          }); 

        }); 

    },
    cutTime(time) {
      time = time.replace("T", " ").replace(".", " ");
      time = time.substr(0, 19);
      return time;
    },
    parseCronlike(cronlike) {
      if (cronlike == "0 0/1 * * * ?") {
        return this.$store.state.lang.Monitor_Time1Min;
      } else if (cronlike == "0 0/3 * * * ?") {
        return this.$store.state.lang.Monitor_Time3Min;
      } else if (cronlike == "0 0/5 * * * ?") {
        return this.$store.state.lang.Monitor_Time5Min;
      } else if (cronlike == "0 0/10 * * * ?") {
        return this.$store.state.lang.Monitor_Time10Min;
      } else if (cronlike == "0 0/30 * * * ?") {
        return this.$store.state.lang.Monitor_Time30Min;
      } else if (cronlike == "0 0 0/1 * * ?") {
        return this.$store.state.lang.Monitor_Time1Hour;
      } else {
        return cronlike;
      }
    },

    async loadMonitorAlarm(){

      var response = await Vue.http.get("GetMonitorAlarms");   

      this.activities = [];
      response.body.data.forEach(x => {

        this.activities.push({
          content:x.body,
          timestamp: this.cutTime(x.createTime)

        }); 

      }); 

    },
    async loadMonitorJob() {

      var response = await Vue.http.get("GetMonitorJobs");   

      response.body.data.forEach((x) => {
        x.createTime = this.cutTime(x.createTime);
        x.cronLike = this.parseCronlike(x.cronLike);
        x.instance = x.instance == "" ? "ALL" : x.instance;
        x.startTime = x.startTime + " - " + x.endTime;
      });

      this.tableData = response.body.data;
    },
    async submitForm(formName) {
      this.$refs[formName].validate(async (valid) => {
        if (valid) {
          if (this.monitor.service == "ALL") {
            this.$message({
              message: this.$i18n.t("Monitor_MustSelectNode"),
              type: "warning",
            });
            return;
          }

          if ((this.monitor.startTime == '' && !this.monitor.endTime == '') || (!this.monitor.startTime == '' && this.monitor.endTime == '')) {
            
            this.$message({
              message: this.$i18n.t("Monitor_Time_Range"),
              type: "warning",
            });
            return;

          } 

          if (
            !this.monitor.responseTimeMonitor.enabled &&
            !this.monitor.errorMonitor.enabled &&
            !this.monitor.callMonitor.enabled
          ) {
            this.$message({
              message: this.$i18n.t("Monitor_MustSelectType"),
              type: "warning",
            });
            return;
          }

          if (this.monitor.service == "ALL") {
            this.monitor.service = "";
          }

          // submit 
          var payload = {
            responseTimeMonitor: {
              status: this.monitor.responseTimeMonitor.enabled ? 1 : 0,
              timeout: this.monitor.responseTimeMonitor.timeout,
              percentage: this.monitor.responseTimeMonitor.percentage,
            },
            errorMonitor: {
              status: this.monitor.errorMonitor.enabled ? 1 : 0,
              percentage: this.monitor.errorMonitor.percentage,
            },
            callMonitor: {
              status: this.monitor.callMonitor.enabled ? 1 : 0,
              min: this.monitor.callMonitor.min,
              max: this.monitor.callMonitor.max,
            },
          };

          var job = {
            id: this.monitor.id,
            title: this.monitor.title,
            description: this.monitor.description,
            cronlike: this.monitor.interval,
            webhook: this.monitor.webhook,
            emails: this.monitor.emails,
            mobiles:"",
            status: this.monitor.enabled ? 1 : 0,
            service: this.monitor.service,
            instance: this.monitor.instance,
            startTime: this.monitor.startTime,
            endTime: this.monitor.endTime,
            payload: JSON.stringify(payload),
          };

          var response = await Vue.http.post("AddOrUpdateMonitorJob", job);

          this.$message({
            message:
              this.monitor.id == ""
                ? this.$i18n.t("Monitor_AddSuccess")
                : this.$i18n.t("Monitor_UpdateSuccess"),
            type: "success",
          });

          this.dialogFormVisible = false;
          this.resetForm();

          await this.loadMonitorJob();

        }
      });
    },

    showdialog() {
      this.resetForm();
      this.dialogFormVisible = true;
      this.locaServiceInstance();
    },
    resetForm() {
      this.monitor = {
        id: "",
        title: "",
        description: "",
        service: "",
        instance: "",
        startTime: "",
        endTime: "",
        enabled: true,
        interval: "0 0/5 * * * ?",
        responseTimeMonitor: {
          enabled: false,
          timeout: 0,
          percentage: 0,
        },
        errorMonitor: {
          enabled: false,
          percentage: 0,
        },
        callMonitor: {
          enabled: false,
          min: 0,
          max: 0,
        },
      };
    },

    alarm_serviceChange(data) {
      this.$store.state.tag.forEach((item) => {
        if (item.service == data) {
          this.tag.instance = [];
          this.tag.instance.push({ value: "ALL", label: "ALL" });

          item.instance.forEach((k) => {
            this.tag.instance.push({ value: k, label: k });
          });

          this.monitor.instance = "ALL";
        }
      });
    },
    locaServiceInstance() {

      this.tag.service = [];
      //this.tag.service.push({ value: "ALL", label: "ALL" });

      this.tag.instance = [];
      this.tag.instance.push({ value: "ALL", label: "ALL" });

      this.$store.state.tag.forEach((item) => {
        this.tag.service.push({ value: item.service, label: item.service });
      });

      this.monitor.service = this.tag.service[0].value;
      this.monitor.instance = "ALL";
    },
  },
  created: function () {},
  async mounted() {
    await this.loadMonitorJob();
    await this.loadMonitorAlarm();
  },
};
</script>
