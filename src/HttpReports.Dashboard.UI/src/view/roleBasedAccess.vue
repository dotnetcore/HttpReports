<template>
  <div>
    <el-tabs
      style="margin-top: 10px"
      tab-position="top"
      @tab-click="handleClick"
    >
      <el-tab-pane label="系统用户">
        <el-card class="box-card">
          <el-row :gutter="20">
            <el-col :span="26">
              <el-form :inline="true" :model="Search" class="demo-form-inline">
                <el-form-item label="用户名">
                  <el-input
                    v-model="Search.UserName"
                    placeholder="请输入名称查询"
                    >></el-input
                  >
                </el-form-item>
                <el-form-item label="用户Code">
                  <el-input
                    v-model="Search.UserCode"
                    placeholder="请输入Code查询"
                  ></el-input>
                </el-form-item>
              </el-form>
            </el-col>

            <el-col :span="10">
              <el-button
                @click="GetUserList"
                icon="el-icon-search"
                size="small"
                type="primary"
                >{{ $t("Query") }}</el-button
              >
              <el-button
                @click="showAssignRoles"
                icon="el-icon-zoom-in"
                size="small"
                type="primary"
                >分配角色</el-button
              >
              <el-button
                @click="showAssignMenu('user')"
                icon="el-icon-zoom-in"
                size="small"
                type="primary"
                >分配菜单</el-button
              >
            </el-col>
          </el-row>

          <div style="width: 100%">
            <el-table
              style="float: left; width: 33%; height: 580px"
              ref="UserTable"
              :border="true"
              :data="tableData.User"
              highlight-current-row
              @current-change="handCurrentChangeUser"
              @selection-change="handSelectChangeUser"
            >
              <el-table-column type="selection" width="55"> </el-table-column>
              <el-table-column fixed prop="code" label="用户Code">
              </el-table-column>

              <el-table-column fixed prop="name" label="用户名字">
              </el-table-column>

              <el-table-column fixed prop="isvalid" label="状态">
                <template slot-scope="scope">
                  <el-tag
                    size="mini"
                    effect="plain"
                    :type="scope.row.isvalid ? 'primary' : 'danger'"
                    >{{ scope.row.isvalid ? "启用" : "停用" }}</el-tag
                  >
                </template>
              </el-table-column>
            </el-table>

            <el-table
              style="float: left; width: 33%; height: 580px"
              :border="true"
              :data="tableData.Role"
            >
              <el-table-column fixed prop="roleName" label="角色名字">
              </el-table-column>

              <el-table-column fixed prop="remark" label="备注">
              </el-table-column>
            </el-table>

            <el-table
              style="width: 33%; height: 580px"
              :border="true"
              :data="tableData.Menu"
            >
              <el-table-column fixed prop="menuName" label="菜单名字">
              </el-table-column>

              <el-table-column fixed prop="menuUrl" label="菜单地址">
              </el-table-column>
            </el-table>
          </div>

          <div class="block">
            <el-pagination
              style="margin-top: 14px"
              @size-change="handleSizeChangeUser"
              @current-change="handleCurrentChangeUser"
              :current-page="userPage"
              :page-sizes="[10, 20, 50, 100]"
              :page-size="userSize"
              layout="total, sizes, prev, pager, next, jumper"
              :total="Datatotal"
            >
            </el-pagination>
          </div>
        </el-card>
      </el-tab-pane>

      <el-tab-pane label="系统角色">
        <el-card class="box-card">
          <el-row :gutter="20">
            <el-col :span="26">
              <el-form :inline="true" :model="Search" class="demo-form-inline">
                <el-form-item label="角色名">
                  <el-input
                    v-model="Search.RoleName"
                    placeholder="请输入名称查询"
                    >></el-input
                  >
                </el-form-item>
              </el-form>
            </el-col>

            <el-col :span="10">
              <el-button
                @click="GetRolesList"
                icon="el-icon-search"
                size="small"
                type="primary"
                >{{ $t("Query") }}</el-button
              >

              <el-button
                size="small"
                @click="showDialogRole"
                icon="el-icon-circle-plus-outline"
                type="primary"
                >添加</el-button
              >

              <el-button
                @click="showAssignMenu('role')"
                icon="el-icon-zoom-in"
                size="small"
                type="primary"
                >分配菜单</el-button
              >
            </el-col>
          </el-row>
          <div style="width: 100%">
            <el-table
              style="float: left; width: 50%; height: 580px"
              :border="true"
              :data="tableData.Role"
              highlight-current-row
              @current-change="handCurrentChangeRoles"
              @selection-change="handSelectChangeRoles"
            >
              <el-table-column type="selection" width="55"> </el-table-column>
              <el-table-column fixed prop="id" label="ID"> </el-table-column>

              <el-table-column fixed prop="roleName" label="角色名字">
              </el-table-column>

              <el-table-column fixed prop="remark" label="备注">
              </el-table-column>

              <el-table-column fixed="right" label="操作" width="100">
                <template slot-scope="scope">
                  <el-button
                    type="text"
                    size="small"
                    @click="showDialogRole(scope.row)"
                    >{{ scope._self.$i18n.t("Monitor_Edit") }}</el-button
                  >

                  <el-button
                    type="text"
                    size="small"
                    @click="deleteRoles(scope.row.id)"
                    >{{ scope._self.$i18n.t("Monitor_Delete") }}</el-button
                  >
                </template>
              </el-table-column>
            </el-table>

            <el-table
              style="float: left; width: 50%; height: 580px"
              :border="true"
              :data="tableData.Menu"
            >
              <el-table-column fixed prop="id" label="ID"> </el-table-column>

              <el-table-column fixed prop="menuName" label="菜单名字">
              </el-table-column>

              <el-table-column fixed prop="menuUrl" label="菜单地址">
              </el-table-column>
            </el-table>
          </div>
        </el-card>
      </el-tab-pane>

      <el-tab-pane label="系统菜单">
        <el-card class="box-card">
          <el-row :gutter="20">
            <el-col :span="26">
              <el-form :inline="true" :model="Search" class="demo-form-inline">
                <el-form-item label="菜单名">
                  <el-input
                    v-model="Search.MenuName"
                    placeholder="请输入名称查询"
                    >></el-input
                  >
                </el-form-item>
              </el-form>
            </el-col>

            <el-col :span="5">
              <el-button
                @click="GetMenuList"
                icon="el-icon-search"
                size="small"
                type="primary"
                >{{ $t("Query") }}</el-button
              >

              <el-button
                size="small"
                @click="showDialogMenu"
                icon="el-icon-circle-plus-outline"
                type="primary"
                >添加</el-button
              >
            </el-col>
          </el-row>

          <el-table :border="true" :data="tableData.Menu" height="580">
            <el-table-column type="selection" width="55"> </el-table-column>
            <el-table-column fixed prop="id" label="ID"> </el-table-column>

            <el-table-column fixed prop="menuName" label="菜单名字">
            </el-table-column>

            <el-table-column fixed prop="menuUrl" label="菜单地址">
            </el-table-column>

            <el-table-column fixed="right" label="操作" width="100">
              <template slot-scope="scope">
                <el-button
                  type="text"
                  size="small"
                  @click="showDialogMenu(scope.row)"
                  >{{ scope._self.$i18n.t("Monitor_Edit") }}</el-button
                >

                <el-button
                  type="text"
                  size="small"
                  @click="deleteMenu(scope.row.id)"
                  >{{ scope._self.$i18n.t("Monitor_Delete") }}</el-button
                >
              </template>
            </el-table-column>
          </el-table>

          <div class="block">
            <el-pagination
              style="margin-top: 14px"
              @size-change="handleSizeChangeMenu"
              @current-change="handleCurrentChangeMenu"
              :current-page="menuPage"
              :page-sizes="[10, 20, 50, 100]"
              :page-size="menuSize"
              layout="total, sizes, prev, pager, next, jumper"
              :total="Datatotal"
            >
            </el-pagination>
          </div>
        </el-card>
      </el-tab-pane>
    </el-tabs>

    <el-dialog
      title="角色编辑"
      top="5%"
      :visible.sync="dialogRoles"
      width="25%"
    >
      <el-form size="small" ref="Roles" :model="Roles" label-width="100px">
        <el-form-item label="角色名称" prop="roleName">
          <el-input v-model="Roles.roleName"></el-input>
        </el-form-item>

        <el-form-item label="备注" prop="remark">
          <el-input v-model="Roles.remark"></el-input>
        </el-form-item>
      </el-form>

      <div slot="footer" class="dialog-footer">
        <el-button size="small" @click="dialogRoles = false">{{
          this.$store.state.lang.Button_Cancel
        }}</el-button>
        <el-button size="small" type="primary" @click="saveRole">{{
          this.$store.state.lang.Button_OK
        }}</el-button>
      </div>
    </el-dialog>

    <el-dialog title="菜单编辑" top="5%" :visible.sync="dialogMenu" width="25%">
      <el-form size="small" ref="Menu" :model="Menu" label-width="100px">
        <el-form-item label="菜单名称" prop="menuName">
          <el-input v-model="Menu.menuName"></el-input>
        </el-form-item>

        <el-form-item label="菜单地址" prop="menuUrl">
          <el-input v-model="Menu.menuUrl"></el-input>
        </el-form-item>
      </el-form>

      <div slot="footer" class="dialog-footer">
        <el-button size="small" @click="dialogMenu = false">{{
          this.$store.state.lang.Button_Cancel
        }}</el-button>
        <el-button size="small" type="primary" @click="saveMenu">{{
          this.$store.state.lang.Button_OK
        }}</el-button>
      </div>
    </el-dialog>

    <el-dialog
      title="角色分配"
      top="5%"
      :visible.sync="dialogAssignRoles"
      width="25%"
    >
      <el-form size="small" ref="Roles" :model="Roles" label-width="100px">
        <el-form-item label="系统角色" prop="Roles">
          <el-select
            v-model="checkRoles"
            multiple
            filterable
            remote
            reserve-keyword
            placeholder="请输入关键词"
            :remote-method="remoteMethodRoles"
            :loading="loading"
          >
            <el-option
              v-for="item in optionsRoles"
              :key="item.id"
              :label="item.roleName"
              :value="item.id"
            >
            </el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button size="small" @click="dialogAssignRoles = false">{{
          this.$store.state.lang.Button_Cancel
        }}</el-button>
        <el-button size="small" type="primary" @click="assignUserRoles">{{
          this.$store.state.lang.Button_OK
        }}</el-button>
      </div>
    </el-dialog>

    <el-dialog
      title="菜单分配"
      top="5%"
      :visible.sync="dialogAssignMenu"
      width="25%"
    >
      <el-form size="small" ref="Menu" :model="Menu" label-width="100px">
        <el-form-item label="系统菜单" prop="Menu">
          <el-select
            v-model="checkMenu"
            multiple
            filterable
            remote
            reserve-keyword
            placeholder="请输入关键词"
            :remote-method="remoteMethodMenu"
            :loading="loading"
          >
            <el-option
              v-for="item in optionsMenu"
              :key="item.id"
              :label="item.menuName"
              :value="item.id"
            >
            </el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button size="small" @click="dialogAssignMenu = false">{{
          this.$store.state.lang.Button_Cancel
        }}</el-button>
        <el-button size="small" type="primary" @click="assignMenu">{{
          this.$store.state.lang.Button_OK
        }}</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { basic } from "@/common/basic.js";
