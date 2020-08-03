import Vue from 'vue'
import Router from 'vue-router'
import Index from '@/view/Index'
import Buttons from '@/view/user/buttons'
import Form from '@/view/User/form'
import Main from '@/view/main/index'
import Animation from '@/view/user/animation'
import Login from '@/view/user/login' 
import Store from '@/view/user/store'



Vue.use(Router)


//push 
const VueRouterPush = Router.prototype.push 
Router.prototype.push = function push (to) {
    return VueRouterPush.call(this, to).catch(err => err)
}

//replace
const VueRouterReplace = Router.prototype.replace
Router.prototype.replace = function replace (to) {
  return VueRouterReplace.call(this, to).catch(err => err)
}


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
    } 
  ]
})

 
