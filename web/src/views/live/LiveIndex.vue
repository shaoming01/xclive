<script setup lang="ts">
import {IconFont} from "@/composables/useFontIcon";
import LivePlay from "@/views/live/LivePlay.vue";
import {CefHelp} from "@/views/live/help/LiveInterface";
import LiveRoomSetting from "@/views/live/LiveRoomSetting.vue";
import {appStore} from "@/stores/appStore";
import LiveShelf from "@/views/live/LiveShelf.vue";
import Logo from "@/assets/xclogo.png";
import Setting from "@/views/live/Setting.vue";
import AiAnchorSetting from "@/views/live/AiAnchorSetting.vue";
import AiScriptSetting from "@/views/live/AiScriptSetting.vue";
import {userStore} from "@/stores/user";
import router from "@/router/router";
import {useTitleBar} from "@/views/live/useTitleBar";
import {ref} from "vue";
import ShelfTaskRun from "@/views/live/com/ShelfTaskRun.vue";
import {IClientPackage} from "@/types/interfaces";
import {message} from "ant-design-vue";


const menus1 = ref<IMenu[]>([
  {label: '直播', icon: 'icon-zhibo1'},
  {label: '直播间', icon: 'icon-zhubo'},
  {label: 'AI主播', icon: 'icon-huashuku'},
  {label: 'AI话术', icon: 'icon-gongju'},
  {label: '上架助手', icon: 'icon-gongju'},
])
const menus2 = ref<IMenu[]>([
  {label: '设置', icon: 'icon-shezhi'},
]);
const current = ref();
const showTitleBar = ref(false);
const mountedMenu = ref<Record<string, { mounted: boolean, key: number }>>({});
watch(() => current.value, () => {
  if (!mountedMenu.value) return;
  if (!mountedMenu.value[current.value])
    mountedMenu.value[current.value] = {mounted: true, key: 1};
  //mountedMenu.value[current.value].key += 1;
})
current.value = '直播';
const livePlayRef = ref<InstanceType<typeof LivePlay>>();

interface IMenu {
  label?: string | undefined,
  icon?: string | undefined,
  pageKey?: number,
}

function clickMenu(menu: IMenu) {
  current.value = menu.label ?? '';
}


const windowSize = ref(2);//2普通，3最大化

function refresh() {
  if (current.value === '直播' && !livePlayRef.value?.canRefresh()) {
    return;
  }
  mountedMenu.value[current.value]?.key && (mountedMenu.value[current.value].key += 1);
}


function iniFrame() {
  if (!CefHelp.isInFrame()) return;
  CefHelp.hideBorder(true);
  showTitleBar.value = true;

  const w = window as any;
  if (!w?.dotnetObject?.hideBorder) return;
  w.windowSizeChanged = function (size: number) {
    windowSize.value = size;
  }
}

const {mouseDown, mouseUp, doubleClick} = useTitleBar();

function logout() {
  const token = ext.getCookie('token')
  user.logout(token).then((_: any) => {
    if (CefHelp.isInFrame())
      CefHelp.hideBorder(false);
    router.replace('/login')
  })
}

const app = appStore();
app.ini();
iniFrame();
const user = userStore()

onMounted(() => {
  ext.asyncLoop(backgroundCheckUpdate, 3600)
})

async function backgroundCheckUpdate() {
  if (!app.appInfo?.tenantId) {
    await ext.sleep(60 * 60 * 1000);
    return;
  }

  await ext.sleep(5000);
  const re = await apiHelper.request<IClientPackage>('/api/sys/GetNewVersion', {tenantId: app.appInfo.tenantId});
  if (!re.success) {
    console.error(re.message);
    return;
  }
  if (!re.data?.version) return;
  const newVersion = re.data.version;
  const thisVersion = app.appInfo.exeVersion;
  if (thisVersion >= newVersion) {
    await ext.sleep(8 * 60 * 60 * 1000)
    return
  }

  //下载
  const downloadRe = await CefHelp.downloadFile(re.data.url, `tmp/${newVersion}.zip`);
  if (!downloadRe.success) {
    return msg.error('下载新版本失败，请尝试手动下载' + downloadRe.message);
  }
  await msg.confirm('新版本下载完成，开始更新')
  await CefHelp.updateNewVersion(`tmp/${newVersion}.zip`);
  await ext.sleep(8 * 60 * 60 * 1000);

}

async function clickClose() {
  if (!await msg.confirm('确认要退出吗？')) return;
  CefHelp.closeWindow()
}
</script>