export default {
  data() {
    return {
      loading: false,
      dialogRoles: false,
      dialogMenu: false,
      dialogAssignRoles: false,
      dialogAssignMenu: false,
      userPage: 1,
      userSize: 10,
      menuPage: 1,
      menuSize: 10,
      Datatotal: 0,
      Search: {
        UserName: "",
        UserCode: "",
        RoleName: "",
        MenuName: "",
      },
      tableData: {
        User: [],
        Role: [],
        Menu: [],
      },
      Roles: {
        id: 0,
        roleName: "",
        remark: "",
      },
      Menu: {
        id: 0,
        menuName: "",
        menuUrl: "",
      },
      selectUser: [],
      selectRole: [],
      optionsRoles: [],
      optionsMenu: [],
      checkRoles: [],
      checkMenu: [],
      assignMenuType: "user",
    };
  },
  created: function () {
    this.GetUserList();
  },
  methods: {
    GetUserList() {
      var self = this;
      self.tableData.Role = [];
      self.tableData.Menu = [];
      self.$http
        .get(`${basic.DOMAIN}/SysUser/GetListByPage`, {
          params: {
            Page: self.userPage,
            Size: self.userSize,
            UserName: self.Search.UserName,
            UserCode: self.Search.UserCode,
          },
        })
        .then((data) => {
          self.tableData.User = data.body.list;
          self.Datatotal = data.body.totalCount;
        });
    },
    GetRolesList() {
      var self = this;
      self.tableData.Menu = [];
      self.$http
        .get(`${basic.DOMAIN}/Roles/GetList?name=${self.Search.RoleName}`)
        .then((data) => {
          self.tableData.Role = data.body;
        });
    },
    GetMenuList() {
      var self = this;
      self.$http
        .get(`${basic.DOMAIN}/Menu/GetListByPage`, {
          params: {
            page: self.menuPage,
            size: self.menuSize,
            name: self.Search.MenuName,
          },
        })
        .then((data) => {
          self.tableData.Menu = data.body.list;
          self.Datatotal = data.body.totalCount;
        });
    },
    showDialogRole(obj) {
      this.dialogRoles = true;
      if (obj) {
        this.Roles.id = obj.id;
        this.Roles.roleName = obj.roleName;
        this.Roles.remark = obj.remark;
      } else {
        this.Roles.roleName = "";
        this.Roles.remark = "";
      }
    },
    showDialogMenu(obj) {
      this.dialogMenu = true;
      console.log(obj);
      if (obj) {
        this.Menu.id = obj.id;
        this.Menu.menuName = obj.menuName;
        this.Menu.menuUrl = obj.menuUrl;
      } else {
        this.Menu.menuName = "";
        this.Menu.menuUrl = "";
      }
    },
    saveMenu() {
      var self = this;
      self.$http.post(`${basic.DOMAIN}/Menu/Save`, self.Menu).then((data) => {
        if (data.body) {
          self.$message({ message: "保存成功", type: "success" });
          self.dialogMenu = false;
          self.GetMenuList();
        }
      });
    },
    saveRole() {
      var self = this;
      self.$http.post(`${basic.DOMAIN}/Roles/Save`, self.Roles).then((data) => {
        if (data.body) {
          self.$message({ message: "保存成功", type: "success" });
          self.dialogRoles = false;
          self.GetRolesList();
        }
      });
    },
    deleteMenu(id) {
      var self = this;
      self
        .$confirm(
          `注意!会同时删除已分配该菜单的所有用户和角色权限,确定要删除${id}吗?`,
          "",
          {
            confirmButtonText: self.$i18n.t("Button_OK"),
            cancelButtonText: self.$i18n.t("Button_Cancel"),
            type: "warning",
          }
        )
        .then(async () => {
          self.$http
            .delete(`${basic.DOMAIN}/Menu/DeleteById/${id}`)
            .then((data) => {
              if (data.body) {
                self.$message({
                  type: "success",
                  message: self.$i18n.t("Monitor_DeleteSuccess"),
                });
                self.GetMenuList();
              } else {
                self.$message({
                  type: "error",
                  message: "删除失败",
                });
              }
            });
        });
    },
    deleteRoles(id) {
      var self = this;
      self
        .$confirm(
          `注意!会同时删除已分配该角色的所有用户权限,确定要删除${id}吗?`,
          "",
          {
            confirmButtonText: self.$i18n.t("Button_OK"),
            cancelButtonText: self.$i18n.t("Button_Cancel"),
            type: "warning",
          }
        )
        .then(async () => {
          self.$http
            .delete(`${basic.DOMAIN}/Roles/DeleteById/${id}`)
            .then((data) => {
              if (data.body) {
                self.$message({
                  type: "success",
                  message: self.$i18n.t("Monitor_DeleteSuccess"),
                });
                self.GetRolesList();
              } else {
                self.$message({
                  type: "error",
                  message: "删除失败",
                });
              }
            });
        });
    },
    showAssignRoles() {
      if (this.selectUser.length) {
        this.checkRoles = [];
        this.dialogAssignRoles = true;
      } else {
        this.$message({
          showClose: true,
          message: "请至少选择一条",
        });
      }
    },
    showAssignMenu(type) {
      this.assignMenuType = type;
      this.checkMenu = [];
      if (type == "user") {
        if (this.selectUser.length) {
          this.dialogAssignMenu = true;
        } else {
          this.$message({
            showClose: true,
            message: "请至少选择一个用户",
          });
        }
      } else {
        if (this.selectRole.length) {
          this.dialogAssignMenu = true;
        } else {
          this.$message({
            showClose: true,
            message: "请至少选择一个角色",
          });
        }
      }
    },
    assignUserRoles() {
      var self = this;
      if (self.checkRoles.length) {
        self.$http
          .post(`${basic.DOMAIN}/SysUser/AssignRoles`, {
            UserCode: self.selectUser.map((item) => {
              return item.code;
            }),
            RoleId: self.checkRoles,
          })
          .then((data) => {
            if (data.body) {
              self.$message({ message: "保存成功", type: "success" });
              self.dialogAssignRoles = false;
            } else {
              self.$message({ message: data.body.message, type: "error" });
            }
          });
      } else {
        self.$message({
          showClose: true,
          message: "请至少选择一条",
        });
      }
    },
    assignMenu() {
      var self = this;
      if (self.checkMenu.length) {
        if (self.assignMenuType == "user") {
          self.assignUserMenu();
        } else {
          self.assignRoleMenu();
        }
      } else {
        self.$message({
          showClose: true,
          message: "请至少选择一条",
        });
      }
    },
    assignUserMenu() {
      var self = this;
      self.$http
        .post(`${basic.DOMAIN}/SysUser/AssignMenu`, {
          UserCode: self.selectUser.map((item) => {
            return item.code;
          }),
          MenuId: self.checkMenu,
        })
        .then((data) => {
          if (data.body) {
            self.$message({ message: "保存成功", type: "success" });
            self.dialogAssignMenu = false;
          } else {
            self.$message({ message: data.body.message, type: "error" });
          }
        });
    },
    assignRoleMenu() {
      var self = this;
      self.$http
        .post(`${basic.DOMAIN}/Roles/AssignMenu`, {
          RolesId: self.selectRole.map((item) => {
            return item.id;
          }),
          MenuId: self.checkMenu,
        })
        .then((data) => {
          if (data.body) {
            self.$message({ message: "保存成功", type: "success" });
            self.dialogAssignMenu = false;
          } else {
            self.$message({ message: data.body.message, type: "error" });
          }
        });
    },
    remoteMethodRoles(query) {
      var self = this;
      if (query !== "") {
        self.loading = true;

        self.$http
          .get(`${basic.DOMAIN}/Roles/GetList?name=${query}`)
          .then((data) => {
            self.loading = false;
            self.optionsRoles = data.body;
          });
      } else {
        this.optionsRoles = [];
      }
    },
    remoteMethodMenu(query) {
      var self = this;
      if (query !== "") {
        self.loading = true;

        self.$http
          .get(
            `${basic.DOMAIN}/Menu/GetListByPage?page=1&size=100&name=${query}`
          )
          .then((data) => {
            self.loading = false;
            self.optionsMenu = data.body.list;
          });
      } else {
        this.optionsMenu = [];
      }
    },
    handSelectChangeUser(selection) {
      this.selectUser = selection;
    },
    handCurrentChangeUser(currentRow, oldCurrentRow) {
      if (currentRow) {
        var self = this;
        self.$http
          .get(`${basic.DOMAIN}/Roles/GetListByUserCode/${currentRow.code}`)
          .then((data) => {
            self.tableData.Role = data.body;
          });

        self.$http
          .get(`${basic.DOMAIN}/Menu/GetListByUserCode/${currentRow.code}`)
          .then((data) => {
            self.tableData.Menu = data.body;
          });
      }
    },
    handCurrentChangeRoles(currentRow, oldCurrentRow) {
      if (currentRow) {
        var self = this;
        self.$http
          .get(`${basic.DOMAIN}/Menu/GetListByRolesId/${currentRow.id}`)
          .then((data) => {
            self.tableData.Menu = data.body;
          });
      }
    },
    handSelectChangeRoles(selection) {
      this.selectRole = selection;
    },
    handleClick(comp, event) {
      switch (comp.index) {
        case "0":
          this.tableData.Role = [];
          this.tableData.Menu = [];
          this.GetUserList();
          break;
        case "1":
          this.tableData.Menu = [];
          this.GetRolesList();
          break;
        case "2":
          this.GetMenuList();
          break;
        default:
          this.GetUserList();
      }
    },

    handleSizeChangeUser(val) {
      this.userSize = val;
      this.GetUserList();
    },
    handleCurrentChangeUser(val) {
      this.userPage = val;
      this.GetUserList();
    },
    handleSizeChangeMenu(val) {
      this.menuSize = val;
      this.GetMenuList();
    },
    handleCurrentChangeMenu(val) {
      this.menuPage = val;
      this.GetMenuList();
    },
  },
};
</script>
