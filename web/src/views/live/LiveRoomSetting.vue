<script setup lang="ts">
import {IFullTableSchema, IModalDataSelectSchema} from "@/types/schema";
import HuangCheAccountCom from "@/views/live/com/HuangCheAccountCom.vue";
import ObserverAccountCom from "@/views/live/com/ObserverAccountCom.vue";
import {IQueryParam} from "@/types/dto";
import {pageApi} from "@/api/pageApi";
import {IByAccountAuthVm, ILiveAccount} from "@/views/live/help/LiveInterface";
import {R} from "@/utils/R";
import {ByApi, IByProduct} from "@/views/live/help/ByApi";
import {DyApi} from "@/views/live/help/DyApi";
import {modalUtil} from "@/utils/modalUtil";
import {accountHelper} from "@/views/live/help/accountHelper";
import {ValueGetterParams} from "ag-grid-community";

const productTable: IFullTableSchema = {
  columns: [{
    field: 'imgUrl', headerName: '图', width: 60, cellRender: {
      comPath: '/src/components/grid/column/ImageRender.vue',
    }
  },
    {field: 'productName', headerName: '商品名称', width: 180},
    {field: 'productId', headerName: '商品Id', width: 150},
  ],
  rowData: [],
}
const productTableRef = ref<IFullTableSchema>(productTable);
//账号
//直播间描述


const productText = ref('');
let productText_Source = '';
const personaText = ref('');
let personaText_Source = '';
const operateIdRef = ref('');

async function iniData() {
  const rRe = await apiHelper.request<ILiveRoomVm[]>('/api/LiveRoom/LiveRoomQueryList', undefined, {});
  if (!rRe.success) return msg.error(rRe.message);
  if (rRe.data && rRe.data?.length > 0) {
    productText_Source = rRe.data[0].productText;
    productText.value = productText_Source;

    personaText_Source = rRe.data[0].personaText;
    personaText.value = personaText_Source;
  }

}

async function addProduct() {
  if (!operateIdRef.value) return msg.error('请选择电商操作账号');

  const accRe = await accountHelper.getAccount(operateIdRef.value)
  if (!accRe.success) {
    return R.error('电商账号信息获取失败');
  }
  const account = accRe.data!;
  const re = await selectShelfProduct(account);
  if (!re.success && re.message != '用户取消') return R.error(re.message);
  const extIdArr = productTableRef.value?.rowData?.map(r => r.productId) ?? [];
  for (const item of re.data!) {
    if (extIdArr.includes(item.productId)) {
      continue;
    }

    const saveRe = await apiHelper.request<ILiveAccountProductVm>('/api/LiveAccountProduct/LiveAccountProductSaveEditVm', {}, item);
    if (!saveRe.success) return R.error(saveRe.message);
  }
  await refreshProduct();
  msg.success('添加成功')

}

async function delProduct() {
  if (!productTableRef.value?.currentRow) return msg.error('请选择商品');
  const id = productTableRef.value?.currentRow['id'];
  const re = await pageApi.deleteIds('/api/LiveAccountProduct/LiveAccountProductDelete', [id]);
  if (!re.success) return R.error(re.message);
  await refreshProduct();
  msg.success('删除成功')

}

async function selectShelfProduct(account: ILiveAccount): Promise<R<ILiveAccountProductVm[]>> {
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
          const queryText = queryObj?.queryObject?.name ?? '';
          const re = await ByApi.getLiveList(cookies);
          if (!re.success) return R.error(re.message);
          const idList = re.data?.data.basic_list?.map(x => x.promotion_id) ?? [];
          const ids = idList.join(',');
          const reDetail = await ByApi.listPromotions(cookies, ids);
          if (!reDetail.success) return R.error(reDetail.message);
          let detailList = reDetail.data?.data?.promotions ?? [];
          if (queryText) {
            detailList = detailList.filter(x => x.title.includes(queryText) || x.product_id == queryText)
          }
          return R.ok(detailList);
        },
        pageSize: 200,
        page: 1,
        showPageBar: false,
        columns: [
          {
            field: 'cover',
            headerName: '图',
            width: 80, cellRender: {
              comPath: '/src/components/grid/column/ImageRender.vue',
            }
          },
          {field: 'product_id', headerName: 'Id', width: 80},
          {field: 'title', headerName: '名称', width: 200},
          {
            field: 'price', headerName: '价格', width: 80, valueGetter: (params: ValueGetterParams) => {
              if (!params.data) return '';
              return `${params.data.price_desc.price_text}:${params.data.price_desc.min_price.origin / 100}`;
            }
          },
          {field: 'stock_num', headerName: '库存', width: 80},

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
  const proList: ILiveAccountProductVm[] = [];
  for (const item of selectRe.data) {
    const cookies = DyApi.parseCookie(auth.cookie);
    const gptRe = await ByApi.getGptDesc(cookies, item.product_id!);
    const gptDesc = gptRe.data?.data?.gtp_generated_product_desc;
    if (!gptRe.success || !gptDesc) return R.error(gptRe.message);
    const h5Re = await ByApi.packH5(cookies, item.product_id!, gptDesc);
    if (!h5Re.success) return R.error(h5Re.message);
    const packDetailRespR = await ByApi.packDetail(cookies, item.promotion_id!)
    if (!packDetailRespR.success) return R.error(packDetailRespR.message);
    proList.push({
      imgUrl: item.cover as string,
      liveAccountId: account.id,
      productName: item.title as string,
      productId: item.product_id as string,
      productJson: JSON.stringify({
        h5Data: h5Re,
        pageDetail: packDetailRespR,
      }),
    });
  }

  return R.ok(proList);

}


