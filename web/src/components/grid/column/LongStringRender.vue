<script setup lang="ts">
const props = defineProps<{
  params: any,
}>();
</script>

<template>
  <APopover trigger="click">
    <template #content>
      <p style="">{{ props.params.value }}</p>

    </template>
    <span class="text-content">{{ props.params.value }}</span>
  </APopover>
</template>
<style scoped lang="scss">
</style>