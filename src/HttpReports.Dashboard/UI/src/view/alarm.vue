<template>
  <div>
    <el-tabs style="margin-top:10px" tab-position="top">
      <el-tab-pane label="监控">
        <el-card class="box-card">
          <el-row>
            <el-button
              size="mini"
              @click="dialogFormVisible = true"
              icon="el-icon-circle-plus-outline"
              type="primary"
            >新增监控</el-button>
          </el-row>

          <el-table :data="tableData" style="width: 100%">
            <el-table-column prop="date" label="日期" width="180"></el-table-column>
            <el-table-column prop="name" label="姓名" width="180"></el-table-column>
            <el-table-column prop="address" label="地址"></el-table-column>
          </el-table>
        </el-card>
      </el-tab-pane>

      <el-tab-pane label="告警">
        <el-card class="box-card">
          <div class="block">
            <el-timeline :reverse="true">
              <el-timeline-item
                v-for="(activity, index) in activities"
                :key="index"
                :timestamp="activity.timestamp"
              >{{activity.content}}</el-timeline-item>
            </el-timeline>
          </div>
        </el-card>
      </el-tab-pane>
    </el-tabs>

    <el-dialog title="新增监控" :visible.sync="dialogFormVisible">
      <el-form size="small" ref="form" :model="monitor" label-width="80px">
        <el-form-item label="标题">
          <el-input v-model="monitor.title"></el-input>
        </el-form-item>

        <el-form-item label="描述">
          <el-input type="textarea" v-model="monitor.description"></el-input>
        </el-form-item>

        <el-form-item label="服务实例">
          <el-select v-model="monitor.service">
            <el-option label="区域一" value="shanghai"></el-option>
            <el-option label="区域二" value="beijing"></el-option>
          </el-select>

          <el-select v-model="monitor.instance" style="margin-left:12px">
            <el-option label="区域一" value="shanghai"></el-option>
            <el-option label="区域二" value="beijing"></el-option>
          </el-select>
        </el-form-item>

        <el-form-item label="邮箱">
          <el-input v-model="monitor.emails"></el-input>
        </el-form-item>

        <el-form-item label="WebHook">
          <el-input v-model="monitor.webhook"></el-input>
        </el-form-item>

        <el-form-item label="执行时间">
          <el-time-select
            placeholder="起始时间"
            v-model="monitor.startTime"
            :picker-options="{
            start: '00:00',
            step: '00:30',
            end: '23:00' }"
          ></el-time-select>
          <el-time-select
            placeholder="结束时间"
            v-model="monitor.endTime"
            :picker-options="{  start: '00:00',
            step: '00:30',
            end: '23:00',
      minTime: startTime   }"
          ></el-time-select>
        </el-form-item>

        <el-form-item label="执行频率">
          <el-select v-model="monitor.interval">
            <el-option label="1分钟" value="1分钟"></el-option>
            <el-option label="3分钟" value="3分钟"></el-option>
          </el-select>
        </el-form-item>

        <el-form-item label="是否开启">
          <el-switch v-model="monitor.enabled"></el-switch>
        </el-form-item>

        <el-tabs type="border-card">
          <el-tab-pane label="响应时间监控">响应时间监控</el-tab-pane>
          <el-tab-pane label="服务错误监控">服务错误监控</el-tab-pane>
          <el-tab-pane label="负载监控">负载监控</el-tab-pane> 
        </el-tabs>
      </el-form>

      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取 消</el-button>
        <el-button type="primary" @click="dialogFormVisible = false">确 定</el-button>
      </div>
    </el-dialog>
  </div>
</template> 

<style>
.el-tabs__header {
  margin: 0;
}
</style>

<script>
export default {
  data() {
    return {
      dialogFormVisible: false,
      monitor: {
        title: "",
        description: "",
        service: "",
        instance: "",
        emails: "",
        webhook: "",
        startTime: "",
        endTime: "",
        enabled: true,
        interval: "",
        name: "",
        region: "",
        date1: "",
        date2: "",
        delivery: false,
        type: [],
        resource: "",
        desc: "",
      },
      tableData: [
        {
          date: "2016-05-02",
          name: "王小虎",
          address: "上海市普陀区金沙江路 1518 弄",
        },
        {
          date: "2016-05-04",
          name: "王小虎",
          address: "上海市普陀区金沙江路 1517 弄",
        },
        {
          date: "2016-05-01",
          name: "王小虎",
          address: "上海市普陀区金沙江路 1519 弄",
        },
        {
          date: "2016-05-03",
          name: "王小虎",
          address: "上海市普陀区金沙江路 1516 弄",
        },
      ],
      activities: [
        {
          content: "活动按期开始",
          timestamp: "2018-04-15",
        },
        {
          content: "通过审核",
          timestamp: "2018-04-13",
        },
        {
          content: "创建成功",
          timestamp: "2018-04-11",
        },
      ],
    };
  },
  created: function () {},
  mounted() {},
};
</script>
