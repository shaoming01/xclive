import router from '@/router/router'
import NProgress from 'nprogress'
import 'nprogress/nprogress.css'
import {userStore} from '@/stores/user'
import {appStore} from "@/stores/appStore";
import {CefHelp} from "@/views/live/help/LiveInterface";

NProgress.configure({showSpinner: false})

const whitelist: string[] = ['/login', '/404']
let scrollTimeout: NodeJS.Timeout | null = null
let contentWindowDom: HTMLElement | null = null


router.beforeEach(async (to, from, next) => {
    const app = appStore();
    app.ini();
    NProgress.start()
    // 设置页面标题
    document.title = `${app.appInfo?.productName}-${app.appInfo?.version}`
    // 路径命中白名单，放行通过
    if (whitelist.includes(to.path)) {
        next();
        return;
    }
    // 判断是否有token
    const token = ext.getCookie('token')
    const user = userStore()
    const redirectUrl = encodeURIComponent(to.fullPath);
    const loginUrl = `/login?redirect=${redirectUrl}`;
    if (!token) {
        next(loginUrl)
    } else if (!user.token) {
        try {
            const re = await user.getUserInfo(token)
            if (!re.success) {
                msg.error(re.message)
                ext.removeCookie('token') // 清除cookie
                next(loginUrl)
            } else {
                if (CefHelp.isInFrame() && to.path !== '/liveIndex') {
                    return next('/liveIndex');
                }
                next()
            }
        } catch (_) {
            msg.error('token失效，请重新登录')
            ext.removeCookie('token') // 清除cookie
            next(loginUrl)
        }
    } else {
        if (CefHelp.isInFrame() && to.path !== '/liveIndex') {
            return next('/liveIndex');
        }
        next()
    }
})

router.afterEach((to, from) => {
    NProgress.done()
    scrollTimeout && clearTimeout(scrollTimeout)
    if (from.path === '/') return
    scrollTimeout = setTimeout(() => {
        if (contentWindowDom) {
            contentWindowDom.scrollTo({top: 0, left: 0})
            return
        }
        contentWindowDom = document.querySelector('#content-window')
        if (contentWindowDom) {
            contentWindowDom.scrollTo({top: 0, left: 0})
        }
    }, 350)
})
