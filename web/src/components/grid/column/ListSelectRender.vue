<script setup lang="ts">
import {IValueDisplay} from "@/types/dto";
import {pageApi} from "@/api/pageApi";

const props = defineProps<{
  params: any,
}>();
const editVal = ref(getVal());
const showEdit = ref(false);
const openList = ref(false);
const options = ref<IValueDisplay[]>([]);

function getVal() {
  return props.params.data[props.params.colDef.field];
}

function change() {
  applyNewVal(editVal.value);
  props.params.refreshCell();
  showEdit.value = false;
}

function applyNewVal(val: string | undefined) {
  props.params.data[props.params.colDef.field] = val;
}

watch(() => showEdit.value, () => {
  openList.value = showEdit.value;

  if (showEdit.value) {
    iniOptions();
  }
})

async function iniOptions() {
  if (options.value.length > 0) {
    return;
  }
  if (!props.params.dataSourceApi) {
    return;
  }
  const apiRe = await pageApi.exeApiCall(props.params.dataSourceApi);
  if (!apiRe.success) {
    console.error('初始化选项出错：' + apiRe.message);
    return;
  }
  options.value = apiRe.data ?? [];
}

</script>

<template>
  <div class="listSelectRender">
    {{ props.params.value }}

    <ADropdown :trigger="['click']" v-model:open="showEdit" class="edit-com">
      <a @click.prevent>
        <EditOutlined/>
      </a>
      <template #overlay>
        <ASelect :autofocus="true"
                 :allowClear="true"
                 show-search
                 :options="options"
                 optionFilterProp="label"
                 @change="change"
                 v-model:value="editVal"
                 v-model:open="openList"
                 :field-names="{ value: 'value',label: 'label'}"
                 :dropdownMenuStyle="{width:'300px'}"
                 :dropdownMatchSelectWidth="200"></ASelect>
      </template>
    </ADropdown>
  </div>
</template>
<style scoped lang="scss">
.edit-com {
  transition: opacity 0.3s;
  opacity: 0; /* 默认隐藏 */
}

.listSelectRender:hover .edit-com {
  opacity: 1; /* 鼠标悬停时显示 */
}
</style>