<script setup lang="ts">
import {IFullTableSchema, showUpdateEmits} from "@/types/schema";
import {ref} from "vue";
import {accountHelper} from "@/views/live/help/accountHelper";
import {IDyAccountAuthVm, ILiveAccount} from "@/views/live/help/LiveInterface";
import {DyApi, IDyServiceCard} from "@/views/live/help/DyApi";
import {R} from "@/utils/R";

const props = defineProps<{
  show?: boolean,
  title?: string,
  observerAccountId?: string,
  handleOk?: (account: ILiveAccount, items: IDyServiceCard[]) => Promise<R>,
}>();
const title = ref(props.title);
const cardTable: IFullTableSchema = {
  columns: [{
    field: 'service_info.service_banner_url', headerName: '图', width: 80, cellRender: {
      comPath: '/src/components/grid/column/ImageRender.vue',
    }
  },
    {field: 'component_title', headerName: '名称', width: 200},
    {field: 'card_id', headerName: '卡片Id', width: 200},
  ],
  rowData: [],
  agOptions: {rowHeight: 60},
  rowSelection: 'multiple',
}
const cardTableRef = ref<IFullTableSchema>(cardTable);

async function ok() {
  if (!props.handleOk) {
    return;
  }
  const selectedRows = cardTableRef.value.options?.getSelectedRows();
  if (!selectedRows?.length) {
    msg.error('请在表格中勾选要操作的数据')
    return;
  }

  const re = await props.handleOk(account!, selectedRows as IDyServiceCard[]);
  if (re.success) {
    cancel();
  }
}

function cancel() {
  emit('update:show', false);
}

watch(() => props.show, () => {
  if (!props.show) return;
  cardTableRef.value.rowData = [];
  ini();
})
let account: ILiveAccount | undefined;

async function ini() {
  if (!props.observerAccountId) {
    cancel();
    return msg.error('请选择观察员账号');
  }
  const accRe = await accountHelper.getAccount(props.observerAccountId!)
  if (!accRe.success) {
    cancel();
    return msg.error('观察员信息获取失败');
  }
  account = accRe.data!;
  title.value = props.title + '：' + account?.name;
  const auth = JSON.parse(account.authJson!) as IDyAccountAuthVm;
  const apiRe = await DyApi.getServiceCard(DyApi.parseCookie(auth.cookie));
  if (!apiRe.success) {
    cancel();
    return msg.error('卡片信息获取失败：' + apiRe.message);
  }

  cardTableRef.value.options?.addNewRows(apiRe.data!);
}

const size = {
  width: '700px',
  height: '450px',
}
const emit = defineEmits<showUpdateEmits>();

</script>

<template>
  <AModal :destroyOnClose="true" v-model:open="props.show"
          :width="size.width"
          :title="title"
          :centered="true"
          :bodyStyle="{height:size.height}"
          :footer="null"
          @cancel="cancel"
  >

    <FlexLayout style="padding: 10px">
      <FlexLayoutContent>
        <FullTable v-model:schema="cardTableRef"></FullTable>
      </FlexLayoutContent>
      <FlexLayoutFooter>
        <ARow :gutter="[10,10]" style="margin-top: 10px;">
          <ACol span="12">
            <AButton style="width: 100%" size="large" type="primary" @click="ok">确定</AButton>
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
