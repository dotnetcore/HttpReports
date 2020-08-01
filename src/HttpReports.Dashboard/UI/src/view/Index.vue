
<template>

  <el-container>

    <el-header>

      <div class="navbar-left">

        <div class="logo" v-show="!isCollapse">
          <img src="http://newoa.hengyinfs.com/Content/style/images/logo-icon.png" />
        </div>

        <div class="title" v-show="isCollapse">
          <span style="font-size:22px;color:#FFF">恒</span>
        </div>

      </div>


      <div class="navbar-center">
        <i @click="changeNavState" class="el-icon-menu"></i>
      </div>

      <div class="navbar-right">

        <div class="nav-item">

          <el-dropdown>

            <span class="el-dropdown-link nav-user">
              <span>{{  this.$store.state.user.username }}</span>
              <!--<img src="http://demo.cssmoban.com/cssthemes6/thse_4_dashloon/assets/images/parson.png" class="img-circle" alt="parson-img">-->
              <i class="el-icon-arrow-down"></i>
            </span>

            <el-dropdown-menu slot="dropdown">
              <el-dropdown-item>修改密码</el-dropdown-item>
              <el-dropdown-item>个人中心</el-dropdown-item>
              <el-dropdown-item>
                <el-link :underline="false" href="/#/user/login">登录</el-link>
              </el-dropdown-item>

              <el-dropdown-item>
                <el-link :underline="false" href="/#/user/login2">登录2</el-link>
              </el-dropdown-item>

            </el-dropdown-menu>

          </el-dropdown>

        </div>

        <div class="nav-item">

        </div>

      </div>

    </el-header>

    <el-container>

      <el-aside style="width:initial;">

        <el-menu :router="true" :unique-opened="true" default-active="1" class="el-menu-vertical-demo" @open="handleOpen" @close="handleClose" :collapse="isCollapse" style="min-height:820px">

          <el-menu-item index="/" route="/">
            <i class="el-icon-s-home"></i>
            <span slot="title">首页</span>
          </el-menu-item>

          <el-submenu index="2">

            <template slot="title">
              <i class="el-icon-menu"></i>
              <span> 基本组件</span>
            </template>

            <el-menu-item index="/index/buttons">按钮</el-menu-item>

            <el-menu-item index="/index/form">表单</el-menu-item>

            <el-menu-item index="/index/animation">动画</el-menu-item>

            <el-menu-item index="/user/store">状态存储VUEX</el-menu-item>

            <el-menu-item>文档管理</el-menu-item>

            <el-menu-item>问题建议</el-menu-item>

          </el-submenu>

          <el-submenu index="3">

            <template slot="title">
              <i class="el-icon-s-custom"></i>
              <span>个人中心</span>
            </template>

            <el-menu-item index="3-1">员工管理</el-menu-item>

            <el-menu-item index="3-2">知识管理</el-menu-item>

            <el-menu-item index="3-3">动态管理</el-menu-item>

            <el-menu-item index="3-4">文档管理</el-menu-item>

            <el-menu-item index="3-5">问题建议</el-menu-item>

          </el-submenu>

          <el-submenu index="4">

            <template slot="title">
              <i class="el-icon-setting"></i>
              <span>设置中心</span>
            </template>

            <el-menu-item index="4-1">网站设置</el-menu-item>

            <el-menu-item index="4-2">系统日志</el-menu-item>

            <el-menu-item index="4-3">角色管理</el-menu-item>

            <el-menu-item index="4-4">菜单管理</el-menu-item>

          </el-submenu>

        </el-menu>

      </el-aside>

      <el-main>

        <!--内容区域-->

        <router-view></router-view>


      </el-main>

    </el-container>

  </el-container>

</template>

<style>

  body {
    background-color: #F3F3F3;
  }

  .el-header {
    background-color: #B3C0D1;
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
        userName: ""
      };
    },
    created: function () {  

      if (this.basic.isEmpty(this.$store.state.user.username)) {

        this.$http.get("api/values/getUserInfo").then((response) => {

          if (response.body.code == 0) {

            this.$store.commit('updateUser', response.body.data); 
          }
          else {
            this.$message({ message: "出错了,请重试!", type: 'error' });
          }
        });

      }

    },
    methods: {
      handleOpen(key, keyPath) {

      },
      handleClose(key, keyPath) {

      },
      changeNavState() {
        this.isCollapse = !this.isCollapse;
      }
    }
  }
</script>
