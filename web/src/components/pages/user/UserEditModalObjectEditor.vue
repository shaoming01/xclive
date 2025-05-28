<template>
  <ModalObjectEditor v-if="schema" v-model:schema="schema"></ModalObjectEditor>
</template>

<script lang="ts" setup>

import {IFullTableSchema, IModalObjectEditSchema} from "@/types/schema";
import {ref} from "vue";

const props = defineProps<{
  schema?: IModalObjectEditSchema,
}>();
console.log('页面初始Schema', props.schema)

const schema = ref<IModalObjectEditSchema>();


schema.value = ext.appendSchema(customSchema(props.schema), createSchema()) as IModalObjectEditSchema;

function customSchema(schema: IModalObjectEditSchema | undefined): IModalObjectEditSchema | undefined {
  const isNew = props.schema?.dataId;
  if (!schema?.objectEditSchema?.fields || !isNew)
    return schema;
  const hiddenFields = ['password', 'password2'];
  hiddenFields.forEach(field => {
    if (!schema.objectEditSchema.fields) return;
    const index = schema.objectEditSchema.fields.findIndex(f => f.field == field);
    if (index >= 0)
      schema.objectEditSchema.fields.splice(index, 1);
  })
  return schema;
}

function createSchema(): IModalObjectEditSchema {
  return {
    objectEditSchema: {
      detailTablesSchema: {
        detailTables: [{
          field: 'userRoles', tableSchema: {
            tableTools: [{
              name: '添加', action: addNewRole,
            }]
          }
        }]
      }
    },
  }
}

async function addNewRole(tableSchema: IFullTableSchema) {
  const selRe = await tableOptionsHelper.select('','RoleSelectVm_ModalDataSelect');
  if (!selRe.success || !selRe.data) return;
  const extRoleIds = tableSchema.rowData?.map(r => r.roleId);
  const newRows = selRe.data.filter(r => !extRoleIds?.includes(r.id)).map(r => {
    return {
      roleId: r.id,
      roleName: r.name,
    }
  });
  tableSchema.options?.addNewRows(newRows);
}
</script>

