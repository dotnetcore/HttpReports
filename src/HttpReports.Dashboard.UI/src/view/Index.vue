
<template>
  <el-container class="index-body">
    <el-dialog
      :title="this.$store.state.lang.Template_EditAccount"
      :visible.sync="UpdateDialogVisible"
      width="30%"
    >
      <el-form
        :model="setUserInfo"
        label-position="left"
        label-width="100px"
        style="padding:0 30px"
        class="editModal"
      >
        <el-form-item :label="this.$store.state.lang.Login_UserName">
          <el-input size="small" v-model="setUserInfo.name"></el-input>
        </el-form-item>

        <el-form-item :label="this.$store.state.lang.Index_OldPwd">
          <el-input size="small" type="password" v-model="setUserInfo.oldPwd"></el-input>
        </el-form-item>

        <el-form-item :label="this.$store.state.lang.Index_NewPwd">
          <el-input size="small" type="password" v-model="setUserInfo.newPwd"></el-input>
        </el-form-item>
      </el-form>

      <span slot="footer" class="dialog-footer">
        <el-button
          size="small"
          @click="UpdateDialogVisible = false"
        >{{ this.$store.state.lang.Button_Cancel }}</el-button>
        <el-button
          size="small"
          type="primary"
          @click="updateUserInfo"
        >{{ this.$store.state.lang.Button_OK }}</el-button>
      </span>
    </el-dialog>

    <!-- <el-dialog
      class="aboutDialog"
      :title="$t('About')"
      :visible.sync="aboutDialogVisible"
      width="36%"
    >
      <h3>HttpReports</h3> 

      <p>
        <span>{{ $t('Docs') }}</span>
         <a
          style="color: #409eff;text-decoration: none;"
          target="_blank"
          href="https://www.yuque.com/httpreports/docs"
        >https://www.yuque.com/httpreports/docs</a>
      </p>

      <p>
        <a
          style="color: #409eff;text-decoration:none;"
          target="_blank"
          href="https://github.com/dotnetcore/HttpReports"
        >https://github.com/dotnetcore/HttpReports</a>
      </p>

      <p> 
         <a
          style="color: #409eff;text-decoration: none;font-size:12px"
          target="_blank"
          href="https://github.com/dotnetcore/HttpReports/blob/master/docs/LICENSE"
        >MIT协议</a>

         <a
          style="color: #409eff;text-decoration:none; margin-left:20px;font-size:12px"
          target="_blank"
          href="https://github.com/dotnetcore"
        >.NET Core Community</a>

      </p> 

      <el-collapse  accordion>
        <el-collapse-item :title="$t('Communication')" name="1">
           
           <img style="width:100%"  src="/static/communication.jpg" /> 

        </el-collapse-item>
        <el-collapse-item :title="$t('Donation')" name="2">
           <img style="width:100%" src="/static/donation.jpg" />

           <a style="color: #409eff;text-decoration: none;"  target="_blank" href="https://www.yuque.com/httpreports/docs/pl212y" >{{ $t('DonationDetail') }}</a> 
        </el-collapse-item> 
      </el-collapse>

      <span slot="footer" class="dialog-footer">
        <el-button
          size="small"
          type="primary"
          @click="aboutDialogVisible = false"
        >{{ $t('Button_OK') }}</el-button>
      </span>
    </el-dialog> -->

    <el-header>
      <div class="navbar-left">
        <div class="logo" v-show="!isCollapse">
          <b style="font-size:18px;color:#FFF">HttpReports</b>
        </div>

        <div class="title" v-show="isCollapse">
          <span style="font-size:22px;color:#FFF;width:20px"></span>
        </div>
      </div>

      <div class="navbar-center">
        <i
          @click="changeNavState"
          :class="[isCollapse?'el-icon-s-unfold arrow':'el-icon-s-fold arrow']"
        ></i>

        <i
          @click="manualReload"
          title="Refresh"
          :class="[loading?'refresh fa-spin el-icon-refresh-right':'refresh el-icon-refresh-right']"
        ></i>
      </div>

      <div class="navbar-right">
        <div class="nav-item">
          <span class="el-dropdown-link nav-user">
            <i
              style="font-size:26px"
              class="el-icon-s-opportunity"
              @click="aboutDialogVisible = true"
            ></i>
          </span>

          <span class="el-dropdown-link nav-user">
            <i style="font-size:26px" class="el-icon-rank" @click="handleFullScreen"></i>
          </span>

          <el-dropdown>
            <span class="el-dropdown-link nav-user">
              <i class="fa fa-language"></i>
            </span>

            <el-dropdown-menu slot="dropdown">
              <el-dropdown-item @click.native="changeLanguage('en-us')">English</el-dropdown-item>
              <el-dropdown-item @click.native="changeLanguage('zh-cn')">中文</el-dropdown-item>
            </el-dropdown-menu>
          </el-dropdown>

          <el-dropdown v-show="false">
            <span class="el-dropdown-link nav-user">
              <i class="fa fa-dashboard"></i>
            </span>

            <el-dropdown-menu slot="dropdown">
              <el-dropdown-item>{{ this.$store.state.lang.Template_Light }}</el-dropdown-item>
              <el-dropdown-item>{{ this.$store.state.lang.Template_Dark }}</el-dropdown-item>
            </el-dropdown-menu>
          </el-dropdown>

          <el-dropdown>
            <span class="el-dropdown-link nav-user">
              <i class="fa fa-user"></i>
            </span>

            <el-dropdown-menu slot="dropdown">
              <el-dropdown-item>{{ this.userName }}</el-dropdown-item>
              <el-dropdown-item
                @click.native="UpdateDialogVisible = true"
              >{{ this.$store.state.lang.Template_EditAccount }}</el-dropdown-item>
              <el-dropdown-item @click.native="logout">{{ this.$store.state.lang.Template_Logout }}</el-dropdown-item>
            </el-dropdown-menu>
          </el-dropdown>
        </div>

        <div class="nav-item"></div>
      </div>
    </el-header>

    <el-container>
      <el-aside style="width:initial;">
        <el-menu
          :router="true"
          :unique-opened="true"
          default-active="1"
          class="el-menu-vertical-demo"
          @open="handleOpen"
          @close="handleClose"
          :collapse="isCollapse"
          style="min-height:860px"
          active-text-color="#409eff"
        >
          <el-menu-item index="/">
            <i class="fa fa-laptop"></i>
            <span slot="title">{{ this.$store.state.lang.Menu_BasicData }}</span>
          </el-menu-item>

          <el-menu-item index="/service">
            <i class="fa fa-bar-chart-o"></i>
            <span slot="title">{{ this.$store.state.lang.Index_ServiceNode }}</span>
          </el-menu-item>

          <el-menu-item index="/topology">
            <i class="fa fa-snowflake-o"></i>
            <span slot="title">{{ this.$store.state.lang.Topology }}</span>
          </el-menu-item>

          <el-menu-item index="/detail">
            <i class="fa fa-bars"></i>
            <span slot="title">{{ this.$store.state.lang.Menu_RequestList }}</span>
          </el-menu-item>

          <el-menu-item index="/alarm">
            <i class="fa fa-rocket"></i>
            <span slot="title">{{ this.$store.state.lang.Menu_Monitor }}</span>
          </el-menu-item>

           <el-menu-item index="/health">
            <i class="fa fa-heartbeat"></i>
            <span slot="title">{{ this.$store.state.lang.Health }}</span>
          </el-menu-item> 

           <el-menu-item index="/limitapi">
            <i class="fa fa-heartbeat"></i>
            <span slot="title">接口限流</span>
          </el-menu-item> 

          <el-menu-item index="/roleBasedAccess">
            <i class="fa fa-heartbeat"></i>
            <span slot="title">权限模块</span>
          </el-menu-item> 
        </el-menu>
      </el-aside>

      <el-main style="padding-top:0">
        <el-card class="box-card">
          <div class="block">
            <el-select
              size="medium"
              style="margin-right:10px"
              v-model="select_service"
              filterable
              @change="serviceChange"
            >
              <el-option
                v-for="item in service"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              ></el-option>
            </el-select>

            <el-select
              size="medium"
              style="margin-right:10px"
              v-model="select_instance"
              filterable
              @change="instanceChange"
            >
              <el-option
                v-for="item in instance"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              ></el-option>
            </el-select>

            <el-date-picker
              style="margin-right:10px"
              size="medium"
              v-model="range"
              type="datetimerange"
              :picker-options="pickerOptions"
              :range-separator="$t('To')"
              :start-placeholder="$t('StartTime')"
              :end-placeholder="$t('EndTime')"
              align="right"
              @change="timeChange"
            ></el-date-picker>

            <el-button @click="manualReload" icon="el-icon-search" size="small" type="primary">{{ $t('Query') }}</el-button>

            <el-divider direction="vertical"></el-divider>

            <el-switch
              @change="autoSwitch"
              class="auto-switch"
              style="margin-right:10px"
              v-model="auto"
              :inactive-text="this.$store.state.lang.Auto"
            ></el-switch>

            <el-input-number
              style="width:80px;margin-right:10px"
              controls-position="right"
              size="mini"
              v-model="num"
              :min="3"
              :step="3"
            ></el-input-number>
          </div>
        </el-card>

        <!--内容区域-->

        <keep-alive>
          <router-view></router-view>
        </keep-alive>
      </el-main>
    </el-container>
  </el-container>
