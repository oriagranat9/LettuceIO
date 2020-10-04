import Vue from 'vue'
import App from './App.vue'
import store from './store'
import {ipcRenderer} from 'electron'
import UniqueId from 'vue-unique-id';
import {BootstrapVue, IconsPlugin} from 'bootstrap-vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import 'bootstrap-vue/dist/bootstrap-vue-icons.min.css'
import '@/assets/lettuceCss.css'
import axios from 'axios'
import VueAxios from 'vue-axios'
import {Color, Titlebar} from 'custom-electron-titlebar'
import Toast from "vue-toastification";

import "vue-toastification/dist/index.css";

new Titlebar({
    backgroundColor: Color.fromHex('#303030'),
    shadow: true,
    menu: false,
    menuPosition: "left"
}).updateTitle("Lettuce.IO");


Vue.prototype.$ipc = ipcRenderer;
Vue.config.productionTip = false;
Vue.use(BootstrapVue);
Vue.use(IconsPlugin);
Vue.use(UniqueId);
Vue.use(VueAxios, axios);
Vue.use(Toast);

new Vue({
    store,
    render: h => h(App)
}).$mount('#app')
