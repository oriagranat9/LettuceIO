<template>
  <div id="app" class="fullPage">
    <main-view class="fullPage"/>
  </div>
</template>

<script>
import MainView from "./views/MainView";

export default {
  name: 'App',
  components: {
    MainView
  },
  methods: {}
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  background: var(--bg-primary);
  position: absolute;
}

.Vue-Toastification__toast {
  max-width: 300px !important;

}

.Vue-Toastification__toast-body {
  font-size: small !important;
}

.fullPage {
  height: 100% !important;
  width: 100% !important;
  overflow: hidden;
}

:root {
  --bg-primary: #282828;
  --bg-secondary: #303030;
  --bg-tertiary: #1c1c1c;
  --hairline-reguar: #434343;
  --brand-primary: #04b013;
  --brand-secondary: #07b01d;
  --border-color: #01b026;
  --border-secondary-color: #0bb008;
  --shadow-color: rgba(0, 227, 42, 0.25);
  --accent-warning: #FFB400;
}
</style>
