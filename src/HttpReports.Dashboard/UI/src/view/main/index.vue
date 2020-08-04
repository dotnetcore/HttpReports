<template>

  <div> 

    <el-row :gutter="20" class="tag">

      <el-col :span="6">

        <el-card class="box-card" >

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

       
        <el-col :span="8"> 

          <el-card class="box-card" style="margin-top:0">

            <div id="c1" style="height:240px"></div>
             
          </el-card> 

        </el-col>  

        <el-col :span="8"> 

          <el-card class="box-card" style="margin-top:0">

            <div id="c2" style="height:240px"></div>
             
          </el-card> 

        </el-col>  

        <el-col :span="8"> 

          <el-card class="box-card" style="margin-top:0">

            <div id="c3" style="height:240px"></div>
             
          </el-card> 

        </el-col>  


    </el-row> 

  </div> 


</template>

<style>  


.tag .box-card{

margin-top: 0;

} 

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
    margin-top: 20px;
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



<script>

import { Line } from '@antv/g2plot';

export default {
  data() {
    return {
      
    };
  },
  created: () => {},
  mounted: () => { 
const data = [
  { year: '1991', value: 3 },
  { year: '1992', value: 4 },
  { year: '1993', value: 3.5 },
  { year: '1994', value: 5 },
  { year: '1995', value: 4.9 },
  { year: '1996', value: 6 },
  { year: '1997', value: 7 },
  { year: '1998', value: 9 },
  { year: '1999', value: 13 },
];

const linePlot = new Line(document.getElementById('c1'), {
  title: {
    visible: true,
    text: '带数据点的折线图',
  },
  description: {
    visible: false,
    text: '将折线图上的每一个数据点显示出来，作为辅助阅读。',
  },
  forceFit: true,
  padding: 'auto',
  data,
  xField: 'year',
  yField: 'value',
  point: {
    visible: true,
  },
  label: {
    visible: true,
    type: 'point',
  },
});

linePlot.render(); 

const linePlot2 = new Line(document.getElementById('c2'), {
  title: {
    visible: true,
    text: '带数据点的折线图',
  },
  description: {
    visible: false,
    text: '将折线图上的每一个数据点显示出来，作为辅助阅读。',
  },
  forceFit: true,
  padding: 'auto',
  data,
  xField: 'year',
  yField: 'value',
  point: {
    visible: true,
  },
  label: {
    visible: true,
    type: 'point',
  },
});

linePlot2.render(); 

const linePlot3 = new Line(document.getElementById('c3'), {
  title: {
    visible: true,
    text: '带数据点的折线图',
  },
  description: {
    visible: false,
    text: '将折线图上的每一个数据点显示出来，作为辅助阅读。',
  },
  forceFit: true,
  padding: 'auto',
  data,
  xField: 'year',
  yField: 'value',
  point: {
    visible: true,
  },
  label: {
    visible: true,
    type: 'point',
  },
});

linePlot3.render(); 


  },
  methods: {
     
  },
};
</script>



