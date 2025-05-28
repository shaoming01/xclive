<script setup lang="ts">
import {ILiveSettingVm} from "@/views/live/help/LiveInterface";

const props = defineProps<{
  current?: string,
}>();
const current = ref(props.current ?? '【关注】');

const dataList = ref([
  {
    tab: '【关注】',
    title: '根据直播间的互动：【关注】语音回复',
    subTitle: '当公屏上有人互动的信息匹配后，随机语音回复下方编辑框中的一行话',
    text: '',
    ref: null,
  }, {
    tab: '【加团】',
    title: '根据直播间的互动：【加团】语音回复',
    subTitle: '当公屏上有人互动的信息匹配后，随机语音回复下方编辑框中的一行话',
    text: '',
    ref: null,
  }, {
    tab: '【点赞】',
    title: '根据直播间的互动：【点赞】语音回复',
    subTitle: '当公屏上有人互动的信息匹配后，随机语音回复下方编辑框中的一行话',
    text: '',
    ref: null,
  }, {
    tab: '【进入】',
    title: '根据直播间的互动：【进入】语音回复',
    subTitle: '当公屏上有人互动的信息匹配后，随机语音回复下方编辑框中的一行话',
    text: '',
    ref: null,
  }, {
    tab: '【送礼物】',
    title: '根据直播间的互动：【送礼物】语音回复',
    subTitle: '当公屏上有人互动的信息匹配后，随机语音回复下方编辑框中的一行话',
    text: '',
    ref: null,
  }, {
    tab: '【插入】',
    title: '根据随机倒计时结束后【插入语音】',
    subTitle: '倒计时结束时播报：报时间、报人数，随机语音插入下方编辑框中的一行话',
    text: '',
    ref: null,
  },
]);

function addTag(tag: string) {
  const item = dataList.value.find(l => l.tab === current.value);
  if (!item) return

  const el = item.ref;
  const textArea = el?.['resizableTextArea']?.['textArea'] as HTMLTextAreaElement | undefined;
  if (!textArea) return

  const start = textArea.selectionStart
  const end = textArea.selectionEnd
  const value = item.text

  item.text = value.slice(0, start) + tag + value.slice(end)

  nextTick(() => {
    textArea.focus()
    const newPos = start + tag.length
    textArea.setSelectionRange(newPos, newPos)
  })


}

async function iniData() {
  const re = await apiHelper.request<ILiveSettingVm>('/api/sys/GetSetting', {typeName: 'LiveSettingVm'});
  if (!re.success) return msg.error(re.message);
  settingData.value = re.data;
  dataList.value.forEach(l => {
    if (l.tab === '【关注】')
      l.text = re.data?.socialReply ?? '';
    if (l.tab === '【加团】')
      l.text = re.data?.fansClubReply ?? '';
    if (l.tab === '【点赞】')
      l.text = re.data?.likeReply ?? '';
    if (l.tab === '【进入】')
      l.text = re.data?.memberReply ?? '';
    if (l.tab === '【送礼物】')
      l.text = re.data?.giftReply ?? '';
    if (l.tab === '【插入】')
      l.text = re.data?.insertVoice ?? '';
  })


}

const settingData = ref<ILiveSettingVm | undefined>();

async function saveSetting() {
  const data: ILiveSettingVm = settingData.value ?? {};
  dataList.value.forEach(l => {
    if (l.tab === '【关注】')
      data.socialReply = l.text;
    if (l.tab === '【加团】')
      data.fansClubReply = l.text;
    if (l.tab === '【点赞】')
      data.likeReply = l.text;
    if (l.tab === '【进入】')
      data.memberReply = l.text;
    if (l.tab === '【送礼物】')
      data.giftReply = l.text;
    if (l.tab === '【插入】')
      data.insertVoice = l.text;
  })
  const re = await apiHelper.request('/api/sys/SaveSetting', {typeName: 'LiveSettingVm',}, data);
  if (!re.success) return msg.error(re.message);
  show.value = false;
  msg.success('保存成功')

}

onMounted(() => {
  iniData();

});
const title = '互动回复设置';
const size = {width: '700px', height: '400px'};
const show = ref(true);

</script>

<template>
  <AModal :destroyOnClose="true" v-model:open="show"
          @ok="saveSetting"
          :width="size.width"
          :title="title"
          :centered="true"
          :bodyStyle="{height:size.height}"
          :footer="null"
  >

    <FlexLayout style="padding: 10px">
      <FlexLayoutContent>
        <ATabs style="height: 100%" v-model:active-key="current">
          <ATabPane :key="item.tab" :tab="item.tab" v-for="item in dataList">
            <FlexLayout style="padding: 5px">
              <FlexLayoutHeader>
                <ARow>
                  <ACol>
                    <H3>
                      {{ item.title }}

                    </H3>
                  </ACol>
                </ARow>
                <ARow>
                  <ACol>
                    {{ item.subTitle }}
                  </ACol>
                </ARow>
              </FlexLayoutHeader>
              <FlexLayoutContent>
                <ARow style="height: 100%">
                  <ATextarea :ref="el => item.ref = el as any" style="height: 100%"
                             v-model:value="item.text"></ATextarea>
                </ARow>
              </FlexLayoutContent>
            </FlexLayout>

          </ATabPane>

        </ATabs>
      </FlexLayoutContent>
      <FlexLayoutFooter>
        <ARow :gutter="10">
          <ACol>
            <AButton @click="addTag('{人数}')">+人数</AButton>
          </ACol>
          <ACol>
            <AButton @click="addTag('{时间}')">+时间</AButton>
          </ACol>
          <ACol>
            <AButton @click="addTag('{月日}')">+月日</AButton>
          </ACol>
          <ACol>
            <AButton @click="addTag('{本年}')">+本年</AButton>
          </ACol>
          <ACol>
            <AButton @click="addTag('{昵称}')">+昵称</AButton>
          </ACol>
          <ACol flex="auto" style="text-align: right">
            <AButton type="primary" @click="saveSetting">保存</AButton>
          </ACol>

        </ARow>
      </FlexLayoutFooter>
    </FlexLayout>
  </AModal>

</template>

<style scoped lang="scss">
:deep(.ant-tabs-content) {
  height: 100%;
}

:deep(.ant-tabs-nav) {
  margin-bottom: 0;
}
</style>
