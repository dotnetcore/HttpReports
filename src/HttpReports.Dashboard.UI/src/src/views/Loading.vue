<template>
  <div class="home">
    <img class="icon" src="../assets/logo.png" />
    <Spin fix>
      <Icon type="ios-loading" size="200" class="spin-icon-load"></Icon>
      <div class="load-text">Loading</div>
    </Spin>
  </div>
</template>

<style>
.icon {
  margin-top: 30px;
}
.load-text {
  font-size: 30px;
}
.spin-icon-load {
  animation: ani-spin 1s linear infinite;
}
@keyframes ani-spin {
  from {
    transform: rotate(0deg);
  }
  50% {
    transform: rotate(180deg);
  }
  to {
    transform: rotate(360deg);
  }
}
</style>