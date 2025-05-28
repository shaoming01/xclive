<script setup lang="ts">
import SchemaRender from "@/components/base/SchemaRender.vue";

const route = useRoute();
const schemaId = ref(route.params.schemaId as string);

console.log('开始渲染schemaId:', schemaId.value);
</script>

<template>
  <SchemaRender :schema-id="schemaId"></SchemaRender>
</template>
