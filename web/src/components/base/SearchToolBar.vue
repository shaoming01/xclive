<script setup lang="ts">
const props = defineProps<{
  doQuery: () => Promise<void>,
  doReset: () => Promise<void>,
  doSave: () => Promise<void>,
}>()
</script>

<template>
  <ASpace>
    <LoadingBtn type="primary" size="middle" :click="props.doQuery">
      <template #icon>
        <SearchOutlined/>
      </template>
      查询
    </LoadingBtn>
    <LoadingBtn type="default" size="middle" :click="props.doReset">
      <template #icon>
        <RedoOutlined/>
      </template>
      清空
    </LoadingBtn>
    <LoadingBtn type="default" size="middle" :click="props.doSave">
      <template #icon>
        <SaveOutlined/>
      </template>
      保存
    </LoadingBtn>
  </ASpace>
</template>
