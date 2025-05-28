<script setup lang="ts">

import {IDataBrowserSchema, IFullTableSchema} from "@/types/schema";
import {ref} from "vue";
import {pageApi} from "@/api/pageApi";
import DataBrowser from "@/components/base/DataBrowser.vue";
import {ILiveScriptRow, ILiveScriptVoiceDetailVm} from "@/views/live/help/LiveInterface";
import {R} from "@/utils/R";


const schema = ref<IDataBrowserSchema>();

async function ini() {
  const re = await pageApi.getSysModuleSchema('AiScriptTemplateVm');
  if (!re.success) return msg.error(re.message);
  schema.value = ext.appendSchema(re.data?.schema, createLocalSchema()) as IDataBrowserSchema;
}

function createLocalSchema(): IDataBrowserSchema {
  return {
    mainTable: {
      tableTools: [
        {
          name: '测试生成话术',
          action: testBuild,
          index: 40,
        }],
      columns: [
        {
          field: 'systemTemplate',
        }
      ]
    },
  }
}

async function testBuild(tableSchema: IFullTableSchema, par?: any) {
  const row = tableSchema.currentRow;
  if (!row) return msg.error('请选择一条规则，再点此按钮');
  if (row.usage == 1 && !await msg.confirm('生成话术运行较慢，一般需要1-2分钟，请耐心等待！')) {
    return;
  }
  let inputText = ''
  if (row.usage == 2) {
    const re = await modalUtil.showStringInput('请模拟输入用户问题', '老王说:什么价格');
    if (!re.success) {
      return
    }
    inputText = re.data ?? '';
  } else if (row.usage == 3) {
    const re = await modalUtil.showStringInput('请模拟输入用户互动内容', '老王点了赞');
    if (!re.success) {
      return
    }
    inputText = re.data ?? '';
  }
  const req = {templateId: row.id, inputText: inputText};
  const newScriptRe = await apiHelper.request<ILiveScriptVoiceDetailVm[]>('api/LiveScript/BuildScriptByTemplate', req);
  if (!newScriptRe.success) return R.error(newScriptRe.message);
  const newArr: ILiveScriptVoiceDetailVm[] = newScriptRe.data ?? [];
  const newRows = newArr.map(d => {
    return d.text
  });
  const respContent = newRows.join('\n');
  await modalUtil.showTextArea('生成结果', respContent);

}

onMounted(async () => {
  await ini();
})
</script>

<template>
  <FlexLayout v-if="schema">
    <FlexLayoutHeader>
      <H3 style="margin-top: 15px;margin-left: 20px;">AI话术设计</H3>
      <ADivider style="margin-top: 15px; margin-bottom: 5px;"></ADivider>
    </FlexLayoutHeader>
    <FlexLayoutContent>
      <DataBrowser v-if="schema" v-model:schema="schema"></DataBrowser>
    </FlexLayoutContent>
  </FlexLayout>
</template>

<style scoped lang="scss">


</style>
