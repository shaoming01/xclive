<script setup lang="ts">
import {IToolBarSchema} from "@/types/schema";

const props = withDefaults(defineProps<{
  schema: IToolBarSchema
}>(), {})

</script>

<template>
  <ARow :gutter="[5,5]" style="margin:5px;" v-if="props.schema?.buttons">
    <a-col v-for="btn in props.schema.buttons">
      <AButton @click="btn.action" :icon="getIcon(btn.icon)">{{ btn.name }}</AButton>
    </a-col>
  </ARow>

</template>

<style scoped>

</style>