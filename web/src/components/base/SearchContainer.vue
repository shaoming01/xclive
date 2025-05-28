<script setup lang="ts">
import SearchToolBar from "@/components/base/SearchToolBar.vue";
import {ISearchContainerSchema, SchemaUpdateEmits} from "@/types/schema";
import {comUtil} from "@/utils/com";
import {ref} from "vue";

const props = defineProps<{
  schema?: ISearchContainerSchema;
}>();
const emit = defineEmits<SchemaUpdateEmits>();
/**
 * 初始化的查询条件
 */
const initialConditions = props.schema?.queryConditions ?? {};
const localCondition = {...initialConditions};
/**
 * 当前查询条件
 */
const conditions = ref(localCondition);
if (props.schema?.searchGroup) {
  props.schema.searchGroup.defaultConditions = initialConditions;
}

function setConditions(newObj: Record<string, any> | undefined) {
  conditions.value = {...newObj};
}

async function doReset() {
  conditions.value = {...initialConditions};
  await doQuery();
}

async function doQuery() {
  if (!props.schema) {
    return console.log('schema not found');
  }
  props.schema.queryConditions = conditions.value;
  emit("update:schema", props.schema);
  if (props.schema?.doQuery)
    await props.schema.doQuery(conditions.value);
}


watch(() => props.schema?.queryConditions, () => {
  setConditions(props.schema?.queryConditions);
}, {immediate: true})


function getCom(comPath: string | undefined) {
  return comUtil.getCom(comPath);
}

onMounted(() => {
  console.log('conditions', conditions)
})
watch(() => props.schema?.searchGroup?.selectedGroupConditions, () => {
  if (props.schema)
    props.schema.queryConditions = props.schema.searchGroup?.selectedGroupConditions;
})

async function doSave() {
  await props.schema?.searchGroup?.doSave(conditions.value);
}

function handleKeydown(e: KeyboardEvent) {
  if (e.key == 'Enter') {
    doQuery();
  }
}
</script>

<template>
  <SearchGroup v-if="props.schema?.searchGroup" v-model:schema="props.schema.searchGroup"></SearchGroup>
  <ARow justify="start" :gutter="[5,5]" style="margin: 5px 0 2px 0;"
        v-if="props.schema?.fields&&props.schema?.fields.length > 0">
    <ACol v-for="field in props.schema.fields" :span="field.span">
      <!--suppress HtmlDeprecatedAttribute -->
      <ARow justify="start" align="middle">
        <ACol :span="field.labelColSpan" class="labelCss">{{ field.label }}：</ACol>
        <ACol :span="field.wrapperColSpan">
          <component v-if="field.module"
                     :is="getCom(field.module.comPath)"
                     v-model:value="conditions[field.field]"
                     v-bind="field.module.props"
                     @keydown="handleKeydown"
          ></component>
        </ACol>
      </ARow>
    </ACol>
    <ACol span="6">
      <SearchToolBar :do-query="doQuery" :do-reset="doReset" :do-save="doSave"></SearchToolBar>
    </ACol>
  </ARow>
</template>
<style scoped lang="scss">
.labelCss {
  text-align: right;
}
</style>

