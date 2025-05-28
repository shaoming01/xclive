export const vueComDoc = {
    "vueComponents": {
        "/src/components/base/DataBrowser.vue": {
            "exportName": "default",
            "displayName": "DataBrowser",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "IDataBrowserSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/base/Icon.vue": {
            "exportName": "default",
            "displayName": "Icon",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "name",
                    "type": "string | undefined",
                    "tags": {
                        "see": [
                            {
                                "description": "{IconSelectInput}",
                                "title": "see"
                            }
                        ]
                    },
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "IconSelectInput",
                    "ignore": false
                }
            ]
        },
        "/src/components/base/LoadingBtn.vue": {
            "exportName": "default",
            "displayName": "LoadingBtn",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "click",
                    "type": "TSFunctionType",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "onClick",
                    "type": "TSFunctionType",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ],
            "slots": [
                {
                    "name": "icon"
                },
                {
                    "name": "default"
                }
            ]
        },
        "/src/components/base/Markdown.vue": {
            "exportName": "default",
            "displayName": "Markdown",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "string | undefined",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/base/ModalDataSelect.vue": {
            "exportName": "default",
            "displayName": "ModalDataSelect",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "IModalDataSelectSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/base/ModalObjectEditor.vue": {
            "exportName": "default",
            "displayName": "ModalObjectEditor",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "IModalObjectEditSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/base/ModuleRender.vue": {
            "exportName": "default",
            "displayName": "ModuleRender",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "moduleId",
                    "type": "string",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "sysModuleId",
                    "type": "string",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "module",
                    "type": "IModuleVm",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/base/ObjectEditor.vue": {
            "exportName": "default",
            "displayName": "ObjectEditor",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "IObjectEditSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/base/SearchContainer.vue": {
            "exportName": "default",
            "displayName": "SearchContainer",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "ISearchContainerSchema",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/base/SearchToolBar.vue": {
            "exportName": "default",
            "displayName": "SearchToolBar",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "doQuery",
                    "type": "TSFunctionType",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "doReset",
                    "type": "TSFunctionType",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "doSave",
                    "type": "TSFunctionType",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/base/Shadow.vue": {
            "exportName": "default",
            "displayName": "Shadow",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "color",
                    "type": "string",
                    "description": "",
                    "required": false,
                    "defaultValue": {
                        "func": false,
                        "value": "'rgba(0, 0, 0, 0.45)'"
                    },
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "contentCenter",
                    "type": "boolean",
                    "description": "",
                    "required": false,
                    "defaultValue": {
                        "func": false,
                        "value": "false"
                    },
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "gapTop",
                    "type": "string",
                    "description": "",
                    "required": false,
                    "defaultValue": {
                        "func": false,
                        "value": "'5rem'"
                    },
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "gapBottom",
                    "type": "string",
                    "description": "",
                    "required": false,
                    "defaultValue": {
                        "func": false,
                        "value": "'5rem'"
                    },
                    "see": "",
                    "ignore": false
                }
            ],
            "events": [
                {
                    "name": "shadowClick"
                }
            ],
            "slots": [
                {
                    "name": "default"
                }
            ]
        },
        "/src/components/base/TableToolBar.vue": {
            "exportName": "default",
            "displayName": "TableToolBar",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "items",
                    "type": "ITableToolBarItemSchema[]",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "tableSchema",
                    "type": "IFullTableSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/base/TableToolBarItem.vue": {
            "exportName": "default",
            "displayName": "TableToolBarItem",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "item",
                    "type": "ITableToolBarItemSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "isInMenu",
                    "type": "boolean",
                    "description": "不用填",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "tableSchema",
                    "type": "IFullTableSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/base/ToolBar.vue": {
            "exportName": "default",
            "displayName": "ToolBar",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "IToolBarSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/edit/bill/SysModuleSelectInput.vue": {
            "exportName": "default",
            "displayName": "SysModuleSelectInput",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "string | undefined",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/edit/bill/SysSchemaSelectInput.vue": {
            "exportName": "default",
            "displayName": "SysSchemaSelectInput",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "string | undefined",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/edit/complex/DataSelectInput.vue": {
            "exportName": "default",
            "displayName": "DataSelectInput",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "string | undefined",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "allowClear",
                    "type": "boolean | undefined",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "disabled",
                    "type": "boolean | undefined",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "placeholder",
                    "type": "string | undefined",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "multiple",
                    "type": "boolean",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "dataSourceApi",
                    "type": "IApiCall",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "dataSource",
                    "type": "IValueDisplay[]",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ],
            "slots": [
                {
                    "name": "default"
                }
            ]
        },
        "/src/components/edit/complex/DateSearch.vue": {
            "exportName": "default",
            "displayName": "DateSearch",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "IDateQueryValue",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "showTime",
                    "type": "boolean",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/edit/complex/NumberSearch.vue": {
            "exportName": "default",
            "displayName": "NumberSearch",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "IQueryBetweenValue",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "precision",
                    "type": "number",
                    "tags": {
                        "description": [
                            {
                                "description": "0无小数",
                                "title": "description"
                            }
                        ]
                    },
                    "description": "小数位数",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/edit/complex/StringSearch.vue": {
            "exportName": "default",
            "displayName": "StringSearch",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "IQueryStringValue",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/edit/complex/TreeSelectInput.vue": {
            "exportName": "default",
            "displayName": "TreeSelectInput",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "string | undefined",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "allowClear",
                    "type": "boolean | undefined",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "disabled",
                    "type": "boolean | undefined",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "placeholder",
                    "type": "string | undefined",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "dataSourceApi",
                    "type": "IApiCall",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "dataSource",
                    "type": "IValueDisplay[]",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "multiple",
                    "type": "boolean",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/edit/complex/VueComSelectInput.vue": {
            "exportName": "default",
            "displayName": "VueComSelectInput",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "string | undefined",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/edit/simple/BoolInput.vue": {
            "exportName": "default",
            "displayName": "BoolInput",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "boolean",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "disabled",
                    "type": "boolean",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/edit/simple/DateInput.vue": {
            "exportName": "default",
            "displayName": "DateInput",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "string",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "style",
                    "type": "string",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/edit/simple/NumberInput.vue": {
            "exportName": "default",
            "displayName": "NumberInput",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "string",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "min",
                    "type": "number",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "max",
                    "type": "number",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "keyboard",
                    "type": "boolean",
                    "description": "键盘行为",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "disabled",
                    "type": "boolean",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "controls",
                    "type": "boolean",
                    "description": "增减按钮",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "precision",
                    "type": "number",
                    "tags": {
                        "description": [
                            {
                                "description": "0无小数",
                                "title": "description"
                            }
                        ]
                    },
                    "description": "小数位数",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "prefixIcon",
                    "type": "string",
                    "tags": {
                        "see": [
                            {
                                "description": "{IconSelectInput}",
                                "title": "see"
                            }
                        ]
                    },
                    "description": "前辍图标",
                    "required": false,
                    "defaultValue": null,
                    "see": "IconSelectInput",
                    "ignore": false
                }
            ]
        },
        "/src/components/edit/simple/StringInput.vue": {
            "exportName": "default",
            "displayName": "StringInput",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "string",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "password",
                    "type": "boolean | undefined",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "rows",
                    "type": "number | undefined",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/edit/tool/IconSelectInput.vue": {
            "exportName": "default",
            "displayName": "IconSelectInput",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "string | undefined",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/edit/tool/JsonStringEditor.vue": {
            "exportName": "default",
            "displayName": "JsonStringEditor",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "string",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/edit/tool/ModuleEditor.vue": {
            "exportName": "default",
            "displayName": "ModuleEditor",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "value",
                    "type": "IModuleVm",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/grid/column/EmptyEditor.vue": {
            "exportName": "default",
            "displayName": "EmptyEditor",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "params",
                    "type": "any",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/grid/column/IconRender.vue": {
            "exportName": "default",
            "displayName": "IconRender",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "params",
                    "type": "any",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/grid/column/ImageRender.vue": {
            "exportName": "default",
            "displayName": "ImageRender",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "params",
                    "type": "any",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/grid/column/ListSelectRender.vue": {
            "exportName": "default",
            "displayName": "ListSelectRender",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "params",
                    "type": "any",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/grid/column/LongStringRender.vue": {
            "exportName": "default",
            "displayName": "LongStringRender",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "params",
                    "type": "any",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/grid/column/RowOptionsRender.vue": {
            "exportName": "default",
            "displayName": "RowOptionsRender",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "params",
                    "type": "any",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/grid/column/VoicePlayerRender.vue": {
            "exportName": "default",
            "displayName": "VoicePlayerRender",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "params",
                    "type": "any",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/grid/DetailTables.vue": {
            "exportName": "default",
            "displayName": "DetailTables",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "IDetailTablesSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/grid/FullTable.vue": {
            "exportName": "default",
            "displayName": "FullTable",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "IFullTableSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/grid/toolBar/Add.vue": {
            "exportName": "default",
            "displayName": "Add",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "tableSchema",
                    "type": "IFullTableSchema",
                    "tags": {
                        "ignore": [
                            {
                                "description": true,
                                "title": "ignore"
                            }
                        ]
                    },
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": true
                },
                {
                    "name": "item",
                    "type": "ITableToolBarItemSchema",
                    "tags": {
                        "ignore": [
                            {
                                "description": true,
                                "title": "ignore"
                            }
                        ]
                    },
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": true
                },
                {
                    "name": "moduleId",
                    "type": "string",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "sysModuleId",
                    "type": "string",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/grid/toolBar/Delete.vue": {
            "exportName": "default",
            "displayName": "Delete",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "tableSchema",
                    "type": "IFullTableSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "item",
                    "type": "ITableToolBarItemSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "moduleId",
                    "type": "string",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/grid/toolBar/Edit.vue": {
            "exportName": "default",
            "displayName": "Edit",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "tableSchema",
                    "type": "IFullTableSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "item",
                    "type": "ITableToolBarItemSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "moduleId",
                    "type": "string",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "sysModuleId",
                    "type": "string",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/grid/toolBar/LocalDelete.vue": {
            "exportName": "default",
            "displayName": "LocalDelete",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "tableSchema",
                    "type": "IFullTableSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "item",
                    "type": "ITableToolBarItemSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "moduleId",
                    "type": "string",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/grid/toolBar/PlayLiveScriptDetail.vue": {
            "exportName": "default",
            "displayName": "PlayLiveScriptDetail",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "tableSchema",
                    "type": "IFullTableSchema",
                    "tags": {
                        "ignore": [
                            {
                                "description": true,
                                "title": "ignore"
                            }
                        ]
                    },
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": true
                },
                {
                    "name": "item",
                    "type": "ITableToolBarItemSchema",
                    "tags": {
                        "ignore": [
                            {
                                "description": true,
                                "title": "ignore"
                            }
                        ]
                    },
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": true
                },
                {
                    "name": "moduleId",
                    "type": "string",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "sysModuleId",
                    "type": "string",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/layout/FlexLayout.vue": {
            "displayName": "FlexLayout",
            "description": "",
            "tags": {},
            "props": [],
            "slots": [
                {
                    "name": "default"
                }
            ]
        },
        "/src/components/layout/FlexLayoutContent.vue": {
            "displayName": "FlexLayoutContent",
            "description": "",
            "tags": {},
            "props": [],
            "slots": [
                {
                    "name": "default"
                }
            ]
        },
        "/src/components/layout/FlexLayoutFooter.vue": {
            "displayName": "FlexLayoutFooter",
            "description": "",
            "tags": {},
            "props": [],
            "slots": [
                {
                    "name": "default"
                }
            ]
        },
        "/src/components/layout/FlexLayoutHeader.vue": {
            "displayName": "FlexLayoutHeader",
            "description": "",
            "tags": {},
            "props": [],
            "slots": [
                {
                    "name": "default"
                }
            ]
        },
        "/src/components/layout/SearchGroup.vue": {
            "exportName": "default",
            "displayName": "SearchGroup",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "ISearchGroupComponentSchema",
                    "description": "",
                    "required": true,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/layout/SplitLayout.vue": {
            "exportName": "default",
            "displayName": "SplitLayout",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "horizontal",
                    "type": "boolean",
                    "description": "",
                    "required": false,
                    "defaultValue": {
                        "func": false,
                        "value": "false"
                    },
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "vertical",
                    "type": "boolean",
                    "description": "",
                    "required": false,
                    "defaultValue": {
                        "func": false,
                        "value": "false"
                    },
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "firstSplitter",
                    "type": "boolean",
                    "description": "",
                    "required": false,
                    "defaultValue": {
                        "func": false,
                        "value": "false"
                    },
                    "see": "",
                    "ignore": false
                }
            ],
            "slots": [
                {
                    "name": "default"
                }
            ]
        },
        "/src/components/layout/SplitPane.vue": {
            "exportName": "default",
            "displayName": "SplitPane",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "minSize",
                    "type": "number",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "maxSize",
                    "type": "number",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                },
                {
                    "name": "size",
                    "type": "number",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ],
            "slots": [
                {
                    "name": "default"
                }
            ]
        },
        "/src/components/pages/ChatGptTalk.vue": {
            "exportName": "default",
            "displayName": "ChatGptTalk",
            "description": "",
            "tags": {},
            "props": []
        },
        "/src/components/pages/GptTtsBrowser.vue": {
            "displayName": "GptTtsBrowser",
            "description": "",
            "tags": {},
            "props": []
        },
        "/src/components/pages/LiveScriptDataBrowser.vue": {
            "exportName": "default",
            "displayName": "LiveScriptDataBrowser",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "IDataBrowserSchema",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/pages/MenuDataBrowser.vue": {
            "exportName": "default",
            "displayName": "MenuDataBrowser",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "IDataBrowserSchema",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/pages/ModuleDataBrowser.vue": {
            "exportName": "default",
            "displayName": "ModuleDataBrowser",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "IDataBrowserSchema",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/pages/ModuleDesigner.vue": {
            "exportName": "default",
            "displayName": "ModuleDesigner",
            "description": "",
            "tags": {},
            "props": []
        },
        "/src/components/pages/role/RoleModalObjectEditor.vue": {
            "exportName": "default",
            "displayName": "RoleModalObjectEditor",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "IModalObjectEditSchema",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/pages/StockDataBrowser.vue": {
            "exportName": "default",
            "displayName": "StockDataBrowser",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "IDataBrowserSchema",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/pages/user/UserEditModalObjectEditor.vue": {
            "exportName": "default",
            "displayName": "UserEditModalObjectEditor",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "IModalObjectEditSchema",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/pages/VueComDataBrowser.vue": {
            "exportName": "default",
            "displayName": "VueComDataBrowser",
            "description": "",
            "tags": {},
            "props": [
                {
                    "name": "schema",
                    "type": "IDataBrowserSchema",
                    "description": "",
                    "required": false,
                    "defaultValue": null,
                    "see": "",
                    "ignore": false
                }
            ]
        },
        "/src/components/test/formTest.vue": {
            "exportName": "default",
            "displayName": "formTest",
            "description": "",
            "tags": {},
            "props": []
        }
    },
    "interfaceTypes": {
        "ISearchGroup": [
            {
                "name": "id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "count",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "index",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "conditions",
                "type": "Record<string, any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "path",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "ITableColumnSchema": [
            {
                "name": "field",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "headerName",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "width",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "editable",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "tip",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "propertyType",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "cellRender",
                "type": "ICellRenderSchema",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "autoRowHeight",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "valueGetter",
                "type": "IValueGetterSchema | ((params: ValueGetterParams) => string)",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "suppressSort",
                "type": "boolean",
                "description": "禁止菜单",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "suppressHeaderMenuButton",
                "type": "boolean",
                "description": "禁止排序",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "ICellRenderSchema": [
            {
                "name": "comPath",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "props",
                "type": "Record<string, any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "canEdit",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IValueGetterSchema": [
            {
                "name": "funcName",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "params",
                "type": "Record<string, any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IQueryParam": [
            {
                "name": "page",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "pageSize",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "queryObject",
                "type": "Record<string, any>",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "orderBy",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IValueDisplay": [
            {
                "name": "value",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "label",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "children",
                "type": "IValueDisplay[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IButton": [
            {
                "name": "icon",
                "type": "string",
                "description": "",
                "required": false,
                "see": "IconSelectInput",
                "ignore": false
            },
            {
                "name": "name",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "action",
                "type": "() => void",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "role",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "sub",
                "type": "IButton[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "ObjectValueUpdateEmits": [],
        "ValueUpdateEmits": [],
        "BoolValueUpdateEmits": [],
        "RowDataUpdateEmits": [],
        "PropUpdateEmits": [],
        "QueryConditionsUpdateEmits": [],
        "ActiveKeyUpdateEmits": [],
        "IEditFieldProp": [
            {
                "name": "label",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "field",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "fieldType",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "defaultValue",
                "type": "any",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "require",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "tip",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "labelColSpan",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "labelColOffset",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "wrapperColSpan",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "wrapperColOffset",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "disabled",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "span",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "offset",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "groupName",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "schema",
                "type": "ISchemaVm",
                "description": "",
                "required": true,
                "see": "SchemaEditor",
                "ignore": false
            }
        ],
        "IDetailTableProp": [
            {
                "name": "tab",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "field",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "tableProp",
                "type": "IFullTableProp",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IFullTableProp": [
            {
                "name": "tableTools",
                "type": "ITableToolBarItemProp[]",
                "description": "工具条",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "tableName",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "gridOptions",
                "type": "GridOptions<any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "queryDataUrl",
                "type": "string | ((queryObj: any) => Promise<R<any[]>>)",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "queryCountUrl",
                "type": "string | ((queryObj: any) => Promise<R<any[]>>)",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "deleteIdsUrl",
                "type": "string | (() => Promise<any>)",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "rowSelection",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "primaryKey",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "headerKey",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "showPageBar",
                "type": "boolean",
                "description": "显示分页控件",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "pageSize",
                "type": "number",
                "description": "每页数量",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "pageSizeOptions",
                "type": "string",
                "description": "表格分页选项，多个逗号分隔",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "page",
                "type": "number",
                "description": "当前分页",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "autoQuery",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "totalCount",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "columns",
                "type": "ITableColumnSchema[]",
                "description": "列字段",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "queryConditions",
                "type": "Record<string, any>",
                "description": "查询条件，响应变化自动查询数据",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "rowData",
                "type": "any[] | []",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "currentRow",
                "type": "Record<string, any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "options",
                "type": "{ getSelectedRows: () => any[]; onlineDeleteSelectedRows: () => Promise<R>; removeIds: (ids: string[]) => void; addNewRows: (rows: any[]) => void; refreshRows: (ids: string[]) => Promise<R>; }",
                "description": "对外提供的Api",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IGridApiObj": [
            {
                "name": "gridApi",
                "type": "GridApi<any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IDataBrowserProp": [
            {
                "name": "searchContainer",
                "type": "ISearchContainerProp",
                "description": "查询条件容器",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "mainTable",
                "type": "IFullTableProp",
                "description": "主表",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "detailTablesProp",
                "type": "IDetailTablesProp",
                "description": "明细表格",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "ITableConfig": [
            {
                "name": "multipleSelect",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "ISearchGroupProp": [
            {
                "name": "selectedGroupConditions",
                "type": "Record<string, any>",
                "description": "当前选择的查询分组的查询条件",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "defaultConditions",
                "type": "Record<string, any>",
                "description": "默认查询条件",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "queryCountUrl",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "doSave",
                "type": "(conditions: Record<string, any>) => Promise<void>",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IDetailTablesProp": [
            {
                "name": "detailTables",
                "type": "IDetailTableProp[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "headerRow",
                "type": "Record<string, any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IToolBarProp": [
            {
                "name": "buttons",
                "type": "IButton[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "ITableToolBarItemProp": [
            {
                "name": "name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "icon",
                "type": "string",
                "description": "",
                "required": false,
                "see": "IconSelectInput",
                "ignore": false
            },
            {
                "name": "index",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "action",
                "type": "(tableProp: IFullTableProp, par?: any) => Promise<any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "type",
                "type": "TableToolType",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "children",
                "type": "ITableToolBarItemProp[]",
                "description": "子菜单",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "schema",
                "type": "ISchemaVm",
                "description": "特定组件（不能包含下拉）",
                "required": false,
                "see": "SchemaEditor",
                "ignore": false
            }
        ],
        "ISearchContainerProp": [
            {
                "name": "searchGroup",
                "type": "ISearchGroupProp",
                "description": "查询分组",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "fields",
                "type": "IEditFieldProp[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "doQuery",
                "type": "(conditions: Record<string, any>) => Promise<void>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "doSave",
                "type": "() => Promise<void>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "queryConditions",
                "type": "Record<string, any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IObjectEditorProp": [
            {
                "name": "data",
                "type": "Record<string, any>",
                "description": "这个表单数据仅做第一次显示用，内部编辑后并不反馈到此值上面 初次显示时，data中的数据会覆盖其他prop中的显示数据",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "valueChanged",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "fields",
                "type": "IEditFieldProp[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "detailTablesProp",
                "type": "IDetailTablesProp",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "options",
                "type": "{ getData: () => Record<string, any>; validate: () => Promise<R>; showData: (val: Record<string, any>) => Promise<void>; }",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IModalObjectEditorProp": [
            {
                "name": "objectEditorProp",
                "type": "IObjectEditorProp",
                "description": "对象编辑结构",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "afterClose",
                "type": "(r: R) => void",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "validate",
                "type": "(r: Record<string, any>) => Promise<R>",
                "description": "外部提供，内部保存前触发",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "save",
                "type": "(r: Record<string, any>) => Promise<R>",
                "description": "保存回调，外部提供，内部在保存动作执行时触发，保存内容到服务器",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "saveDataUrl",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "getDataUrl",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "dataId",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "title",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "sizeMode",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IProp": [],
        "IModalDataSelectProp": [
            {
                "name": "dataBrowserProp",
                "type": "IDataBrowserProp",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "afterClose",
                "type": "(r: R<any[]>) => void",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "title",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "sizeMode",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "ISchemaVm": [
            {
                "name": "id",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "schemaName",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "sysSchemaName",
                "type": "string",
                "description": "",
                "required": false,
                "see": "SysSchemaSelectInput",
                "ignore": false
            },
            {
                "name": "comPath",
                "type": "string",
                "description": "",
                "required": false,
                "see": "VueComSelectInput",
                "ignore": false
            },
            {
                "name": "categoryPath",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "props",
                "type": "Record<string, any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IMenuVm": [
            {
                "name": "title",
                "type": "string",
                "description": "菜单名称",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "desc",
                "type": "string",
                "description": "说明",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "icon",
                "type": "string",
                "description": "图标",
                "required": false,
                "see": "IconSelectInput",
                "ignore": false
            },
            {
                "name": "url",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "parentId",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "hidden",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IQueryStringValue": [
            {
                "name": "type",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "value",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IDateQueryValue": [
            {
                "name": "complexValue",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IStartEndDate": [
            {
                "name": "start",
                "type": "Dayjs",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "end",
                "type": "Dayjs",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IQueryBetweenValue": [
            {
                "name": "start",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "end",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IApiCall": [
            {
                "name": "apiUrl",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "queryParams",
                "type": "Record<string, any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "postParams",
                "type": "Record<string, any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "cacheable",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IAppInfo": [
            {
                "name": "version",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "exeVersion",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "productName",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "tenantId",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IUserRegisterVm": [
            {
                "name": "userName",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "password",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "cardKey",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IClientPackage": [
            {
                "name": "updateContent",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "version",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "url",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IUserLoginInfo": [
            {
                "name": "userId",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "name",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "days",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "token",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "SchemaUpdateEmits": [],
        "showUpdateEmits": [],
        "IEditFieldSchema": [
            {
                "name": "label",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "field",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "fieldType",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "defaultValue",
                "type": "any",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "require",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "allowClear",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "tip",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "placeholder",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "labelColSpan",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "labelColOffset",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "wrapperColSpan",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "wrapperColOffset",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "disabled",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "span",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "offset",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "groupName",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "module",
                "type": "IModuleVm",
                "description": "",
                "required": true,
                "see": "ModuleEditor",
                "ignore": false
            }
        ],
        "IDetailTableSchema": [
            {
                "name": "tab",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "field",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "tableSchema",
                "type": "IFullTableSchema",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IFullTableSchema": [
            {
                "name": "tableTools",
                "type": "ITableToolBarItemSchema[]",
                "description": "工具条",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "tableName",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "gridOptions",
                "type": "GridOptions<any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "queryDataUrl",
                "type": "string | ((queryObj: any) => Promise<R<any[]>>)",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "queryCountUrl",
                "type": "string | ((queryObj: any) => Promise<R<number>>)",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "deleteIdsUrl",
                "type": "string | (() => Promise<any>)",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "rowSelection",
                "type": "\"multiple\" | \"single\"",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "primaryKey",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "headerKey",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "showPageBar",
                "type": "boolean",
                "description": "显示分页控件",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "pageSize",
                "type": "number",
                "description": "每页数量",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "autoQuery",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "pageSizeOptions",
                "type": "string",
                "description": "表格分页选项，多个逗号分隔",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "page",
                "type": "number",
                "description": "当前分页",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "totalCount",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "columns",
                "type": "ITableColumnSchema[]",
                "description": "列字段",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "queryConditions",
                "type": "Record<string, any>",
                "description": "查询条件，响应变化自动查询数据",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "rowData",
                "type": "any[] | []",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "currentRow",
                "type": "Record<string, any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "options",
                "type": "{ getSelectedRows: () => any[]; onlineDeleteSelectedRows: () => Promise<R>; removeIds: (ids: string[]) => void; addNewRows: (rows: any[]) => void; refreshRows: (ids: string[]) => Promise<R>; scrollToLast: () => void; redrawRows: () => void; }",
                "description": "对外提供的Api",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "agOptions",
                "type": "{ rowHeight?: number; }",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IDataBrowserSchema": [
            {
                "name": "searchContainer",
                "type": "ISearchContainerSchema",
                "description": "查询条件容器",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "mainTable",
                "type": "IFullTableSchema",
                "description": "主表",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "detailTablesSchema",
                "type": "IDetailTablesSchema",
                "description": "明细表格",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "ISearchGroupComponentSchema": [
            {
                "name": "selectedGroupConditions",
                "type": "Record<string, any>",
                "description": "当前选择的查询分组的查询条件",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "defaultConditions",
                "type": "Record<string, any>",
                "description": "默认查询条件",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "queryCountUrl",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "doSave",
                "type": "(conditions: Record<string, any>) => Promise<void>",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IDetailTablesSchema": [
            {
                "name": "detailTables",
                "type": "IDetailTableSchema[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "headerRow",
                "type": "Record<string, any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IToolBarSchema": [
            {
                "name": "buttons",
                "type": "IButton[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "ITableToolBarItemSchema": [
            {
                "name": "name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "icon",
                "type": "string",
                "description": "",
                "required": false,
                "see": "IconSelectInput",
                "ignore": false
            },
            {
                "name": "index",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "action",
                "type": "(tableSchema: IFullTableSchema, par?: any) => Promise<any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "type",
                "type": "TableToolType",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "children",
                "type": "ITableToolBarItemSchema[]",
                "description": "子菜单",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "module",
                "type": "IModuleVm",
                "description": "特定组件（不能包含下拉）",
                "required": false,
                "see": "ModuleEditor",
                "ignore": false
            }
        ],
        "ISearchContainerSchema": [
            {
                "name": "searchGroup",
                "type": "ISearchGroupComponentSchema",
                "description": "查询分组",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "fields",
                "type": "IEditFieldSchema[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "doQuery",
                "type": "(conditions: Record<string, any>) => Promise<void>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "doSave",
                "type": "() => Promise<void>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "queryConditions",
                "type": "Record<string, any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IObjectEditSchema": [
            {
                "name": "data",
                "type": "Record<string, any>",
                "description": "这个表单数据仅做第一次显示用，内部编辑后并不反馈到此值上面 初次显示时，data中的数据会覆盖其他schema中的显示数据",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "valueChanged",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "fields",
                "type": "IEditFieldSchema[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "detailTablesSchema",
                "type": "IDetailTablesSchema",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "options",
                "type": "{ getData: () => Record<string, any>; validate: () => Promise<R>; showData: (val: Record<string, any>) => Promise<void>; }",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IModalObjectEditSchema": [
            {
                "name": "objectEditSchema",
                "type": "IObjectEditSchema",
                "description": "对象编辑结构",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "afterClose",
                "type": "(r: R) => void",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "validate",
                "type": "(r: Record<string, any>) => Promise<R>",
                "description": "外部提供，内部保存前触发",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "save",
                "type": "(r: Record<string, any>) => Promise<R>",
                "description": "保存回调，外部提供，内部在保存动作执行时触发，保存内容到服务器",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "saveDataUrl",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "getDataUrl",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "dataId",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "title",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "sizeMode",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "centered",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "ISchema": [],
        "IModalDataSelectSchema": [
            {
                "name": "dataBrowserSchema",
                "type": "IDataBrowserSchema",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "afterClose",
                "type": "(r: R<any[]>) => void",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "title",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "sizeMode",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "centered",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IModuleVm": [
            {
                "name": "id",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "sysModuleId",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "moduleName",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "sysModuleName",
                "type": "string",
                "description": "",
                "required": false,
                "see": "SysModuleSelectInput",
                "ignore": false
            },
            {
                "name": "comPath",
                "type": "string",
                "description": "",
                "required": false,
                "see": "VueComSelectInput",
                "ignore": false
            },
            {
                "name": "categoryPath",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "props",
                "type": "Record<string, any>",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IVueComData": [
            {
                "name": "vueComponents",
                "type": "Record<string, IVueComInfo>",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "interfaceTypes",
                "type": "Record<string, IFieldInfo[]>",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "enumTypes",
                "type": "Record<string, IEnumItemInfo[]>",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IVueComInfo": [
            {
                "name": "displayName",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "description",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "exportName",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "tags",
                "type": "any",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "props",
                "type": "IFieldInfo[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "slots",
                "type": "INameProp[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "expose",
                "type": "INameProp[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IFieldInfo": [
            {
                "name": "name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "type",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "description",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "required",
                "type": "boolean",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "see",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "defaultValue",
                "type": "any",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "tags",
                "type": "any",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "ignore",
                "type": "boolean",
                "description": "不需要创建编辑器",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IEnumItemInfo": [
            {
                "name": "name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "value",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "INameProp": [
            {
                "name": "name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IByLiveAccountResp": [
            {
                "name": "code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "msg",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "data",
                "type": "{ buyin_account_id: string; user_name: string; user_id: string; origin_uid: string; shop_id: string; }",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IByPromotionResp": [
            {
                "name": "code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "msg",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "total",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "data",
                "type": "[{ promotion_id: string; product_id: string; price: number; cos_fee: number; cos_ratio: number; title: string; new_cover: string; price_text: string; on_shelf_info: { can_show: boolean; }; }]",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IByLiveListResp": [
            {
                "name": "code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "msg",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "data",
                "type": "{ basic_list: [{ promotion_id: string; product_id: string; }]; }",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IByPromotionLiveResp": [
            {
                "name": "code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "msg",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "data",
                "type": "{ promotions: { promotion_id: string; product_id: string; title: string; cover: string; price_desc: { min_price: { origin: number; integer: string; decimal: string; }; price_text: string; }; stock_num: string; }[]; }",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IByBindToLiveResp": [
            {
                "name": "code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "msg",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "data",
                "type": "{ success_count: number; failure_count: number; partial_failure_count: number; failure_list?: [{ bind_status: number; bind_reason: string; product_id: string; title: string; }]; }",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IByPackH5Resp": [
            {
                "name": "status_code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "promotion_h5",
                "type": "{ basic_info_data: { price_info: { price: { min_price: number; suffix: number; }; discount_price: { prefix: string; min_price: number; suffix: string; }; }; title_info: { title: string; }; }; }",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IBYGptProductDescResp": [
            {
                "name": "code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "msg",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "data",
                "type": "{ gtp_generated_product_desc: string; }",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IByPackDetailResp": [
            {
                "name": "status_code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "detail_info",
                "type": "{ product_format: { format: { name: string; message: { desc: string; }[]; }[]; }[]; detail_imgs_new: { image: { url_list: string[]; }; }[]; }",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IByUnBindToLiveResp": [
            {
                "name": "code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "msg",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IByProductResp": [
            {
                "name": "code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "msg",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "data",
                "type": "{ total: number; list: IByProduct[]; }",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IByProduct": [
            {
                "name": "product_id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "promotion_id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "cover",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "price",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "bind_time",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "price_text",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "detail_url",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "add_source",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "commission",
                "type": "{ cos_ratio: number; commission_type: number; }",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IDyLiveAccount": [
            {
                "name": "status_code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "status_message",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "douyin_unique_id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "nick_name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "douyin_uid",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "company_name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IDyLiveStatus": [
            {
                "name": "code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "msg",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "data",
                "type": "{ status: number; room_id: string; shop_first_up: boolean; }",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IDyLiveCardSelected": [
            {
                "name": "status_code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "status_msg",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "card_list",
                "type": "[{ TransformType: number; card_id: string; is_agg_card: boolean; }]",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IDyLiveShelfCard": [
            {
                "name": "status_code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "status_msg",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "card_list",
                "type": "[{ card_id: string; service_info: { service_title: string; }; }]",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IDyAggCardSaveResp": [
            {
                "name": "status_code",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "status_msg",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "agg_card_id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IDyServiceCard": [
            {
                "name": "component_title",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "card_id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "service_info",
                "type": "{ service_title: string; service_id: string; service_banner_url: string; }",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IShelfTask": [
            {
                "name": "id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "type",
                "type": "IShelfTaskType",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "title",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "taskData",
                "type": "ISleepTaskData | IFengcheTaskData | IHuangcheTaskData",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "ISleepTaskData": [
            {
                "name": "begin",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "end",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IFengcheTaskData": [
            {
                "name": "accountId",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "accountName",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "cards",
                "type": "IDyServiceCard[]",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IHuangcheTaskData": [
            {
                "name": "accountId",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "accountName",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "products",
                "type": "IByProduct[]",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "ILiveAccount": [
            {
                "name": "id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "platform",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "roleType",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "platformUserId",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "authJson",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "LiveAccountSaveDto": [
            {
                "name": "accountId",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "platformAccountId",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "platformAccountName",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "platform",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "roleType",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "authJson",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IDyAccountAuthVm": [
            {
                "name": "douyin_unique_id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "nick_name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "douyin_uid",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "company_name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "cookie",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IByAccountAuthVm": [
            {
                "name": "buyin_account_id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "user_name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "user_id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "origin_uid",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "shop_id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "cookie",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "ICookie": [
            {
                "name": "name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "value",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "domain",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IHeaderItem": [
            {
                "name": "key",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "value",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IWebRoomInfo": [
            {
                "name": "roomId",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "roomTitle",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "roomUserCount",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "status",
                "type": "string",
                "description": "2正在直播，4停止",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "uniqueId",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "avatar",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IBarrageMessage": [
            {
                "name": "Type",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "Data",
                "type": "any",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "DouyinMsgBase": [
            {
                "name": "MsgId",
                "type": "string",
                "description": "弹幕ID",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "User",
                "type": "IDouyinUser",
                "description": "用户数据",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "Content",
                "type": "string",
                "description": "消息内容",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "RoomId",
                "type": "number",
                "description": "房间号",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IDouyinUser": [
            {
                "name": "id",
                "type": "string",
                "description": "真实ID",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "ShortId",
                "type": "number",
                "description": "ShortId",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "DisplayId",
                "type": "string",
                "description": "自定义ID",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "NickName",
                "type": "string",
                "description": "昵称",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "Level",
                "type": "number",
                "description": "未知",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "PayLevel",
                "type": "number",
                "description": "支付等级",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "Gender",
                "type": "number",
                "description": "性别 1男 2女",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "Birthday",
                "type": "number",
                "description": "生日",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "Telephone",
                "type": "string",
                "description": "手机",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "Avatar",
                "type": "string",
                "description": "头像地址",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "SecUid",
                "type": "string",
                "description": "用户主页地址",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "FansClub",
                "type": "IDouyinFansClub",
                "description": "粉丝团信息",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "FollowerCount",
                "type": "number",
                "description": "粉丝数",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "FollowStatus",
                "type": "number",
                "description": "关注状态 0 未关注 1 已关注 2 不明",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "FollowingCount",
                "type": "number",
                "description": "关注数",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IDouyinFansClub": [
            {
                "name": "ClubName",
                "type": "string",
                "description": "粉丝团名称",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "Level",
                "type": "number",
                "description": "粉丝团等级，没加入则0",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "DouyinMsgFansClub": [
            {
                "name": "Type",
                "type": "number",
                "description": "粉丝团消息类型, 升级1，加入2",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "Level",
                "type": "number",
                "description": "粉丝团等级",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "DouyinMsgGift": [
            {
                "name": "GiftId",
                "type": "number",
                "description": "礼物ID",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "GiftName",
                "type": "string",
                "description": "礼物名称",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "GroupId",
                "type": "number",
                "description": "礼物分组ID",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "ComboCount",
                "type": "number",
                "description": "本次(增量)礼物数量",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "GroupCount",
                "type": "number",
                "description": "组内礼物数量",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "TotalCount",
                "type": "number",
                "description": "总礼物数量",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "RepeatEnd",
                "type": "number",
                "description": "是否结束重复 1: 结束，0: 未结束",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "RepeatCount",
                "type": "number",
                "description": "礼物数量(连续的)",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "DiamondCount",
                "type": "number",
                "description": "抖币价格",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "ToUser",
                "type": "IDouyinUser",
                "description": "送礼目标(连麦直播间有用)",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "DouyinMsgLike": [
            {
                "name": "Count",
                "type": "number",
                "description": "点赞数量",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "Total",
                "type": "number",
                "description": "总共点赞数量",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "DouyinMsgMember": [
            {
                "name": "MemberCount",
                "type": "number",
                "description": "当前直播间人数",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "DouyinMsgRoomUserSeq": [
            {
                "name": "OnlineUserCount",
                "type": "number",
                "description": "当前直播间用户数量",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "TotalUserCount",
                "type": "number",
                "description": "累计直播间用户数量",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "TotalUserCountStr",
                "type": "string",
                "description": "累计直播间用户数量 显示文本",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "OnlineUserCountStr",
                "type": "string",
                "description": "当前直播间用户数量 显示文本",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "DouyinMsgShare": [
            {
                "name": "ShareType",
                "type": "ShareTypeEnum",
                "description": "分享目标",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "ShareTarget",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "DouyinMsgChat": [],
        "DouyinMsgControl": [],
        "DouyinMsgSocial": [],
        "ILiveScriptVoiceDetailVm": [
            {
                "name": "id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "text",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IVoicePlayStatus": [
            {
                "name": "playState",
                "type": "number",
                "description": "0,stoped,1,Playing,2,Paused",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "isInsert",
                "type": "boolean",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "mainAudioTotal",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "mainAudioCurrent",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "insertAudioTotal",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "insertAudioCurrent",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "mainQueueCount",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "insertQueueCount",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "mainId",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "insertId",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "mainText",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "insertText",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IVoicePlayData": [
            {
                "name": "id",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "soundCardId",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "playType",
                "type": "number",
                "description": "1,追加；2插入；3清空后再播放",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "voice",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "text",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "volume",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "ILiveScriptRow": [
            {
                "name": "id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "content",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "type",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "voiceBase64",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IAiVerticalAnchorEditVm": [
            {
                "name": "id",
                "type": "number",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "name",
                "type": "string",
                "description": "名称",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "primaryTtsModelId",
                "type": "number",
                "description": "主播语音",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "secondaryTtsModelId",
                "type": "number",
                "description": "助播语音",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "scriptTemplateIds",
                "type": "string",
                "description": "生成话术模板（支持多个）",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "chatTemplateIds",
                "type": "string",
                "description": "聊天回复模板（支持多个）",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "ILiveSettingVm": [
            {
                "name": "socialReply",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "fansClubReply",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "likeReply",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "memberReply",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "giftReply",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "insertVoice",
                "type": "string",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "interactMsgCount",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "replyMsgCount",
                "type": "number",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            },
            {
                "name": "replySetting",
                "type": "IReplySettingItem[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "IReplySettingItem": [
            {
                "name": "name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "content",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "matchRules",
                "type": "ReplyMatchRule[]",
                "description": "",
                "required": false,
                "see": null,
                "ignore": false
            }
        ],
        "ReplyMatchRule": [
            {
                "name": "keyword",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "isFuzzy",
                "type": "boolean",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IShelfTaskConfigEditVm": [
            {
                "name": "id",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "name",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            },
            {
                "name": "dataJson",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ],
        "IVoiceVm": [
            {
                "name": "voice",
                "type": "string",
                "description": "",
                "required": true,
                "see": null,
                "ignore": false
            }
        ]
    },
    "enumTypes": {
        "SwitchType": [
            {
                "name": "on",
                "value": "on"
            },
            {
                "name": "off",
                "value": "off"
            }
        ],
        "EnvType": [
            {
                "name": "development",
                "value": "development"
            },
            {
                "name": "staging",
                "value": "staging"
            },
            {
                "name": "production",
                "value": "production"
            }
        ],
        "TableToolType": [
            {
                "name": "primary",
                "value": "primary"
            },
            {
                "name": "normal",
                "value": "normal"
            },
            {
                "name": "divider",
                "value": "divider"
            }
        ],
        "Type2": [
            {
                "name": "'仅下拉'",
                "value": "1"
            },
            {
                "name": "staging",
                "value": "staging"
            },
            {
                "name": "Val",
                "value": "1"
            },
            {
                "name": "Val2",
                "value": "2"
            },
            {
                "name": "Val3",
                "value": "3"
            },
            {
                "name": "Val4",
                "value": "1234"
            }
        ],
        "DateSelectType": [
            {
                "name": "昨天",
                "value": "昨天"
            },
            {
                "name": "今天",
                "value": "今天"
            },
            {
                "name": "近3天",
                "value": "近3天"
            },
            {
                "name": "近7天",
                "value": "近7天"
            },
            {
                "name": "近30天",
                "value": "近30天"
            },
            {
                "name": "本周",
                "value": "本周"
            },
            {
                "name": "本月",
                "value": "本月"
            },
            {
                "name": "上月",
                "value": "上月"
            },
            {
                "name": "近3月",
                "value": "近3月"
            },
            {
                "name": "近12月",
                "value": "近12月"
            },
            {
                "name": "今年",
                "value": "今年"
            },
            {
                "name": "去年",
                "value": "去年"
            }
        ],
        "IShelfTaskType": [
            {
                "name": "等待停顿",
                "value": "等待停顿"
            },
            {
                "name": "风车上架",
                "value": "风车上架"
            },
            {
                "name": "风车下架",
                "value": "风车下架"
            },
            {
                "name": "风车讲解",
                "value": "风车讲解"
            },
            {
                "name": "黄车上架",
                "value": "黄车上架"
            },
            {
                "name": "黄车下架",
                "value": "黄车下架"
            },
            {
                "name": "黄车讲解",
                "value": "黄车讲解"
            }
        ],
        "ShareTypeEnum": [
            {
                "name": "Wechat",
                "value": "1"
            },
            {
                "name": "CircleOfFriends",
                "value": "2"
            },
            {
                "name": "Weibo",
                "value": "3"
            },
            {
                "name": "Qzone",
                "value": "4"
            },
            {
                "name": "QQ",
                "value": "5"
            },
            {
                "name": "Douyin",
                "value": "112"
            }
        ]
    }
}