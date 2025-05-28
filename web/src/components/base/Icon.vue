<template>
  <IconFont :type="props.name??''"></IconFont>
</template>

<script lang="ts" setup>
import {IconFont} from "@/composables/useFontIcon";

const props = defineProps<{
  /**
   * @see {IconSelectInput}
   */
  name?: string | undefined
}>();

</script>