</template>

<style>
.aboutDialog .el-dialog__body {
  padding: 0px 20px;
}

.el-menu-item i {
  font-size: 18px;
  margin-right: 8px;
}

.index-body {
  background-color: #f3f3f3;
}

.editModal .el-form-item__label {
  font-size: 12px;
}

.el-dropdown-link i {
  font-size: 24px;
  color: #e4e2e2;
  width: 50px;
  line-height: 60px;
}

.el-header {
  background-color: #b3c0d1;
  color: #333;
  line-height: 60px;
}

.el-aside {
  color: #333;
}

.auto-switch .el-switch__label {
  color: #409eff;
}

.auto-switch .is-active {
  color: initial;
}

.el-menu-vertical-demo:not(.el-menu--collapse) {
  width: 220px;
}
</style>

<script>
import { mapState } from "vuex";
import { basic } from "@/common/basic.js";

export default {
  data() {
    return {
      fullscreen: false,
      isCollapse: false,
      UpdateDialogVisible: false,
      aboutDialogVisible: false,
      autoTimer: null,
      userName: localStorage.getItem("username"),
      setUserInfo: {
        name: localStorage.getItem("username"),
        oldPwd: "",
        newPwd: "",
      },
      range: [this.getLastTime(), new Date()],
      select_service: "",
      select_instance: "",
      service: true,
      value2: true,
      auto: false,
      loading: false,
      num: 3,
      service: [],
      instance: [],
      pickerOptions: {
        shortcuts: [
          {
            text: this.$i18n.t("Time_15m"),
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 60 * 1000 * 15);
              picker.$emit("pick", [start, end]);
            },
          },
          {
            text: this.$i18n.t("Time_30m"),
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 60 * 1000 * 30);
              picker.$emit("pick", [start, end]);
            },
          },
          {
            text: this.$i18n.t("Time_1h"),
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 60 * 1000 * 60);
              picker.$emit("pick", [start, end]);
            },
          },
          {
            text: this.$i18n.t("Time_4h"),
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 60 * 1000 * 60 * 4);
              picker.$emit("pick", [start, end]);
            },
          },
          {
            text: this.$i18n.t("Time_12h"),
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 60 * 1000 * 60 * 12);
              picker.$emit("pick", [start, end]);
            },
          },
          {
            text: this.$i18n.t("Time_24h"),
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 60 * 1000 * 60 * 24);
              picker.$emit("pick", [start, end]);
            },
          },
        ],
      },
      value1: [new Date(2000, 10, 10, 10, 10), new Date(2000, 10, 11, 10, 10)],
      value2: "",
    };
  },
  created: function () {
    this.initServiceInstance();
  },
  updated: () => {},
  computed: mapState({
    lang: (state) => state.lang,
  }),

  computed: mapState({
    basic_loading: (state) => state.basic_loading,
    topology_loading: (state) => state.topology_loading,
    detail_loading: (state) => state.detail_loading,
    service_loading: (state) => state.service_loading,
    alarm_loading: (state) => state.alarm_loading,
    health_loading:(state) => state.health_loading,
    index_loading_timestamp: (state) => state.index_loading_timestamp,
  }),
  watch: {
    index_loading_timestamp(newval, oldVal) {
      this.loading = false;
    },
    basic_loading(newVal, oldVal) {
      var path = this.$router.app._route.path;
      if (path == "/" || path == "/basic") {
        this.loading = newVal;
      }
    },
    service_loading(newVal, oldVal) {
      var path = this.$router.app._route.path;
      if (path == "/service") {
        this.loading = newVal;
      }
    },
    alarm_loading(newVal, oldVal) {
      var path = this.$router.app._route.path;
      if (path == "/alarm") {
        this.loading = newVal;
      }
    },
    topology_loading(newVal, oldVal) {
      var path = this.$router.app._route.path;
      if (path == "/topology") {
        this.loading = newVal;
      }
    },
     health_loading(newVal, oldVal) {
      var path = this.$router.app._route.path;
      if (path == "/health") {
        this.loading = newVal;
      }
    },
    detail_loading(newVal, oldVal) {
      var path = this.$router.app._route.path;
      if (path == "/detail") {
        this.loading = newVal;
      }
    },
  },
  mounted() {},
  methods: {
    getLastTime(minutes = 30) {
      var now = new Date();
      now.setMinutes(now.getMinutes() - minutes);
      return now;
    },

    manualReload(){  

       var second = parseInt((new Date().getTime() - this.range[1].getTime()) / 1000); 
        
       if (second <= 5) {

          this.range = [
              basic.addSecond(this.range[0], second),
              basic.addSecond(this.range[1], second),
           ]; 
         
       } 

      this.reload();

    },
    refresh() {
      this.range = [
        basic.addSecond(this.range[0], this.num),
        basic.addSecond(this.range[1], this.num),
      ];
      this.reload();
    },

    autoSwitch(data) {
      if (data) {
        this.autoTimer = setInterval(this.refresh, this.num * 1000);
      } else {
        if (this.autoTimer != null) {
          clearInterval(this.autoTimer);
        }
      }
    },

    serviceChange(data) {
      this.$store.state.tag.forEach((item) => {
        if (item.service == data) {
          this.instance = [];
          this.instance.push({ value: "ALL", label: "ALL" });

          item.instance.forEach((k) => {
            this.instance.push({ value: k, label: k });
          });

          this.select_instance = "ALL";
        }
      });

      this.reload();
    },
    instanceChange(data) {
      this.reload();
    },
    timeChange(data) {
      if (data == null) {
        
        this.range = [this.getLastTime(), new Date()];

        return;
      }

      if (
        new Date(data[1]).getTime() - new Date(data[0]).getTime() >
        1000 * 60 * 60 * 24
      ) {
        this.$message({
          message: this.$store.state.lang.TimeRange_ToLong,
          type: "warning",
        });

        this.range = [this.getLastTime(), new Date()];

        return;
      }

      this.reload();
    },
    reload() {
      this.$store.commit("set_query", {
        service: this.select_service,
        instance: this.select_instance,
        start: basic.dateFormat(new Date(this.range[0])),
        end: basic.dateFormat(new Date(this.range[1])),
      });
    },
    handleFullScreen() {
      let element = document.documentElement;
      if (this.fullscreen) {
        if (document.exitFullscreen) {
          document.exitFullscreen();
        } else if (document.webkitCancelFullScreen) {
          document.webkitCancelFullScreen();
        } else if (document.mozCancelFullScreen) {
          document.mozCancelFullScreen();
        } else if (document.msExitFullscreen) {
          document.msExitFullscreen();
        }
      } else {
        if (element.requestFullscreen) {
          element.requestFullscreen();
        } else if (element.webkitRequestFullScreen) {
          element.webkitRequestFullScreen();
        } else if (element.mozRequestFullScreen) {
          element.mozRequestFullScreen();
        } else if (element.msRequestFullscreen) {
          // IE11
          element.msRequestFullscreen();
        }
      }
      this.fullscreen = !this.fullscreen;
    },

    handleClose(done) {
      this.$confirm("确认关闭？")
        .then((_) => {
          done();
        })
        .catch((_) => {});
    },
    changeLanguage(type) {

      this.$i18n.locale = type;

      localStorage.setItem("locale", type);

      this.$http
        .post("ChangeLanguage", {
          Language: type,
        })
        .then((response) => {
          //this.$message({ message: "Switch: " + type, type: "success" });

          this.$http.get(`/static/lang/${type}.json`).then((res) => {
            this.$store.commit("set_lang", res.body);  
            window.location.reload();
          });
        });
    },
    updateUserInfo() {
      if (
        this.basic.isEmpty(this.setUserInfo.name) ||
        this.basic.isEmpty(this.setUserInfo.oldPwd) ||
        this.basic.isEmpty(this.setUserInfo.newPwd)
      ) {
        this.$message({
          message: this.$store.state.lang.User_NotNull,
          type: "warning",
        });
        return;
      }

      if (this.setUserInfo.oldPwd == this.setUserInfo.newPwd) {
        this.$message({
          message: this.$store.state.lang.User_OldNewPass,
          type: "warning",
        });
        return;
      }

      var username = localStorage.getItem("username");

      this.$http
        .post("UpdateAccountInfo", {
          username: username,
          newUserName: this.setUserInfo.name,
          oldPwd: this.setUserInfo.oldPwd,
          newPwd: this.setUserInfo.newPwd,
        })
        .then((response) => {
          localStorage.setItem("username", this.setUserInfo.name);

          this.$message({
            message: this.$store.state.lang.User_UpdateSuccess,
            type: "success",
          });
          this.UpdateDialogVisible = false;

          this.logout();
        });
    },
    logout() {
      localStorage.setItem("token", "");
      this.$store.commit("set_token", "");
      this.$router.push({ path: "/user/login" });
    },
    handleOpen(key, keyPath) {},
    handleClose(key, keyPath) {},
    changeNavState() {
      this.isCollapse = !this.isCollapse;
    },
    initServiceInstance() {
      this.$http.post("GetServiceInstance", {}).then((response) => {
        this.$store.commit("set_tag", response.body.data);

        this.service = [];
        this.service.push({ value: "ALL", label: "ALL" });

        this.instance = [];
        this.instance.push({ value: "ALL", label: "ALL" });

        this.$store.state.tag.forEach((item) => {
          this.service.push({ value: item.service, label: item.service });
        });

        this.select_service = "ALL";
        this.select_instance = "ALL";
      });
    },
  },
};
</script>
