<script setup lang="ts">
import {showUpdateEmits} from "@/types/schema";

const props = defineProps<{
  userName?: string,
  password?: string,
  show?: boolean,
}>();
const cardKey = ref();

async function check() {
  if (!props.userName) {
    emit('update:show', false);
    return msg.error('请输入用户名');
  }
  if (!props.password) {
    emit('update:show', false);
    return msg.error('请输入密码');
  }

}

async function submit() {
  if (!cardKey.value) {
    return msg.error('请输入卡密');
  }
  const re = await apiHelper.request('/api/user/BindCardKey', {
    username: props.userName,
    password: props.password,
    cardKey: cardKey.value
  });
  if (!re.success) {
    return msg.error(re.message);
  }
  emit('update:show', false);
  return msg.success('充值成功');
}

async function cancel() {
  emit('update:show', false);
}

const size = {
  width: '500px',
  height: '150px',
}
const emit = defineEmits<showUpdateEmits>();
watch(() => props.show, () => {
  if (props.show) {
    check()
  }
})

</script>
<template>
  <AModal :destroyOnClose="true" v-model:open="props.show"
          :width="size.width"
          title="新账号注册"
          :centered="true"
          :bodyStyle="{height:size.height}"
          :footer="null"
          @cancel="cancel"
  >

    <FlexLayout style="padding: 10px">
      <FlexLayoutContent>
        <ARow :gutter="[20,20]" style="width: 100%;">
          <ACol span="24">
            <AInput size="large" v-model:value="cardKey" placeholder="请输入卡密"></AInput>
          </ACol>
        </ARow>
      </FlexLayoutContent>
      <FlexLayoutFooter>
        <ARow :gutter="[10,10]">
          <ACol span="12">
            <AButton style="width: 100%" size="large" type="primary" @click="submit">充值</AButton>
          </ACol>
          <ACol span="12">
            <AButton style="width: 100%" size="large" @click="cancel">取消</AButton>
          </ACol>
        </ARow>
      </FlexLayoutFooter>
    </FlexLayout>
  </AModal>

</template>
<style scoped lang="scss">

</style>
