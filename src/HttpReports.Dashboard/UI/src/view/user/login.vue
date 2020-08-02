
<style type="text/css">
body {
  height: 100%;
  background-color: #f3f3f9;
}

.card-body {
  margin: 0 auto;
  width: 540px;
  height: 400px;
  margin-top: 10%;
  display: block;
  background-color: #fff;
  border-radius: 12px;
  box-shadow: 6px 11px 41px -28px #a99de7;
  padding: 30px;
  text-align: center;
}

.card-body h3 {
  font-size: 22px;
  margin-bottom: 0.5rem;
  font-family: inherit;
  line-height: 1.2;
  color: #222222;
  margin-bottom: 42px;
}

.el-input__inner {
  margin-top: 24px;
  height: 48px;
}

.login-form__btn.submit {
  margin-top: 52px;
  padding: 10px 40px;
  background: #7571f9;
  font-weight: 700;
  display: inline-block;
  font-weight: 400;
  text-align: center;
  white-space: nowrap;
  vertical-align: middle;
  user-select: none;
  border: 1px solid transparent;
  font-size: 18px;
  line-height: 1.5;
  border-radius: 0.25rem;
  color: #fff;
  width: 100%;
  cursor: pointer;
  transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out,
    border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
}
</style>


<template>
  <div class="back-ground-login2">
    <div class="card-body">
      <h3>后台管理系统</h3>

      <el-input v-model="username" placeholder="请输入用户名"></el-input>

      <el-input type="password" v-model="password" placeholder="请输入密码"></el-input>

      <button @click="submit" class="btn login-form__btn submit w-100">登 录</button>
    </div>
  </div>
</template>   


<script>
export default {
  data() {
    return {
      username: "",
      password: "",
    };
  },
  created: function () {},
  methods: {
    submit(item) {  

      var that = this;    

      if (this.basic.isEmpty(this.username) || this.basic.isEmpty(this.password)) {
        this.$message({ message: "请输入用户名或密码！", type: "warning" });
        return;
      } 
      
      // 网络请求模块
      this.$http
        .post("userlogin", {
          UserName: this.username,
          Password: this.password,
        })
        .then((response) => {
          
          if (response.body.code != 1) {
            this.$message({ message: response.body.msg, type: "error" });
            return;
          }

          localStorage.setItem("token", response.body.data);
          localStorage.setItem("username",this.username)
          that.$store.commit("set_token", response.body.data); 


          this.$message({
            message: "登录成功",
            type: "success",
            duration: 1000,
            onClose: function () {
              that.$router.push({ path: "/" });
            }
          }); 


        });
    },
  },
};
</script>


