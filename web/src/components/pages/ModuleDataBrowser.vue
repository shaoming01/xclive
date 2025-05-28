<template>
  <DataBrowser v-if="schema" v-model:schema="schema"></DataBrowser>
</template>

<script lang="ts" setup>

import {IDataBrowserSchema} from "@/types/schema";
import {ref} from "vue";
import {modalUtil} from "@/utils/modalUtil";

const router = useRouter()
const props = defineProps<{
  schema?: IDataBrowserSchema,
}>();
const schema = ref<IDataBrowserSchema>();
schema.value = ext.appendSchema(props.schema, createSchema()) as IDataBrowserSchema;

function createSchema(): IDataBrowserSchema {
  const buttons = [
    {
      name: '新增', action: async () => {
        await router.push({
          path: '/moduleDesigner',
          query: {title: '模块新增'},
        });
      }
    },
    {
      name: '修改', action: async () => {
        if (!schema.value?.mainTable?.currentRow) {
          return msg.error('请选择1条数据后进行此操作')
        }
        const module = schema.value.mainTable.currentRow;
        await router.push({
          path: '/moduleDesigner',
          query: {moduleId: module['id'], title: '模块设计-' + module['moduleName']}
        });
      }
    },
    {
      name: '预览', index: 40, action: async () => {
        if (!schema.value?.mainTable?.currentRow) {
          return msg.error('请选择1条数据后进行此操作')
        }
        const module = schema.value.mainTable.currentRow;
        await router.push({
          path: `/module/` + module['sysModuleId'],
          query: {title: '模块预览-' + module['moduleName']}
        });
      }
    },
    {
      name: '添加成菜单', index: 50, action: async () => {
        if (!schema.value?.mainTable?.currentRow) {
          return msg.error('请选择1条数据后进行此操作')
        }
        const module = schema.value.mainTable.currentRow;

        const url = `/module/` + module['sysModuleId'];
        await modalUtil.showModalEditorByModule('', 'MenuEditVm_ModalObjectEditor', {objectEditSchema: {data: {'url': url}}});
      }
    }];


  return {
    mainTable: {
      tableTools: buttons,
      gridOptions: {
        groupDefaultExpanded: -1,
        treeData: true,
        getDataPath: row => {
          const path = row['categoryPath'];
          const name = row['moduleName'];
          if (!path)
            return [name];
          const arr = path.split(':');
          arr.push(name);
          return arr;
        },
      }

    },
  };
}


</script>

