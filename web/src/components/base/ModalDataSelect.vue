<script setup lang="ts">
import {IDataBrowserSchema, IModalDataSelectSchema} from "@/types/schema";
import zhCN from "ant-design-vue/es/locale/zh_CN";
import {modalUtil} from "@/utils/modalUtil";

const show = ref(true);

const props = defineProps<{
  schema: IModalDataSelectSchema,
}>();
const closeRe = R.error<any[]>('');
const size = ref({width: '400px', height: '300px'});

function afterClose() {
  props.schema.afterClose && props.schema.afterClose(closeRe);
}

async function ok() {
  if (!props.schema.dataBrowserSchema.mainTable?.options?.getSelectedRows)
    return msg.error('表格未初始化');
  const data = props.schema.dataBrowserSchema.mainTable?.options?.getSelectedRows();
  if (!data || data.length <= 0)
    return msg.error('未选择数据');
  closeRe.success = true;
  closeRe.data = data;
  show.value = false;
}

function getLocalSchema(): IDataBrowserSchema {
  return {}
}

function ini() {
  ext.appendSchema(props.schema.dataBrowserSchema, getLocalSchema());
}

watch(() => props.schema.sizeMode, () => {
  size.value = modalUtil.calcModalSize(props.schema.sizeMode ?? 0)
}, {immediate: true})

ini();
</script>

<template>
  <!--这个Modal会动态添加到页面，所以共享不到主APP设置的语言-->
  <AConfigProvider :locale="zhCN">
    <AModal :destroyOnClose="true" v-model:open="show"
            :after-close="afterClose"
            @ok="ok"
            :width="size.width"
            :title="props.schema.title??'数据选择'"
            :bodyStyle="{height:size.height}"
            :centered="props.schema.centered"
    >
      <DataBrowser v-model:schema="props.schema.dataBrowserSchema">
      </DataBrowser>
    </AModal>
  </AConfigProvider>
</template>
