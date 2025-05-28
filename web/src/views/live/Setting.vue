<script setup lang="ts">
import ObserverAccountCom from "@/views/live/com/ObserverAccountCom.vue";
import HuangCheAccountCom from "@/views/live/com/HuangCheAccountCom.vue";
import {ref} from "vue";
import {ILiveSettingVm} from "@/views/live/help/LiveInterface";


const observerAccountId = ref('');
const operateAccountId = ref('');

const interactMsgCount = ref(3);
const replyMsgCount = ref(3);

async function ini() {
  const re = await apiHelper.request<ILiveSettingVm>('/api/sys/GetSetting', {typeName: 'LiveSettingVm'});
  if (!re.success) return msg.error(re.message);
  interactMsgCount.value = re.data?.interactMsgCount ?? 3;
  replyMsgCount.value = re.data?.replyMsgCount ?? 3;
}

async function saveSetting() {
  const re1 = await apiHelper.request<ILiveSettingVm>('/api/sys/GetSetting', {typeName: 'LiveSettingVm'});
  if (!re1.success) return msg.error(re1.message);
  const settingData = re1.data!;
  let needSave = false;
  if (settingData.interactMsgCount != interactMsgCount.value) {
    settingData.interactMsgCount = interactMsgCount.value;
    needSave = true;
  }
  if (settingData.replyMsgCount != replyMsgCount.value) {
    settingData.replyMsgCount = replyMsgCount.value;
    needSave = true;
  }
  if (!needSave) {
    return
  }

  const re2 = await apiHelper.request('/api/sys/SaveSetting', {
    typeName: 'LiveSettingVm',
  }, settingData);
  if (!re2.success) return msg.error(re2.message);
  msg.success('保存成功')

}

onMounted(() => {
  ini()
})
</script>

<template>
  <FlexLayout style="background-color: white; padding: 10px;">

    <FlexLayoutContent>
      <ARow :gutter="[10,10]" style="width: 100%; ">
        <ACol span="24">
          <ADivider orientation="center" style="color: #999999">主播账户设置</ADivider>
        </ACol>
        <ACol span="24">
          <ObserverAccountCom v-model:value="observerAccountId"></ObserverAccountCom>
        </ACol>
        <ACol span="24">
          <HuangCheAccountCom v-model:value="operateAccountId"></HuangCheAccountCom>
        </ACol>
        <ACol span="24">
          <ADivider orientation="center" style="color: #999999">互动、回复消息设置</ADivider>
        </ACol>
        <ACol span="24">
          <ARow :align="'middle'" :gutter="[10,10]" style="width: 100%;color:rgb(35,35,35) ">
            <ACol>每次回复互动消息条数：</ACol>
            <ACol>
              <AInputNumber min="1" v-model:value="interactMsgCount" @blur="saveSetting"></AInputNumber>
            </ACol>
          </ARow>
        </ACol>
        <ACol span="24">
          <ARow :align="'middle'" :gutter="[10,10]" style="width: 100%; margin-top: 10px;color:rgb(35,35,35)">
            <ACol>每次回复直播间提问条数：</ACol>
            <ACol>
              <AInputNumber min="1" v-model:value="replyMsgCount" @blur="saveSetting"></AInputNumber>
            </ACol>
          </ARow>
        </ACol>
      </ARow>
    </FlexLayoutContent>
  </FlexLayout>
</template>

<style scoped lang="scss">


</style>
