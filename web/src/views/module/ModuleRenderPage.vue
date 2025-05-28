<script setup lang="ts">
import ModuleRender from "@/components/base/ModuleRender.vue";
import _ from "lodash";

const route = useRoute();
const parVal = route.params.moduleId as string;
const moduleId = ref('');
const sysModuleId = ref('');
if (_.toNumber(parVal) > 0) {
  moduleId.value = parVal;
} else {
  sysModuleId.value = parVal;
}

console.log('开始渲染moduleId:', moduleId.value);
</script>

<template>
  <ModuleRender :module-id="moduleId" :sys-module-id="sysModuleId"></ModuleRender>
</template>
