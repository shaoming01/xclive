<script setup lang="ts">
import {DefaultOptionType, SelectValue} from 'ant-design-vue/es/select'
import type {Layout} from '@/types/layout'
import {userStore} from '@/stores/user'
import {useGlobalStore} from "@/stores/global";
import pinyin from "pinyin";
import TabsBar from "@/layout/TabsBar.vue";

const loading = inject<Layout.Loading>('loading')
const useSharedIsMobile = createSharedComposable(isMobile)
const _isMobile = useSharedIsMobile(setSidebarCollapsed)
const user = userStore()
const searchValue = ref('')
const searchOptions = shallowRef<DefaultOptionType[]>([])
const isFullscreen = ref(false)
const router = useRouter()
const searchCache: Record<string, any> = {}
let timeout: NodeJS.Timeout
const global = useGlobalStore();
onBeforeMount(() => {
  document.onfullscreenchange = () => {
    isFullscreen.value = !isFullscreen.value
  }
})

function setSidebarCollapsed() {
  global.state.sidebarRelated.collapsed = _isMobile.value
  global.state.sidebarRelated.shadowCollapsed = global.state.sidebarRelated.collapsed
}


function logout() {
  if (loading) loading.logout = true
  const token = ext.getCookie('token')

  user.logout(token).then((_: any) => {
    router.replace('/login')
  })
}

function toggleSidebar() {
  if (!global.state.sidebarRelated) return
  if (!global.state.sidebarRelated.collapsed) {
    global.state.sidebarRelated.collapsed = true
    nextTick(() => {
      global.state.sidebarRelated.shadowCollapsed = true
    })
  } else {
    timeout && clearTimeout(timeout)
    global.state.sidebarRelated.shadowCollapsed = false
    timeout = setTimeout(() => {
      global.state.sidebarRelated.collapsed = false
    }, 120)
  }
}

function searchChange(value: SelectValue) {
  if (!value) {
    searchOptions.value = []
    return
  }
  const keyword = (value as string).toLowerCase();
  if (Reflect.has(searchCache, keyword)) {
    searchOptions.value = searchCache[keyword]
    return
  }
  const menus = global.state.menus;
  const filteredRoutes = menus.filter(menu => {
    if (menu.hidden) return false;
    if (menu.title?.includes(keyword)) return true;

    const keywords = pinyin(menu.title, {style: 0});
    const simpleKeyword = keywords.join('');
    if (simpleKeyword.includes(keyword))
      return true;
    const firstLetter = pinyin(menu.title, {style: 4}).join('');
    return firstLetter.includes(keyword)
  })
  searchOptions.value = filteredRoutes.map(menu => {
    return {label: menu.title, value: menu.url, menu: menu}
  })
  searchCache[value as string] = searchOptions.value
}

function searchSelect(value: any) {
  searchValue.value = ''
  router.push(value)
}
</script>

<template>
  <header>
    <section>
      <MenuFoldOutlined v-if="_isMobile"
                        :class="['icon-sidebar-trigger', global.state.sidebarRelated?.collapsed && 'collapsed']"
                        @click="toggleSidebar"/>
      <TabsBar style="width: 100%"></TabsBar>
    </section>

    <ASpace size="middle" style="margin-right: 1rem; font-size: 1rem;">
      <AAutoComplete v-model:value="searchValue" style="width: 15rem" :dropdownMatchSelectWidth="250"
                     :filterOption="false" :options="searchOptions" @change="searchChange" @select="searchSelect">
        <AInputSearch placeholder="菜单名称/拼音/首字母" allowClear/>
        <template #option="item">
          <span><Icon :name="item.menu.icon"></Icon> {{ item.label }}</span>

        </template>
      </AAutoComplete>
      <ATooltip v-if="!isFullscreen" title="全屏">
        <FullscreenOutlined style="cursor: pointer;" @click="() => launchFullscreen()"/>
      </ATooltip>
      <ATooltip v-else title="退出全屏">
        <FullscreenExitOutlined style="cursor: pointer;" @click="() => exitFullscreen()"/>
      </ATooltip>
      <ADivider type="vertical" style="background-color: #e1e1e1; height: 1rem; margin: 0"/>
    </ASpace>
    <section>
      <ADropdown :trigger="['click']">
        <div style="display: inline-flex; align-items: center; cursor:pointer;">
          <UserOutlined style="margin-right: .5rem;"/>
          <span>{{ user.name }}</span>
          <DownOutlined style="margin-left: .5rem;margin-right: .5rem"/>
        </div>
        <template #overlay>
          <AMenu>
            <AMenuItem>
              <span @click="logout">
                <LogoutOutlined style="margin-right: .5rem;"/>登出
              </span>
            </AMenuItem>
          </AMenu>
        </template>
      </ADropdown>
    </section>
  </header>
</template>

<style scoped lang="scss">
header {
  height: 2.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: nowrap;
  margin-bottom: 3px;

  section {
    &:first-of-type {
      display: inline-flex;
      flex-wrap: nowrap;
      align-items: center;
      flex-shrink: 0;
      overflow: hidden;
      flex: 1
    }

    &:last-of-type {
      display: inline-flex;
      flex-wrap: nowrap;
      flex-shrink: 0;
      align-items: center;
    }
  }
}

.icon-sidebar-trigger {
  cursor: pointer;
  margin-right: 1.2rem;
  font-size: 1.2rem;

  &.collapsed {
    transform: rotate(180deg);
  }
}
</style>