<template>
  <ObjectEditor v-if="moduleSchema" v-model:schema="moduleSchema"></ObjectEditor>
  <ObjectEditor ref="propEdtRef" v-if="modulePropEditSchema" v-model:schema="modulePropEditSchema"></ObjectEditor>
</template>

<script lang="ts" setup>
import {computed, ref} from 'vue'
import {IModuleVm, IObjectEditSchema, ObjectValueUpdateEmits} from "@/types/schema";
import ObjectEditor from "@/components/base/ObjectEditor.vue";

const emit = defineEmits<ObjectValueUpdateEmits>();

const props = defineProps<{
  value: IModuleVm,
}>()
const localVal = ref<IModuleVm>({});
const value = computed({
  get() {
    return props.value ?? localVal.value;
  },
  set(val: IModuleVm | undefined) {
    localVal.value = val ?? {};
    emit("update:value", val);
  }
});

const propEdtRef = ref<InstanceType<typeof ObjectEditor>>();

const moduleSchema = ref<IObjectEditSchema>();
const modulePropEditSchema = ref<IObjectEditSchema>();
const builder = useSchemaBuilder();

function applyValue() {
  localVal.value = props.value;
  moduleSchema.value = builder.createTypeSchema('IModuleVm') ?? {};
  moduleSchema.value.fields = moduleSchema.value.fields?.filter(f => f.field == 'comPath')
  moduleSchema.value.fields?.forEach((field) => {
    field.span = 24;
    field.labelColSpan = 4;
    field.wrapperColSpan = 20;
  })
  moduleSchema.value.data = value.value ?? {};
  if (value.value?.comPath) {
    const propVal = builder.createVueSchema(value.value.comPath) ?? {};
    propVal.data = props.value?.props;
    modulePropEditSchema.value = propVal;
  } else {
    modulePropEditSchema.value = {}
  }
}
applyValue();

watch(() => moduleSchema.value?.data?.comPath, () => {
  if (!moduleSchema?.value?.data)
    return;

  //属性编辑器
  const propVal = builder.createVueSchema(moduleSchema.value.data?.comPath) ?? {};
  propVal.data = props.value?.props;
  modulePropEditSchema.value = propVal
  value.value = {...value.value, comPath: moduleSchema.value.data?.comPath};
})

watch(() => modulePropEditSchema.value?.data, () => {
  value.value = {...value.value, props: modulePropEditSchema.value?.options?.getData()};
})

</script>

<style scoped lang="scss">
</style>