<script setup lang="ts">

import {IDateQueryValue, ObjectValueUpdateEmits} from "@/types/schema";
import dayjs, {Dayjs} from "dayjs";
import {DateSelectType, editFiledHelper} from "@/utils/editFiledHelper";

const props = defineProps<{
  value?: IDateQueryValue,
  showTime?: boolean,
}>()
const emit = defineEmits<ObjectValueUpdateEmits>();

const selectList = ref(Object.keys(DateSelectType).filter(key => isNaN(Number(key))));
const selValue = ref('');
const dtStart = ref<Dayjs>();
const dtEnd = ref<Dayjs>();
const displayValue = ref('');
const visible = ref();
const fmt1 = 'YYYY-MM-DD';
const fmt2 = 'YYYY-MM-DD HH:mm:ss'

function applyNewValue() {
  const se = editFiledHelper.getStartEndByComplexValue(props.value?.complexValue);
  dtStart.value = se.start;
  dtEnd.value = se.end;
  selValue.value = calcSelectValue(dtStart.value, dtEnd.value);
  updateDisplayString();
}

function calcSelectValue(start: dayjs.Dayjs | undefined, end: dayjs.Dayjs | undefined): string {
  const startEndStr = getDisplayString(start, end);
  const map = getSelectMap();

  for (const [key, value] of map) {
    if (value === startEndStr) {
      return key;
    }
  }

  return ''
}

function changeSelect() {
  const startEnd = editFiledHelper.getStartEndBySelect(selValue.value);
  dtStart.value = startEnd.start;
  dtEnd.value = startEnd.end;
  visible.value = false;
  dateInputChanged();
}


function getSelectMap(): Map<string, string> {
  const map = new Map<string, string>;
  Object.keys(DateSelectType).filter(key => isNaN(Number(key))).forEach(key => {
    const startEnd = editFiledHelper.getStartEndBySelect(key);
    map.set(key, getDisplayString(startEnd.start, startEnd.end));

  })
  return map;
}


function dateInputChanged() {
  const complexVal = calcSelectValue(dtStart.value, dtEnd.value) || getDisplayString(dtStart.value, dtEnd.value);
  const val: IDateQueryValue = {
    complexValue: complexVal,
  }
  emit('update:value', val);
  selValue.value = calcSelectValue(dtStart.value, dtEnd.value);
}

function getDisplayString(start: dayjs.Dayjs | undefined, end: dayjs.Dayjs | undefined): string {
  if (!start && !end) return ''
  const fmt = props.showTime ? fmt2 : fmt1;
  let str = (start?.format(fmt) ?? '') + '~' + (end?.format(fmt) ?? '');
  str = str.replace(' 00:00:00', '').replace(' 00:00:00', '')
  return str;
}

function updateDisplayString() {
  if (selValue.value) {
    displayValue.value = selValue.value;
  } else {
    displayValue.value = getDisplayString(dtStart.value, dtEnd.value);
  }

}

function handleInput(event: Event) {
  if (event instanceof PointerEvent) {
    clearValue();
    dateInputChanged();
    return;
  }
  console.log('handleInput', event);
  // 不更新输入值，阻止输入
  event.preventDefault();
}

function clearValue() {
  visible.value = false;
  displayValue.value = ''
  selValue.value = '';
  dtStart.value = undefined;
  dtEnd.value = undefined;
}

watch(() => props.value, applyNewValue, {immediate: true, deep: true,});

watch(() => dtStart.value, updateDisplayString);
watch(() => dtEnd.value, updateDisplayString);
watch(() => selValue.value, updateDisplayString);
</script>

<template>
  <APopover title="" trigger="click" v-model:open="visible">
    <template #content>
      <div style="width: 530px;margin: 20px;">
        <a-radio-group v-model:value="selValue" @change="changeSelect">
          <ARow :gutter="[5,5]">
            <ACol :span="8" v-for="sel in selectList">
              <a-radio :value="sel">{{ sel }}</a-radio>
            </ACol>
          </ARow>
        </a-radio-group>
        <ARow style="margin-top: 10px;" align="middle">
          <ACol :span="4" style="text-align: right">
            开始日期：
          </ACol>
          <ACol :span="8">
            <ADatePicker :showTime="props.showTime" v-model:value="dtStart" @change="dateInputChanged"></ADatePicker>
          </ACol>
          <ACol :span="4" style="text-align: right">
            结束日期：
          </ACol>
          <ACol :span="8">
            <ADatePicker :showTime="props.showTime" v-model:value="dtEnd" @change="dateInputChanged"></ADatePicker>
          </ACol>
        </ARow>
      </div>

    </template>
    <AInput :value="displayValue" allow-clear @input="handleInput"></AInput>
  </APopover>
</template>