<template>
  <ARow class="main">
    <ACol class="leftSideBar">
      <FlexLayout>
        <FlexLayoutHeader class="dragToMoveWindow">
          <APopover trigger="click" placement="rightTop">
            <template #content>
              <ARow style="width: 300px;margin-top: 10px;" :gutter="10">
                <ACol flex="1" style="text-align: center"><span style="font-weight: bold">账户信息</span>
                  <ADivider style="margin-top: 5px;margin-bottom: 5px;"/>
                </ACol>
              </ARow>
              <ARow style="width: 300px;margin-top: 10px;" :gutter="10">
                <ACol span="8" style="text-align: right">用户名：</ACol>
                <ACol style="color:green">{{ user.name }}</ACol>
              </ARow>
              <ARow style="width: 300px; margin-top: 10px;" :gutter="10">
                <ACol span="8" style="text-align: right;">剩余时间：</ACol>
                <ACol style="color:green">{{ user.days }}</ACol>
              </ARow>
              <ARow style="width: 300px;margin-bottom: 10px;">
                <ACol flex="1" style="text-align: right">
                  <AButton @click="logout">退出登录</AButton>
                </ACol>
              </ARow>

            </template>

            <div class="menuItem">
              <img :src="Logo" style="width:40px;height:40px;" :alt="app.appInfo?.productName"/>
            </div>
          </APopover>
        </FlexLayoutHeader>
        <FlexLayoutContent style="padding-top:5px;" class="dragToMoveWindow">
          <div class="menuItem" v-for="item in menus1" @click="clickMenu(item)">
            <IconFont :type="item.icon??''" :class="current==item.label?'ic hot':'ic'"></IconFont>
            <span :class="current==item.label?'hot':''">{{ item.label }}</span>
          </div>
        </FlexLayoutContent>
        <FlexLayoutFooter>
          <div class="menuItem" v-for="item in menus2" @click="clickMenu(item)">
            <IconFont :type="item.icon??''" :class="current==item.label?'ic hot':'ic'"></IconFont>
            <span :class="current==item.label?'hot':''">{{ item.label }}</span>
          </div>
        </FlexLayoutFooter>
      </FlexLayout>
    </ACol>
    <ACol flex="1" class="rightSide">
      <FlexLayout>
        <FlexLayoutHeader v-if="showTitleBar">
          <ARow align="middle" style="padding: 5px;" @mousedown="mouseDown"
                @mouseup="mouseUp"
                @dblclick="doubleClick" :gutter="0">
            <ACol flex="1" style="user-select: none; ">
              {{ app.appInfo?.productName }} V{{ app.appInfo?.version }}
            </ACol>

            <ACol>
              <ATooltip title="刷新当前页面">
                <AButton type="text" :icon="getIcon('icon-refresh1')" @click="refresh">
                </AButton>
              </ATooltip>

            </ACol>
            <ACol>
              <AButton type="text" :icon="getIcon('icon-zuixiaohua')" @click="CefHelp.minWindow()">
              </AButton>
            </ACol>
            <ACol>
              <AButton type="text" :icon="getIcon(windowSize==2?'icon-zuidahua':'icon-icon-repeat')"
                       @click="CefHelp.switchWindow()">
              </AButton>
            </ACol>
            <ACol>
              <AButton type="text" :icon="getIcon('icon-guanbi')" @click="clickClose">
              </AButton>
            </ACol>
          </ARow>
          <ADivider style="margin: 0;padding: 0;"></ADivider>
        </FlexLayoutHeader>
        <FlexLayoutContent>
          <LivePlay ref="livePlayRef" :key="mountedMenu['直播']?.key"
                    v-if="mountedMenu['直播']?.mounted"
                    v-show="current=='直播'"></LivePlay>
          <LiveRoomSetting :key="mountedMenu['直播间']?.key"
                           v-if="mountedMenu['直播间']?.mounted"
                           v-show="current=='直播间'"></LiveRoomSetting>
          <AiAnchorSetting :key="mountedMenu['AI主播']?.key"
                           v-if="mountedMenu['AI主播']?.mounted"
                           v-show="current=='AI主播'"></AiAnchorSetting>
          <AiScriptSetting :key="mountedMenu['AI话术']?.key"
                           v-if="mountedMenu['AI话术']?.mounted"
                           v-show="current=='AI话术'"></AiScriptSetting>
          <LiveShelf :key="mountedMenu['上架助手']?.key"
                     v-if="mountedMenu['上架助手']?.mounted"
                     v-show="current=='上架助手'"></LiveShelf>
          <Setting :key="mountedMenu['设置']?.key"
                   v-if="mountedMenu['设置']?.mounted"
                   v-show="current=='设置'"></Setting>
        </FlexLayoutContent>
      </FlexLayout>
    </ACol>
  </ARow>


</template>

<style scoped>
.main {
  width: 100%;
  height: 100%;
}

.leftSideBar {
  width: 60px;
  border: 1px solid #202020;
  border-right: none;
  background-color: #2f2f2f;
  padding: 10px 3px 20px 3px;
}

.rightSide {
  border: 1px solid #d0d0d0;
  border-left: none;
  width: 100%;
  height: 100%;
  overflow: hidden;
}

.menuItem {
  margin-top: 15px;
  flex-direction: column;
  display: flex;
  align-items: center;
  cursor: pointer;
  color: #d8dad5;
}

.menuItem .ic {
  font-size: 28px;
}

.menuItem span {
  font-size: 12px;
}

/* 鼠标悬停时的效果 */
.menuItem:hover .ic {
  transform: scale(1.1);
  transition: all 0.3s ease-out;

}

.menuItem:hover {
  color: #07c160; /* 文字变成绿色 */
}

.menuItem .hot {
  color: #07c160 !important;
}

</style>
