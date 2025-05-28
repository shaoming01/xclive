<template>
  <DataBrowser v-if="schema" v-model:schema="schema"></DataBrowser>
</template>

<script lang="ts" setup>

import {IDataBrowserSchema, IModuleVm} from "@/types/schema";
import {IQueryParam} from "@/types/dto";
import {R} from "@/utils/R";
import {IVueComData} from "@/types/vueComData";
import {vueComDoc} from "@/utils/vueComDoc";


const props = defineProps<{
  schema?: IDataBrowserSchema,
}>();
const schema = ref<IDataBrowserSchema>();
schema.value = ext.appendSchema(props.schema, createSchema()) as IDataBrowserSchema;
const router = useRouter()

function createSchema(): IDataBrowserSchema {
  return {
    mainTable: {
      tableTools: [{
        name: '添加模块', action: async () => {
          const row = schema.value?.mainTable?.currentRow;
          if (!row)
            return msg.error('请选择一个组件');
          const module: IModuleVm = {
            id: '0',
            moduleName: '未命名',
            comPath: row['path'],
            props: {},
            categoryPath: '',
          };
          const moduleJson = JSON.stringify(module);
          return router.push({
            path: '/moduleDesigner',
            query: {moduleJson: moduleJson, title: '模块新增'},
          });

        }
      }]
      , columns: [
        {width: 150, editable: false, headerName: '组件名称', field: 'name'},
        {width: 150, editable: false, headerName: 'id', field: 'id'},
        {width: 300, editable: false, headerName: '组件路径', field: 'path'},
        {width: 300, editable: false, headerName: '说明', field: 'desc'},
      ],
      showPageBar: false,
      gridOptions: {
        treeData: true,
        groupDefaultExpanded: -1,
        getDataPath: row => {
          let path = row.path.replace('/src/', '');
          return path.split('/');
        }
      },
      queryDataUrl: queryComponents

    },
    detailTablesSchema: {
      detailTables: [
        {
          tab: '属性列表',
          tableSchema: {
            columns: [
              {field: 'propName', headerName: '属性名称', width: 180},
              {field: 'propType', headerName: '属性类型', width: 300},
              {field: 'require', headerName: '必须', width: 120},
              {field: 'defaultValue', headerName: '默认值', width: 120},
              {field: 'desc', headerName: '说明', width: 240},
            ], gridOptions: {
              treeData: true,
              groupDefaultExpanded: -1,
              getDataPath: row => {
                const _row = row as IPropVm
                return _row.path
              }
            },
            queryDataUrl: queryComponentProps,
            showPageBar: false,

          },
          field: 'props'
        }]
    }
  }
}

async function queryComponents(queryObj: IQueryParam | undefined): Promise<R<any[]>> {
  const comData = vueComDoc as IVueComData;
  const list = [];

  for (let comPath in comData.vueComponents) {
    list.push({
      id: comPath,
      path: comPath,
      name: comData.vueComponents[comPath].displayName,
      desc: comData.vueComponents[comPath].description
    })
  }

  //查询目录中的组件列出来
  return R.ok(list);
}

watch(() => schema.value?.mainTable?.currentRow, () => {
  if (schema.value?.detailTablesSchema)
    schema.value.detailTablesSchema.headerRow = schema.value?.mainTable?.currentRow;
})


async function queryComponentProps(queryObj: IQueryParam | undefined): Promise<R<any[]>> {
  if (!queryObj || !queryObj.queryObject) return R.ok([]);
  const headerId = queryObj?.queryObject.headerId;
  if (!headerId) return R.ok([]);
  const comData = vueComDoc as IVueComData;
  const com = comData.vueComponents[headerId];
  const props = com.props ?? [];
  const reList: IPropVm[] = [];


  for (const prop of props) {
    reList.push({
      path: [prop.name],
      desc: prop.description,
      defaultValue: prop.defaultValue,
      propName: prop.name,
      propType: prop.type,
      require: prop.required ?? false,
    })
    const subProps = getTypeProps(prop.type, [prop.name]);
    reList.push(...subProps)

  }

  console.log('查询明细:' + JSON.stringify(queryObj))
  //查询目录中的组件列出来
  return R.ok(reList);
}

interface IPropVm {
  path: string[],
  propName: string,
  propType: string,
  require?: boolean,
  defaultValue: string | undefined,
  desc: string
}

function getTypeProps(type: string, basePath: string[]): IPropVm[] {
  const reList: IPropVm[] = [];

  const typeName = type.endsWith('[]') ? type.substring(0, type.length - 2) : type;
  if (!typeName) return reList;//单纯的数组类型，类型就是[]
  const comData = vueComDoc as IVueComData;

  if (!comData.interfaceTypes[typeName]) return reList;
  for (const typeInfo of comData.interfaceTypes[typeName]) {
    const subPath = [...basePath];
    subPath.push(typeInfo.name);
    reList.push({
      path: subPath,
      desc: typeInfo.description,
      propName: typeInfo.name,
      propType: typeInfo.type,
      require: typeInfo.required,
      defaultValue: undefined,
    })
    if (typeInfo.type == type) {
      return reList;//有个属性类型是自己，防止死循环
    }
    const subProps = getTypeProps(typeInfo.type, subPath);
    reList.push(...subProps)
  }

  return reList;
}


onMounted(() => {
  if (schema.value && schema.value.mainTable)
    schema.value.mainTable.queryConditions = {};
})
console.log(schema)


</script>

