<script setup lang="ts">
import ObserverAccountCom from "@/views/live/com/ObserverAccountCom.vue";
import HuangCheAccountCom from "@/views/live/com/HuangCheAccountCom.vue";
import {IFullTableSchema, IModalDataSelectSchema} from "@/types/schema";
import {ref} from "vue";
import FengcheSelect from "@/views/live/com/FengcheSelect.vue";
import {R} from "@/utils/R";
import {
  IDyServiceCard,
  IFengcheTaskData,
  IShelfTask,
  IShelfTaskType,
  ISleepTaskData, DyApi, IHuangcheTaskData
} from "@/views/live/help/DyApi";
import _ from "lodash";
import {IByAccountAuthVm, IDyAccountAuthVm, ILiveAccount} from "@/views/live/help/LiveInterface";
import {modalUtil} from "@/utils/modalUtil";
import HuangcheSelect from "@/views/live/com/HuangcheSelect.vue";
import {IQueryParam} from "@/types/dto";
import ShelfTaskRun from "@/views/live/com/ShelfTaskRun.vue";
import {accountHelper} from "@/views/live/help/accountHelper";
import {ByApi, IByProduct, IByProductResp} from "@/views/live/help/ByApi";


const observerAccountId = ref('');
const operateAccountId = ref('');
const fengcheShow = ref(false);
const huangcheShow = ref(false);


async function appendFengcheTask(account: ILiveAccount, items: IDyServiceCard[]): Promise<R> {
  if (!items.length) return R.error('请勾选要操作的卡片');
  if (items.length > 1 && taskType.value.includes('讲解')) {
    return R.error('讲解卡片每次只能选1张');
  }
  const data: IFengcheTaskData = {
    accountId: account.id!,
    accountName: account.name!,
    cards: items,
  };
  const title = `${account.name}：${items.map(i => i.component_title).join(';')}`;
  const task: IShelfTask = {
    id: _.uniqueId(),
    type: taskType.value,
    title: title,
    taskData: data,
  };
  shelfTaskRunRef.value?.appendTask(task);
  return R.ok();

}

async function appendHuangcheTask(account: ILiveAccount, items: IByProduct[], taskType: IShelfTaskType): Promise<R> {
  if (!items.length) return R.error('请勾选要操作的卡片');
  if (items.length > 1 && taskType.includes('讲解')) {
    return R.error('讲解卡片每次只能选1张');
  }
  const data: IHuangcheTaskData = {
    accountId: account.id!,
    accountName: account.name!,
    products: items,
  };
  const title = `${items.length}件【${items[0].name}】`;
  const task: IShelfTask = {
    id: _.uniqueId(),
    type: taskType,
    title: title,
    taskData: data,
  };
  shelfTaskRunRef.value?.appendTask(task);
  return R.ok();

}

function shelfFengcheClick() {
  taskType.value = '风车上架';
  fengcheShow.value = true;
}

function unShelfFengcheClick() {
  taskType.value = '风车下架';
  fengcheShow.value = true;
}

function popFengcheClick() {
  taskType.value = '风车讲解';
  fengcheShow.value = true;
}

