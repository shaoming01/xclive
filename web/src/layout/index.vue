<script setup lang="ts">
import HeadBar from './HeadBar.vue'
import Logo from '@/assets/logo.png'
import {useGlobalStore} from "@/stores/global";
import SideBar from "@/layout/SideBar.vue";
import {comUtil} from "@/utils/com";
import {appStore} from "@/stores/appStore";


const useSharedIsMobile = createSharedComposable(isMobile)
const _isMobile = useSharedIsMobile(setSidebarCollapsed)
const global = useGlobalStore();
const Shadow = comUtil.getCom('/src/components/base/Shadow.vue');
onBeforeMount(() => {
  setSidebarCollapsed()
})

function setSidebarCollapsed() {
  global.state.sidebarRelated.collapsed = _isMobile.value
  global.state.sidebarRelated.shadowCollapsed = global.state.sidebarRelated.collapsed
}


const route = useRoute()
const pageKey = computed(() => {
  let key = global.state.keyCache.get(route.fullPath) ?? route.fullPath;
  key = key.replace(/[^\w]/gi, '')
  console.log('获取控件KEY:' + key)
  return key;
});
const app = appStore()
app.ini();

function sideCollapsedSwitch() {
  global.state.sidebarRelated.collapsed = !global.state.sidebarRelated.collapsed;
}

</script>

<template>
  <ALayout style="height: 100%">
    <ALayoutSider v-if="!_isMobile" v-model:collapsed="global.state.sidebarRelated.collapsed" collapsible
                  :trigger="null"
                  :width="global.state.sidebarRelated.width"
                  :collapsedWidth="global.state.sidebarRelated.collapsedWidth" breakpoint="md">
      <div
          style="position: relative; z-index: 999; display: flex; flex-direction: column; width: 100%; height: 100%;padding-top: 30px;background-color: #1c1e23">
        <RouterLink to="/">
          <div
              v-if="(global.state.sidebarRelated.shadowCollapsed || global.state.sidebarRelated.collapsed) && global.state.sidebarRelated.collapsedText"
              class="flex-center logo-collapsed">
            <ATooltip :title="app.appInfo?.productName" placement="right">
              <Icon name="icon-shayu" style="font-size: 32px;"></Icon>
            </ATooltip>

          </div>
          <div v-else>
            <img :src="Logo" alt="Logo" class="logo">
            <div
                style="color:#3759be;font-weight: bold;font-size: 16px;text-align: center;width: 100%;margin-bottom: 20px;">
              {{ app.appInfo?.productName }}
            </div>
          </div>

        </RouterLink>
        <SideBar></SideBar>
        <div class="sideCollapsedSwitch" @click="sideCollapsedSwitch">
          <Icon :name="global.state.sidebarRelated.collapsed?'icon-cebianlan-copy':'icon-to-left'"></Icon>
        </div>
      </div>
    </ALayoutSider>
    <ALayout>
      <ALayoutHeader style="background-color: white;padding:0;margin:0;height:auto;line-height:normal;">
        <HeadBar></HeadBar>

      </ALayoutHeader>
      <ALayoutContent id="content-window" style="height:100%;overflow: hidden;">
        <RouterView v-slot="{ Component, route }">
          <KeepAlive :include="Array.from(global.state.keepAlivePages)">
            <component :is="global.getRouteComponent(Component,route.fullPath)" :key="pageKey"/>
          </KeepAlive>
        </RouterView>
      </ALayoutContent>
    </ALayout>
  </ALayout>
  <Teleport to="body">
    <Transition name="slide-right" mode="out-in" appear>
      <Shadow v-if="_isMobile && !global.state.sidebarRelated.collapsed"
              @shadowClick="global.state.sidebarRelated.collapsed = true">
        <div class="block sidebar-mobile">
          <RouterLink to="/">
            <img :src="Logo" alt="Logo" class="logo">
          </RouterLink>
          <SideBar></SideBar>
        </div>
      </Shadow>
    </Transition>
  </Teleport>
</template>

<style scoped>
.sidebar-mobile {
  width: 15rem;
  height: 96vh;
  position: absolute;
  top: 2vh;
  left: 2vw;
  padding: 0;
  display: flex;
  flex-direction: column;
  background-color: #1c1e23;
  border-radius: 10px;

}

.content-view {
  width: 100%;
  height: 100%;
}

.sidebar-shadow {
  position: absolute;
  left: 0;
  top: 0;
  z-index: 1;
  width: 13rem;
  height: 100%;
  transition: transform ease 0.2s;
}

.logo {
  width: 100%;
  height: 3rem;
  object-fit: contain;
  image-rendering: optimizeQuality;
  animation: fadeIn 1s ease;
}

.logo-collapsed {
  font-size: .8rem;
  width: 100%;
  height: 3rem;
  text-align: center;
  overflow: hidden;
  animation: fadeIn 1s ease;
}

.sideCollapsedSwitch {
  line-height: 36px;
  width: 100%;
  background-color: rgb(46, 48, 52);
  text-align: center;
  cursor: pointer;

}

.sideCollapsedSwitch:hover {
  background-color: rgb(73, 74, 74);
}
</style>
