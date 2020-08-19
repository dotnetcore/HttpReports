
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
      >
        <el-form-item :label="this.$store.state.lang.Login_UserName">
          <el-input v-model="setUserInfo.name"></el-input>
        </el-form-item>

        <el-form-item :label="this.$store.state.lang.Index_OldPwd">
          <el-input type="password" v-model="setUserInfo.oldPwd"></el-input>
        </el-form-item>

        <el-form-item :label="this.$store.state.lang.Index_NewPwd">
          <el-input type="password" v-model="setUserInfo.newPwd"></el-input>
        </el-form-item>
      </el-form>

      <span slot="footer" class="dialog-footer">
        <el-button @click="UpdateDialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="updateUserInfo">确 定</el-button>
      </span>
    </el-dialog>

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
      </div>

      <div class="navbar-right">
        <div class="nav-item">
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
            <span slot="title">{{ this.$store.state.lang.ServiceTag }}</span>
          </el-menu-item>

          <el-menu-item index="/detail">
            <i class="fa fa-bars"></i>
            <span slot="title">{{ this.$store.state.lang.Menu_RequestList }}</span>
          </el-menu-item>

          <el-menu-item index="/alarm">
            <i class="fa fa-rocket"></i>
            <span slot="title">{{ this.$store.state.lang.Menu_Monitor }}</span>
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
              placeholder="请选择"
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
              placeholder="请选择"
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
              range-separator="至"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
              align="right"
              @change="timeChange"
            ></el-date-picker>

            <el-divider direction="vertical"></el-divider>

            <el-switch style="margin-right:10px" v-model="auto" inactive-text="自动刷新"></el-switch> 
          
            <el-input-number
              style="width:80px;margin-right:10px"
              controls-position="right"
              size="mini"
              v-model="num"
              :step="3"
            ></el-input-number>
          </div>
        </el-card>

        <!--内容区域-->

        <router-view></router-view>
      </el-main>
    </el-container>
  </el-container>
</template>

<style>
.el-menu-item i {
  font-size: 18px;
  margin-right: 8px;
}

.index-body {
  background-color: #f3f3f3;
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

.el-menu-vertical-demo:not(.el-menu--collapse) {
  width: 220px;
}
</style>

<script>     

import { mapState } from 'vuex' 
import { basic } from '@/common/basic.js'

 export default {

  data() {  

   return { 

      fullscreen: false,
      isCollapse: false, 
      UpdateDialogVisible: false,
      userName: localStorage.getItem("username"),
      setUserInfo: {
        name: localStorage.getItem("username"),
        oldPwd: "",
        newPwd: "",
      },
      range:[ this.getLastTime(),new Date()],
      select_service: "",
      select_instance: "",
      service: true,
      value2: true,
      auto: true,
      num: 3,
      service: [],
      instance: [], 
      pickerOptions: {
        shortcuts: [
          {
            text:this.$i18n.t('Time_15m'),
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 60 * 1000 * 15 );
              picker.$emit("pick", [start, end]);
            },
          },
          {
            text:this.$i18n.t('Time_30m'),
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 60 * 1000 * 30 );
              picker.$emit("pick", [start, end]);
            },
          },
           {
            text:this.$i18n.t('Time_1h'),
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 60 * 1000 * 60 );
              picker.$emit("pick", [start, end]);
            },
          },
          {
            text:this.$i18n.t('Time_4h'),
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 60 * 1000 * 60 * 4 );
              picker.$emit("pick", [start, end]);
            },
          },
           {
            text:this.$i18n.t('Time_12h'),
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 60 * 1000 * 60 * 12 );
              picker.$emit("pick", [start, end]);
            },
          },
            {
            text:this.$i18n.t('Time_24h'),
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 60 * 1000 * 60 * 24 );
              picker.$emit("pick", [start, end]);
            },
          } 
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
    lang: state => state.lang 
}),
  mounted() { 

  },
  methods: {  
    getLastTime(minutes = 150){
        var now = new Date;
        now.setMinutes (now.getMinutes () - minutes);
        return now;
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
    instanceChange(data){

       this.reload();

    },  
    timeChange(data){ 
      
      this.reload();

    },
    reload(){    

      this.$store.commit("set_query",{ 
        service:this.select_service,
        instance:this.select_instance,
        start:basic.dateFormat(new Date(this.range[0])),
        end:basic.dateFormat(new Date(this.range[1])) 
        
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

     localStorage.setItem('locale',type);

      this.$http
        .post("ChangeLanguage", {
          Language: type,
        })
        .then((response) => { 

          //this.$message({ message: "Switch: " + type, type: "success" });

          this.$http.get(`/static/lang/${type}.json`).then((res) => {

            this.$store.commit("set_lang", res.body); 

             //this.$forceUpdate();

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
