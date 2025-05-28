<script setup lang="ts">
import {
  IDyAccountAuthVm,
  IShelfTaskConfigEditVm
} from "@/views/live/help/LiveInterface";
import {DyApi, IFengcheTaskData, IHuangcheTaskData, IShelfTask, ISleepTaskData} from "@/views/live/help/DyApi";
import {IFullTableSchema} from "@/types/schema";
import {ref} from "vue";
import {modalUtil} from "@/utils/modalUtil";
import _ from "lodash";
import {CellDoubleClickedEvent, RowClassParams} from "ag-grid-community";
import {ByApi} from "@/views/live/help/ByApi";
import {accountHelper} from "@/views/live/help/accountHelper";
import {R} from "@/utils/R";

const props = defineProps<{
  value?: string,
}>();

const taskListTable: IFullTableSchema = {
  columns: [{field: 'type', headerName: '类型', width: 80},
    {field: 'title', headerName: '内容', width: 200},
    {
      field: 'option', headerName: '操作', width: 100, autoRowHeight: true, cellRender: {
        comPath: '/src/components/grid/column/RowOptionsRender.vue',
        props: {buttons: [{name: '删除', type: 'link', action: deleteTask}]}
      }
    },
  ],
  rowData: [],
  agOptions: {rowHeight: 50},
  gridOptions: {
    getRowStyle(params: RowClassParams<any, any>) {
      const style: Record<string, string> = {};
      if (params.rowIndex == currentRowIndex.value) {
        style.background = '#7df831';
      }
      return style;
    },
    async onCellDoubleClicked(event: CellDoubleClickedEvent<any>) {
      if (event.rowIndex == null || event.rowIndex < 0)
        return;
      currentRowIndex.value = event.rowIndex;
    },
  }

}
const taskListTableRef = ref<IFullTableSchema>(taskListTable);
const runType = ref(0);//0不运行；1，运行1次；2循环运行
const shelfTaskConfigId = ref('');

function runOnceClick() {
  if (runType.value == 1) runType.value = 0;
  else runType.value = 1;

}

function runCycleClick() {
  if (runType.value == 2) runType.value = 0;
  else runType.value = 2;
}

watch(() => shelfTaskConfigId.value, async () => {
  currentRowIndex.value = 0;
  if (!shelfTaskConfigId.value) {
    shelfTaskConfig.value = undefined;
    taskListTableRef.value.rowData = [];
    return;
  }
  const getRe = await apiHelper.request<IShelfTaskConfigEditVm>('/api/ShelfTaskConfig/ShelfTaskConfigGetEditVm', {id: shelfTaskConfigId.value});
  if (!getRe.success) {
    return msg.error(getRe.message)
  }
  shelfTaskConfig.value = getRe.data;
  const taskList = JSON.parse(shelfTaskConfig.value?.dataJson ?? '[]') as IShelfTask[];
  taskListTableRef.value.rowData = taskList;
});

async function deleteClick() {
  if (!shelfTaskConfigId.value) return msg.error('请选择要删除的配置名称');
  if (!await msg.confirm('确认要删除该配置吗？')) return;
  const delRe = await apiHelper.request('/api/ShelfTaskConfig/ShelfTaskConfigDelete', {ids: shelfTaskConfigId.value});
  if (!delRe.success) return msg.error(delRe.message);
  shelfTaskConfigId.value = '';
  shelfTaskConfig.value = undefined;
  msg.success('删除成功');
}

const shelfTaskConfig = ref<IShelfTaskConfigEditVm | undefined>();

