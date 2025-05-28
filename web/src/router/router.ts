import {createRouter, createWebHashHistory, RouteRecordRaw} from "vue-router"
import Layout from '@/layout/index.vue'

const constantRoutes: RouteRecordRaw[] = [
    {
        path: '/',
        component: Layout,
        redirect: '/module/101',
        children: [], components: undefined, end: undefined, sensitive: undefined, strict: undefined,
    },
    {
        path: '/404',
        component: () => import('@/views/404.vue'),
        meta: {hidden: true, title: '404'},
    },
    {
        path: '/:pathMatch(.*)*',
        redirect: '/404',
        meta: {hidden: true},
    }
]

export const routes: RouteRecordRaw[] = [
    {
        path: '/login',
        name: 'Login',
        component: () => import('@/views/account/login.vue'),
        meta: {hidden: true, title: '登录'}
    }, {
        path: '/liveIndex',
        name: 'liveIndex',
        component: () => import('@/views/live/LiveIndex.vue'),
        meta: {hidden: true, title: '直播助手'}
    }, {
        path: '/module/:moduleId',
        name: '/moduleRender',
        component: Layout,
        children: [
            {
                path: '',
                name: 'ModuleRenderPage',
                component: () => import('@/views/module/ModuleRenderPage.vue'),
                meta: {title: '模块渲染页面'},
            },
        ]
    }, {
        path: '/moduleDesigner',
        name: 'moduleDesigner',
        component: Layout,
        children: [
            {
                path: '',
                component: () => import('@/components/pages/ModuleDesigner.vue'),
                meta: {title: '模块设计页面'},
            },
        ]
    },
]

const router = createRouter({
    history: createWebHashHistory(import.meta.env.BASE_URL),
    routes: [...routes, ...constantRoutes]
})

export default router