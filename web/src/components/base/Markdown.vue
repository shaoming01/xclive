<template>
  <MdPreview v-model="props.value"/>
</template>

<script lang="ts" setup>
import {MdPreview} from 'md-editor-v3';
import 'md-editor-v3/lib/preview.css';

const props = defineProps<{
  value?: string | undefined
}>();

</script>

