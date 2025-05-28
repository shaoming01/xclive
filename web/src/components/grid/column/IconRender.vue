<script setup lang="ts">
import {IconFont} from "@/composables/useFontIcon";

const props = defineProps<{
  params: any,
}>();
</script>

<template>
  <span style="font-size: 14px;">
    <IconFont :type="props.params.value"></IconFont>{{ props.params.value }}
  </span>
</template>
