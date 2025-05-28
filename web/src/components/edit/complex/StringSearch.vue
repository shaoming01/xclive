<script setup lang="ts">
import {IQueryStringValue, ObjectValueUpdateEmits} from "@/types/schema";

const props = defineProps<{
  value?: IQueryStringValue,
}>()
const localValue = ref<IQueryStringValue>({});
const emit = defineEmits<ObjectValueUpdateEmits>();

function valueChanged() {
  emit("update:value", localValue.value);
}

watch(() => props.value, () => {
  localValue.value = props.value ?? {type: 3};
}, {immediate: true, deep: true})

</script>

<template>
  <a-input v-model:value="localValue.value" :disabled="localValue?.type == 7 || localValue?.type == 8"
           @change="valueChanged">
    <template #addonBefore>
      <a-select v-model:value="localValue.type" :dropdownMatchSelectWidth="false" @change="valueChanged">
        <a-select-option :value="1">等于</a-select-option>
        <a-select-option :value="2">不等于</a-select-option>
        <a-select-option :value="3">包含</a-select-option>
        <a-select-option :value="5">不包含</a-select-option>
        <a-select-option :value="7">为空</a-select-option>
        <a-select-option :value="8">不为空</a-select-option>
      </a-select>
    </template>

  </a-input>
</template>
