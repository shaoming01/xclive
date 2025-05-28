<script setup lang="ts">
import {
  CefHelp, IByAccountAuthVm, IDyAccountAuthVm, LiveAccountSaveDto
} from "@/views/live/help/LiveInterface";
import {pageApi} from "@/api/pageApi";
import {R} from "@/utils/R";
import {ValueUpdateEmits} from "@/types/interfaces";
import core from "markdown-it-highlightjs/types/core";
import {DyApi} from "@/views/live/help/DyApi";
import {ByApi} from "@/views/live/help/ByApi";

const props = defineProps<{
  value?: string,
}>();
const emit = defineEmits<ValueUpdateEmits>();

const operateIdRef = ref(props.value);
const optKey = ref(1);
watch(() => operateIdRef.value, (newValue: string | undefined) => {
  emit("update:value", newValue);
})

async function addNewLiveOperate() {
  const closeR = await CefHelp.closeBrowser();
  if (!closeR.success) return msg.error(closeR.message);
  const r = await CefHelp.startBrowser('https://buyin.jinritemai.com/mpa/account/login', 'buyin');
  if (!r.success)
    return msg.error('启动浏览器失败：' + r.message);
  msg.success('启动浏览器成功，请在浏览器中登录账号');
}

async function checkLiveOperate() {
  const r = await CefHelp.getBrowserCookie();
  if (!r.success || !(r.data?.length)) {
    await refreshLiveOperateList();
    return msg.error('未从浏览器检测到新账号' + r.message);//浏览器没启动处理方式
  }
  const rAccount = await ByApi.getAccountInfo(r.data);
  if (!rAccount.success) {
    await refreshLiveOperateList();
    return msg.error('获取登录账号页面出错：' + rAccount.message);//浏览器没启动处理方式
  }
  const account = rAccount.data!.data!;
  const cookieStr = DyApi.cookieToString(r.data);
  const auth: IByAccountAuthVm = {
    buyin_account_id: account.buyin_account_id,
    user_name: account.user_name,
    user_id: account.user_id,
    origin_uid: account.origin_uid,
    shop_id: account.shop_id,
    cookie: cookieStr,
  };
  const dto: LiveAccountSaveDto = {
    platform: 1,
    roleType: 2,
    platformAccountId: auth.buyin_account_id,
    platformAccountName: auth.user_name,
    accountId: operateIdRef.value,
    authJson: JSON.stringify(auth),
  }
  const saveRe = await apiHelper.request('/api/liveAccount/SaveLiveAccount', undefined, dto);
  if (!saveRe.success) return msg.error(saveRe.message);
  await refreshLiveOperateList();
  optKey.value++;
  msg.success(auth.user_name + '添加成功，自动关闭浏览器');
  const closeRe = await CefHelp.closeBrowser();
  if (!closeRe.success) return msg.error(closeRe.message);

}

async function refreshLiveOperateList() {
  optKey.value++;
}

async function deleteLiveOperate() {
  if (!operateIdRef.value) return msg.error('请选择要删除的账号');
  if (!await msg.confirm('确认要删除选择的账号吗？删除以后重新添加需要重新登录直播账号')) return;
  const r = await deleteAccount(operateIdRef.value);
  if (r.success) {
    operateIdRef.value = '';
    await refreshLiveOperateList();
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
  <ARow align="middle" :gutter="[8,0]" style="margin-top: 10px;">
    <ACol style="text-align:right;">操作员
      <ATooltip>
        <template #title>请添加抖音账号，用于操作小黄车上下架，需要具备相应的后台操作权限</template>
        <QuestionCircleOutlined/>
      </ATooltip>
    </ACol>
    <ACol flex="auto">
      <DataSelectInput v-model:value="operateIdRef" :key="optKey"
                       :dataSourceApi="{apiUrl:'api/sys/ListValueDisplay',postParams:{type:7}}"></DataSelectInput>
    </ACol>
    <ACol>
      <ASpace>
        <AButton type="default" :icon="getIcon('icon-tianjia')" @click="addNewLiveOperate">添加</AButton>
        <AButton type="default" :icon="getIcon('icon-refresh')" @click="checkLiveOperate">刷新</AButton>
        <AButton type="default" :icon="getIcon('icon-delete1')" @click="deleteLiveOperate">删除</AButton>
      </ASpace>
    </ACol>
  </ARow>


</template>

