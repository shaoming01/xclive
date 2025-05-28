<template>
  <ATabs v-model:activeKey="activeKey" type="card" style="height: 100%"
         v-if="props.schema.detailTables&&props.schema.detailTables.length>0">
    <ATabPane class="ant-tabs-content" v-for="tab in props.schema.detailTables" :tab="tab.tab" :key="tab.tab">
      <FullTable v-model:schema="tab.tableSchema"
                 :ref="(el) => detailTableRefs[tab.tab??''] = <InstanceType<typeof FullTable>>el"/>
    </ATabPane>
  </ATabs>
</template>
<script setup lang="ts">

import {IDetailTablesSchema} from "@/types/schema";
import FullTable from "@/components/grid/FullTable.vue";
import {ref} from "vue";

const props = withDefaults(defineProps<{
  schema: IDetailTablesSchema,
}>(), {});

const firstTab = computed(() => {
  if (!props.schema.detailTables || props.schema.detailTables.length <= 0) return '';
  return props.schema.detailTables[0].tab ?? '';
})

const activeKey = ref<string>();
const detailTableRefs = ref<Record<string, InstanceType<typeof FullTable>>>({});

watch(() => props.schema.headerRow, queryDetailTable)
watch(() => activeKey.value, queryDetailTable)
watch(() => firstTab.value, () => activeKey.value = firstTab.value,
    {
      immediate: true
    }
)

async function queryDetailTable() {
  if (!props.schema.detailTables) return;
  await nextTick();
  const table = props.schema.detailTables.find(t => t.tab == activeKey.value);
  if (!table) return console.warn('没有显示明细');

  const headerIdKey = table.tableSchema.headerKey ?? '';
  const primaryKey = table.tableSchema.primaryKey ?? '';
  const condition = {} as any;
  const headerIdVal = props.schema.headerRow ? props.schema.headerRow[primaryKey] : '';
  if (!headerIdVal) return;
  condition[headerIdKey] = headerIdVal;
  table.tableSchema.queryConditions = condition;
  console.log('查询明细:' + activeKey.value)
}


</script>


<style scoped lang="scss">
:deep(.ant-tabs-content) {
  height: 100%;
}

:deep(.ant-tabs-nav) {
  margin-bottom: 0;
}
</style>