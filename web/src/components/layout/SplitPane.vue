<script setup lang="ts">
import {Pane} from 'splitpanes'

const props = defineProps<{
  minSize?: number,
  maxSize?: number,
  size?: number,
}>()
</script>

<template>
  <Pane style="background-color: white" v-bind="props">
    <slot></slot>
  </Pane>
</template>
