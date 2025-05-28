<script setup lang="ts">
import {ref} from 'vue';
import {pageApi} from "@/api/pageApi";
import {IQueryParam, ISearchGroup} from "@/types/dto";
import {
  IModalObjectEditSchema,
  IObjectEditSchema,
  ISearchGroupComponentSchema,
  SchemaUpdateEmits
} from "@/types/schema";
import {R} from "@/utils/R";
import {modalUtil} from "@/utils/modalUtil";
import Sortable from 'sortablejs';

const activeKey = ref<string>('');
const me = getCurrentInstance();
let sortable: any = undefined;

const groups = ref<Array<ISearchGroup> | undefined>();//所有分组列表
const loadingIndex = ref<number>(-1);//正在刷新的分组
const fullGroups = computed(() => {
  const tGroup = [...(groups.value ?? []) as ISearchGroup[]];
  if (tGroup && tGroup.length > 0)
    tGroup.unshift({id: '', name: '全部', count: undefined, index: 0});
  return tGroup.sort((a, b) => a.index - b.index);
})

const emit = defineEmits<SchemaUpdateEmits>();
const props = defineProps<{
  schema: ISearchGroupComponentSchema,
}>();
watch(() => activeKey.value, changeGroup);

async function changeGroup() {
  let conditions = {};
  if (activeKey.value == '') {
    conditions = props.schema.defaultConditions ?? {}
  } else {
    const re = await pageApi.getGroupConditions(activeKey.value);
    if (!re.success) {
      return msg.error(re.message);
    }
    conditions = re.data ?? {};
  }

  props.schema.selectedGroupConditions = conditions;
  emit('update:schema', props.schema);
}

props.schema.doSave = doSave;

async function doSave(conditions: Record<string, any>) {
  const saveAct = async (data: Record<string, any> | undefined): Promise<R> => {
    if (!data || !data['value1'])
      return R.error('返回数据为空');
    const newGroup: ISearchGroup = {
      conditions: conditions,
      name: data['value1'] as string,
      id: '0',
      index: Math.floor(Date.now() / 1000),
      path: getPath()
    };
    const re = await pageApi.saveUserSearchGroup(newGroup);
    if (!re.success) {
      return R.error(re.message);
    }
    groups.value?.push(re.data as ISearchGroup);
    return R.ok();
  }
  const schema: IObjectEditSchema = {
    fields: [
      {
        field: 'value1',
        label: '分组名称',
        module: {comPath: '/src/components/edit/simple/StringInput.vue',},
        fieldType: 'string',
        groupName: '',
        span: 24,
        labelColSpan: 4,
        wrapperColSpan: 20,
        require: true,
      }
    ], data: {value1: ''}
  }
  const modalSchema: IModalObjectEditSchema = {
    objectEditSchema: schema,
    save: saveAct,
    title: '创建新的查询分组',
    sizeMode: 1,
  };
  await modalUtil.showModalEditor(modalSchema);
}

async function loadGroups() {
  let re = await pageApi.getPageSearchGroups(getPath());
  if (!re.success) {
    msg.error(re.message);
    return;
  }
  groups.value = re.data ?? [] as ISearchGroup[];
}

console.log('SearchGroupPath:', getPath())

function getPath() {
  let url = window.location.href;
  const appendVal = 'qcUrl=' + props.schema.queryCountUrl;
  const index = url.indexOf('?');
  if (index >= 0) {
    url = insertAt(url, appendVal + '&', index + 1)

  } else {
    url += '?' + appendVal;
  }
  return url;
}

function insertAt(str: string, insertStr: string, position: number): string {
  return str.slice(0, position) + insertStr + str.slice(position);
}

const showDeleteBtn = ref(false);

function switchShowDeleteBtn() {
  showDeleteBtn.value = !showDeleteBtn.value;
  if (showDeleteBtn.value) {
    iniDrag();
    msg.success('已开启修改，请通过分组名称右边按钮操作修改，如移动位置、改名、删除等')

  }
}

function iniDrag() {
  if (sortable != undefined) return;
  const ops1 = {
    animation: 300,
    handle: '.drag-handle',
    onMove: (e: any, originalEvent: any) => {
      const index = getElementIndex(e.related as HTMLElement);
      return index > 0;
    },
    onEnd: exchangeIndex,
  };
  const el = me?.proxy?.$el.getElementsByClassName('ant-tabs-nav-list')[0];
  sortable = Sortable.create(el, ops1);
}

async function exchangeIndex(evt: any) {
  evt.returnValue = false;
  const oldIndex = evt.oldIndex as number;
  const newIndex = evt.newIndex as number;
  if (!oldIndex || !newIndex || oldIndex == newIndex) return;
  const oldGroup = fullGroups.value[oldIndex];
  const newGroup = fullGroups.value[newIndex];
  const tmpIndex = oldGroup.index;
  oldGroup.index = newGroup.index;
  newGroup.index = tmpIndex;
  const updateRe1 = await pageApi.saveUserSearchGroup(oldGroup as ISearchGroup);
  if (!updateRe1.success) {
    return msg.error(updateRe1.message)
  }
  const updateRe2 = await pageApi.saveUserSearchGroup(newGroup as ISearchGroup);
  if (!updateRe2.success) {
    return msg.error(updateRe2.message)
  }
  msg.success('更新成功');

}

