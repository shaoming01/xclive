<template>
  <SplitLayout>
    <SplitPane v-if="propEditSchema" :min-size="20">
      <FlexLayout>
        <FlexLayoutHeader>
          <ACard>
            <template #title><span style="color:#1890ff">模块信息</span></template>
            <template #extra>
              <AButton @click="saveModule" type="primary">保存模块</AButton>
            </template>
            <ObjectEditor v-model:schema="moduleEditSchema"></ObjectEditor>
          </ACard>

        </FlexLayoutHeader>

        <FlexLayoutContent>
          <ACard>
            <template #title><span style="color:#1890ff">属性编辑</span></template>
            <template #extra>
              <ASpace>
                <AButton @click="updatePreview" typave="primary" title="默认自己预览">预览</AButton>
                <AButton @click="previewPropValue" type="default">编辑自定义JSON</AButton>
                <AButton @click="previewSysSchema" type="default">查看系统JSON</AButton>
                <AButton @click="clearCustomSchema" type="default">清除自定义内容</AButton>
              </ASpace>
            </template>

            <ObjectEditor style="height: 100%!important;"
                          v-model:schema="propEditSchema"></ObjectEditor>
          </ACard>

        </FlexLayoutContent>
      </FlexLayout>
    </SplitPane>
    <SplitPane v-if="previewModule" style="padding: 5px;" :min-size="20">
      <ModuleRender v-model:module="previewModule" :key="modulePreviewKey"></ModuleRender>
    </SplitPane>
  </SplitLayout>
</template>

<script lang="ts" setup>

import {IModalObjectEditSchema, IModuleVm, IObjectEditSchema} from "@/types/schema";
import {useSchemaBuilder} from "@/composables/useSchemaBuilder";
import ObjectEditor from "@/components/base/ObjectEditor.vue";
import {pageApi} from "@/api/pageApi";
import {modalUtil} from "@/utils/modalUtil";

const route = useRoute();
const moduleEditSchema = ref<IObjectEditSchema>({});
const propEditSchema = ref<IObjectEditSchema>({});
const builder = useSchemaBuilder();
const modulePreviewKey = ref(0);

const previewModule = computed(() => {
  //切断页面关联，防止预览页面编辑数据返到配置中去
  const previewModule: IModuleVm = {
    comPath: moduleEditSchema.value?.data?.comPath,
    props: ext.deepCopy(propEditSchema.value.data),
  }
  return {...previewModule, version: modulePreviewKey.value};
})

async function getModule(): Promise<IModuleVm | undefined> {
  if (route.query.moduleJson) {
    return JSON.parse(route.query.moduleJson as string) as IModuleVm;
  }
  if (route.query.moduleId) {
    const re = await pageApi.getModule(route.query.moduleId as string);
    if (!re.success) {
      msg.error(re.message);
      return undefined;
    }
    return re.data as IModuleVm;
  }
  return {};
}


async function createModuleEditSchema(): Promise<IObjectEditSchema> {
  const schemaRe = await pageApi.getSysModuleSchema('ModuleEditVm');
  if (!schemaRe.success) {
    return {}
  }
  const schema = schemaRe.data?.schema as IObjectEditSchema;
  schema.data = await getModule();
  //补充监听字段
  schema.data = schema.data ?? {}
  schema.data.sysModuleName = schema.data.sysModuleName ?? undefined;
  schema.data.comPath = schema.data.comPath ?? undefined;

  return schema;
}

async function moduleChanged() {
  const comPath = moduleEditSchema.value?.data?.comPath;
  if (!comPath) {
    console.log('comPath为空，清空属性编辑器')
    propEditSchema.value = {};
    return;
  }
  let sysSchema = await getSysSchema(moduleEditSchema.value?.data?.sysModuleName);
  const fullProps = ext.mergeSchema(sysSchema, ext.deepCopy(moduleEditSchema.value?.data?.props));
  const editSchema = builder.createVueSchema(comPath) ?? {};
  editSchema.data = fullProps;
  propEditSchema.value = editSchema;
  console.log('初始化属性编辑Schema：', propEditSchema.value.data)


}

