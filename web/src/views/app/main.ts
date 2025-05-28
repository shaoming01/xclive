import App from './App.vue'
import router from '@/router/router'

import '@/styles/index.scss' // 全局样式
import './permission'

const app = createApp(App);
app.use(createPinia());
app.use(router);
app.config.errorHandler = (err, instance, info) => {
    console.error('全局错误捕获:', err, instance, info)
}
app.mount('#app');
