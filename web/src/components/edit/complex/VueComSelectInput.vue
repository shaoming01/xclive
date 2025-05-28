<script setup lang="ts">
import {computed} from 'vue'
import {IDataBrowserSchema, IModalDataSelectSchema, ValueUpdateEmits} from "@/types/schema";
import {IQueryParam} from "@/types/dto";
import {R} from "@/utils/R";
import {vueComDoc} from "@/utils/vueComDoc";
import {IVueComData} from "@/types/vueComData";
import {modalUtil} from "@/utils/modalUtil";

const props = defineProps<{
  value: string | undefined,
}>()
const value = computed({
  get() {
    return props.value;
  },
  set(val: string | undefined) {
    emit("update:value", val);
  }
});
const emit = defineEmits<ValueUpdateEmits>();

async function selectComPath() {
  const dataSelectSchema: IDataBrowserSchema = {
    mainTable: {
      columns: [
        {width: 150, editable: false, headerName: '组件名称', field: 'name'},
        {width: 80, editable: false, headerName: 'id', field: 'id'},
        {width: 300, editable: false, headerName: '组件路径', field: 'path'},
        {width: 100, editable: false, headerName: '说明', field: 'desc'},
      ],
      autoQuery: true,
      showPageBar: false,
      gridOptions: {
        treeData: true,
        groupDefaultExpanded: -1,
        getDataPath: row => {
          let path = row.path.replace('/src/components/', '');
          return path.split('/');
        }
      },
      queryDataUrl: queryComponents
    }
  }
  const modalDataSelectSchema: IModalDataSelectSchema = {dataBrowserSchema: dataSelectSchema, sizeMode: 6};
  const modalDataSelectSchemaRef = ref(modalDataSelectSchema);

  const selRe = await modalUtil.showDataSelect(modalDataSelectSchemaRef.value as IModalDataSelectSchema);
  if (selRe.success && selRe.data && selRe.data.length > 0) {
    const reRow = selRe.data[0];
    value.value = reRow['id'];
  }

}

async function queryComponents(queryObj: IQueryParam | undefined): Promise<R<any[]>> {
  const comData = vueComDoc as IVueComData;
  const list = [];

  for (let comPath in comData.vueComponents) {
    list.push({
      id: comPath,
      path: comPath,
      name: comData.vueComponents[comPath].displayName,
      desc: comData.vueComponents[comPath].description
    })
  }

  //查询目录中的组件列出来
  return R.ok(list);
}
</script>

<template>
  <AInput v-model:value="value">
    <template #addonAfter>
      <Icon type="icon-a-014_fangdajing" @click="selectComPath"></Icon>
    </template>
  </AInput>
</template>