async function getSysSchema(moduleName: string | undefined): Promise<undefined | any> {
  if (!moduleName) return;
  const re = await pageApi.getSysModuleSchema(moduleName);
  if (!re.success) {
    msg.error(re.message);
    return;
  }
  console.log('获取系统属性' + moduleName, re.data);
  return re.data;
}

//属性修改
watch(() => propEditSchema.value?.data, updatePreview, {deep: true})

async function previewPropValue() {
  const propsData = propEditSchema.value?.options?.getData();
  const sysSchema = await getSysSchema(moduleEditSchema.value?.data?.sysModuleName);
  const diff = ext.getExtraFields(sysSchema, propsData, ['name', 'field']);
  console.log('对比Schema', sysSchema, propsData, diff);

  const schema: IObjectEditSchema = {
    fields: [
      {
        field: 'json',
        label: '',
        module: {comPath: '/src/components/edit/tool/JsonStringEditor.vue',},
        fieldType: '',
        groupName: '',
        span: 24,
        labelColSpan: -1,
        wrapperColSpan: 24,
      }
    ], data: {json: JSON.stringify(diff ?? {}, null, 2)}
  }
  const modalSchema: IModalObjectEditSchema = {
    objectEditSchema: schema, save: async (data) => {
      if (!data)
        return R.error('data为空');
      const json = data['json'] ?? ''
      if (propEditSchema.value)
        propEditSchema.value.data = ext.mergeSchema(sysSchema, JSON.parse(json));

      return R.ok();
    }, sizeMode: 6,
  };
  return modalUtil.showModalEditor(modalSchema);
}

async function previewSysSchema() {
  const sysSchema = await getSysSchema(moduleEditSchema.value?.data?.sysModuleName);

  const schema: IObjectEditSchema = {
    fields: [
      {
        field: 'json',
        label: '',
        module: {comPath: '/src/components/edit/tool/JsonStringEditor.vue',},
        fieldType: '',
        groupName: '',
        span: 24,
        labelColSpan: -1,
        wrapperColSpan: 24,
      }
    ], data: {json: JSON.stringify(sysSchema ?? {}, null, 2)}
  }
  const modalSchema: IModalObjectEditSchema = {
    objectEditSchema: schema, save: async (data) => {
      if (!data)
        return R.error('data为空');
      const json = data['json'] ?? ''
      if (propEditSchema.value)
        propEditSchema.value.data = JSON.parse(json);

      return R.ok();
    }, sizeMode: 6,
  };
  return modalUtil.showModalEditor(modalSchema);
}

async function clearCustomSchema() {
  if (!moduleEditSchema.value?.data?.props)
    return;
  moduleEditSchema.value.data.props = {};
  await moduleChanged();
  await updatePreview();
}

async function updatePreview() {
  modulePreviewKey.value++;
}

async function saveModule() {
  const propsData = propEditSchema.value?.options?.getData();
  const sysSchema = await getSysSchema(moduleEditSchema.value?.data?.sysModuleName);
  const diff = ext.getExtraFields(sysSchema, propsData, ['name', 'field']);

  if (!moduleEditSchema.value?.options)
    return msg.error('找不到编辑器对象');
  const data = moduleEditSchema.value?.options?.getData() ?? {};
  data['props'] = diff;
  console.log('保存Module', data);
  const re = await pageApi.saveModule(data);
  if (!re.success) {
    return msg.error(re.message)
  }
  await moduleEditSchema.value?.options?.showData(re.data);
  msg.success('保存成功');

}

async function ini() {
  moduleEditSchema.value = await createModuleEditSchema();
}

//组件路径修改
watch(() => moduleEditSchema.value?.data?.comPath, () => {
  moduleChanged()
})
watch(() => moduleEditSchema.value?.data?.sysModuleName, () => {
  moduleChanged()
})

ini();

</script>
<style scoped lang="scss">
:deep(.ant-card-body) {
  height: 100%;
  padding: 5px;
  overflow-y: auto !important;
  flex: auto;

}

.ant-card {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.ant-card-head {
  flex: none;
}


</style>

