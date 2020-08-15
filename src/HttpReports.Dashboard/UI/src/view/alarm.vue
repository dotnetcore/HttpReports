<template>
  <div>
    <el-card class="box-card">
      <el-tabs tab-position="left">
        <el-tab-pane label="用户管理">
        
        </el-tab-pane>
        <el-tab-pane label="配置管理">配置管理</el-tab-pane> 
      </el-tabs>
    </el-card>
  </div>
</template>

<style>
.box-card {
  margin-top: 20px;
}
</style>

<script> 

export default { 
  data() {
    return {
        startVal: 0,
        endVal: 2017
      }
  },
  created: function () {},
};
</script>
