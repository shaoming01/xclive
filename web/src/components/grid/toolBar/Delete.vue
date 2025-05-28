<script setup lang="ts">
import {IFullTableSchema, ITableToolBarItemSchema} from "@/types/schema";

const props = defineProps<{
  tableSchema: IFullTableSchema,
  item: ITableToolBarItemSchema,
  moduleId: string,
}>();

async function clicked(): Promise<void> {
  return tableOptionsHelper.del(props.tableSchema);
}

</script>

<template>
  <LoadingBtn
      :type="props.item.type=='primary'?'primary':'default'"
      :click="clicked"
      :icon="getIcon(props.item.icon)">{{ props.item.name }}
  </LoadingBtn>
</template>
