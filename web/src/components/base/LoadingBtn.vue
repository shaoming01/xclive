<script setup lang="ts">
import {ref, nextTick} from 'vue';

const props = defineProps<{
  click?: () => Promise<any>,
  onClick?: () => Promise<any>,
}>()
const loading = ref(false);

async function clicked() {
  try {
    loading.value = true
    await nextTick();
    // 等待外部 click 处理完成
    if (props.click) {
      await props.click();
    }
    if (props.onClick) {
      await props.onClick();
    }
  } finally {
    loading.value = false
  }

}
</script>

<template>
  <AButton :loading="loading" @click="clicked">
    <slot name="icon"></slot>
    <slot></slot>
  </AButton>
</template>

<style scoped>

</style>