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


     <el-row :gutter="20">

       
        <el-col :span="12"> 

          <el-card class="box-card" style="margin-top:0">

            <div id="d1" style="height:240px"></div>
             
          </el-card> 

        </el-col>  

        <el-col :span="12"> 

          <el-card class="box-card" style="margin-top:0">

            <div id="d2" style="height:240px"></div>
             
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
import { Chart } from '@antv/g2';

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
 
 
 const data3 = [
  [0, 0, 10],
  [0, 1, 19],
  [0, 2, 8],
  [0, 3, 24],
  [0, 4, 67],
  [1, 0, 92],
  [1, 1, 58],
  [1, 2, 78],
  [1, 3, 117],
  [1, 4, 48],
  [2, 0, 35],
  [2, 1, 15],
  [2, 2, 123],
  [2, 3, 64],
  [2, 4, 52],
  [3, 0, 72],
  [3, 1, 132],
  [3, 2, 114],
  [3, 3, 19],
  [3, 4, 16],
  [4, 0, 38],
  [4, 1, 5],
  [4, 2, 8],
  [4, 3, 117],
  [4, 4, 115],
  [5, 0, 88],
  [5, 1, 32],
  [5, 2, 12],
  [5, 3, 6],
  [5, 4, 120],
  [6, 0, 13],
  [6, 1, 44],
  [6, 2, 88],
  [6, 3, 98],
  [6, 4, 96],
  [7, 0, 31],
  [7, 1, 1],
  [7, 2, 82],
  [7, 3, 32],
  [7, 4, 30],
  [8, 0, 85],
  [8, 1, 97],
  [8, 2, 123],
  [8, 3, 64],
  [8, 4, 84],
  [9, 0, 47],
  [9, 1, 114],
  [9, 2, 31],
  [9, 3, 48],
  [9, 4, 91],
  [10, 0, 10],
  [10, 1, 19],
  [10, 2, 8],
  [10, 3, 24],
  [10, 4, 67],
  [11, 0, 92],
  [11, 1, 58],
  [11, 2, 78],
  [11, 3, 117],
  [11, 4, 48],
  [12, 0, 35],
  [12, 1, 15],
  [12, 2, 123],
  [12, 3, 64],
  [12, 4, 52],
  [13, 0, 72],
  [13, 1, 132],
  [13, 2, 114],
  [13, 3, 19],
  [13, 4, 16],
  [14, 0, 38],
  [14, 1, 5],
  [14, 2, 8],
  [14, 3, 117],
  [14, 4, 115],
  [15, 0, 88],
  [15, 1, 32],
  [15, 2, 12],
  [15, 3, 6],
  [15, 4, 120],
  [16, 0, 13],
  [16, 1, 44],
  [16, 2, 88],
  [16, 3, 98],
  [16, 4, 96],
  [17, 0, 31],
  [17, 1, 1],
  [17, 2, 82],
  [17, 3, 32],
  [17, 4, 30],
  [18, 0, 85],
  [18, 1, 97],
  [18, 2, 123],
  [18, 3, 64],
  [18, 4, 84],
  [19, 0, 47],
  [19, 1, 114],
  [19, 2, 31],
  [19, 3, 48],
  [19, 4, 91],
];



const source = data3.map((arr) => {
  return {
    name: arr[0],
    day: arr[1],
    sales: arr[2],
  };
});

const chart = new Chart({
  container: 'd1',
  autoFit: true,
  height: 340,
});

chart.data(source);

chart.scale('name', {
  type: 'cat',
  values: ['21:00', '21:10', '21:20', '21:30', '21:40', '21:50', '22:00', '22:10', 'Tim', 'Laura','Alexander', 'Marie', 'Maximilian', 'Sophia', 'Lukas', 'Maria', 'Leon', 'Anna', 'Tim', 'Laura'],
});
chart.scale('day', {
  type: 'cat',
  values: ['0-200', '200-400', '400-600', '600-800', '800-1000'],
});
chart.scale('sales', {
  nice: true,
});

chart.axis('name', {
  tickLine: null,
  grid: {
    alignTick: false,
    line: {
      style: {
        lineWidth: 1,
        lineDash: null,
        stroke: '#f0f0f0',
      },
    },
  },
});

chart.axis('day', {
  title: null,
  grid: {
    alignTick: false,
    line: {
      style: {
        lineWidth: 1,
        lineDash: null,
        stroke: '#f0f0f0',
      },
    },
  },
});

chart.legend(false); 

chart.tooltip({
  showMarkers: false,
});

chart
  .polygon()
  .position('name*day')
  .color('sales', '#BAE7FF-#1890FF-#0050B3')
  .label('sales', {
    offset: -2,
    style: {
      fill: '#fff',
      shadowBlur: 2,
      shadowColor: 'rgba(0, 0, 0, .45)',
    },
  })
  .style({
    lineWidth: 1,
    stroke: '#fff',
  });

  chart.interaction('brush-visible');

chart.interaction('element-active');

chart.render(); 



  },
  methods: {
     
  },
}; 

</script>



