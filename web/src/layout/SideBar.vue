<script lang="ts" setup>
import {IMenuVm} from "@/types/schema";
import {useGlobalStore} from "@/stores/global";
import {getIcon} from "@/composables/useFontIcon";
import {message} from "ant-design-vue";

const router = useRouter()
const route = useRoute()
const selectedKeys2 = ref<string[]>([]) // 菜单默认选中项
const global = useGlobalStore();

// 默认展开选中项所在菜单
const openKeys = ref<string[]>();
const items = ref<IAntdMenu[]>();

watch(() => global.state.currentMenu, () => {
  const currentMenu = global.state.currentMenu;
  selectedKeys2.value = [currentMenu?.id + '']
  const key = route.fullPath.replace(/[^\w]/gi, '');
  if (!global.state.keepAlivePages.has(key)) {
    global.state.keepAlivePages.add(key);
    console.log('keepAlivePages:add:' + key);
  }
  // 只在展开菜单的时候更新active菜单项
  if (!global.state.sidebarRelated?.collapsed) {
    updateOpenKeys()
  }
}, {immediate: true})

// 展开菜单的时候需要更新active菜单项
watch(() => global.state.sidebarRelated?.collapsed, (collapsed) => {
  if (collapsed) return
  updateOpenKeys()
})

function updateOpenKeys() {
  openKeys.value = global.state.currentPath;
}

watch(() => global.state.menus, () => {
  items.value = toAntdMenus(global.state.menus);
})

interface IAntdMenu {
  key: string,
  label?: string | undefined,
  title?: string | undefined,
  icon?: any,
  children?: IAntdMenu[],
}

function toAntdMenus(menus: IMenuVm[], parentId: string = '0'): IAntdMenu[] | undefined {
  let reList: IAntdMenu[] = [];
  const subMenus = menus.filter(m => m.parentId == parentId && !m.hidden);
  if (subMenus.length == 0)
    return undefined;
  for (const menu of subMenus) {
    if (menu.hidden)
      continue;
    const aMenu = {
      key: menu.id,
      label: menu.title,
      title: menu.title,
      icon: () => getIcon(menu.icon),
      children: toAntdMenus(menus, menu.id),
    };
    reList.push(aMenu);
  }
  return reList;

}

function clickedMenu(event: any) {
  const menu = global.state.menus.find(m => m.id == event.key);
  const url = menu?.url ?? '';
  router.push(url).catch(err => {
    console.error(err);
  });
}


async function ini() {
  const re = await global.initMenus();
  if (!re.success) message.error('初始化系统出错：' + re.message);
}


ini();

</script>

<template>
  <div style="height: 100%;">
    <AMenu
        v-model:openKeys="openKeys"
        v-model:selectedKeys="selectedKeys2"
        class="dark-menu"
        mode="inline"
        theme="dark"
        :items="items"
        @click="clickedMenu"
    />
  </div>
</template>
<style scoped lang="scss">
.dark-menu {
  width: 100%;
  background-color: transparent;
}

:deep(.dark-menu .ant-menu-item-active), :deep(.dark-menu .ant-menu-submenu-active) {
  background-color: #2E3034 !important;
  color: rgb(250, 250, 250) !important;
}

:deep(.dark-menu .ant-menu-item-active .ant-menu-item-icon), :deep(.dark-menu .ant-menu-submenu-active .ant-menu-item-icon) {
  font-size: 16px;
  transition: all 0.6s;
}

:deep(.dark-menu .ant-menu-item-selected) {
  background-color: #4e69fd !important;
  transition: all 0.6s ease;
}

:deep(.dark-menu .ant-menu-sub) {
  background-color: #1c1e23 !important;
  color: rgba(243, 243, 243, 0.824) !important;
}

:deep(.dark-menu .ant-menu-item-icon) {
  display: inline;
  width: 16px;

}


</style>
