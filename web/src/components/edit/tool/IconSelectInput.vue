<script setup lang="ts">
import {computed} from 'vue'
import {ValueUpdateEmits} from "@/types/schema";
import {getAllIcon} from "@/composables/useFontIcon";

const props = defineProps<{
  value: string | undefined,
}>()
const localValue = ref('');
const value = computed({
  get() {
    return props.value ?? localValue.value;
  },
  set(val: string | undefined) {
    emit("update:value", val);
    localValue.value = val ?? '';
  }
});

const emit = defineEmits<ValueUpdateEmits>();
const visible = ref();

function selectIcon(iconName: string) {
  value.value = iconName;
  visible.value = false;
}

const icons = computed(() => {
  const vals = getAllIcon();
  vals.unshift('');
  return vals;
})
</script>

<template>

  <APopover v-model:open="visible">
    <template #content>
      <div style="height: 400px;width: 500px; font-size: 18px;">
        <ARow :gutter="[10,10]" align="middle"
              style="text-align: center;height: 100%;width:100%;overflow-y: scroll;overflow-x: hidden"
              :wrap="true">

          <ACol :span="6" @click="()=>selectIcon(iconName)" v-for="iconName in icons"
                :style="iconName==props.value?'border: #1890ff 1px solid;':''">
            <Icon :type="iconName" style="font-size: 24px;"></Icon>
            <br/>
            {{ iconName || '无图标' }}
          </ACol>
        </ARow>
      </div>
    </template>

    <AInput v-model:value="value" readonly @click="">
      <template #prefix>
        <Icon :type="value??''" v-if="value"></Icon>
      </template>
    </AInput>
  </APopover>
</template>
