
  <template>
  <div>
    <el-row>
      <el-col :span="24">
        <el-card class="box-card">
          <el-form :inline="true" :model="formInline" class="demo-form-inline" labelPosition="top">
            <el-form-item label="审批人">
              <el-input size="medium" v-model="formInline.user"></el-input>
            </el-form-item>

            <el-form-item label="审批人">
              <el-input size="medium" v-model="formInline.user"></el-input>
            </el-form-item>

            <el-form-item label="审批人">
              <el-input size="medium" v-model="formInline.user"></el-input>
            </el-form-item>

            <el-form-item label="审批人">
              <el-input size="medium" v-model="formInline.user"></el-input>
            </el-form-item>

            <el-form-item label="审批人">
              <el-input size="medium" v-model="formInline.user"></el-input>
            </el-form-item>

            <el-form-item label="审批人">
              <el-input size="medium" v-model="formInline.user"></el-input>
            </el-form-item>
          </el-form>

          <el-table :data="tableData" stripe style="width: 100%">
            <el-table-column prop="date" label="日期" width="180"></el-table-column>
            <el-table-column prop="name" label="姓名" width="180"></el-table-column>
            <el-table-column prop="address" label="地址"></el-table-column>
          </el-table>

          <el-pagination style="margin-top:20px" background layout="prev, pager, next" :total="1000"></el-pagination>


        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>
export default {
  data() {
    return {
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
      formInline: {
        user: "",
        region: "",
      },
    };
  },
  methods: {
    onSubmit() {
      console.log("submit!");
    },
  },
};
</script>


