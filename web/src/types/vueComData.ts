export interface IVueComData {
    vueComponents: Record<string, IVueComInfo>
    interfaceTypes: Record<string, IFieldInfo[]>
    enumTypes: Record<string, IEnumItemInfo[]>
}

export interface IVueComInfo {
    displayName: string;
    description: string;
    exportName?: string;
    tags?: any | undefined;
    props?: IFieldInfo[];
    slots?: INameProp[] | undefined
    expose?: INameProp[] | undefined
}

export interface IFieldInfo {
    name: string;
    type: string;
    description: string;
    required?: boolean;
    see?: string | undefined | null;
    defaultValue?: object | undefined | null | any;
    tags?: any | undefined;
    /**
     * 不需要创建编辑器
     */
    ignore?: boolean;
}

export interface IEnumItemInfo {
    name: string;
    value: string;
}

export interface INameProp {
    name: string
}