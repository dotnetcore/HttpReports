<template>

  <div>

    <h3>{{ this.$store.state.count  }}</h3>

    <h3>{{ this.$store.state.username  }}</h3>

    <el-button @click="update" type="primary">主要按钮</el-button>

  </div>

</template>

<script>
  export default { 
    methods: {
      update() {

        if (this.$store.state.username == "Spring Lee") { 
          this.$store.commit('updateUserName',"Summer Lee");
        }
        else {

          this.$store.commit('updateUserName', "Spring Lee");

        }
      }
    }
  }
</script>

