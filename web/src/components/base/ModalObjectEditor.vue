<script setup lang="ts">
import {IModalObjectEditSchema} from "@/types/schema";
import ObjectEditor from "@/components/base/ObjectEditor.vue";
import zhCN from "ant-design-vue/es/locale/zh_CN";
import {pageApi} from "@/api/pageApi";
import {modalUtil} from "@/utils/modalUtil";

const show = ref(true);

const props = defineProps<{
  schema: IModalObjectEditSchema,
}>();
const size = ref({width: '400px', height: '300px'});
const editorSchema = ref(props.schema.objectEditSchema);
const closeRe = R.error('');

function afterClose() {
  props.schema.afterClose && props.schema.afterClose(closeRe);
}

function handleCancel() {
  if (!editorSchema.value.valueChanged) return;
  show.value = !confirm("表单已经修改，强行关闭将不会保存内容？");
}

async function ok() {
  if (!editorSchema.value?.options) {
    msg.error('找不到表单对象')
    return
  }
  const formChk = await editorSchema.value.options.validate();
  if (!formChk.success) {
    msg.error(formChk.message)
    return
  }
  const data = editorSchema.value.options.getData();
  if (props.schema.validate) {
    const remoteValidateRe = await props.schema.validate(data);
    if (!remoteValidateRe.success) {
      msg.error(remoteValidateRe.message)
      return
    }
  }
  if (props.schema.save) {
    const saveRe = await props.schema.save(data);
    if (!saveRe.success) {
      msg.error(saveRe.message)
      return
    }
    show.value = false;
    msg.success('保存成功')
  } else if (props.schema.saveDataUrl) {
    const saveRe = await pageApi.save(props.schema.saveDataUrl, data);
    if (!saveRe.success) {
      msg.error(saveRe.message)
      return
    }
    props.schema.objectEditSchema.data = saveRe.data;
    show.value = false;
    closeRe.success = true;
    closeRe.data = saveRe.data;
    msg.success('保存成功')
  } else {
    closeRe.success = true;
    closeRe.data = data;
    show.value = false;
  }

}


async function iniData() {
  if (!props.schema.getDataUrl || !props.schema.dataId) return;
  const re = await pageApi.getById(props.schema.getDataUrl, props.schema.dataId);
  if (!re.success) return msg.error('获取数据出错：' + re.message);
  editorSchema.value.data = re.data;
}

watch(() => props.schema.sizeMode, () => {
  size.value = modalUtil.calcModalSize(props.schema.sizeMode ?? 0)
}, {immediate: true})
iniData();
</script>

<template>
  <!--这个Modal会动态添加到页面，所以共享不到主APP设置的语言-->
  <AConfigProvider :locale="zhCN" v-if="props.schema">
    <AModal :destroyOnClose="true" v-model:open="show"
            :after-close="afterClose"
            @cancel="handleCancel"
            @ok="ok"
            :width="size.width"
            :title="props.schema.title??'数据编辑'"
            :bodyStyle="{height:size.height}"
            :centered="props.schema.centered"
    >
      <ObjectEditor v-model:schema="editorSchema"></ObjectEditor>
    </AModal>
  </AConfigProvider>
</template>
