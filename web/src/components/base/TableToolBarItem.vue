<script setup lang="ts">
import {IFullTableSchema, ITableToolBarItemSchema} from "@/types/schema";


const props = defineProps<{
  item: ITableToolBarItemSchema
  /**
   * 不用填
   */
  isInMenu?: boolean,
  tableSchema: IFullTableSchema,
}>()
const subButtons = computed(() => {
  if (!props.item || !props.item.children || props.item.children.length == 0)
    return undefined;

  return props.item.children.sort((a, b) => {
    return (a.index ?? 0) - (b.index ?? 0)
  });

})
const comType = computed<'button' | 'module' | 'menu' | 'menuItem' | 'subMenu' | 'divider'>(() => {
  if (props.item?.type == 'divider') return 'divider';
  if (props.item?.module?.comPath) return 'module';
  const hasChild = subButtons.value && subButtons.value.length > 0;

  if (!props.isInMenu)
    return hasChild ? 'menu' : 'button';

  return hasChild ? 'subMenu' : 'menuItem';
})

//console.log(props.item.title + '类型：' + comType.value);

async function clicked(): Promise<void> {
  if (!props.item.action) return;
  await props.item.action(props.tableSchema);
}
</script>

<template>
  <ModuleRender v-if="comType=='module'" :module="props.item?.module" v-bind="props"></ModuleRender>
  <LoadingBtn v-if="comType=='button'"
              :type="props.item.type=='primary'?'primary':'default'"
              :click="clicked"
              :icon="getIcon(props.item.icon)">{{ props.item.name }}
  </LoadingBtn>

  <ADropdownButton v-if="comType=='menu'">
    <Icon :name="props.item.icon"/>
    {{ props.item.name }}
    <template #overlay>
      <a-menu>
        <TableToolBarItem v-for="child in subButtons" :item="child" :is-in-menu="true"
                          :table-schema="props.tableSchema"></TableToolBarItem>
      </a-menu>
    </template>
  </ADropdownButton>

  <ASubMenu v-if="comType=='subMenu'" :key="props.item.name" :title="props.item.name" :is-in-menu="true">
    <TableToolBarItem v-for="child in subButtons" :item="child" :is-in-menu="true"
                      :table-schema="props.tableSchema"></TableToolBarItem>
  </ASubMenu>

  <AMenuItem @click="clicked" v-if="comType=='menuItem'" :label="props.item.name" :key="props.item.name"
             :title="props.item.name"
             :icon="getIcon(props.item.icon)">
    {{ props.item.name }}
  </AMenuItem>

  <AMenuDivider @click="clicked" v-if="comType=='divider'">
    {{ props.item.name }}
  </AMenuDivider>
</template>

<style scoped>

</style>