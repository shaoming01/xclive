<script setup lang="ts">
import type {RouteLocationNormalizedLoaded} from 'vue-router'
import {Modal} from 'ant-design-vue'
import {useGlobalStore} from "@/stores/global";
import {IMenuVm} from "@/types/schema";

const router = useRouter()
const route = useRoute()
const tabs = ref<IMenuVm[]>([]);
const activeKey = ref<string>('');
const global = useGlobalStore();

async function addTab() {
  const extTab = tabs.value.find(item => item.url == route.fullPath);
  if (extTab) {
    moveToTab(extTab.id)
    return;
  }
  await ext.waitFor(() => global.state.menus.length > 0, 5000);
  let menu = global.state.menus.find(m => m.url == route.fullPath);
  if (!menu) {
    const title = (route.query?.title as string) ?? route.meta?.title ?? '';
    menu = {
      hidden: true,
      id: calcId(route.fullPath),
      url: route.fullPath,
      title: title,
      icon: route.meta?.icon as string || '',
      desc: '', parentId: '0',
    }
  }
  tabs.value.push(menu);
  await nextTick();
  moveToTab(menu.id)
}

/**
 * 修改当前标签页的标题
 * @param tabName 修改后的标题
 */
function setCurrentTabName(tabName: string) {
  if (!activeKey.value)
    return;
  const tab = tabs.value.find(t => t.id == activeKey.value);
  if (!tab)
    return;
  tab.title = tabName;
}

function calcId(url: string) {
  if (!url) return '';
  return url.replace(/[^\w]/gi, '');
}

function moveToTab(menuId: string) {
  activeKey.value = menuId;
}


function refreshPage() {
  global.updatePageKey(route.fullPath)
  return;

}

/**
 * @param tab 关闭的路由
 * @param noRedirect 关闭结束后是否自动跳转去别的尚存的路由
 * @param forceClose 是否强制关闭，开启后将无视关闭确认弹框
 */
async function closeTab(tab: IMenuVm, noRedirect?: boolean, forceClose?: boolean) {
  if (!forceClose) {
    const confirm = await checkCloseTab(tab)
    if (!confirm) return
  }
  const closePath = tab.url
  const closeIndex = tabs.value.findIndex(item => item.url === closePath)
  tabs.value.splice(closeIndex, 1)
  global.deleteKeepAlivePage(tab.url)
  if (noRedirect) return
  if (tabs.value.length > 0) {
    if (closePath === route.fullPath) {
      const nextTab = tabs.value[tabs.value.length - 1]
      const {url} = nextTab
      return router.push(url)
    }
  } else {
    return router.push('/');
  }
}

async function checkCloseTab(tab: IMenuVm) {
  return new Promise((resolve) => {
    Modal.confirm({
      title: '关闭提示',
      icon: getIcon('icon-a-014_jisuanqi'),
      content: `确定关闭页面「${tab.title || '无标题'}」吗?`,
      okText: '确认',
      cancelText: '取消',
      onOk() {
        resolve(true)
      },
      onCancel() {
        resolve(false)
      }
    })
  })
}

function closeRightSideTabs(target: RouteLocationNormalizedLoaded) {
  if (target.fullPath !== route.fullPath) {
    router.replace('/redirect' + target.fullPath)
  }
  const index = tabs.value.findIndex(item => item.url === target.fullPath)
  for (let i = index + 1; i < tabs.value.length; i++) {
    const tab = tabs.value[i]
    nextTick(() => {
      closeTab(tab, true)
    })
  }
}


function closeOtherTabs(saveTab: RouteLocationNormalizedLoaded) {
  if (saveTab.fullPath !== route.fullPath) {
    router.replace('/redirect' + saveTab.fullPath)
  }
  for (let i = tabs.value.length - 1; i >= 0; i--) {
    const tab = tabs.value[i]
    if (tab.url === saveTab.fullPath) continue
    nextTick(() => {
      closeTab(tab, true)
    })
  }
}

function switchTab() {
  const tab = tabs.value.find(t => t.id == activeKey.value);
  if (tab && tab.url != route.fullPath)
    router.push(tab.url);
}

function handleRightClick(tab: IMenuVm) {
  console.log('右键：', tab.url)
}

function closeCurrent() {
  if (!global.state.currentMenu) return;
  closeTab(global.state.currentMenu)
}

watch(() => route.fullPath, addTab, {immediate: true})
watch(() => activeKey.value, () => {
  switchTab();
})
</script>

<template>
  <ATabs v-model:activeKey="activeKey" :tabBarGutter="12">
    <ATabPane v-for="(tab, index) in tabs" :key="tab.id">
      <template #tab>
        <a-dropdown :trigger="['contextmenu']" class="tab-title">
          <span style="padding-left: 15px;" @contextmenu.prevent="()=>{handleRightClick(tab)}">
          <Icon :name="tab.icon" v-if="tab.icon"></Icon>
          {{ tab.title || '无标题' }}
             <CloseOutlined @click.stop.prevent="closeTab(tab)" class="close-btn"/>
        </span>
          <template #overlay>
            <a-menu style="width: 200px;">
              <a-menu-item key="1" @click="refreshPage">刷新</a-menu-item>
              <a-menu-item key="2" @click="closeCurrent">关闭</a-menu-item>
              <a-menu-item key="3">锁定</a-menu-item>
            </a-menu>
          </template>
        </a-dropdown>
      </template>

    </ATabPane>
    <template #rightExtra>
      <ADropdown>
        <template #overlay>
          <AMenu style="width:150px;">
            <AMenuItem key="1" @click="refreshPage()">
              <Icon type="icon-shuaxin"/>
              刷新
            </AMenuItem>
            <AMenuItem key="2" @click="closeOtherTabs(route)">
              <ColumnWidthOutlined/>
              关闭其他
            </AMenuItem>
            <AMenuItem key="3" @click="closeRightSideTabs(route)">
              <VerticalRightOutlined/>
              关闭右侧
            </AMenuItem>
          </AMenu>
        </template>

        <Icon type="icon-gengduo-shu" style="margin-right: 10px;"/>
      </ADropdown>
    </template>
    <template #leftExtra>
      <span style="margin-right: 15px;"></span>
    </template>
  </ATabs>


</template>

<style scoped lang="scss">
:deep(.ant-tabs-nav) {
  margin-bottom: 0 !important;
}

.close-btn {
  color: red;
  margin-right: 0 !important;
  transition: opacity 0.3s;
  opacity: 0;
}

.tab-title:hover .close-btn {
  opacity: 1;
}

</style>