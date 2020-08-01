<template>

  <div>

    <el-row>
      <el-carousel :interval="5000" arrow="always">
        <el-carousel-item v-for="item in 4" :key="item">
          <h3 style="text-align:center">title</h3>
        </el-carousel-item>
      </el-carousel>
    </el-row>

    <el-row :gutter="20">

      <el-col :span="6">

        <el-card class="box-card">

          <i class="el-icon-s-custom icon-rounded pull-left bgc1"></i>

          <div class="stats">
            <h5><strong>452</strong></h5>
            <span>Total Revenue</span>
          </div>

        </el-card>

      </el-col>

      <el-col :span="6">

        <el-card class="box-card">

          <i class="el-icon-s-custom icon-rounded pull-left bgc2"></i>

          <div class="stats">
            <h5><strong>452</strong></h5>
            <span>Total Revenue</span>
          </div>

        </el-card>

      </el-col>

      <el-col :span="6">

        <el-card class="box-card">

          <i class="el-icon-s-custom icon-rounded pull-left bgc3"></i>

          <div class="stats">
            <h5><strong>452</strong></h5>
            <span>Total Revenue</span>
          </div>

        </el-card>

      </el-col>

      <el-col :span="6">

        <el-card class="box-card">

          <i class="el-icon-s-custom icon-rounded pull-left bgc4"></i>

          <div class="stats">
            <h5><strong>452</strong></h5>
            <span>Total Revenue</span>
          </div>

        </el-card>


      </el-col>

    </el-row>

    <el-row :gutter="20">

      <el-col :span="18">

        <el-col :span="6" v-for="o in 4" :key="o"> 

          <el-card :body-style="{ padding: '0px' }">
            <img src="https://shadow.elemecdn.com/app/element/hamburger.9cf7b091-55e9-11e9-a976-7f4d0b07eef6.png" class="image">
            <div style="padding: 14px;">
              <span>好吃的汉堡</span>
              <div class="bottom clearfix">
                <time class="time">2018-02-01</time>
                <el-button type="text" class="button">操作按钮</el-button>
              </div>
            </div>
          </el-card> 

        </el-col> 

      </el-col>

      <el-col :span="6">
        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <span>卡片名称</span>
            <el-button style="float: right; padding: 3px 0" type="text">操作按钮</el-button>
          </div>
          <div v-for="o in 10" :key="o" class="text item">
            {{'列表内容 ' + o }}
          </div>
        </el-card>
      </el-col>


    </el-row> 

  </div> 


</template>

<style>  


   .el-carousel__item h3 {
    color: #475669;
    font-size: 18px;
    opacity: 0.75;
    line-height: 300px;
    margin: 0;
  }
  
  .el-carousel__item:nth-child(2n) {
    background-color: #99a9bf;
  }
  
  .el-carousel__item:nth-child(2n+1) {
    background-color: #d3dce6;
  }


  .el-row {
    margin-bottom: 20px;
    &:last-child

  {
    margin-bottom: 0;
  }

  }

  .el-col {
    border-radius: 4px;
  }

  .bg-purple-dark {
    background: #99a9bf;
  }

  .bg-purple {
    background: #d3dce6;
  }

  .bg-purple-light {
    background: #e5e9f2;
  }

  .grid-content {
    border-radius: 4px;
    min-height: 36px;
    min-height:220px;
  }

  .row-bg {
    padding: 10px 0;
    background-color: #f9fafc;
  }
</style>
