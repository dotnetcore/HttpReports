<template>
  <div>
    <el-card class="box-card">
       <div style="height:620px" id="main-chart"></div>
    </el-card>
  </div>
</template>

<style>
.box-card {
  margin-top: 20px;
}
</style>

<script> 

import G6 from '@antv/g6';

export default { 
  data() {
    return {
        startVal: 0,
        endVal: 2017
      }
  },
  created: function () {},
  mounted(){ 

      const data = {
  nodes: [
    {
      id: '0',
      label: '0',
    },
    {
      id: '1',
      label: '1',
    },
    {
      id: '2',
      label: '2',
    },
    {
      id: '3',
      label: '3',
    },
    {
      id: '4',
      label: '4',
    },
    {
      id: '5',
      label: '5',
    },
    {
      id: '6',
      label: '6',
    },
    {
      id: '7',
      label: '7',
    },
    {
      id: '8',
      label: '8',
    },
    {
      id: '9',
      label: '9',
    },
  ],
  edges: [
    {
      source: '0',
      target: '1',
    },
    {
      source: '0',
      target: '2',
    },
    {
      source: '0',
      target: '3',
    },
    {
      source: '0',
      target: '4',
    },
    {
      source: '0',
      target: '5',
    },
    {
      source: '0',
      target: '7',
    },
    {
      source: '0',
      target: '8',
    },
    {
      source: '0',
      target: '9',
    },
    {
      source: '2',
      target: '3',
    },
    {
      source: '4',
      target: '5',
    },
    {
      source: '4',
      target: '6',
    },
    {
      source: '5',
      target: '6',
    },
  ],
};

const width = document.getElementById('main-chart').scrollWidth;
const height = document.getElementById('main-chart').scrollHeight || 500;
const graph = new G6.Graph({
  container: 'main-chart',
  width,
  height,
  layout: {
    type: 'force',
    preventOverlap: true,
    nodeSize: 100,
  },
  modes: {
    default: ['drag-node'],
  },
  defaultNode: {
    size: 50,
    color: '#5B8FF9',
    style: {
      lineWidth: 2,
      fill: '#C6E5FF',
    },
  },
  defaultEdge: {
    size: 1,
    color: '#e2e2e2',
  },
});
graph.data(data);
graph.render();

function refreshDragedNodePosition(e) {
  const model = e.item.get('model');
  model.fx = e.x;
  model.fy = e.y;
}
graph.on('node:dragstart', (e) => {
  graph.layout();
  refreshDragedNodePosition(e);
});
graph.on('node:drag', (e) => {
  refreshDragedNodePosition(e);
});





  }
};
</script>
