import {GridOptions} from "ag-grid-community";
import {IQueryParam, ITableColumnSchema} from "@/types/dto";
import {R} from "@/utils/R";
import {Dayjs} from "dayjs";
import type {GridApi} from "ag-grid-community/dist/types/core/api/gridApi";

export interface IButton {
    /**
     * @see {IconSelectInput}
     */
    icon?: string,
    name?: string,
    action?: () => void,
    role?: string,
    sub?: IButton[],
}

export interface ObjectValueUpdateEmits {
    (e: "update:value", val: object | undefined): void;
}

export interface ValueUpdateEmits {
    (e: "update:value", val: string | undefined): void;
}

export interface BoolValueUpdateEmits {
    (e: "update:value", val: boolean | undefined): void;
}

export interface RowDataUpdateEmits {
    (e: "update:rowData", val: any[] | undefined): void;
}

export interface SchemaUpdateEmits {
    (e: "update:schema", val: Record<string, any> | undefined): void;
}

export interface showUpdateEmits {
    (e: "update:show", val: boolean | undefined): void;
}

export interface QueryConditionsUpdateEmits {
    (e: "update:queryConditions", val: Record<string, any> | undefined): void;
}

export interface ActiveKeyUpdateEmits {
    (e: "update:activeKey", val: string | undefined): void;
}


export interface IEditFieldSchema {
    label: string,
    field: string,
    fieldType: string,
    defaultValue?: any,
    require?: boolean,
    allowClear?: boolean,
    tip?: string,
    placeholder?: string,
    labelColSpan?: number,
    labelColOffset?: number,
    wrapperColSpan?: number,
    wrapperColOffset?: number,
    disabled?: boolean | undefined,
    span?: number,
    offset?: number,
    groupName?: string,
    /**
     * @see {ModuleEditor}
     */
    module: IModuleVm,
}

export interface IDetailTableSchema {
    tab?: string;
    field: string;
    tableSchema: IFullTableSchema;
}

export interface IFullTableSchema {
    /**
     * 工具条
     */
    tableTools?: ITableToolBarItemSchema[] | undefined;
    tableName?: string;
    gridOptions?: GridOptions | undefined;
    queryDataUrl?: string | ((queryObj: IQueryParam | undefined) => Promise<R<any[]>>);
    queryCountUrl?: string | ((queryObj: IQueryParam | undefined) => Promise<R<number>>);
    deleteIdsUrl?: string | (() => Promise<any>);
    rowSelection?: 'multiple' | 'single' | undefined;
    primaryKey?: string | undefined;
    headerKey?: string | undefined;
    /**
     * 显示分页控件
     */
    showPageBar?: boolean;
    /**
     * 每页数量
     */
    pageSize?: number;
    autoQuery?: boolean;
    /**
     * 表格分页选项，多个逗号分隔
     */
    pageSizeOptions?: string;
    /**
     * 当前分页
     */
    page?: number;
    totalCount?: number;
    /**
     * 列字段
     */
    columns?: ITableColumnSchema[];

    /**
     * 查询条件，响应变化自动查询数据
     */
    queryConditions?: Record<string, any> | undefined;
    rowData?: any[] | [] | undefined;
    currentRow?: Record<string, any> | undefined;

    /**
     * 对外提供的Api
     */
    options?: {
        getSelectedRows: () => (any[] | undefined),
        onlineDeleteSelectedRows: () => Promise<R>,
        /**
         * 本地移除
         * @param ids
         */
        removeIds: (ids: string[] | undefined) => void;
        addNewRows: (rows: any[] | undefined) => void;
        /**
         * 根据Id查询数据，放入表格中
         * @param ids
         */
        refreshRows: (ids: string[]) => Promise<R>;
        scrollToLast: () => void;
        redrawRows: () => void;
    },
    agOptions?: {
        rowHeight?: number;
    }
}

export interface IGridApiObj {
    gridApi?: GridApi | undefined
}

export interface IDataBrowserSchema {

    /**
     * 查询条件容器
     */
    searchContainer?: ISearchContainerSchema | undefined;
    /**
     * 主表
     */
    mainTable?: IFullTableSchema | undefined,
    /**
     * 明细表格
     */
    detailTablesSchema?: IDetailTablesSchema | undefined;
}

export interface ITableConfig {
    multipleSelect?: boolean,
}

