<script setup lang="ts">
import {IFullTableSchema, ITableToolBarItemSchema} from "@/types/schema";

const props = withDefaults(defineProps<{
  items?: ITableToolBarItemSchema[],
  tableSchema: IFullTableSchema,
}>(), {})

const buttons = computed(() => {
  if (!props.items || props.items.length == 0)
    return undefined;

  return props.items.sort((a, b) => {
    return (a.index ?? 0) - (b.index ?? 0)
  });

})
</script>

<template>
  <ARow :gutter="[5,5]" style="margin:2px 0 3px 1px;" v-if="buttons">
    <a-col v-for="btn in buttons">
      <TableToolBarItem :item="btn" :tableSchema="props.tableSchema"></TableToolBarItem>
    </a-col>
  </ARow>

</template>

<style scoped>

</style>