async function selectProduct(account: ILiveAccount): Promise<R<IByProduct[]>> {
  const auth = JSON.parse(account.authJson!) as IByAccountAuthVm;
  const schema: IModalDataSelectSchema = {
    dataBrowserSchema: {
      searchContainer: {
        fields: [{
          field: 'name',
          label: '商品名称',
          fieldType: 'string',
          defaultValue: '',
          module: {
            comPath: '/src/components/edit/simple/StringInput.vue',
          }
        }]
      },
      mainTable: {
        autoQuery: true,
        queryDataUrl: async (queryObj: IQueryParam | undefined): Promise<R<any[]>> => {
          const cookies = DyApi.parseCookie(auth.cookie);
          const re = await ByApi.shopProducts(cookies, queryObj?.page ?? 1, queryObj?.pageSize ?? 20, queryObj?.queryObject?.name ?? '');
          const list = re.data?.data?.list ?? [];
          return R.ok(list);
        },
        queryCountUrl: async (queryObj: IQueryParam | undefined): Promise<R<number>> => {
          const cookies = DyApi.parseCookie(auth.cookie);
          const re = await ByApi.shopProducts(cookies, queryObj?.page ?? 1, queryObj?.pageSize ?? 20, queryObj?.queryObject?.name ?? '');
          const total = re.data?.data?.total ?? 0;
          return R.ok(total);
        },
        pageSize: 20,
        pageSizeOptions: '20,50',
        page: 1,
        showPageBar: true,
        columns: [
          {
            field: 'cover',
            headerName: '图',
            width: 80, cellRender: {
              comPath: '/src/components/grid/column/ImageRender.vue',
            }
          },
          {field: 'add_source', headerName: '添加方式', width: 80},
          {field: 'product_id', headerName: 'Id', width: 80},
          {field: 'bind_time', headerName: '上架时间', width: 80},
          {field: 'name', headerName: '名称', width: 200},
          {field: 'price', headerName: '价格', width: 80},
          {field: 'commission.cos_ratio', headerName: '佣金', width: 80},

        ],
        agOptions: {rowHeight: 60},
        rowSelection: 'multiple',
      }
    },
    sizeMode: 5,
    centered: true,
    title: '选择商品',

  }

  const selectRe = await modalUtil.showDataSelect(schema);
  if (!selectRe.success || !selectRe.data?.length) {
    return R.error('用户取消');
  }
  return R.ok(selectRe.data as IByProduct[]);

}

async function shelfHuangcheClick() {
  if (!operateAccountId.value) return msg.error('请选择电商操作账号');

  const accRe = await accountHelper.getAccount(operateAccountId.value)
  if (!accRe.success) {
    return R.error('电商账号信息获取失败');
  }
  const account = accRe.data!;
  const re = await selectProduct(account);
  if (!re.success) {
    if (re.message == '用户取消') {
      return;
    }
    return msg.error(re.message);
  }
  const items = re.data!;

  const addRe = await appendHuangcheTask(account, items, "黄车上架");
  if (!addRe.success) {
    return msg.error(addRe.message);
  }
  msg.success('添加成功');
}

async function unShelfHuangcheClick() {
  if (!operateAccountId.value) return msg.error('请选择电商操作账号');

  const accRe = await accountHelper.getAccount(operateAccountId.value)
  if (!accRe.success) {
    return R.error('电商账号信息获取失败');
  }
  const account = accRe.data!;
  const re = await selectProduct(account);
  if (!re.success) {
    if (re.message == '用户取消') {
      return;
    }
    return msg.error(re.message);
  }
  const items = re.data!;

  const addRe = await appendHuangcheTask(account, items, "黄车下架");
  if (!addRe.success) {
    return msg.error(addRe.message);
  }
  msg.success('添加成功');

}

async function popHuangcheClick() {
  if (!operateAccountId.value) return msg.error('请选择电商操作账号');

  const accRe = await accountHelper.getAccount(operateAccountId.value)
  if (!accRe.success) {
    return R.error('电商账号信息获取失败');
  }
  const account = accRe.data!;
  const re = await selectProduct(account);
  if (!re.success) {
    if (re.message == '用户取消') {
      return;
    }
    return msg.error(re.message);
  }
  const items = re.data!;

  const addRe = await appendHuangcheTask(account, items, "黄车讲解");
  if (!addRe.success) {
    return msg.error(addRe.message);
  }
  msg.success('添加成功');


}

const qujian = ref(true);
const sleepStart = ref(30);
const sleepEnd = ref(60);


const taskType = ref<IShelfTaskType>('风车上架');

function addSleepTask() {
  const data: ISleepTaskData = {
    begin: qujian.value ? sleepStart.value : sleepEnd.value,
    end: sleepEnd.value,
  };
  const title = qujian.value ? `等待${sleepStart.value}到${sleepEnd.value}秒` : `等待${sleepEnd.value}秒`;
  const task: IShelfTask = {
    id: _.uniqueId(),
    type: '等待停顿',
    title: title,
    taskData: data,
  };
  shelfTaskRunRef.value?.appendTask(task);

}

const shelfTaskRunRef = ref<InstanceType<typeof ShelfTaskRun>>();
</script>