async function refreshCount() {
  if (!groups.value) return;
  for (const [index, group] of fullGroups.value.entries()) {
    loadingIndex.value = index;
    let condition: Record<string, any> | undefined = {};
    if (group.id) {
      const re = await pageApi.getGroupConditions(group.id);
      if (!re.success) {
        return msg.error(re.message);
      }
      condition = re.data;
    } else {
      condition = props.schema.defaultConditions ?? {}
    }

    const par: IQueryParam = {page: 1, pageSize: 1, queryObject: condition};
    const countRe = await pageApi.queryCount(props.schema.queryCountUrl, par);
    if (!countRe.success) {
      return msg.error(countRe.message);
    }
    group.count = countRe.data;
  }
  loadingIndex.value = -1;

}

async function doRefreshGroup() {
  showDeleteBtn.value = false;
  return await refreshCount();
}

async function delGroup(id: string) {
  if (!await msg.confirm('确认要删除该分组？')) {
    return;
  }
  const re = await pageApi.deleteSearchGroup(id);
  if (!re.success) {
    return msg.error('删除分组出错:' + re.message)
  }
  if (!groups.value) return;

  const index = groups.value?.findIndex(item => item.id === id);
  if (index > -1) {
    groups.value?.splice(index, 1);
  }
}

async function editGroup(id: string) {
  const group = groups.value?.find(g => g.id == id) as ISearchGroup;
  if (!group) return msg.error('无效分组，请刷新后重试');
  const saveAct = async (data: Record<string, any> | undefined): Promise<R> => {
    if (!data)
      return R.error('返回数据为空');
    const re = await pageApi.saveUserSearchGroup(data as ISearchGroup);
    if (!re.success) {
      return R.error(re.message);
    }
    group.name = (re.data as ISearchGroup).name;
    groups.value?.push();
    return R.ok();
  }
  const schema: IObjectEditSchema = {
    fields: [
      {
        field: 'name',
        label: '分组名称',
        module: {comPath: '/src/components/edit/simple/StringInput.vue',},
        fieldType: 'string',
        groupName: '',
        span: 24,
        labelColSpan: 4,
        wrapperColSpan: 20,
        require: true,
      }
    ], data: ext.deepCopy(group)
  }
  const modalSchema: IModalObjectEditSchema = {
    objectEditSchema: schema,
    save: saveAct,
    title: '创建新的查询分组',
    sizeMode: 1,
  };
  return await modalUtil.showModalEditor(modalSchema);

}

function getElementIndex(element: HTMLElement): number {
  const parent = element.parentElement;
  if (!parent) return -1; // 如果没有父元素，返回 -1
  return Array.prototype.indexOf.call(parent.children, element);
}


onMounted(async () => {
  await loadGroups();
  await refreshCount();
})
</script>

<template>
  <ATabs v-model:activeKey="activeKey" size="small" v-if="fullGroups&&fullGroups.length>0">
    <ATabPane v-for="(item,index) in fullGroups" :key="item.id" :data-id="item.id">
      <template #tab>
        {{ item.name }}
        <span>(
          <span :style="item.count??0>0?'color: red':''" v-show="item.count!==undefined" v-if="loadingIndex!=index">
            {{ item.count }}
          </span>
<LoadingOutlined style="margin-left: 10px;" v-if="loadingIndex==index"/>
          )</span>
        <span style="margin-left: 10px;">
         <ATooltip title="移动位置">
          <DragOutlined class="drag-handle" v-if="showDeleteBtn&&item.id" style="cursor: move;"/>
        </ATooltip>
        <ATooltip title="修改名称">
          <EditOutlined v-if="showDeleteBtn&&item.id" @click="()=>editGroup(item.id)"/>
        </ATooltip>
        <ATooltip title="删除">
          <DeleteOutlined v-if="showDeleteBtn&&item.id" @click="()=>delGroup(item.id)"/>
        </ATooltip>
        </span>

      </template>
    </ATabPane>
    <!--suppress VueUnrecognizedSlot -->
    <template #rightExtra>
      <ASpace>
        <LoadingBtn type="default" size="small" :click="doRefreshGroup">
          <template #icon>
            <RetweetOutlined/>
          </template>
          刷新分组
        </LoadingBtn>
        <AButton type="default" size="small" @click="switchShowDeleteBtn">
          <template #icon>
            <EditOutlined/>
          </template>
          修改分组
        </AButton>


      </ASpace>

    </template>
  </ATabs>

</template>


<style scoped lang="scss">
:deep(.ant-tabs-nav) {
  margin-bottom: 0;
}

.drag-handle {
  cursor: grab;
}
</style>