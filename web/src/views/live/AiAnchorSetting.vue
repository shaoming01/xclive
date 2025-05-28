<script setup lang="ts">

import {IDataBrowserSchema} from "@/types/schema";
import {ref} from "vue";
import {pageApi} from "@/api/pageApi";
import DataBrowser from "@/components/base/DataBrowser.vue";


const schema = ref<IDataBrowserSchema>();

async function ini() {
  const re = await pageApi.getSysModuleSchema('LiveAiAnchorVm');
  if (!re.success) return msg.error(re.message);
  schema.value = re.data?.schema as IDataBrowserSchema;
}

onMounted(async () => {
  await ini();
})
</script>

<template>
  <FlexLayout v-if="schema">
    <FlexLayoutHeader>
      <H3 style="margin-top: 15px;margin-left: 20px;">AI主播设置</H3>
      <ADivider style="margin-top: 15px; margin-bottom: 5px;"></ADivider>
    </FlexLayoutHeader>
    <FlexLayoutContent>
      <DataBrowser v-if="schema" v-model:schema="schema"></DataBrowser>
    </FlexLayoutContent>
  </FlexLayout>
</template>

<style scoped lang="scss">


</style>
