<template>
  <FlexLayout>
    <FlexLayoutHeader v-if="props.schema.tableTools&&props.schema.tableTools.length>0">
      <TableToolBar v-bind:items="props.schema.tableTools" :table-schema="props.schema"></TableToolBar>
    </FlexLayoutHeader>

    <FlexLayoutContent>
      <AgGridVue
          :localeText="AG_GRID_LOCALE_CN"
          class="ag-theme-quartz"
          style="width: 100%; height: 100%;"
          :columnDefs="columns"
          v-model:rowData="props.schema.rowData"
          ref="gridRef"
          :gridOptions="props.schema.gridOptions"
      ></AgGridVue>
    </FlexLayoutContent>
    <FlexLayoutFooter style="padding:3px 5px;background-color: white;text-align: right;"
                      v-if="props.schema.showPageBar">
      <a-pagination
          v-model:current="props.schema.page"
          v-model:pageSize="props.schema.pageSize"
          :total="props.schema.totalCount"
          :page-size-options="pageSizeOptions"
          :show-total="(total:any, range:any) => `从${range[0]}-${range[1]} 共 ${total} 条`"
          @change="pageChanged"
      />
    </FlexLayoutFooter>

  </FlexLayout>
</template>
<script setup lang="ts">

import {AgGridVue} from "ag-grid-vue3";
import "ag-grid-enterprise";
import {IFullTableSchema, IGridApiObj, SchemaUpdateEmits} from "@/types/schema";
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-quartz.css';
import {ColDef, GridOptions, GridReadyEvent} from "ag-grid-community";
import {ext} from "@/utils/ext";
import {pageApi} from "@/api/pageApi";
import {LicenseManager} from "ag-grid-enterprise";
import {AG_GRID_LOCALE_CN} from '@ag-grid-community/locale';
import {agGridSelectionUtil} from "@/utils/agGridSelectionUtil";
import {IQueryParam} from "@/types/dto";
import {R} from "@/utils/R";
import type {CellFocusedEvent} from "ag-grid-community/dist/types/core/events";
import _ from "lodash";
import {agGridColumnUtil} from "@/utils/agGridColumnUtil";

LicenseManager.prototype.validateLicense = () => true;
LicenseManager.prototype.isDisplayWatermark = () => false;

const props = defineProps<{
  schema: IFullTableSchema,
}>();
console.log('TableSchema:', props.schema);

const emit = defineEmits<SchemaUpdateEmits>();
const defaultColDef = ref<ColDef>({
  filter: true,
  menuTabs: ['filterMenuTab', 'generalMenuTab', 'columnsMenuTab'],
  width: 120,
  columnChooserParams: undefined,
  cellClass: 'ag-center-cell',

});
const api: IGridApiObj = {};
let gridHelper: agGridSelectionUtil;
const colUtil = new agGridColumnUtil(api, props.schema);


watch(() => props.schema.rowData, () => {
  console.debug('表格数据更新:' + props.schema.tableName)
})


const gridRef = shallowRef<InstanceType<typeof AgGridVue> | null>(null)
watch(() => props.schema.queryConditions, () => {//初始查询一次
  props.schema.page = 1;
  doQuery(props.schema.queryConditions)
}, {immediate: props.schema.autoQuery})

const autoGroupColumnDef: ColDef = {
  width: 200,
}
const columns = ref<ColDef[] | undefined>();
const pageSizeOptions = computed(() => {
  const str = props.schema.pageSizeOptions ?? '';
  return str.split(',');
})

function emitUpdate() {
  emit("update:schema", props.schema);
}

function updateRowData(data?: any[] | [] | undefined) {
  props.schema.rowData = data ?? [];

}

async function pageChanged() {
  if (props.schema.queryDataUrl) {
    const rowData = await queryData(props.schema.queryConditions) as [];
    updateRowData(rowData)
  }
}

/**
 * 需要优化成更平滑的操作
 * @param addRows
 */
