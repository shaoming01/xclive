<script setup lang="ts">
import {computed} from 'vue'
import {ValueUpdateEmits} from "@/types/schema";

const props = defineProps<{
  value: string,
  min?: number,
  max?: number,
  /**
   * 键盘行为
   */
  keyboard?: boolean,
  disabled?: boolean,
  /**
   * 增减按钮
   */
  controls?: boolean,
  /**
   * 小数位数
   * @description 0无小数
   */
  precision?: number,
  /**
   * 前辍图标
   * @see {IconSelectInput}
   */
  prefixIcon?: string,
}>()
const value = computed({
  get() {
    return props.value;
  },
  set(val: string | undefined) {
    emit("update:value", val);
  }
});
const emit = defineEmits<ValueUpdateEmits>();
</script>

<template>
  <AInputNumber v-bind="{...$attrs,...props}" v-model:value="value" >
    <template #prefix>
      <Icon :type="props.prefixIcon??''"></Icon>
    </template>
  </AInputNumber>
</template>
