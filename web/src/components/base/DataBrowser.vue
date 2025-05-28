<template>
  <ALayout style="height:100%" v-if="props?.schema">
    <ALayoutHeader style="background-color: white;padding:0 5px;margin:0;line-height:0;height:auto;">
      <SearchContainer v-if="props.schema?.searchContainer?.fields?.length??0>0"
                       v-model:schema="props.schema.searchContainer"></SearchContainer>
    </ALayoutHeader>
    <ALayoutContent style="background-color: white;">
      <SplitLayout horizontal>
        <SplitPane :min-size="20" :size="100"
                   v-if="props.schema?.mainTable?.columns&&props.schema?.mainTable?.columns.length>0">
          <FullTable v-model:schema="props.schema.mainTable"/>
        </SplitPane>
        <SplitPane style="background-color: white" :min-size="20" :size="80"
                   v-if="props.schema?.detailTablesSchema&&props.schema?.detailTablesSchema.detailTables&&props.schema?.detailTablesSchema.detailTables.length>0">
          <DetailTables v-model:schema="props.schema.detailTablesSchema"></DetailTables>
        </SplitPane>
      </SplitLayout>

    </ALayoutContent>
  </ALayout>
</template>

<script lang="ts" setup>
/**
 * 数据浏览组件
 */
import FullTable from "@/components/grid/FullTable.vue";
import {IDataBrowserSchema} from "@/types/schema";
import DetailTables from "@/components/grid/DetailTables.vue";
import SearchContainer from "@/components/base/SearchContainer.vue";
import {editFiledHelper} from "@/utils/editFiledHelper";

const props = defineProps<{
  schema: IDataBrowserSchema
}>();

if (!props.schema) {
  msg.error('schema无值，无法初始化')
}
if (!ext.isRef(props.schema))
  msg.error('schema不是响应式，本组件无法正常工作')

if (props.schema?.searchContainer?.fields) {//此时搜索控件可能还没有渲染出来
  props.schema.searchContainer.queryConditions = editFiledHelper.createDefault(props.schema?.searchContainer?.fields);
}
watch(() => props.schema?.searchContainer?.queryConditions, () => {
  if (props.schema.mainTable)
    props.schema.mainTable.queryConditions = props.schema.searchContainer?.queryConditions;
}, {immediate: true})
watch(() => props.schema?.mainTable?.currentRow, () => {
  if (!props.schema.detailTablesSchema)
    return;
  props.schema.detailTablesSchema.headerRow = props.schema.mainTable?.currentRow;
})

</script>

