<template>
  <div>
    <el-tabs style="margin-top: 10px" tab-position="top" @tab-click="handleClick">
      <el-tab-pane label="接口限流">
        <el-card class="box-card">
          <el-row>
            <!-- <el-button
              size="mini"
              @click="GetGeneralRules"
              icon="el-icon-refresh"
              >刷新</el-button
            > -->
            <el-button
              size="mini"
              @click="addJob"
              icon="el-icon-circle-plus-outline"
              type="primary"
              >添加</el-button
            >
          </el-row>
          <el-table :data="tableData" style="width: 100%">
            <el-table-column fixed prop="key" label="接口" width="150">
            </el-table-column>
            <el-table-column prop="endpoint" label="端点" width="250">
            </el-table-column>
            <el-table-column prop="limit" label="限额"> </el-table-column>
            <el-table-column prop="periodVal" label="周期值"> </el-table-column>
            <el-table-column
              prop="periodUnit"
              label="周期单位"
              :formatter="periodUnitformatter"
            >
            </el-table-column>
            <el-table-column prop="isEnable" label="状态" width="150">
              <template slot-scope="scope">
                <el-tag
                  size="mini"
                  effect="plain"
                  :type="scope.row.isEnable == true ? 'primary' : 'danger'"
                  >{{
                    scope.row.isEnable == true
                      ? scope._self.$i18n.t("Monitor_Runing")
                      : scope._self.$i18n.t("Monitor_Stoping")
                  }}</el-tag
                >
              </template>
            </el-table-column>
            <el-table-column fixed="right" label="操作" width="100">
              <template slot-scope="scope">
                <el-button
                  type="text"
                  size="small"
                  @click="editJob(scope.row.key)"
                  >{{ scope._self.$i18n.t("Monitor_Edit") }}</el-button
                >

                <el-button
                  type="text"
                  size="small"
                  @click="deleteJob(scope.row.key)"
                  >{{ scope._self.$i18n.t("Monitor_Delete") }}</el-button
                >
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-tab-pane>

      <el-tab-pane label="Ip白名单">
        <el-card class="box-card">
          <div class="block">
            <el-row>
              <el-button
                size="mini"
                @click="save"
                icon="el-icon-circle-plus-outline"
                type="primary"
                >保存</el-button
              >
            </el-row>
            <br />
            <el-input
              type="textarea"
              :rows="2"
              placeholder="请输入内容"
              v-model="ipWhite"
            >
            </el-input>
          </div>
        </el-card>
      </el-tab-pane>
    </el-tabs>

    <el-dialog top="5%" :visible.sync="dialogFormVisible">
      <el-form
        size="small"
        ref="limitRule"
        :model="limitRule"
        label-width="100px"
      >
        <el-form-item label="接口" prop="key">
          <el-input
            :disabled="disabledInput"
            v-model="limitRule.key"
          ></el-input>
        </el-form-item>

        <el-form-item label="端点" prop="endpoint">
          <el-input
            :disabled="disabledInput"
            v-model="limitRule.endpoint"
            placeholder="*:/api/values/*?"
          ></el-input>
        </el-form-item>

        <el-form-item label="限额">
          <el-input-number
            :precision="0"
            :min="0"
            :step="1"
            size="small"
            v-model="limitRule.limit"
          ></el-input-number>
        </el-form-item>

        <el-form-item label="开关">
          <el-switch v-model="limitRule.isEnable"></el-switch>
        </el-form-item>

        <el-form-item label="周期值" prop="periodVal">
          <el-input-number
            :precision="0"
            :min="1"
            :step="1"
            size="small"
            v-model="limitRule.periodVal"
          ></el-input-number>
        </el-form-item>

        <el-form-item label="周期单位">
          <el-select v-model="limitRule.periodUnit" placeholder="请选择">
            <el-option label="秒" value="s"></el-option>
            <el-option label="分" value="m"></el-option>
            <el-option label="时" value="h"></el-option>
            <el-option label="天" value="d"></el-option>
          </el-select>
        </el-form-item>
      </el-form>

      <div slot="footer" class="dialog-footer">
        <el-button size="small" @click="dialogFormVisible = false">{{
          this.$store.state.lang.Button_Cancel
        }}</el-button>
        <el-button
          size="small"
          type="primary"
          @click="submitForm('limitRule')"
          >{{ this.$store.state.lang.Button_OK }}</el-button
        >
      </div>
    </el-dialog>
  </div>