function addNewRows(addRows: any[] | undefined) {
  if (!addRows || addRows.length == 0)
    return;
  props.schema.rowData = props.schema.rowData ?? [];
  const needAddRows = [];
  const needUpdateRows = [];
  for (let i = 0; i < addRows.length; i++) {
    const row = addRows[i];
    const primaryKey = props.schema.primaryKey ?? '';
    const newRowIndex = props.schema.rowData.findIndex(r => r[primaryKey] && r[primaryKey] == row[primaryKey])
    if (newRowIndex > -1) {
      needUpdateRows.push(addRows[i]);
      props.schema.rowData[newRowIndex] = addRows[i];
    } else {
      needAddRows.push(addRows[i]);
    }
  }

  api?.gridApi?.applyTransaction({
    update: needUpdateRows,
  });

  let isAtBottom = false;
  const dispNodes = api.gridApi?.getRenderedNodes();
  if (dispNodes != null && dispNodes.length > 0) {
    const lastNode = dispNodes[dispNodes.length - 1];
    const scrollCount = lastNode.rowIndex ? lastNode.rowIndex + 1 : 0;
    const totalCount = api.gridApi?.getDisplayedRowCount();
    isAtBottom = totalCount == scrollCount;
  }


  props.schema.rowData.push(...needAddRows);
  api?.gridApi?.applyTransaction({
    add: needAddRows,
  });
  if (isAtBottom) {
    api.gridApi?.ensureIndexVisible(api.gridApi?.getDisplayedRowCount() - 1)
  }
  //props.schema.rowData = [...props.schema.rowData];
}

async function onlineDeleteSelectedRows(): Promise<R> {
  if (!props.schema.deleteIdsUrl)
    return R.error('未配置删除Url，无法执行此操作')
  const rows = api.gridApi?.getSelectedRows();
  if (!rows || rows.length == 0)
    return R.ok();
  const ids = rows.map(r => r.id);
  const idsStr = ids.join();
  const delRe = await apiHelper.request(props.schema.deleteIdsUrl as string, {ids: idsStr});
  if (!delRe.success) return delRe;
  removeIds(ids);
  return R.ok();
}

/**
 * todo 需要更平滑的删除，通过找到node然后删除Node
 * @param ids
 */
function removeIds(ids: string[] | undefined) {
  if (!ids) return;
  props.schema.rowData = props.schema.rowData ?? [];
  const primaryKey = props.schema.primaryKey ?? 'id';
  props.schema.rowData = props.schema.rowData.filter(r => !ids.includes(r[primaryKey]));

}

async function queryData(conditions: Record<string, any> | undefined): Promise<any[]> {
  //无Url代表此表格数据由代码赋值，不用自己查询
  if (!props.schema.queryDataUrl) {
    return [];
  }
  const queryPar: IQueryParam = {
    page: props.schema.page ?? 1,
    pageSize: props.schema.pageSize ?? 20,
    queryObject: conditions
  };
  const queryDataRe = typeof props.schema.queryDataUrl === 'function' ?
      await props.schema.queryDataUrl(queryPar) :
      await pageApi.queryList(props.schema.queryDataUrl, queryPar);
  if (!queryDataRe.success) {
    msg.error('查询数据接口出错：' + queryDataRe.message)
    return [];
  }
  return queryDataRe.data ?? [];
}

async function queryCount(conditions: Record<string, any> | undefined): Promise<number> {
  //无Url代表此表格数据由代码赋值，不用自己查询
  if (!props.schema.queryCountUrl || !props.schema.showPageBar) {
    return 0;
  }
  const queryPar = {
    page: props.schema.page ?? 1,
    pageSize: props.schema.pageSize ?? 20,
    queryObject: conditions
  };
  
  const queryRe = typeof props.schema.queryCountUrl === 'function' ?
      await props.schema.queryCountUrl(queryPar) :
      await pageApi.queryCount(props.schema.queryCountUrl, queryPar);
  if (!queryRe.success) {
    msg.error('查询数据接口出错：' + queryRe.message)
    return 0;
  }
  return queryRe.data ?? 0;
}


async function doQuery(conditions: Record<string, any> | undefined) {
  api.gridApi?.setGridOption('loading', true);
  await nextTick();
  if (props.schema.queryDataUrl) {
    const rowData = await queryData(conditions) as [];
    updateRowData(rowData)
  }
  if (props.schema.queryCountUrl)
    props.schema.totalCount = await queryCount(conditions);
  emitUpdate();
  api.gridApi?.setGridOption('loading', false);

}

