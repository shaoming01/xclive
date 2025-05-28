<template>
  <DataBrowser v-if="schema" v-model:schema="schema"></DataBrowser>
</template>

<script lang="ts" setup>

import {IDataBrowserSchema} from "@/types/schema";
import {ref} from "vue";

const props = defineProps<{
  schema?: IDataBrowserSchema,
}>();
const schema = ref<IDataBrowserSchema>(ext.appendSchema(props.schema, createSchema()));

function createSchema(): IDataBrowserSchema {
  return {}
}

onMounted(() => {
  if (schema.value && schema.value.mainTable)
    schema.value.mainTable.queryConditions = {};
})
console.log(schema)

</script>
<style scoped lang="scss">

</style>

