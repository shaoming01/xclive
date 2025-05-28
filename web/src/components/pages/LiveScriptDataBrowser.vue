<template>
  <DataBrowser v-if="schema" v-model:schema="schema"></DataBrowser>
</template>

<script lang="ts" setup>

import {IDataBrowserSchema} from "@/types/schema";
import {ref} from "vue";

const props = defineProps<{
  schema?: IDataBrowserSchema,
}>();
console.log('页面初始Schema', props.schema)

const schema = ref<IDataBrowserSchema>();
schema.value = ext.appendSchema(props.schema, createSchema()) as IDataBrowserSchema;

async function getAssistantResp() {
  if (!props.schema?.mainTable?.currentRow) {
    return msg.error('请选择1条数据后进行此操作')
  }
  const id = props.schema.mainTable.currentRow['id'];
  const re = await apiHelper.request('/api/LiveScript/BuildScript', {id: id});
  if (!re.success) {
    return msg.error(re.message);
  }
  props.schema.mainTable.options?.refreshRows([id]);
  msg.success('生成成功')
}

function createSchema(): IDataBrowserSchema {

  return {
    mainTable: {
      tableTools: [
        {
          action: getAssistantResp, index: 50, name: '生成话术'
        }]
    },
    detailTablesSchema: {
      detailTables: [{field: 'LiveScriptVoiceDetailVm', tableSchema: {tableTools: [{name: '播放', module: {comPath: '/src/components/grid/toolBar/PlayLiveScriptDetail.vue'}}]}}]
    }

  }
}


</script>

