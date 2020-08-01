<template>

  <div>  

    <el-card class="box-card">

      <h3>基础用法</h3>

      <el-row style="margin-top:20px;">
        <el-button>默认按钮</el-button>
        <el-button type="primary">主要按钮</el-button>
        <el-button type="success">成功按钮</el-button>
        <el-button type="info">信息按钮</el-button>
        <el-button type="warning">警告按钮</el-button>
        <el-button type="danger">危险按钮</el-button>
      </el-row>

      <el-row style="margin-top:20px;">
        <el-button plain>朴素按钮</el-button>
        <el-button type="primary" plain>主要按钮</el-button>
        <el-button type="success" plain>成功按钮</el-button>
        <el-button type="info" plain>信息按钮</el-button>
        <el-button type="warning" plain>警告按钮</el-button>
        <el-button type="danger" plain>危险按钮</el-button>
      </el-row>

      <el-row style="margin-top:20px;">
        <el-button round>圆角按钮</el-button>
        <el-button type="primary" round>主要按钮</el-button>
        <el-button type="success" round>成功按钮</el-button>
        <el-button type="info" round>信息按钮</el-button>
        <el-button type="warning" round>警告按钮</el-button>
        <el-button type="danger" round>危险按钮</el-button>
      </el-row>

      <el-row style="margin-top:20px;">
        <el-button icon="el-icon-search" circle></el-button>
        <el-button type="primary" icon="el-icon-edit" circle></el-button>
        <el-button type="success" icon="el-icon-check" circle></el-button>
        <el-button type="info" icon="el-icon-message" circle></el-button>
        <el-button type="warning" icon="el-icon-star-off" circle></el-button>
        <el-button type="danger" icon="el-icon-delete" circle></el-button>
      </el-row>


    </el-card>

    <el-card class="box-card">

      <h3>禁用状态</h3>

      <el-row style="margin-top:20px;">
        <el-button disabled>默认按钮</el-button>
        <el-button type="primary" disabled>主要按钮</el-button>
        <el-button type="success" disabled>成功按钮</el-button>
        <el-button type="info" disabled>信息按钮</el-button>
        <el-button type="warning" disabled>警告按钮</el-button>
        <el-button type="danger" disabled>危险按钮</el-button>
      </el-row>

      <el-row style="margin-top:20px;">
        <el-button plain disabled>朴素按钮</el-button>
        <el-button type="primary" plain disabled>主要按钮</el-button>
        <el-button type="success" plain disabled>成功按钮</el-button>
        <el-button type="info" plain disabled>信息按钮</el-button>
        <el-button type="warning" plain disabled>警告按钮</el-button>
        <el-button type="danger" plain disabled>危险按钮</el-button>
      </el-row>
    </el-card>

    <el-card class="box-card">

      <h3>文字按钮</h3>

      <el-row style="margin-top:20px;">
        <el-button type="text">文字按钮</el-button>
        <el-button type="text" disabled>文字按钮</el-button>
      </el-row>

    </el-card>

    <el-card class="box-card">

      <h3>图标按钮</h3>

      <el-row style="margin-top:20px;">
        <el-button type="primary" icon="el-icon-edit"></el-button>
        <el-button type="primary" icon="el-icon-share"></el-button>
        <el-button type="primary" icon="el-icon-delete"></el-button>
        <el-button type="primary" icon="el-icon-search">搜索</el-button>
        <el-button type="primary">上传<i class="el-icon-upload el-icon--right"></i></el-button>
      </el-row>

    </el-card>

  </div> 

</template>

<style>

  .box-card {
    margin-top:20px;
  }


</style>

<script>
  export default {
    data() {
      return {
        isCollapse: false,
        userName:""
      };
    },
    created: function () { 
      
    } 
  }
</script>
