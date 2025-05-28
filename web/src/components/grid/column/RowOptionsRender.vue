<script setup lang="ts">

const props = defineProps<{
  params: any,
}>();

const buttons = computed(() => {
  return props.params.buttons as IButtonInfo[] | undefined
});

function clickBtn(btn: IButtonInfo) {
  btn.action(props.params.data);
}

interface IButtonInfo {
  name: string;
  type: 'primary' | 'ghost' | 'dashed' | 'link' | 'text' | 'default';
  action: (data: any) => void;
}
</script>

<template>
  <ARow :wrap="false" :gutter="5">
    <ACol v-for="btn in buttons">
      <AButton @click="()=>clickBtn(btn)" :type="btn.type??'normal'">{{ btn.name }}</AButton>
    </ACol>
  </ARow>
</template>
