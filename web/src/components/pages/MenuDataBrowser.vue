<template>
  <DataBrowser v-if="schema" v-model:schema="schema"></DataBrowser>
</template>

<script lang="ts" setup>

import {IDataBrowserSchema, IMenuVm} from "@/types/schema";
import {ref} from "vue";

const props = defineProps<{
  schema?: IDataBrowserSchema,
}>();
console.log('页面初始Schema', props.schema)

const schema = ref<IDataBrowserSchema>();
schema.value = ext.appendSchema(props.schema, createSchema()) as IDataBrowserSchema;

function createSchema(): IDataBrowserSchema {
  return {
    mainTable: {
      gridOptions: {
        groupDefaultExpanded: -1,
        treeData: true,
        getDataPath: row => {
          let path = getMenuPath(row);
          path.push(row.title);
          return path;
        },
        rowSelection: "single",
      }

    },
  }
}

function getMenuPath(menu: IMenuVm): string[] {
  const arr: string[] = [];
  const menus: IMenuVm[] = schema.value?.mainTable?.rowData ?? [];
  const parentMenu = menus.find(m => m.id == menu.parentId);
  if (!parentMenu)
    return arr;
  arr.unshift(parentMenu.title);
  if (menu.id != menu.parentId)//防止死循环
    arr.unshift(...getMenuPath(parentMenu))
  return arr;
}


</script>

