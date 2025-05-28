<script setup lang="ts">
import {ILiveSettingVm} from "@/views/live/help/LiveInterface";
import {Key} from "ant-design-vue/es/_util/type";
import _ from "lodash";

const settingData = ref<ILiveSettingVm | undefined>();

const current = ref('0');

function addTag(tag: string) {
  const el = refMap.value[current.value];
  if (!el) return
  const item = settingData.value?.replySetting?.[_.toNumber(current.value)];
  if (!item) return
  const textArea = el?.['resizableTextArea']?.['textArea'] as HTMLTextAreaElement | undefined;
  if (!textArea) return

  const start = textArea.selectionStart
  const end = textArea.selectionEnd
  const value = item.content

  item.content = value.slice(0, start) + tag + value.slice(end)

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
  if (!settingData.value?.replySetting?.length) {
    addNewGroup();
  }
}

async function saveSetting() {
  const re = await apiHelper.request('/api/sys/SaveSetting', {
    typeName: 'LiveSettingVm',
  }, settingData.value);
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
const refMap = ref<Record<string, any>>({});

function addNewGroup() {
  if (!settingData.value) settingData.value = {}
  if (!settingData.value.replySetting) settingData.value.replySetting = [];
  settingData.value.replySetting.push({name: '新分组', content: '', matchRules: [{keyword: '', isFuzzy: true}]});
  current.value = (settingData.value.replySetting.length - 1).toString();

}

function remove(targetKey1: string) {
  settingData.value?.replySetting?.splice(_.toNumber(targetKey1), 1);
  if (current.value >= targetKey1) {
    current.value = Math.max(_.toNumber(current.value) - 1, 0).toString();
  }
}

function removeKeywordIndex(index: number) {
  const item = settingData.value?.replySetting?.[_.toNumber(current.value)];
  if (!item) return
  item.matchRules?.splice(index, 1);
}

function addKeyword() {
  const item = settingData.value?.replySetting?.[_.toNumber(current.value)];
  if (!item) return
  if (!item.matchRules)
    item.matchRules = [];
  item.matchRules.push({keyword: '', isFuzzy: true});

}

const onEdit = (e: MouseEvent | Key | KeyboardEvent, action: "add" | "remove") => {
  if (action === 'add') {
    addNewGroup();
  } else {
    remove(e as string);
  }
};
</script>

<template>
  <AModal :destroyOnClose="true" v-model:open="show"
          @ok="saveSetting"
          :width="size.width"
          :centered="true"
          :bodyStyle="{height:size.height}"
          :footer="null"
  >
    <template #title>
      <div>通用关键词语音回复</div>
      <div style="font-size: 14px;font-weight: normal;color: #4d4e52">
        当公屏上有人评论的关键词匹配到后，随机语音回复下方编辑框中的一行话
      </div>
    </template>

    <FlexLayout style="padding: 10px">
      <FlexLayoutHeader>

      </FlexLayoutHeader>
      <FlexLayoutContent>
        <ATabs style="height: 100%" v-model:active-key="current" type="editable-card" @edit="onEdit">

          <ATabPane :closable="true" :key="index.toString()" :tab="item.name"
                    v-for="(item,index) in settingData?.replySetting">
            <FlexLayout style="padding: 5px">
              <FlexLayoutHeader>
                <ARow align="middle">
                  <ACol>分组名称：</ACol>
                  <ACol>
                    <AInput v-model:value="item.name"></AInput>
                  </ACol>
                </ARow>
                <ARow align="middle" :gutter="10" style="margin-top: 5px">
                  <ACol>关键词：</ACol>
                  <ACol>
                    <AButton :icon="getIcon('icon-tianjia')" @click="addKeyword">添加关键词</AButton>
                  </ACol>
                </ARow>
                <ARow :gutter="[10 ,10]" style="margin-top: 5px" align="middle">
                  <ACol span="8" v-for="(keywordItem,keywordIndex) in item.matchRules">
                    <ARow align="middle" :gutter="2">
                      <ACol span="12">
                        <AInput v-model:value="keywordItem.keyword"></AInput>
                      </ACol>
                      <ACol>
                        <ACheckbox v-model:value="keywordItem.isFuzzy">模糊</ACheckbox>
                      </ACol>
                      <ACol>
                        <Icon name="icon-guanbi" @click="()=>removeKeywordIndex(keywordIndex)"></Icon>
                      </ACol>
                    </ARow>

                  </ACol>
                </ARow>
                <ARow style="margin-top: 5px">
                  <ACol>回复内容：</ACol>
                </ARow>
              </FlexLayoutHeader>
              <FlexLayoutContent style="min-height: 50px;">
                <ARow style="height: 100%;;">
                  <ATextarea :ref="el =>refMap[index]  = el as any" style="height: 100%"
                             v-model:value="item.content"></ATextarea>
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
