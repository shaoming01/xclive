export interface ISearchGroup {
    id: string;
    name: string;
    count?: number | undefined;
    index: number;
    conditions?: Record<string, any>;
    path?: string,
}

export interface ITableColumnSchema {
    field?: string,
    headerName?: string,
    width?: number,
    editable?: boolean,
    tip?: string | undefined,
    propertyType?: string | undefined,
    cellRender?: ICellRenderSchema,
    autoRowHeight?: boolean,
    valueGetter?: IValueGetterSchema | ((params: ValueGetterParams) => string) | undefined,//兼容原Ag的方法
    /**
     * 禁止菜单
     */
    suppressSort?: boolean,
    /**
     * 禁止排序
     */
    suppressHeaderMenuButton?: boolean,
}

export interface ICellRenderSchema {
    comPath?: string,
    props?: Record<string, any>,
    canEdit?: boolean | undefined,
}

export interface IValueGetterSchema {
    funcName?: string,
    params?: Record<string, any>,
}

export interface IQueryParam {
    page: number,
    pageSize: number,
    queryObject: Record<string, any> | undefined,
    orderBy?: string | undefined,
}

export interface IValueDisplay {
    value: string,
    label: string,
    children?: IValueDisplay[],
}