async function saveClick() {
  if (!taskListTableRef.value?.rowData?.length) {
    msg.error('上架任务中没内容需要保存');
    return;
  }
  let configName = shelfTaskConfig.value?.name;
  if (!configName) {
    const re = await modalUtil.showStringInput('输入配置名称', '');
    if (!re.success || !re.data) return;
    configName = re.data;
  }

  const taskList = taskListTableRef.value.rowData as IShelfTask[];
  const json = JSON.stringify(taskList);
  const saveData: IShelfTaskConfigEditVm = {
    id: shelfTaskConfigId.value,
    name: configName,
    dataJson: json,
  }
  const saveRe = await apiHelper.request<IShelfTaskConfigEditVm>('/api/ShelfTaskConfig/ShelfTaskConfigSaveEditVm', undefined, saveData);
  if (!saveRe.success) {
    msg.error(saveRe.message)
    return;
  }
  shelfTaskConfig.value = saveRe.data;
  shelfTaskConfigId.value = saveRe.data?.id ?? '';
  msg.success('保存成功');
}

onMounted(() => {
  ext.asyncLoop(backgroundRunShelf, 1000);

})
const currentRowIndex = ref(0);
watch(() => currentRowIndex.value, () => {
  taskListTableRef.value.options?.redrawRows();
})

async function runSleepTask(task: IShelfTask) {
  const taskData = task.taskData as ISleepTaskData;
  const sec = _.random(taskData.begin, taskData.end);
  for (let i = 0; i < sec && runType.value > 0; i++) {
    await ext.sleep(1000);
  }
  msg.success(`等待${sec}秒完成，开始运行下一条任务`);

}


async function runFengcheShelfTask(task: IShelfTask) {
  const taskData = task.taskData as IFengcheTaskData;
  const optRe = await DyApi.shelfOpt(taskData.accountId, taskData.cards.map(c => c.card_id), 'shelf');
  if (!optRe.success) return msg.error('上架操作失败：' + optRe.message);
  msg.success(`【${task.type}】${task.title} 执行成功！`)
}


async function runFengcheUnShelfTask(task: IShelfTask) {
  const taskData = task.taskData as IFengcheTaskData;
  const optRe = await DyApi.shelfOpt(taskData.accountId, taskData.cards.map(c => c.card_id), 'unShelf');
  if (!optRe.success) return msg.error('上架操作失败：' + optRe.message);
  msg.success(`【${task.type}】${task.title} 执行成功！`)
}

async function runFengchePopTask(task: IShelfTask) {
  const taskData = task.taskData as IFengcheTaskData;
  const optRe = await DyApi.popOpt(taskData.accountId, taskData.cards.map(c => c.card_id)[0], 'pop');
  if (!optRe.success) return msg.error('上架操作失败：' + optRe.message);
  msg.success(`【${task.type}】${task.title} 执行成功！`)

}

async function runHuangCheShelfTask(task: IShelfTask) {
  const taskData = task.taskData as IHuangcheTaskData;
  const accRe = await accountHelper.getAccount(taskData.accountId);
  if (!accRe.success) {
    return R.error(accRe.message)
  }
  const auth = JSON.parse(accRe.data!.authJson!) as IDyAccountAuthVm;
  const cookies = DyApi.parseCookie(auth.cookie);

  const optRe = await ByApi.bindToLive(cookies, taskData.products);
  if (!optRe.success) return msg.error('上架操作失败：' + optRe.message);
  const succCount = optRe.data?.data?.success_count ?? 0;
  if (optRe.data?.data?.failure_list?.length) {
    const errMsg = optRe.data?.data?.failure_list.map(l => l.bind_reason).join(',');
    return msg.error(`【${task.type}】${task.title} 执行成功${succCount}条,出错:${errMsg}`);

  }
  msg.success(`【${task.type}】${task.title} 执行成功:${succCount}条！`)
}

async function runHuangCheUnShelfTask(task: IShelfTask) {
  const taskData = task.taskData as IHuangcheTaskData;
  const accRe = await accountHelper.getAccount(taskData.accountId);
  if (!accRe.success) {
    return R.error(accRe.message)
  }
  const auth = JSON.parse(accRe.data!.authJson!) as IDyAccountAuthVm;
  const cookies = DyApi.parseCookie(auth.cookie);

  const optRe = await ByApi.unBindToLive(cookies, taskData.products);
  if (!optRe.success && !optRe.message?.includes('有商品不在直播商品列表中，无法解绑'))
    return msg.error('下架操作失败：' + optRe.message);

  msg.success(`【${task.type}】${task.title} 执行成功！`)
}

