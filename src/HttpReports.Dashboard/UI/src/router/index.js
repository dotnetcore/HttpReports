import Vue from 'vue'
import Router from 'vue-router'
import Index from '@/view/Index'
import Buttons from '@/view/user/buttons'
import Form from '@/view/User/form'
import Main from '@/view/main/index'
import Animation from '@/view/user/animation'
import Login from '@/view/user/login'
import Login2 from '@/view/user/login2';
import Store from '@/view/user/store'



Vue.use(Router)


export default new Router({
  routes: [
    {
      path: '/',
      name: 'index',
      component: Index,
      children: [
        {
          path: '/',
          component: Main
        },
        {
          path: '/index/buttons',
          component: Buttons
        },
        {
          path: '/index/form',
          component: Form
        },
        {
          path: '/index/animation',
          component: Animation
        },
        {
          path: '/user/store',
          name: 'store',
          component: Store
        }
      ]
    },
    {
      path: '/user/login',
      name: 'login',
      component: Login
    },
    {
      path: '/user/login2',
      name: 'login2',
      component: Login2
    }
  ]
})

