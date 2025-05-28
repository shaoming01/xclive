<script setup lang="ts">

import {IQueryBetweenValue, ObjectValueUpdateEmits} from "@/types/schema";

const props = defineProps<{
  value?: IQueryBetweenValue,
  /**
   * 小数位数
   * @description 0无小数
   */
  precision?: number,
}>()
const localValue = ref<IQueryBetweenValue>({});
const emit = defineEmits<ObjectValueUpdateEmits>();

function valueChanged() {
  emit("update:value", localValue.value);
}

watch(() => props.value, () => {
  localValue.value = props.value ?? {};
}, {immediate: true, deep: true})
</script>

<template>
  <AInputNumber v-bind="props" decimalSeparator="" v-model:value="localValue.start"
                @change="valueChanged"></AInputNumber>
  ~
  <AInputNumber v-bind="props" decimalSeparator="" v-model:value="localValue.end" @change="valueChanged"></AInputNumber>
</template>
