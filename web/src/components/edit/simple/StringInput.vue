<script setup lang="ts">
import {computed} from 'vue'
import {ValueUpdateEmits} from "@/types/schema";

const props = defineProps<{
  value: string,
  password?: boolean | undefined,
  rows?: number | undefined,
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
const mode = computed(() => {
  if (props.password) return 3;
  if (props?.rows && props.rows > 1) return 2;
  return 1;
})
</script>

<template>
  <AInput v-bind="{...$attrs,...props}" v-model:value="value" v-if="mode==1"/>
  <ATextarea v-bind="{...$attrs,...props}" v-model:value="value" v-if="mode==2" :rows="props.rows" :show-count="true"/>
  <AInputPassword v-bind="{...$attrs,...props}" v-model:value="value" v-if="mode==3"/>
</template>