async function runHuangChePopTask(task: IShelfTask) {
  const taskData = task.taskData as IHuangcheTaskData;
  const accRe = await accountHelper.getAccount(taskData.accountId);
  if (!accRe.success) {
    return R.error(accRe.message)
  }
  const auth = JSON.parse(accRe.data!.authJson!) as IDyAccountAuthVm;
  const cookies = DyApi.parseCookie(auth.cookie);
  const promotionId = taskData.products[0].promotion_id;
  const optRe = await ByApi.setCurrentToLive(cookies, promotionId, false);
  if (!optRe.success)
    return msg.error('下架操作失败：' + optRe.message);

  msg.success(`【${task.type}】${task.title} 执行成功！`)
}


async function backgroundRunShelf() {
  if (runType.value == 0) return;

  const taskList = taskListTableRef.value.rowData as IShelfTask[];
  const current = currentRowIndex.value;
  //运行到最后了
  if (current >= taskList.length) {//运行结束了
    if (runType.value == 1) {
      currentRowIndex.value = 0;
      runType.value = 0;
      msg.success('单次运行完成')
      return;
    } else {
      currentRowIndex.value = 0;
      msg.success('上架任务一轮结束，继续循环')
      return;
    }
  }
  const task = taskList[current];
  if (task.type == "等待停顿") {
    await runSleepTask(task);
  } else if (task.type == "风车上架") {
    await runFengcheShelfTask(task);

  } else if (task.type == "风车讲解") {
    await runFengchePopTask(task);

  } else if (task.type == "风车下架") {
    await runFengcheUnShelfTask(task);

  } else if (task.type == "黄车上架") {
    await runHuangCheShelfTask(task);

  } else if (task.type == "黄车下架") {
    await runHuangCheUnShelfTask(task);

  } else if (task.type == "黄车讲解") {
    await runHuangChePopTask(task);

  }
  currentRowIndex.value += 1;

}

function appendTask(task: IShelfTask) {
  taskListTableRef.value?.options?.addNewRows([task]);
}

function deleteTask(data1: IShelfTask) {
  taskListTableRef.value?.options?.removeIds([data1.id])
}

defineExpose({appendTask});

</script>

<template>
  <FlexLayout>
    <FlexLayoutHeader>
      <ARow :align="'middle'" :gutter="[10,10]" style="height:100%">
        <ACol><h4>运行：</h4></ACol>
        <ACol flex="1">
          <DataSelectInput :disabled="runType>0" placeholder="选择配置" :allowClear="true"
                           v-model:value="shelfTaskConfigId"
                           :dataSourceApi="{apiUrl:'api/sys/ListValueDisplay',postParams:{type:13}}"/>
        </ACol>
        <ACol>
          <AButton :disabled="runType>0" :icon="getIcon('icon-save1')" @click="saveClick"></AButton>
        </ACol>
        <ACol>
          <AButton :disabled="runType>0" :icon="getIcon('icon-delete2')" @click="deleteClick"></AButton>
        </ACol>
        <ACol>
          <ATooltip :title="runType==1?'点击停止运行':'点击开始单次运行'">
            <AButton @click="runOnceClick">{{ runType == 1 ? '停止' : '单次' }}</AButton>

          </ATooltip>
        </ACol>
        <ACol>
          <ATooltip :title="runType==2?'点击停止运行':'点击开始循环运行'">
            <AButton type="primary" @click="runCycleClick">{{ runType == 2 ? '停止' : '循环' }}</AButton>

          </ATooltip>
        </ACol>
      </ARow>
    </FlexLayoutHeader>
    <FlexLayoutContent style="padding-top: 10px;">
      <FullTable :schema="taskListTableRef"></FullTable>
    </FlexLayoutContent>
  </FlexLayout>


</template>

