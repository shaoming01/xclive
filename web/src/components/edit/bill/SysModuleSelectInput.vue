<script setup lang="ts">
import {IApiCall} from "@/types/schema";

const props = defineProps<{
  value?: string | undefined,
}>()
const dataSourceApi: IApiCall = {
  apiUrl: '/api/sys/ListValueDisplay',
  cacheable: true,
  postParams: {type: 2}
}
</script>

<template>
  <DataSelectInput
      v-model:value="props.value"
      :data-source-api="dataSourceApi"
  >
  </DataSelectInput>
</template>