async function refreshProduct() {
  if (!operateIdRef.value) {
    productTableRef.value.rowData = []
    return
  }
  const query: IQueryParam = {
    page: 1,
    pageSize: 100,
    queryObject: {
      liveAccountId: operateIdRef.value,
    }
  }
  const rRe = await apiHelper.request<ILiveAccountProductVm[]>('/api/LiveAccountProduct/LiveAccountProductQueryList', undefined, query);
  if (!rRe.success) return msg.error(rRe.message);
  const list = rRe.data ?? [];
  if (productTableRef.value)
    productTableRef.value.rowData = list;

}

watch(() => operateIdRef.value, refreshProduct, {immediate: true})

async function liveRoomChanged() {
  if (productText_Source == productText.value && personaText.value == personaText_Source) {
    return;
  }
  const
      saveRe = await apiHelper.request('/api/LiveRoom/SaveDefault', undefined, {
        productText: productText.value,
        personaText: personaText.value,
      });
  if (!saveRe.success) return msg.error(saveRe.message);
  productText_Source = productText.value
  personaText_Source = personaText.value
  msg.success('直播间描述保存成功');
}


iniData()

interface ILiveRoomVm {
  id: string;
  name: string;
  productText: string;
  personaText: string;
}

interface ILiveAccountProductVm {
  id?: string;
  liveAccountId?: string;
  productName?: string;
  productId?: string;
  imgUrl?: string;
  productJson?: string;
}

</script>

<template>
  <FlexLayout>
    <FlexLayoutContent>
      <ARow style="width: 100%;height: 100%;padding: 0 5px;" :gutter="10">
        <ACol span="12">
          <FlexLayout>
            <FlexLayoutHeader>
              <ACard class="groupCard">
                <template #title>
                  直播间描述
                  <ATooltip>
                    <template #title>基本参考话术，描述商品的卖点、价格、人群</template>
                    <QuestionCircleOutlined/>
                  </ATooltip>
                </template>
                <ATextarea :rows="8" v-model:value="productText" @blur="liveRoomChanged"></ATextarea>
              </ACard>
            </FlexLayoutHeader>
            <FlexLayoutContent>
              <ACard class="groupCard flexCard">
                <template #title>角色个性设置
                  <ATooltip>
                    <template #title>角色设定、限制的要求、注意内容、风格等</template>
                    <QuestionCircleOutlined/>
                  </ATooltip>
                </template>
                <ATextarea style="height: 100%" v-model:value="personaText" @blur="liveRoomChanged"></ATextarea>
              </ACard>

            </FlexLayoutContent>
          </FlexLayout>


        </ACol>
        <ACol span="12">

          <FlexLayout>
            <FlexLayoutHeader>
              <ACard class="groupCard">
                <template #title>
                  直播账号登录
                  <ATooltip>
                    <template #title>登录主播、观察员账号</template>
                    <QuestionCircleOutlined/>
                  </ATooltip>
                </template>
                <ObserverAccountCom></ObserverAccountCom>
                <HuangCheAccountCom v-model:value="operateIdRef"></HuangCheAccountCom>
              </ACard>
            </FlexLayoutHeader>
            <FlexLayoutContent style="margin:5px 0;">
              <ACard class="flexCard">
                <template #title>
                  <ARow align="middle" :gutter="5">
                    <ACol>
                      直播间主讲商品
                      <ATooltip>
                        <template #title>从直播间上架商品中选择讲商品</template>
                        <QuestionCircleOutlined/>
                      </ATooltip>
                    </ACol>
                    <ACol>
                      <AButton type="default" @click="addProduct">添加</AButton>
                    </ACol>
                    <ACol>
                      <AButton type="default" @click="delProduct">删除</AButton>
                    </ACol>
                    <ACol>
                      <AButton type="default" @click="refreshProduct">刷新</AButton>
                    </ACol>
                  </ARow>
                </template>


                <FullTable :schema="productTableRef"></FullTable>

              </ACard>
            </FlexLayoutContent>


          </FlexLayout>

        </ACol>
      </ARow>
    </FlexLayoutContent>
  </FlexLayout>


</template>

<style scoped lang="scss">
:deep(.ant-card-body) {
  padding: 5px;
}

.groupCard {
  margin-top: 5px;
}

.flexCard {
  height: 100%;
  margin: 0;
  display: flex;
  flex-direction: column;
}

:deep(.flexCard .ant-card-body) {
  flex: 1;
}

</style>
