<script setup lang="ts">
import {showUpdateEmits} from "@/types/schema";

const props = defineProps<{
  userName?: string,
  show?: boolean,
}>();
const userName = ref(props.userName);
const password = ref();
const password2 = ref();
const cardKey = ref();

async function submit() {
  if (!userName.value) {
    return msg.error('请输入用户名');
  }
  if (!password.value) {
    return msg.error('请输入密码');
  }
  if (password.value != password2.value) {
    return msg.error('两次密码输入不同，请重新输入密码和重复密码');
  }
  if (!cardKey.value) {
    return msg.error('两次密码输入不同，请重新输入密码和重复密码');
  }
  const re = await apiHelper.request('/api/user/Register', undefined, {
    username: userName.value,
    password: password.value,
    cardKey: cardKey.value
  });
  if (!re.success) {
    return msg.error(re.message);
  }
  emit('update:show', false);
  return msg.success('注册成功');
}

async function cancel() {
  emit('update:show', false);
}

const size = {
  width: '500px',
  height: '300px',
}
const emit = defineEmits<showUpdateEmits>();

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
            <AInput size="large" v-model:value="userName" placeholder="用户名，建议直接使用手机号"></AInput>
          </ACol>
          <ACol span="24">
            <AInput size="large" v-model:value="password" placeholder="请输入密码"></AInput>
          </ACol>
          <ACol span="24">
            <AInput size="large" v-model:value="password2" placeholder="请重新输入重复密码"></AInput>
          </ACol>
          <ACol span="24">
            <AInput size="large" v-model:value="cardKey" placeholder="请输入卡密"></AInput>
          </ACol>
        </ARow>
      </FlexLayoutContent>
      <FlexLayoutFooter>
        <ARow :gutter="[10,10]">
          <ACol span="12">
            <AButton style="width: 100%" size="large" type="primary" @click="submit">注册</AButton>
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
