<script setup lang="ts">
const props = defineProps<{
  params: any,
}>();

/**
 * 空编辑器，用于填充编辑，有些Render带了编辑功能，所以不需要再提供编辑器
 */
function getValue() {
  return props.params.value;
}

onMounted(() => {
  //  emptyEditor.__asyncLoader();//加载后执行取消，要是放在setup中的话组件需要预加载
  props.params.stopEditing();
})
</script>
