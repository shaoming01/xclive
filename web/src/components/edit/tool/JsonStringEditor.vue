<script setup lang="ts">
import {computed} from 'vue'
import {ValueUpdateEmits} from "@/types/schema";
import {createJSONEditor, MenuItem, Mode} from "vanilla-jsoneditor";

const props = defineProps<{
  value: string,
}>()
const value = computed({
  get() {
    return props.value;
  },
  set(val: string | undefined) {
    emit("update:value", val);
  }
});
const emit = defineEmits<ValueUpdateEmits>();
const jsonEditor = ref();


const init = () => {
  createJSONEditor({
    target: jsonEditor.value as Element,
    props: {
      content: {
        text: value.value,
      },
      readOnly: false,
      mainMenuBar: true,
      statusBar: true,
      onRenderMenu: function (items: MenuItem[]) {
        return [items[0], items[3], items[4], items[5]];
      },
      mode: Mode.text,
      onChange: (updatedContent: any, _previousContent: any, {contentErrors}: { contentErrors: any }) => {
        if (!contentErrors) {
          value.value = updatedContent.text;
        }
      },
    }
  })

}
onMounted(() => {
  init();
})

</script>

<template>
  <div ref="jsonEditor" style="width: 100%;height: 100%"></div>
</template>
