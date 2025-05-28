<script setup lang="ts">
import {Splitpanes} from 'splitpanes'
import '@/styles/splitpanes.css'

const props = withDefaults(defineProps<{
  horizontal?: boolean,
  vertical?: boolean,
  firstSplitter?: boolean,
}>(), {horizontal: false, firstSplitter: false, vertical: false})
</script>

<template>
  <Splitpanes class="default-theme" v-bind="props">
    <slot></slot>
  </Splitpanes>
</template>
