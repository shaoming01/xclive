import {IMenuVm} from "@/types/schema";
import {R} from "@/utils/R";
import {pageApi} from "@/api/pageApi";
import type {Layout} from "@/types/layout";

/**
 * 系统全局对象或功能
 */

export const useGlobalStore = defineStore('global', () => {
    const route = useRoute()

    // 定义你的全局状态
    const state = reactive<{
        menus: IMenuVm[]
        currentMenu?: IMenuVm,
        currentPath?: string[],
        keepAlivePages: Set<string>
        loading: boolean,
        comCache: Map<string, any>,
        keyCache: Map<string, string>,
        sidebarRelated: Layout.SidebarRelated,
    }>(
        {
            menus: [],
            keepAlivePages: new Set<string>(),
            loading: false,
            comCache: new Map<string, any>(),
            keyCache: new Map<string, string>(),
            sidebarRelated: {
                collapsed: true,
                shadowCollapsed: true,
                width: '14rem',
                collapsedWidth: '3rem',
                collapsedText: 'SB'
            },
        });
    watch(() => route.fullPath, updateCurrentMenu)
    watch(() => state.menus, updateCurrentMenu)

    function updateCurrentMenu() {
        state.currentMenu = state.menus.find(m => m.url == route.fullPath);
        if (state.currentMenu) {
            state.currentPath = calcMenuPath(state.currentMenu.parentId)
        }
    }

    function calcMenuPath(parentId: string): string[] {
        if (!parentId || parentId == '0') return [];
        const menu = state.menus.find(m => m.id == parentId);
        if (!menu) {
            return []
        }
        const pathArr = [];
        pathArr.push(...calcMenuPath(menu.parentId));
        pathArr.push(menu.id);
        return pathArr;
    }

    async function ini(): Promise<R> {
        const r1 = await iniMenus();
        if (!r1.success)
            return r1;
        updateCurrentMenu();
        return R.ok();
    }

    async function iniMenus(): Promise<R> {
        const re = await pageApi.queryList('/api/Menu/MenuQueryList', {
            page: 1, pageSize: 1000, queryObject: {}
        });
        if (!re.success)
            return re;
        state.menus = re.data ?? [];
        appendCustomMenu();
        return R.ok();
    }

    function appendCustomMenu() {
        state.menus.push({
            hidden: true,
            title: '模块设计',
            id: "0",
            parentId: "0",
            url: '/moduleDesigner',
            desc: '',
            icon: 'icon-sheji',
        })

    }

    /**
     * 修改组件名称，否则无法匹配到缓存里的路径
     * @param component
     * @param key
     */
    function getRouteComponent(component: any, key: string) {
        key = key.replace(/[^\w]/gi, '');
        if (!component) {
            console.error('component is null:' + key)
            return component;
        }
        if (state.comCache.has(key))
            return state.comCache.get(key);
        const com = {
            ...component,
            type: {...component['type'], name: key}
        };
        state.comCache.set(key, com);
        return com;
    }

    function updatePageKey(url: string) {
        state.keyCache.set(url, url + ext.random(1, 1000000));
    }

    function deleteKeepAlivePage(url: string) {
        const key = url.replace(/[^\w]/gi, '');
        if (state.keepAlivePages?.has(key)) {
            state.keepAlivePages.delete(key)
        }
    }


    return {
        initMenus: ini,
        state,
        getRouteComponent,
        deleteKeepAlivePage,
        updatePageKey,
    };
});

