<template>
  <ModalObjectEditor v-if="schema" v-model:schema="schema"></ModalObjectEditor>
</template>

<script lang="ts" setup>

import {
  IDetailTableSchema, IFullTableSchema,
  IModalObjectEditSchema
} from "@/types/schema";
import {ref} from "vue";
import {tableOptionsHelper} from "@/utils/tableOptionsHelper";

const props = defineProps<{
  schema?: IModalObjectEditSchema,
}>()
const schema = ref<IModalObjectEditSchema>();

function createCustomSchema(): IModalObjectEditSchema {
  const detailTable: IDetailTableSchema = {
    field: 'userDetails', tableSchema: {
      tableTools: [{
        name: '添加用户', action: async (tableSchema: IFullTableSchema) => {
          const selectRe = await tableOptionsHelper.select('', 'UserEditVm_ModalObjectEditor');
          if (!selectRe.success) return;
          tableSchema.options?.addNewRows(selectRe.data)
        }
      }, {
        name: '删除', action: tableOptionsHelper.del,
      }]
    }
  };
  return {objectEditSchema: {detailTablesSchema: {detailTables: [detailTable]}}}
}


function iniSchema() {
  schema.value = ext.mergeSchema(props.schema, createCustomSchema());
}

iniSchema();

</script>

