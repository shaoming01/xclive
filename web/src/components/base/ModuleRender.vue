<template>
  <component v-if="module&&comIs" :is="comIs" v-bind="{...$attrs,...module.props}"></component>
  <span v-if="errMsg">{{ errMsg }}</span>
</template>

<script lang="ts" setup>

import {IModuleVm} from "@/types/schema";
import {pageApi} from "@/api/pageApi";
import {comUtil} from "@/utils/com";


const module = ref<IModuleVm>();
const errMsg = ref();
const props = defineProps<{
  moduleId?: string,
  sysModuleId?: string,
  module?: IModuleVm,
}>();


async function ini() {
  module.value = props.module;
  console.log('初始Module', JSON.stringify(module.value));
  if (props.moduleId || props.sysModuleId) {
    const re = await pageApi.getFullModule(props.moduleId, props.sysModuleId);
    if (!re.success) {
      errMsg.value = '加载模块元数据失败:' + re.message
      return msg.error(errMsg.value);
    }
    module.value = re.data as IModuleVm;
  }

  if (!module.value || !module.value.comPath) {
    errMsg.value = '模块内容为空，无法初始化'
    return;
  }

}

const comIs = computed(() => {
  if (!(module?.value?.comPath)) {
    errMsg.value = '组件路径为空'
    return undefined;
  }
  return comUtil.getCom(module?.value?.comPath)
})

ini();
watch(() => props.moduleId, () => {
  ini();
})
watch(() => props.module?.comPath, () => {
  ini();
})
watch(() => props.module?.props, () => {
  ini();
})
</script>