function onCellFocused(event: CellFocusedEvent) {
  const rowIndex = event.rowIndex;
  if (!_.isNumber(rowIndex)) return;
  const node = api.gridApi?.getDisplayedRowAtIndex(event.rowIndex ?? -1)
  if (!node) return //防止报错
  if (props.schema.gridOptions?.rowSelection != 'multiple')
    node.setSelected(true)
  console.log(api.gridApi?.getGridId() + '表格选择变化：', props.schema.currentRow?.id, node.data?.id)
  props.schema.currentRow = node.data;
  emitUpdate()
}


function createLocalSchema(): IFullTableSchema {
  return {
    gridOptions: createLocalOptions(),
    options: {
      getSelectedRows: () => {
        return api.gridApi?.getSelectedRows();
      },
      removeIds: removeIds,
      addNewRows: addNewRows,
      onlineDeleteSelectedRows: onlineDeleteSelectedRows,
      refreshRows: refreshRows,
      redrawRows: () => {
        api.gridApi?.redrawRows()
      },
      scrollToLast: () => {
        api.gridApi?.ensureIndexVisible(api.gridApi?.getDisplayedRowCount() - 1);
      },
    }
  };
}

async function refreshRows(ids: string[]): Promise<R> {
  const conditions = {ids: ids};
  const queryRe = await queryData(conditions);
  if (!queryRe || queryRe.length == 0) {
    return R.error('未查询到数据');
  }
  addNewRows(queryRe);
  return R.ok();
}

function createLocalOptions(): GridOptions {
  return {
    multiSortKey: 'ctrl',//排序支持ctrl
    enableFillHandle: true,//可以拖动进行填充编辑
    suppressRowDeselection: false, //如果true是的话，如果您按住Ctrl并单击行或按，则不会取消选择行Space。默认：false
    suppressRowClickSelection: true, //如果为true，则如果按住Ctrl键并单击行或按空格键，则不会取消选择行。默认值：false
    enableRangeSelection: true, //设置为true启用范围选择。默认：false
    suppressAutoSize: false, //禁止为列自动调整列大小。换句话说，双击列标题的边缘将不会自动调整大小。默认：false
    suppressRowDrag: true, //设置为true禁止行拖动。默认：false
    suppressCopyRowsToClipboard: true, //如果为true，则单击行时不会选择行。当您想独占选择复选框时使用。默认值：false
    stopEditingWhenCellsLoseFocus: true, //失去焦点结束编辑
    allowContextMenuWithControlKey: true,
    rowHeight: props.schema.agOptions?.rowHeight ?? 32,
    //pivotMode: true,
    pivotPanelShow: 'onlyWhenPivoting',
    pivotRowTotals: 'before',
    enableCharts: true,
    onGridReady(event: GridReadyEvent<any>) {
      api.gridApi = event.api;
      if (!gridHelper)
        gridHelper = new agGridSelectionUtil(api.gridApi, props.schema.gridOptions?.rowSelection);

    },
    //onCellClicked: cellClicked,
    onCellFocused: onCellFocused,
    //createChartContainer: createChartContainer,
    //onFirstDataRendered: onFirstDataRendered,
    columnMenu: 'legacy',
    headerHeight: 35,
    defaultColDef: defaultColDef.value as ColDef,
    tooltipShowDelay: 800,
    rowSelection: props.schema.rowSelection == 'multiple' ? 'multiple' : 'single',
    autoGroupColumnDef: autoGroupColumnDef,
  }
}


async function ini() {
  ext.appendSchema(props.schema, createLocalSchema());
  columns.value = await colUtil.createColumns();
}


ini();
</script>


<!--suppress CssUnusedSymbol -->
<style scoped>
:deep(.ag-root-wrapper) {
  /*最小表格高度，防止外层按高度显示*/
  min-height: 100px !important;
}

/**
过滤组件扩展
 */
:deep(.ag-filter-ext-filter-one::before) {
  content: '';
  width: 0;
  height: 0;
  border-bottom: 12px solid #7ab950;
  border-left: 12px solid transparent;
  position: absolute;
  left: -12px;
  bottom: 0;
}

:deep(.ag-filter-ext-filter-one::after) {
  content: '';
  width: 0;
  height: 0;
  border-top: 12px solid #7ab950;
  border-left: 12px solid transparent;
  position: absolute;
  left: -12px;
  top: 0;
}

:deep(.ag-virtual-list-item:hover .ag-filter-ext-filter-one) {
  display: block !important;
}


:deep(.editable-header) {
  color: #0094ff;
}

:deep(.ag-center-cell) {
  display: flex;
  align-items: center;
}
</style>
