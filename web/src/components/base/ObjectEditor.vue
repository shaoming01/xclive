<template>
  <SplitLayout horizontal v-if="props.schema">
    <SplitPane :min-size="20" v-if="props.schema.fields&&props.schema.fields.length>0">
      <AForm ref="formRef" :model="formData"
             style="height: 100%;overflow-y: auto;padding: 3px;display:flex;flex-direction: column;">
        <ACard v-for="(fields,groupName) in groupedFieldSchemas"
               :title="groupName"
               size="small"
               :hoverable="true"
               :bordered="true"
               style="margin-bottom: 5px;flex: auto;cursor: default">
          <ARow>
            <ACol v-for="field in fields" :span="field.span??8" :offset="field.offset??0">
              <AFormItem :label="field.label"
                         :name="getNamePath(field.field)"
                         :label-col="{span:field.labelColSpan??8,offset:field.labelColOffset??0}"
                         :wrapper-col="{span:field.wrapperColSpan??12,offset:field.wrapperColOffset??0}"
                         :rules="createFieldRules(field)"
                         :tooltip="field.tip"
              >
                <component v-if="field.module"
                           :is="getCom(field.module.comPath)"
                           :value="_.get(formData,field.field)"
                           @update:value="(val:any)=>{_.set(formData,field.field,val)}"
                           :disabled="field.disabled"
                           :placeholder="field.placeholder"
                           :allowClear="field.allowClear"
                           v-bind="field.module.props"
                ></component>
              </AFormItem>
            </ACol>
          </ARow>

        </ACard>
      </AForm>
    </SplitPane>
    <SplitPane :minSize="20"
               v-if="props.schema.detailTablesSchema&&props.schema.detailTablesSchema.detailTables&&props.schema.detailTablesSchema.detailTables.length>0">
      <DetailTables v-model:schema="props.schema.detailTablesSchema"></DetailTables>
    </SplitPane>
  </SplitLayout>

</template>

<script lang="ts" setup>
import {computed, ref} from 'vue'
import {
  IEditFieldSchema,
  IObjectEditSchema, SchemaUpdateEmits
} from "@/types/schema";
import SplitLayout from "@/components/layout/SplitLayout.vue";
import SplitPane from "@/components/layout/SplitPane.vue"
import {FormInstance} from "ant-design-vue";
import {R} from "@/utils/R";
import {groupBy} from "lodash";
import _ from "lodash";
import {comUtil} from "@/utils/com";

const props = defineProps<{
  schema: IObjectEditSchema,
}>()
const emit = defineEmits<SchemaUpdateEmits>();
let suppressUpdate = false;
const formRef = ref<FormInstance>();
const formData = ref<Record<string, any>>({});
const instanceId = `ObjectEditor-${ext.random(1000, 9999)}`;


const groupedFieldSchemas = computed(() => {
  if (!props.schema.fields) return {};
  return groupBy(props.schema.fields, item => item.groupName ?? '');
});

/**
 * 因为访问的是路径，可能路径是不存在的，所以会报错，不用理会，填了值以后路径有效了，一样可以验证
 * please transfer a valid name path to form item
 * @param field
 */
function getNamePath(field: string) {
  return _.split(field, '.');
}

function createFieldRules(field: IEditFieldSchema) {
  if (field.require) {
    return [{required: true,}]
  }
  return undefined;
}

function getId() {
  if (!props.schema)
    return instanceId + ':null-schema'
  if (props.schema && props.schema.fields && props.schema.fields?.length > 0) {
    return instanceId + ':' + props.schema.fields[0].field
  }
  if (props.schema && props.schema.detailTablesSchema && props.schema.detailTablesSchema && props.schema.detailTablesSchema.detailTables && props.schema.detailTablesSchema.detailTables.length > 0) {
    return instanceId + ':' + props.schema.detailTablesSchema.detailTables[0].field
  }
  return instanceId + ':empty-schema'
}

function getCom(comPath: string | undefined) {
  return comUtil.getCom(comPath);

}

/**
 * 将传入值绑定到Form和Detail表中去
 * @param val
 */
async function showData(val: Record<string, any> | undefined) {
  if (!props.schema || suppressUpdate) return;
  suppressUpdate = true;
  formData.value = val ?? {};
  if (props.schema.detailTablesSchema && props.schema.detailTablesSchema.detailTables) {
    for (const table of props.schema.detailTablesSchema.detailTables) {
      _.set(formData.value, table.field, _.get(formData.value, table.field) ?? [])
      table.tableSchema.rowData = _.get(formData.value, table.field);

      watch(() => table.tableSchema.rowData, () => {
        console.log('监听到表格内容有变化，将内容反馈到form数据中去' + getId())
        _.set(formData.value, table.field, table.tableSchema.rowData);
      })
    }
  }
  await nextTick();
  suppressUpdate = false;
  props.schema.valueChanged = false;
}

async function validate(): Promise<R> {
  return new Promise(resolve => {
    if (!formRef.value) return resolve(R.ok());
    formRef.value.validate().then(() => {
      resolve(R.ok())
    }).catch((err) => {
      const errs: string[] = [];
      if (err?.errorFields?.length > 0) {
        err?.errorFields?.forEach((er: any) => {
          errs.push(...er['errors']);
        })
      }
      resolve(R.error('表格校验错误：' + errs.join(',')));
    });
  })
}

function getData() {
  return formData.value;
}

async function emitUpdate() {
  if (suppressUpdate) return;
  suppressUpdate = true;
  console.log(`${instanceId}表单变化开始回馈` + getId())

  props.schema.valueChanged = true;
  props.schema.data = formData.value;
  emit('update:schema', props.schema);
  await nextTick();
  suppressUpdate = false;

}

showData(props.schema?.data);
watch(() => formData.value, emitUpdate, {deep: true})
watch(() => props.schema?.data, (value) => {
  if (suppressUpdate) return;
  console.log('外部传入的Data有变化' + getId(), value);
  showData(props.schema.data);
})
watch(() => props.schema.options, () => {//外部有可能会重新给schema赋值，需要重新触发初始化options方法
  if (props.schema.options) return;
  props.schema.options = {
    showData, validate, getData
  }
}, {immediate: true})

</script>

<style scoped lang="scss">
</style>