<script setup lang="ts">
import {computed} from 'vue'
import {BoolValueUpdateEmits} from "@/types/schema";

const props = defineProps<{
  value: boolean,
  disabled?: boolean,
}>()
const value = computed({
  get() {
    return props.value;
  },
  set(val: boolean | undefined) {
    emit("update:value", val);
  }
});
const emit = defineEmits<BoolValueUpdateEmits>();
</script>

<template>
  <ACheckbox v-bind="{...$attrs,...props}" v-model:checked="value" :disabled="props.disabled"></ACheckbox>
</template>
