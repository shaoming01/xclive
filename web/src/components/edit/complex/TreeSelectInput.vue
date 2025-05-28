<script setup lang="ts">
import {computed} from 'vue'
import {IApiCall, ValueUpdateEmits} from "@/types/schema";
import {pageApi} from "@/api/pageApi";
import {IValueDisplay} from "@/types/dto";

const props = defineProps<{
  value: string | undefined,
  allowClear: boolean | undefined,
  disabled: boolean | undefined,
  placeholder: string | undefined,
  dataSourceApi?: IApiCall,
  dataSource?: IValueDisplay[]
  multiple?: boolean,
}>()
const value = computed({
  get() {
    if (!props.value) return localVal.value;
    if (props.multiple) {
      if (props.value && props.value.length > 0) return props.value.split(',');
      return [];
    }
    return props.value;
  },
  set(val: string | string[] | undefined) {
    localVal.value = val;
    if (Array.isArray(val)) {
      const updateVal = val.join(',');
      emit("update:value", updateVal);
    } else
      emit("update:value", val);
  }
});
const localVal = ref<string | string[] | undefined>();
const emit = defineEmits<ValueUpdateEmits>();
const options = ref<IValueDisplay[]>([]);


async function iniDataSource() {
  if (props.dataSourceApi?.apiUrl) {
    const apiRe = await pageApi.exeApiCall(props.dataSourceApi);
    if (!apiRe.success) {
      console.error('初始化选项出错：' + apiRe.message);
      return;
    }
    options.value = apiRe.data ?? [];

  } else if (props.dataSource?.length) {
    options.value = props.dataSource;
  } else {
    console.error('dataSourceType和datasource不能都为空');

  }
  console.info('初始化选项：' + options.value);
}

watch(() => props.dataSourceApi, () => {
  iniDataSource();
})
watch(() => props.dataSource, () => {
  iniDataSource();
}, {immediate: true})


</script>

<template>
  <ATreeSelect
      :allowClear="props.allowClear"
      :disabled="props.disabled"
      show-search
      blockNode
      size="small"
      :multiple="props.multiple"
      v-model:value="value"
      :placeholder="props.placeholder"
      :treeData="options"
      tree-node-filter-prop="name"
      :showLine="true"
      :field-names="{
        children: 'children',
        value: 'value',
        label: 'label',
      }"
      style="width: 100%;">
  </ATreeSelect>
</template>