</template>

<style>
.box-card {
  margin-top: 20px;
}
</style>

<script>
import { basic } from '@/common/basic.js' 

export default {
  data() {
    return {
      dialogFormVisible: false,
      disabledInput: true,
      isUpdate: false,
      tableData: [],
      limitRule: {
        key: "",
        endpoint: "",
        periodVal: 10,
        periodUnit: "s",
        limit: 0,
        isEnable: true,
      },
      ipWhite: "127.0.0.1",
    };
  },
  async mounted() {
    var self = this;
    document.onkeydown = function (e) {
      let key = window.event.keyCode;
      if (key == 69 && window.event.ctrlKey) {
        window.event.preventDefault();
        self.disabledInput = !self.disabledInput;
      }
    };
  },
  created: function () {
    this.GetGeneralRules();
  },
  methods: {
    async editJob(key) {
      this.resetForm();
      this.dialogFormVisible = true;
      this.isUpdate = true;
      this.$http
        .get(`${basic.DOMAIN}/IpRateLimit/GetGeneralRulesByKey?key=${key}`)
        .then((data) => {
          if (data.body) {
            this.limitRule = data.body;
          }
        });
    },
    async addJob() {
      this.resetForm();
      this.dialogFormVisible = true;
      this.disabledInput = false;
      this.isUpdate = false;
    },
    async save() {
      var self = this;
      self.$http
        .post(`${basic.DOMAIN}/IpRateLimit/SyncSaveIpWhite?ipWhite=${self.ipWhite}`)
        .then((data) => {
          self.$message({ message: "保存成功", type: "success" });
          self.GetIpWhiteList();
        });
    },
    async deleteJob(key) {
      var self = this;
      self
        .$confirm(`确定要删除${key}吗?`, "", {
          confirmButtonText: self.$i18n.t("Button_OK"),
          cancelButtonText: self.$i18n.t("Button_Cancel"),
          type: "warning",
        })
        .then(async () => {
          self.$http
            .post(`${basic.DOMAIN}/IpRateLimit/RemoveLimitRules?key=${key}`)
            .then((data) => {
              if (data.body) {
                self.$message({
                  type: "success",
                  message: self.$i18n.t("Monitor_DeleteSuccess"),
                });
                self.GetGeneralRules();
              } else {
                self.$message({
                  type: "error",
                  message: "删除失败",
                });
              }
            });
        });
    },
    async submitForm(formName) {
      var self = this;
      if (self.isUpdate) {
        self.$http
          .post(`${basic.DOMAIN}/IpRateLimit/UpdateLimitRules`, self.limitRule)
          .then((data) => {
            self.$message({ message: "保存成功", type: "success" });
            self.GetGeneralRules();
            self.dialogFormVisible = false;
            self.resetForm();
          });
      } else {
        self.$http
          .post(`${basic.DOMAIN}/IpRateLimit/AddLimitRules`, self.limitRule)
          .then((data) => {
            self.$message({ message: "保存成功", type: "success" });
            self.GetGeneralRules();
            self.dialogFormVisible = false;
            self.resetForm();
          });
      }
    },
    resetForm() {
      this.limitRule = {
        key: "",
        endpoint: "",
        periodVal: 10,
        periodUnit: "s",
        limit: 0,
        isEnable: true,
      };
      this.disabledInput = true;
    },
    GetGeneralRules() {
      var self = this;
      self.$http.get(`${basic.DOMAIN}/IpRateLimit/GetGeneralRules`).then((data) => {
        var obj = data.body;
        var res = [];
        // self.tableData = data.body;
        for (const key in obj) {
          if (Object.hasOwnProperty.call(obj, key)) {
            res.push(obj[key]);
            ``;
          }
        }
        self.tableData = res;
      });
    },
    GetIpWhiteList() {
      var self = this;
      self.$http.get(`${basic.DOMAIN}/IpRateLimit/GetIpWhiteList`).then((data) => {
        self.ipWhite = data.body;
      });
    },
    handleClick(comp,event){
      if(comp.index == 1){
        this.GetIpWhiteList();
      }else{
        this.GetGeneralRules();
      }
    },
    periodUnitformatter(row, column, value) {
      var res = "";
      switch (value) {
        case "s":
          res = "秒";
          break;
        case "m":
          res = "分";
          break;
        case "h":
          res = "时";
          break;
        case "d":
          res = "天";
          break;
      }
      return res;
    },
  },
};
</script>
