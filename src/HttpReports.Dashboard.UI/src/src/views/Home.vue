<template>
  <div class="home">
    <Layout>
      <Header></Header>
      <Layout>
        <Sider hide-trigger>
          <Menu active-name="1">
            <MenuItem name="1" to="/">
              <Icon type="md-document" />基础数据
            </MenuItem>
            <MenuItem name="趋势数据" to="/Trend">
              <Icon type="md-globe" />趋势数据
            </MenuItem>
            <MenuItem name="3" to="/Request">
              <Icon type="md-grid" />请求列表
            </MenuItem>
            <MenuItem name="4" to="/Monitor">
              <Icon type="md-leaf" />预警监控
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

export default {
  name: "home",
  computed: mapState({
    localize: state => state.localize,
  })
};
</script>