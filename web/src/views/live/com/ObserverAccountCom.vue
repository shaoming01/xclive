<script setup lang="ts">
import {
  CefHelp, IDyAccountAuthVm,
  LiveAccountSaveDto
} from "@/views/live/help/LiveInterface";
import {pageApi} from "@/api/pageApi";
import {R} from "@/utils/R";
import {DyApi} from "@/views/live/help/DyApi";
import {showUpdateEmits} from "@/types/schema";
import {ValueUpdateEmits} from "@/types/interfaces";

const props = defineProps<{
  value?: string,
}>();
const emit = defineEmits<ValueUpdateEmits>();

const observerIdRef = ref(props.value);
const obsKey = ref(1);
watch(() => observerIdRef.value, (newValue: string | undefined) => {
  emit("update:value", newValue);
})

async function addNewObserver() {
  const closeR = await CefHelp.closeBrowser();
  if (!closeR.success) return msg.error(closeR.message);
  const r = await CefHelp.startBrowser('https://www.douyin.com/user/self/', 'douyin');
  if (!r.success)
    return msg.error('启动浏览器失败：' + r.message);
  msg.success('启动浏览器成功，请在浏览器中登录账号');
}

async function refreshObserverList() {
  obsKey.value++;
}

async function checkObserverAccount() {
  const r = await CefHelp.getBrowserCookie();
  if (!r.success || !(r.data?.length)) {
    await refreshObserverList();
    return msg.error('未从浏览器检测到新账号' + r.message);//浏览器没启动处理方式
  }
  const rAccount = await DyApi.getAccountInfo(r.data);
  if (!rAccount.success) {
    await refreshObserverList();
    return msg.error('获取登录账号页面出错：' + rAccount.message);//浏览器没启动处理方式
  }
  const account = rAccount.data!;
  const cookieStr = DyApi.cookieToString(r.data);
  const auth: IDyAccountAuthVm = {
    company_name: account.company_name,
    nick_name: account.nick_name,
    douyin_unique_id: account.douyin_unique_id,
    douyin_uid: account.douyin_uid,
    cookie: cookieStr,
  };
  const dto: LiveAccountSaveDto = {
    platform: 1,
    roleType: 1,
    platformAccountName: auth.nick_name,
    platformAccountId: auth.douyin_unique_id,
    accountId: observerIdRef.value,
    authJson: JSON.stringify(auth),
  }
  const saveRe = await apiHelper.request('/api/liveAccount/SaveLiveAccount', undefined, dto);
  if (!saveRe.success) return msg.error(saveRe.message);
  await refreshObserverList();
  obsKey.value++;
  msg.success(auth.nick_name + '添加成功，自动关闭浏览器');
  const closeRe = await CefHelp.closeBrowser();
  if (!closeRe.success) return msg.error(closeRe.message);
}

async function deleteObserverList() {
  if (!observerIdRef.value) return msg.error('请选择要删除的账号');
  if (!await msg.confirm('确认要删除选择的账号吗？删除以后重新添加需要重新登录直播账号')) return;
  const r = await deleteAccount(observerIdRef.value);
  if (r.success) {
    observerIdRef.value = '';
    await refreshObserverList();
  }
}

async function deleteAccount(id: string): Promise<R> {
  const re = await pageApi.deleteIds('/api/LiveAccount/LiveAccountDelete', [id]);
  if (!re.success) {
    msg.error(re.message);
    return R.error(re.message);
  }
  return R.ok()
}

</script>

<template>
  <ARow align="middle" :gutter="[8,0]">
    <ACol style="text-align:right;">观察员
      <ATooltip>
        <template #title>请添加抖音账号，用于回复，评论、保镖，直撞间管理（需要具备相应的后台操作权限）</template>
        <QuestionCircleOutlined/>
      </ATooltip>
    </ACol>
    <ACol flex="auto">
      <DataSelectInput v-model:value="observerIdRef" :key="obsKey"
                       :dataSourceApi="{apiUrl:'api/sys/ListValueDisplay',postParams:{type:6}}"></DataSelectInput>
    </ACol>
    <ACol>
      <ASpace>
        <AButton type="default" :icon="getIcon('icon-tianjia')" @click="addNewObserver">添加</AButton>
        <AButton type="default" :icon="getIcon('icon-refresh')" @click="checkObserverAccount">刷新</AButton>
        <AButton type="default" :icon="getIcon('icon-delete1')" @click="deleteObserverList">删除</AButton>
      </ASpace>
    </ACol>
  </ARow>


</template>

