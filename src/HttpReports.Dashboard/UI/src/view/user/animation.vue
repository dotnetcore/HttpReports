<template>

  <div id="demo">
    <button v-on:click="show = !show">
      Toggle
    </button>
    <transition   leave-active-class="animated slideOutLeft" enter-active-class="animated slideInLeft"  name="fade">
      <p v-if="show">hello</p>
    </transition>
  </div>

</template> 
 

<script>
  export default {
    data() {
      return {
        show: true
      };
    },
    methods: {
       
    }
  }
</script>