export interface ISearchGroupComponentSchema {
    /**
     * 当前选择的查询分组的查询条件
     */
    selectedGroupConditions?: Record<string, any> | undefined,
    /**
     * 默认查询条件
     */
    defaultConditions?: Record<string, any> | undefined,
    queryCountUrl: string;
    doSave: (conditions: Record<string, any>) => Promise<void>;
}

export interface IDetailTablesSchema {
    detailTables?: IDetailTableSchema[] | undefined;
    headerRow?: Record<string, any> | undefined;
}

export interface IToolBarSchema {
    buttons?: IButton[] | undefined
}

export interface ITableToolBarItemSchema {
    name: string,
    /**
     * @see {IconSelectInput}
     */
    icon?: string,
    index?: number | undefined,
    action?: (tableSchema: IFullTableSchema, par?: any) => Promise<void | any>,
    type?: TableToolType,
    /**
     * 子菜单
     */
    children?: ITableToolBarItemSchema[],
    /**
     * 特定组件（不能包含下拉）
     * @see {ModuleEditor}
     */
    module?: IModuleVm,
}

export interface ISearchContainerSchema {
    /**
     * 查询分组
     */
    searchGroup?: ISearchGroupComponentSchema;
    fields?: IEditFieldSchema[] | undefined;
    doQuery?: (conditions: Record<string, any>) => Promise<void>;
    doSave?: () => Promise<void>;
    queryConditions?: Record<string, any> | undefined;
}

export interface IObjectEditSchema {
    /**
     * 这个表单数据仅做第一次显示用，内部编辑后并不反馈到此值上面
     * 初次显示时，data中的数据会覆盖其他schema中的显示数据
     */
    data?: Record<string, any> | undefined;
    valueChanged?: boolean | undefined;
    fields?: IEditFieldSchema[] | undefined;
    detailTablesSchema?: IDetailTablesSchema | undefined;
    options?: {
        getData: () => Record<string, any> | undefined;
        validate: () => Promise<R>;
        showData: (val: Record<string, any> | undefined) => Promise<void>;
    }
}

export interface IModalObjectEditSchema extends ISchema {
    /**
     * 对象编辑结构
     */
    objectEditSchema: IObjectEditSchema,
    afterClose?: (r: R) => void,
    /**
     * 外部提供，内部保存前触发
     */
    validate?: ((r: Record<string, any> | undefined) => Promise<R>) | undefined,
    /**
     * 保存回调，外部提供，内部在保存动作执行时触发，保存内容到服务器
     */
    save?: ((r: Record<string, any> | undefined) => Promise<R>) | undefined,
    saveDataUrl?: string | undefined,
    getDataUrl?: string | undefined,
    dataId?: string | undefined,
    title?: string,
    sizeMode?: number,
    centered?: boolean,


}

export interface ISchema {

}

export interface IModalDataSelectSchema extends ISchema {
    dataBrowserSchema: IDataBrowserSchema,
    afterClose?: (r: R<any[]>) => void,
    title?: string,
    sizeMode?: number,
    centered?: boolean,
}

export interface ISchema {

}

export interface IModuleVm {
    id?: string,
    sysModuleId?: string,
    moduleName?: string,
    /**
     * @see {SysModuleSelectInput}
     */
    sysModuleName?: string,
    /**
     * @see {VueComSelectInput}
     */
    comPath?: string,
    categoryPath?: string,
    props?: Record<string, any>,
}

export interface IMenuVm {
    /**
     * 菜单名称
     */
    title: string,
    /**
     * 说明
     */
    desc?: string,
    /**
     * 图标
     * @see {IconSelectInput}
     */
    icon?: string,
    url: string,
    id: string,
    parentId: string,
    hidden?: boolean,
}

export type TableToolType = 'primary' | 'normal' | 'divider'

export enum Type2 {
    '仅下拉' = 1,
    staging = "staging",
    Val = 1,
    Val2 = 2,
    Val3 = '3',
    Val4 = 1234
}

export interface IQueryStringValue {
    type?: number,
    value?: string,
}

export interface IDateQueryValue {
    complexValue?: string,
}

export interface IStartEndDate {
    start?: Dayjs,
    end?: Dayjs,
}

export interface IQueryBetweenValue {
    start?: number,
    end?: number,
}

export interface IApiCall {
    apiUrl: string,
    queryParams?: Record<string, any> | undefined,
    postParams?: Record<string, any> | undefined,
    cacheable?: boolean | undefined,
}