<template>
  <FlexLayout style="background-color: white; padding: 10px;">
    <FlexLayoutHeader>
      <ARow style="margin-top: 5px;">
        <ACol>
          <H3>上架助手</H3>
        </ACol>
        <ACol>
          <ATooltip>
            <template #title>风车、电商黄车等上架操作及定时任务</template>
            <QuestionCircleOutlined/>
          </ATooltip>
        </ACol>
      </ARow>
      <ADivider style="margin-top: 15px; margin-bottom: 15px;"></ADivider>
    </FlexLayoutHeader>
    <FlexLayoutContent>
      <ARow :gutter="[10,10]" style="width: 100%; height: 100%">
        <ACol span="12" style="height: 100%;">
          <FlexLayout>
            <FlexLayoutHeader>
              <ObserverAccountCom v-model:value="observerAccountId"></ObserverAccountCom>
              <HuangCheAccountCom v-model:value="operateAccountId"></HuangCheAccountCom>
            </FlexLayoutHeader>
            <FlexLayoutContent>
              <ADivider style="margin: 10px 0;"></ADivider>
              <ARow align="middle" :gutter="[10,10]" style="width:100%">
                <ACol>
                  <Icon style="font-size: 16px" name="icon-naozhong"></Icon>
                </ACol>
                <ACol flex="1"><span style="font-size: 14px;color: #0094ff;font-weight: bold;">等待停顿（秒）</span>
                </ACol>
                <ACol v-if="qujian">
                  <AInputNumber v-model:value="sleepStart" style="width: 60px;"></AInputNumber>
                </ACol>
                <ACol v-if="qujian">~</ACol>
                <ACol>
                  <AInputNumber v-model:value="sleepEnd" style="width: 60px;"></AInputNumber>
                </ACol>
                <ACol>
                  <ACheckbox v-model:checked="qujian">区间</ACheckbox>
                </ACol>
                <ACol>
                  <AButton @click="addSleepTask">加入</AButton>
                </ACol>
              </ARow>
              <ADivider style="margin: 10px 0;"></ADivider>
              <ARow align="middle" :gutter="[10,10]" style="width:100%">
                <ACol>
                  <Icon style="font-size: 16px" name="icon-a-appround26"></Icon>
                </ACol>
                <ACol flex="1"><span style="font-size: 14px;color: #0094ff;font-weight: bold;">企业服务-风车</span>
                </ACol>
                <ACol>
                  <AButton @click="shelfFengcheClick">上架</AButton>
                  <FengcheSelect v-bind:observer-account-id="observerAccountId" :title="taskType"
                                 :handle-ok="appendFengcheTask"
                                 v-model:show="fengcheShow"></FengcheSelect>
                </ACol>
                <ACol>
                  <AButton @click="unShelfFengcheClick">下架</AButton>
                </ACol>
                <ACol>
                  <AButton @click="popFengcheClick">讲解</AButton>
                </ACol>
              </ARow>
              <ADivider style="margin: 10px 0;"></ADivider>
              <ARow align="middle" :gutter="[10,10]" style="width:100%">
                <ACol>
                  <Icon style="font-size: 16px ;color:rgb(255,184,44)" name="icon-xiaohuangche"></Icon>
                </ACol>
                <ACol flex="1"><span style="font-size: 14px;color: #0094ff;font-weight: bold;">电商服务-黄车</span>
                </ACol>
                <ACol>
                  <AButton @click="shelfHuangcheClick">上架</AButton>
                </ACol>
                <ACol>
                  <AButton @click="unShelfHuangcheClick">下架</AButton>
                </ACol>
                <ACol>
                  <AButton @click="popHuangcheClick">讲解</AButton>
                </ACol>
              </ARow>
              <ADivider style="margin: 10px 0;"></ADivider>


            </FlexLayoutContent>
          </FlexLayout>


        </ACol>
        <ACol span="12">
          <ShelfTaskRun ref="shelfTaskRunRef"></ShelfTaskRun>

        </ACol>
      </ARow>

    </FlexLayoutContent>
  </FlexLayout>


</template>

<style scoped lang="scss">


</style>
