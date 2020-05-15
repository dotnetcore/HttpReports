<template>
  <div class="home">
    <Layout>
      <Header>
        <div class="top-menu-box">
          <Menu mode="horizontal">
            <Submenu name="1">
              <template slot="title">
                <Icon type="md-construct" />
                {{localize.language}}
              </template>
              <MenuItem
                :name="language"
                :key="language"
                v-for="(language) in languages"
                @click.native="setLanguage(language)"
              >{{language}}</MenuItem>
            </Submenu>
          </Menu>
        </div>
      </Header>
      <Layout>
        <Sider hide-trigger>
          <Menu active-name="1">
            <MenuItem :name="localize.menu_BasicData" to="/">
              <Icon type="md-document" />
              {{localize.menu_BasicData}}
            </MenuItem>
            <MenuItem :name="localize.menu_TrendData" to="/Trend">
              <Icon type="md-globe" />
              {{localize.menu_TrendData}}
            </MenuItem>
            <MenuItem :name="localize.menu_RequestList" to="/Request">
              <Icon type="md-grid" />
              {{localize.menu_RequestList}}
            </MenuItem>
            <MenuItem :name="localize.menu_Monitor" to="/Monitor">
              <Icon type="md-leaf" />
              {{localize.menu_Monitor}}
            </MenuItem>
          </Menu>
        </Sider>
        <Layout :style="{padding: '0 24px 24px'}">
          <router-view />
        </Layout>
      </Layout>
    </Layout>
  </div>
</template>

<script>
import { mapState } from 'vuex'
import api from "../api";

export default {
  name: "home",
  data: () => {
    return {
      languages: []
    }
  },
  computed: mapState({
    localize: state => state.localize,
  }),
  mounted: async function () {
    let res = await api.getAvailableLanguages();
    this.languages = res.data.data;
  },
  methods: {
    async setLanguage (language) {
      console.log(language);
      let response = await api.getLocalizeLanguage(language);
      this.$store.state.localize = response.data.data;
    }
  }
};

</script>

<style>
.top-menu-box {
  float: right;
}
</style>