<script setup lang="ts">
import {ref} from "vue";
import {FormInstance} from "ant-design-vue";
import _ from 'lodash';

const formData = ref<Record<string, any>>({a: {b: 'valueText', c: true}});
const formRef = ref<FormInstance>();

function validate() {
  {
    formRef.value?.validate().then(() => {
    }).catch((err) => {
      const msg1 = JSON.stringify(err);
      msg.error(msg1);
    });
  }
}

function showVal() {
  msg.error(JSON.stringify(formData.value));
}


const field1 = {field: 'a.b'};

function getNamePath(input: string) {
  return input ? input.split('.') : '';
}

const obj = {};
_.set(obj, "a.b", 'a.b')
_.set(obj, "a.c", 'a.c')
console.log(_.split('asb', '.'))

msg.error(JSON.stringify(obj));

</script>

<template>
  <AButton @click="validate">验证</AButton>
  <AButton @click="showVal">显示值</AButton>
  <AForm ref="formRef" :model="formData" style="height: 100%;overflow-y: auto;padding: 3px;">
    <ARow>
      <ACol>
        <AFormItem label="字段1"
                   :name="getNamePath(field1.field)"
                   :rules="[{required: true}]"
                   tooltip="asdfasdfsdf"
        >
          <AInput
              :value="_.get(formData,field1.field)"
              @update:value="(newVal)=>{
                _.set(formData,field1.field,newVal);
              }"
          ></AInput>
        </AFormItem>

      </ACol>
    </ARow>
  </AForm>
</template>

<style scoped lang="scss">

</style>