import Vue from 'vue'
import App from './App.vue'
import store from './store'
import {ipcRenderer} from 'electron'
import {BootstrapVue, IconsPlugin} from 'bootstrap-vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import '@/assets/lettuceCss.css'

Vue.prototype.$ipc = ipcRenderer;
Vue.config.productionTip = false;
Vue.use(BootstrapVue);
Vue.use(IconsPlugin);

new Vue({
    store,
    render: h => h(App)
}).$mount('#app')
