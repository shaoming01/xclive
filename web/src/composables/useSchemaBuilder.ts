import {ref} from 'vue';
import {IDetailTableSchema, IEditFieldSchema, IFullTableSchema, IObjectEditSchema} from "@/types/schema";
import {vueComDoc} from "@/utils/vueComDoc";
import {IEnumItemInfo, IFieldInfo, IVueComData} from "@/types/vueComData";
import {ITableColumnSchema} from "@/types/dto";
import {modalUtil} from "@/utils/modalUtil";

export function useSchemaBuilder() {
    const comData = vueComDoc as IVueComData;

    /**
     * 创建Vue组件属性编辑器的Schema，这个Schema给到编辑器，编辑器就能渲染出编辑UI，编辑完成得到的属性结构可以用于渲染该VUE组件
     * @param vueComPath
     */
    function createVueSchema(vueComPath: string): IObjectEditSchema | undefined {
        if (!vueComPath) return undefined;
        if (!comData.vueComponents.hasOwnProperty(vueComPath)) return undefined;
        const vueType = comData.vueComponents[vueComPath];
        if (!vueType.props)
            return undefined;


        return createEditSchemaByProps(vueType.props.filter(p => !p.ignore));
    }

    /**
     * 创建某Type的编辑控件的Schema
     * @param typeName
     */
    function createTypeSchema(typeName: string): IObjectEditSchema | undefined {
        if (!typeName) {
            console.warn('找不到Type定义' + typeName)
            return undefined;
        }
        const props = comData.interfaceTypes[typeName];
        if (!props)
            return undefined;
        return createEditSchemaByProps(props.filter(p => !p.ignore));

    }

    function createEditSchemaByProps(props: IFieldInfo[], typeTree: string[] = [], nameSpace?: string, nameSpaceText?: string): IObjectEditSchema | undefined {
        let schema: IObjectEditSchema = {};
        for (const prop of props) {

            const typeInfo = getTypeInfo(prop.type);
            const stackLength = typeTree.filter(t => t == prop.type).length;//递归层数，数组允许2层，普通对象1层
            if ((typeInfo.isArray && stackLength > 2) || (!typeInfo.isArray && stackLength > 0)) {
                console.log(prop.type + '中有属性引用了自身，防止死循环，不解析此字段')
                continue;
            }
            if (typeInfo.isArray) {
                const edt = createDetailSchema(prop, typeTree, nameSpace, nameSpaceText);
                if (edt) {
                    schema.detailTablesSchema = schema.detailTablesSchema ?? {}
                    schema.detailTablesSchema.detailTables = schema.detailTablesSchema.detailTables ?? []
                    schema.detailTablesSchema.detailTables.push(edt);
                }
            } else if (typeInfo.isNumber || typeInfo.isString || typeInfo.isBool || prop.see || typeInfo.enumItems) {
                const edt = createFieldEditorSchema(prop, nameSpace, nameSpaceText);
                if (edt) {
                    schema.fields = schema.fields ?? []
                    schema.fields.push(edt);
                }

            } else if (typeInfo.typeProps) {
                const tmpTree = [...typeTree];
                tmpTree.push(prop.type)
                const localNameSpace = (nameSpace ? nameSpace + '.' : '') + prop.name;
                const localNameSpaceText = (nameSpaceText ? nameSpaceText + '.' : '') + (prop.description || prop.name);

                const subSchema = createEditSchemaByProps(typeInfo.typeProps, tmpTree, localNameSpace, localNameSpaceText);
                if (subSchema) {
                    schema = ext.appendSchema(schema, subSchema)
                }
            } else if (typeInfo.isFunction) {
            } else {
            }
        }
        return schema;
    }


    function createDetailSchema(prop: IFieldInfo, typeTree: string[] = [], nameSpace?: string, nameSpaceText?: string): IDetailTableSchema | undefined {
        if (prop.name == 'mainTable') {
            debugger
        }
        const baseNameSpace = nameSpace ? nameSpace + '.' : '';
        const baseNameSpaceText = nameSpaceText ? nameSpaceText + '.' : '';
        const typeInfo = getTypeInfo(prop.type);
        if (!typeInfo.isArray || !typeInfo.arrayType || typeInfo.arrayType == prop.type)
            return;

        const comData = vueComDoc as IVueComData;
        const typeProps = comData.interfaceTypes[typeInfo.arrayType]?.filter(p => !p.ignore);
        if (!typeProps) {
            console.warn('找不到类型:' + typeInfo.arrayType);
            return undefined;
        }

        const cols: ITableColumnSchema[] = typeProps.map(prop => {
            return {
                headerName: ext.isNullOrEmpty(prop.description) ? prop.name : prop.description,
                field: prop.name,
                editable: true,
                width: 150,
            }
        });

        const tableSchema: IFullTableSchema = {
            columns: cols,
            tableTools: [
                {
                    name: '添加', action: async () => {
                        const typeEditSchema = createEditSchemaByProps(typeProps, [...typeTree, prop.type]);
                        if (!typeEditSchema) {
                            console.warn('获取不到该类型的结构：' + typeInfo.arrayType);
                            return;
                        }
                        typeEditSchema.data = {};//因为结构中有数据，所以每次使用这个结构前需要把数据处理掉
                        return modalUtil.showModalEditor({
                            objectEditSchema: typeEditSchema,
                            sizeMode: 5,
                            save: async (data) => {
                                if (!tableSchemaRef.value || !tableSchemaRef.value.rowData) {
                                    return R.error('拿不到表格结构对象')
                                }
                                tableSchemaRef.value.rowData.push(data as never)
                                tableSchemaRef.value.rowData = [...tableSchemaRef.value.rowData];
                                return R.ok();
                            }
                        })
                    },
                }, {
                    name: '修改', action: async () => {
                        if (!tableSchemaRef.value.currentRow) {
                            return msg.error('请选择1条数据后进行此操作')
                        }
                        const typeEditSchema = createEditSchemaByProps(typeProps, [...typeTree, prop.type]);
                        if (!typeEditSchema) {
                            console.warn('获取不到该类型的结构：' + typeInfo.arrayType);
                            return;
                        }
                        typeEditSchema.data = {...tableSchemaRef.value.currentRow}
                        const edtSchemaRef = ref(typeEditSchema);
                        return modalUtil.showModalEditor({
                            objectEditSchema: edtSchemaRef.value,
                            save: async (data) => {
                                if (!tableSchemaRef.value || !tableSchemaRef.value.rowData) {
                                    return R.error('拿不到表格结构对象')
                                }
                                ext.coverObject(tableSchemaRef.value.currentRow, data);

                                //强制表格刷新
                                tableSchemaRef.value.rowData = [...tableSchemaRef.value.rowData];
                                return R.ok();
                            },
                            sizeMode: 5,
                        })
                    },
                }, {
                    name: '删除', action: async () => {
                        if (!tableSchemaRef.value.currentRow) {
                            return msg.error('请选择1条数据后进行此操作')
                        }
                        if (!await msg.confirm('请确认删除选中数据?')) {
                            return;
                        }
                        tableSchemaRef.value.rowData = tableSchemaRef.value.rowData ?? [];
                        const index = tableSchemaRef.value.rowData.findIndex(item => item === tableSchemaRef.value.currentRow);
                        if (index < 0)
                            return msg.error('找不到要删除的数据,请刷新后重试');
                        tableSchemaRef.value.rowData.splice(index, 1)
                        tableSchemaRef.value.rowData = [...tableSchemaRef.value.rowData];

                    },
                }, {
                    name: '刷新表格', action: async () => {
                        tableSchemaRef.value.rowData = tableSchemaRef.value.rowData ?? [];
                        tableSchemaRef.value.rowData = [...tableSchemaRef.value.rowData];
                    },
                },
            ],
            rowData: [],
        };
        const tableSchemaRef = ref<IFullTableSchema>(tableSchema);
        return {
            tableSchema: tableSchemaRef.value,
            tab: baseNameSpaceText + (prop.description || prop.name),
            field: baseNameSpace + prop.name,
        };
    }

    function createFieldEditorSchema(prop: IFieldInfo, nameSpace?: string, nameSpaceText?: string): IEditFieldSchema | undefined {
        const baseNameSpace = nameSpace ? nameSpace + '.' : '';
        const typeInfo = getTypeInfo(prop.type);
        if (prop.see) {

            const re: IEditFieldSchema = {
                field: baseNameSpace + prop.name,
                label: ext.isNullOrEmpty(prop.description) ? prop.name : prop.description,
                groupName: nameSpaceText ?? '',
                fieldType: prop.type,
                require: prop.required,
                module: {
                    comPath: getComPath(prop.see), props: {}, id: '', moduleName: '', categoryPath: '',
                },
            };
            if (prop.see == 'ModuleEditor') {
                re.span = 24;
                re.labelColSpan = 0;
                re.wrapperColSpan = 24;
                re.label = '';
                re.groupName = '子组件'
            }
            return re;
        } else if (typeInfo.isString || typeInfo.isNumber) {
            return {
                field: baseNameSpace + prop.name,
                label: ext.isNullOrEmpty(prop.description) ? prop.name : prop.description,
                groupName: nameSpaceText ?? '',
                fieldType: prop.type,
                require: prop.required,
                module: {
                    comPath: getComPath('StringInput'),
                    props: {}, id: '', moduleName: '', categoryPath: '',
                },
            }
        } else if (typeInfo.isBool) {
            return {
                field: baseNameSpace + prop.name,
                label: ext.isNullOrEmpty(prop.description) ? prop.name : prop.description,
                groupName: nameSpaceText ?? '',
                fieldType: prop.type,
                require: prop.required,
                module: {
                    comPath: getComPath('BoolInput'),
                    props: {}, id: '', moduleName: '', categoryPath: '',
                },
            }

        } else if (typeInfo.enumItems) {
            return {
                field: baseNameSpace + prop.name,
                label: ext.isNullOrEmpty(prop.description) ? prop.name : prop.description,
                groupName: nameSpaceText ?? '',
                fieldType: prop.type,
                require: prop.required,
                module: {
                    comPath: getComPath('DataSelectInput'),
                    props: {
                        dataSource: typeInfo.enumItems.map(it => {
                            return {value: it.value, label: it.name}
                        })
                    }, id: '', moduleName: '', categoryPath: '',
                },
            }

        }
    }

    function getComPath(comName: string): string | undefined {
        const comData = vueComDoc as IVueComData;
        const path = Object.keys(comData.vueComponents).find(key => comData.vueComponents[key].displayName == comName);
        if (!path) {
            console.warn('vueComDoc中找不到控件定义' + comName);
        }
        return path;
    }

    function getTypeInfo(type: string): ITypeInfo {
        const types = type.split('|').map(t => t.trim());
        //先判断简单类型
        if (types.includes('string'))
            return {isString: true}
        if (types.includes('boolean'))
            return {isBool: true}
        if (types.includes('number'))
            return {isNumber: true}
        if (types.some(t => t == 'TSParenthesizedType' || t.includes('=>')))
            return {isFunction: true}
        if (types.some(t => t == '[]' || t.endsWith('[]'))) {
            const tt = types.find(t => t == '[]' || t.endsWith('[]')) ?? '[]';
            return {
                isArray: true,
                arrayType: tt.substring(0, type.length - 2)
            }
        }

        const comData = vueComDoc as IVueComData;
        const interfaceTypes = types.find(t => comData.interfaceTypes.hasOwnProperty(t));
        if (interfaceTypes) {
            return {typeProps: comData.interfaceTypes[interfaceTypes]?.filter(p => !p.ignore)}
        }
        const enumTypes = types.find(t => comData.enumTypes.hasOwnProperty(t));
        if (enumTypes) {
            return {enumItems: comData.enumTypes[enumTypes]}
        }
        return {}
    }

    interface ITypeInfo {
        isArray?: boolean,
        isString?: boolean,
        isBool?: boolean,
        isNumber?: boolean,
        isFunction?: boolean,
        enumItems?: IEnumItemInfo[],
        arrayType?: string,
        typeProps?: IFieldInfo[],
    }


    return {
        createVueSchema, createTypeSchema
    };
}