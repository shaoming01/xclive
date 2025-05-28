<script setup lang="ts">
import {computed} from 'vue'
import {ValueUpdateEmits} from "@/types/schema";
import dayjs, {Dayjs} from 'dayjs';

const dateFormat = 'YYYY/MM/DD';

const props = defineProps<{
  value: string,
  style?: string,
}>()
const value = computed({
  get() {
    if (!props.value) {
      return undefined;
    }
    return dayjs(props.value);
  },
  set(val: Dayjs | undefined) {
    let valStr = '';
    if (val)
      valStr = val.format(dateFormat);
    emit("update:value", valStr);
  }
});
const emit = defineEmits<ValueUpdateEmits>();
</script>

<template>
  <ADatePicker v-bind="{...$attrs,...props}" v-model:value="value">
  </ADatePicker>
</template>
