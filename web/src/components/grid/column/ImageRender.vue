<script setup lang="ts">

const props = defineProps<{
  params: any,
}>();
//尺寸不用设置，直接由外框的列宽和行高决定
</script>

<template>
  <AImage :src="props.params.value"></AImage>
</template>
