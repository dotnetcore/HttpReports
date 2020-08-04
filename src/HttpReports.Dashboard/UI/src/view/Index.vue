
<template>
  <el-container>
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
          style="margin-top:16px"
          @click="changeNavState"
          :class="[isCollapse?'el-icon-s-unfold':'el-icon-s-fold']"
        ></i>
      </div>

      <div class="navbar-right">
        <div class="nav-item">
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

          <el-menu-item index="/index/buttons" route="/index/buttons">
            <i class="fa fa-bar-chart-o"></i>
            <span slot="title">{{ this.$store.state.lang.Menu_TrendData }}</span>
          </el-menu-item>

          <el-menu-item index="/d">
            <i class="fa fa-bars"></i>
            <span slot="title">{{ this.$store.state.lang.Menu_RequestList }}</span>
          </el-menu-item>

          <el-menu-item index="/dd">
            <i class="fa fa-rocket"></i>
            <span slot="title">{{ this.$store.state.lang.Menu_Monitor }}</span>
          </el-menu-item>

          <el-menu-item index="/ddd">
            <i class="fa fa-flask"></i>
            <span slot="title">{{ this.$store.state.lang.Performance }}</span>
          </el-menu-item>
        </el-menu>
      </el-aside>

      <el-main style="padding-top:0">
        <el-card class="box-card">
          <div class="block">
            <el-select size="medium" style="margin-right:10px" v-model="value" placeholder="请选择" filterable>
              <el-option
                v-for="item in options1"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              ></el-option>
            </el-select>

            <el-select size="medium" style="margin-right:10px" v-model="value" placeholder="请选择" filterable>
              <el-option
                v-for="item in options1"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              ></el-option>
            </el-select>

            <el-date-picker style="margin-right:10px" size="medium"
              v-model="value2"
              type="datetimerange"
              :picker-options="pickerOptions"
              range-separator="至"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
              align="right"
            ></el-date-picker>

            <el-button size="small" style="margin-right:20px" type="primary" icon="el-icon-search">搜索</el-button>

            <el-divider direction="vertical"></el-divider>

            <el-switch style="margin-right:10px"
  v-model="value1" 
  inactive-text="自动刷新">
</el-switch>

<el-input-number style="width:80px;margin-right:10px"  controls-position="right"  size="mini" v-model="num" :step="3"></el-input-number>

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

body {
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
export default {
  data() {
    return {
      isCollapse: false,
      UpdateDialogVisible: false,
      userName: localStorage.getItem("username"),
      setUserInfo: {
        name: localStorage.getItem("username"),
        oldPwd: "",
        newPwd: "",
      },
      value: '', 
       value1: true,
        value2: true,
         num: 3,
      options1: [
        {
          value: "选项1",
          label: "黄金糕",
        },
        {
          value: "选项2",
          label: "双皮奶",
        },
        {
          value: "选项3",
          label: "蚵仔煎",
        },
        {
          value: "选项4",
          label: "龙须面",
        },
        {
          value: "选项5",
          label: "北京烤鸭",
        },
      ],
      pickerOptions: {
        shortcuts: [
          {
            text: "最近一周",
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
              picker.$emit("pick", [start, end]);
            },
          },
          {
            text: "最近一个月",
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
              picker.$emit("pick", [start, end]);
            },
          },
          {
            text: "最近三个月",
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 3600 * 1000 * 24 * 90);
              picker.$emit("pick", [start, end]);
            },
          },
        ],
      },
      value1: [new Date(2000, 10, 10, 10, 10), new Date(2000, 10, 11, 10, 10)],
      value2: "",
    };
  },
  created: function () {},
  methods: {
    handleClose(done) {
      this.$confirm("确认关闭？")
        .then((_) => {
          done();
        })
        .catch((_) => {});
    },
    changeLanguage(type) {
      this.$http
        .post("ChangeLanguage", {
          Language: type,
        })
        .then((response) => {
          this.$message({ message: "Switch: " + type, type: "success" });

          this.$http.get(`/static/lang/${type}.json`).then((res) => {
            this.$store.commit("set_lang", res.body);
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
  },
};
</script>
