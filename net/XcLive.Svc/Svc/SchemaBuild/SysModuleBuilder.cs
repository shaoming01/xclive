using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Schema;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Models.ViewModel;

namespace SchemaBuilder.Svc.Svc.SchemaBuild;

public static class SysModuleBuilder
{
    private static readonly Dictionary<string, object> SchemaCache = new();


    static SysModuleBuilder()
    {
        var dic1 = GetAllDataBrowserSchema();
        dic1.ForEach(pair => { SchemaCache.TryAdd(pair.Key, pair.Value); });
        var dic2 = GetAllObjectEditSchema();
        dic2.ForEach(pair => { SchemaCache.TryAdd(pair.Key, pair.Value); });
        var dic3 = GetAllModalObjectEditSchema();
        dic3.ForEach(pair => { SchemaCache.TryAdd(pair.Key, pair.Value); });
        var dic4 = GetAllDataSelectSchema(dic1);
        dic4.ForEach(pair => { SchemaCache.TryAdd(pair.Key, pair.Value); });
    }

    public static object? GetSchema(string schemaName)
    {
        return SchemaCache.TryGetOrDefault(schemaName);
    }

    public static List<ValueDisplay> List()
    {
        return SchemaCache.Select(pair => new ValueDisplay()
        {
            Value = pair.Key,
            Label = $"{pair.Key}({pair.Value.GetType().Name})",
        }).ToList();
    }

    private static Dictionary<string, DataBrowserProp> GetAllDataBrowserSchema()
    {
        var re = new Dictionary<string, DataBrowserProp>();
        FillTableSchema(re);
        FillSearchSchema(re);
        return re;
    }

    private static Dictionary<string, ObjectEditorProp> GetAllObjectEditSchema()
    {
        var re = new Dictionary<string, ObjectEditorProp>();
        FillObjectEditSchema(re);
        return re;
    }

    private static Dictionary<string, ModalObjectEditorProp> GetAllModalObjectEditSchema()
    {
        var dic = new Dictionary<string, ModalObjectEditorProp>();

        typeof(ITable).Assembly.GetTypes().Select(type => new
        {
            Type = type,
            Atts = type.ReadAttributes<ModalObjectEditorAttribute>()
        }).Where(attInfo => attInfo.Atts.Has()).ForEach(attInfo =>
        {
            var objectEditAtt = attInfo.Type.ReadAttributes<ObjectEditorAttribute>().FirstOrDefault();

            var fields = ReadEditFields(attInfo.Type);
            var detailTables = ReadDetailFields(attInfo.Type);

            attInfo.Atts.ForEach(att =>
            {
                var sysModuleName = att.SysModuleName;
                if (!sysModuleName.Has())
                {
                    return;
                }

                var prop = new ModalObjectEditorProp()
                {
                    schema = new ModalObjectEditSchema()
                    {
                        ObjectEditSchema = new ObjectEditSchema()
                        {
                            fields = fields,
                            detailTablesSchema = new DetailTablesSchema()
                            {
                                detailTables = detailTables
                            }
                        },
                        SizeMode = att.SizeMode,
                        Title = att.Title,
                        SaveDataUrl = att.SaveDataUrl,
                        GetDataUrl = att.GetDataUrl,
                        Centered = att.Centered,
                    },
                };
                dic[sysModuleName] = prop;
            });
        });

        return dic;
    }

    private static Dictionary<string, ModalDataSelectProp> GetAllDataSelectSchema(
        Dictionary<string, DataBrowserProp> dic1)
    {
        var dic = new Dictionary<string, ModalDataSelectProp>();

        typeof(ITable).Assembly.GetTypes().Select(type => new
        {
            Type = type,
            Atts = type.ReadAttributes<ModalDataSelectAttribute>()
        }).Where(attInfo => attInfo.Atts.Has()).ForEach(attInfo =>
        {
            var fullTableAttr = attInfo.Type.ReadAttributes<FullTableAttribute>().FirstOrDefault();
            if (fullTableAttr == null)
            {
                return;
            }

            var dataBrowser = dic1.TryGetOrDefault(fullTableAttr.SysModuleName);
            if (dataBrowser == null)
            {
                return;
            }

            attInfo.Atts.ForEach(att =>
            {
                var sysModuleName = att.SysModuleName;
                if (!sysModuleName.Has())
                {
                    return;
                }

                var prop = new ModalDataSelectProp
                {
                    schema = new ModalDataSelectSchema
                    {
                        DataBrowserSchema = dataBrowser.schema,
                        SizeMode = att.SizeMode,
                        Title = att.Title,
                    },
                };
                dic[sysModuleName] = prop;
            });
        });

        return dic;
    }

    private static void FillSearchSchema(Dictionary<string, DataBrowserProp> re)
    {
        typeof(ITable).Assembly.GetTypes().Select(type => new
        {
            Type = type,
            Atts = type.ReadAttributes<SearchContainerAttribute>()
        }).Where(attInfo => attInfo.Atts.Has()).ForEach(attInfo =>
        {
            var fields = ReadEditFields(attInfo.Type);

            attInfo.Atts.ForEach(att =>
            {
                var vueName = att.SysModuleName;
                if (!vueName.Has())
                {
                    return;
                }

                if (!re.ContainsKey(vueName))
                {
                    re[vueName] = new DataBrowserProp()
                    {
                        schema = new DataBrowserSchema(),
                    };
                }

                re[vueName].schema.searchContainer = new SearchContainerSchema
                {
                    searchGroup = re[vueName].schema.searchContainer?.searchGroup,
                    fields = fields,
                };
            });
        });
    }

    private static void FillTableSchema(Dictionary<string, DataBrowserProp> re)
    {
        typeof(ITable).Assembly.GetTypes().Select(type => new
        {
            Type = type,
            Atts = type.ReadAttributes<FullTableAttribute>()
        }).Where(attInfo => attInfo.Atts.Has()).ForEach(attInfo =>
        {
            attInfo.Atts.ForEach(att =>
            {
                var vueName = att.SysModuleName;
                if (!vueName.Has())
                {
                    return;
                }

                var table = GetTableSchema(attInfo.Type, att);

                if (!re.ContainsKey(vueName))
                {
                    re[vueName] = new DataBrowserProp() { schema = new DataBrowserSchema() };
                }

                if (att.ElType == TableType.MainTable)
                {
                    re[vueName].schema.mainTable = table;
                    if (table.queryCountUrl.Has())
                    {
                        re[vueName].schema.searchContainer ??= new SearchContainerSchema();
                        re[vueName].schema.searchContainer.searchGroup = new SearchGroupSchema()
                            { queryCountUrl = table.queryCountUrl };
                    }
                }
                else
                {
                    re[vueName].schema.detailTablesSchema ??= new DetailTablesSchema()
                    {
                        detailTables = new List<DetailTableSchema>()
                    };
                    re[vueName].schema.detailTablesSchema?.detailTables.Add(new DetailTableSchema()
                    {
                        field = attInfo.Type.Name,
                        tab = att.Title,
                        tableSchema = table
                    });
                }
            });
        });
    }

    private static FullTableSchema GetTableSchema(Type type, FullTableAttribute att)
    {
        var columns = ReadTableColumns(type);
        var tools = ReadTableTools(type);
        var table = new FullTableSchema()
        {
            queryCountUrl = att.QueryCountUrl,
            deleteIdsUrl = att.DeleteIdsUrl,
            queryDataUrl = att.QueryDataUrl,
            showPageBar = att.QueryCountUrl.Has(),
            columns = columns,
            rowSelection = att.MultiSelection ? "multiple" : "single",
            primaryKey = att.PrimaryKey,
            headerKey = att.HeaderIdKey,
            page = 1,
            pageSize = att.PageSize,
            autoQuery = att.AutoQuery,
            pageSizeOptions = att.PageSizeOptions,
            tableTools = tools,
        };
        return table;
    }

    private static void FillObjectEditSchema(Dictionary<string, ObjectEditorProp> re)
    {
        typeof(ITable).Assembly.GetTypes().Select(type => new
        {
            Type = type,
            Atts = type.ReadAttributes<ObjectEditorAttribute>()
        }).Where(attInfo => attInfo.Atts.Has()).ForEach(attInfo =>
        {
            var fields = ReadEditFields(attInfo.Type);
            var detailTables = ReadDetailFields(attInfo.Type);

            attInfo.Atts.ForEach(att =>
            {
                var vueName = att.SysModuleName;
                if (!vueName.Has())
                {
                    return;
                }

                if (!re.ContainsKey(vueName))
                {
                    re[vueName] = new ObjectEditorProp()
                    {
                        schema = new ObjectEditSchema()
                    };
                }

                var schema = new ObjectEditorProp()
                {
                    schema = new ObjectEditSchema()
                    {
                        fields = fields,
                        detailTablesSchema = new DetailTablesSchema()
                        {
                            detailTables = detailTables
                        },
                    }
                };
                re[vueName] = schema;
            });
        });
    }

    private static List<TableColumnSchema> ReadTableColumns(Type type)
    {
        var columns = type.GetProperties().Where(w => w.CanRead).Select(x => new
        {
            Attr = x.ReadAttributes<TableColumnAttribute>().FirstOrDefault(),
            Property = x
        }).Where(w => w.Attr != null).Select(x =>
        {
            Debug.Assert(x.Attr != null, "x.Attr != null");
            var field = x.Attr.Field.IsNullOrWhiteSpace() ? x.Property.Name : x.Attr.Field;
            return new TableColumnSchema
            {
                field = field.ToLowerFirstChar(),
                width = x.Attr.Width,
                tip = x.Attr.Tip,
                editable = x.Attr.Editable,
                propertyType = x.Property.PropertyType.GetRealType().Name,
                headerName = x.Attr.Title,
                cellRender = CreateCellRender(x.Property, x.Attr),
                valueGetter = CreateValueGetter(x.Property, x.Attr),
            };
        }).ToList();
        return columns;
    }

    private static List<TableToolBarItemSchema> ReadTableTools(Type type)
    {
        var toolsAttrs = type.ReadAttributes<TableToolAttribute>();
        var list = toolsAttrs.Select(att => new TableToolBarItemSchema
        {
            Type = att.ButtonType,
            Name = att.Name,
            Index = att.Index,
            Icon = att.ButtonIcon,
            Module = CreateToolModule(att),
        }).ToList();
        return list;
    }

    private static ModuleVm? CreateToolModule(TableToolAttribute att)
    {
        switch (att.Type)
        {
            case ToolType.Add:
                return new ModuleVm
                {
                    ComPath = "/src/components/grid/toolBar/Add.vue",
                    Props = new Dictionary<string, object>()
                    {
                        {
                            "moduleId", att.ModuleId
                        },
                        {
                            "sysModuleId", att.SysModuleId
                        }
                    }
                };
            case ToolType.Edit:
                return new ModuleVm
                {
                    ComPath = "/src/components/grid/toolBar/Edit.vue",
                    Props = new Dictionary<string, object>()
                    {
                        {
                            "moduleId", att.ModuleId
                        },
                        {
                            "sysModuleId", att.SysModuleId
                        }
                    }
                };
            case ToolType.Delete:
                return new ModuleVm
                {
                    ComPath = "/src/components/grid/toolBar/Delete.vue",
                };
            case ToolType.LocalDelete:
                return new ModuleVm
                {
                    ComPath = "/src/components/grid/toolBar/LocalDelete.vue",
                };
            case ToolType.Custom:
                return new ModuleVm
                {
                    ComPath = att.ComPath,
                };
        }

        return null;
    }

    private static List<EditFieldSchema> ReadEditFields(Type type)
    {
        var list = type.GetProperties().Where(w => w.CanRead).Select(x => new
        {
            Attr = x.ReadAttributes<FieldEditorAttribute>().FirstOrDefault(),
            Property = x
        }).Where(w => w.Attr != null).Select(x =>
        {
            Debug.Assert(x.Attr != null, "x.Attr != null");
            return new EditFieldSchema
            {
                field = x.Property.Name.ToLowerFirstChar(),
                fieldType = x.Property.PropertyType.GetRealType().Name,
                label = x.Attr.Title,
                module = GetModule(x.Property, x.Attr),
                labelColOffset = x.Attr.LabelColOffset,
                offset = x.Attr.Offset,
                labelColSpan = x.Attr.LabelColSpan,
                span = x.Attr.Span,
                wrapperColSpan = x.Attr.WrapperColSpan,
                wrapperColOffset = x.Attr.WrapperColOffset,
                require = x.Attr.Require,
                allowClear = x.Attr.AllowClear,
                tip = x.Attr.Tip,
                defaultValue = CalcDefaultValue(x.Property, x.Attr),
                groupName = x.Attr.GroupTitle,
                disabled = x.Attr.Disabled,
            };
        }).ToList();
        return list;
    }

    private static object? CalcDefaultValue(PropertyInfo prop, FieldEditorAttribute attr)
    {
        if (attr.Default == null)
        {
            return null;
        }

        return attr.Default;
    }

    private static List<DetailTableSchema> ReadDetailFields(Type type)
    {
        var list = type.GetProperties().Where(w => w.CanRead).Select(x => new
        {
            Attr = x.ReadAttributes<FullTableAttribute>().FirstOrDefault(),
            Property = x
        }).Where(w => w.Attr != null).Select(x =>
        {
            Debug.Assert(x.Attr != null, "x.Attr != null");
            return new DetailTableSchema
            {
                field = x.Property.Name.ToLowerFirstChar(),
                tab = x.Attr.Title,
                tableSchema = GetTableSchema(x.Property, x.Attr)
            };
        }).ToList();
        return list;
    }

    private static FullTableSchema GetTableSchema(PropertyInfo property, FullTableAttribute attr)
    {
        var propType = property.PropertyType;
        var iEnumerableType = propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            ? propType
            : propType.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        var elementType = iEnumerableType?.GetGenericArguments().First();
        Debug.Assert(elementType != null, "elementType != null");
        var columns = ReadTableColumns(elementType);
        var tools = ReadTableTools(property);
        var table = new FullTableSchema()
        {
            queryCountUrl = attr.QueryCountUrl,
            deleteIdsUrl = attr.DeleteIdsUrl,
            queryDataUrl = attr.QueryDataUrl,
            showPageBar = attr.QueryCountUrl.Has(),
            columns = columns,
            rowSelection = attr.MultiSelection ? "multiple" : "single",
            primaryKey = attr.PrimaryKey,
            headerKey = attr.HeaderIdKey,
            page = 1,
            pageSize = attr.PageSize,
            autoQuery = attr.AutoQuery,
            pageSizeOptions = attr.PageSizeOptions,
            tableTools = tools,
        };
        return table;
    }

    private static List<TableToolBarItemSchema> ReadTableTools(PropertyInfo property)
    {
        var toolsAttrs = property.ReadAttributes<TableToolAttribute>();
        var list = toolsAttrs.Select(att => new TableToolBarItemSchema
        {
            Type = att.ButtonType,
            Name = att.Name,
            Index = att.Index,
            Icon = att.ButtonIcon,
            Module = CreateToolModule(att),
        }).ToList();
        return list;
    }

    private static ValueGetter? CreateValueGetter(PropertyInfo property, TableColumnAttribute attr)
    {
        var proType = property.PropertyType.GetRealType();

        if (attr.ValueListType != ValueDisplayType.Undefined || proType.IsEnum)
        {
            return new ValueGetter
            {
                FuncName = "listValueGetter",
                Params = new Dictionary<string, object>()
                {
                    {
                        "dataSourceApi", new ApiCallSchema
                        {
                            ApiUrl = "/api/sys/ListValueDisplay",
                            Cacheable = true,
                            PostParams = new ValueDisplayQuery
                            {
                                Type = attr.ValueListType,
                                EnumTypeName = proType.IsEnum ? proType.FullName : null,
                            }
                        }
                    }
                }
            };
        }

        return null;
    }

    private static CellRender? CreateCellRender(PropertyInfo property, TableColumnAttribute attr)
    {
        if (attr.RenderType == CellRenderType.IconRender)
        {
            return new CellRender
            {
                ComPath = "/src/components/grid/column/IconRender.vue",
                Props = null,
            };
        }
        else if (attr.RenderType == CellRenderType.VoicePlayerRender)
        {
            return new CellRender
            {
                ComPath = "/src/components/grid/column/VoicePlayerRender.vue",
                Props = null,
            };
        }
        else if (attr.RenderType == CellRenderType.ImageRender)
        {
            return new CellRender
            {
                ComPath = "/src/components/grid/column/ImageRender.vue",
                Props = null,
            };
        }
        else if (attr.RenderType == CellRenderType.LongStringRender)
        {
            return new CellRender
            {
                ComPath = "/src/components/grid/column/LongStringRender.vue",
                Props = null,
            };
        }
        else if (attr.RenderType == CellRenderType.ListSelectRender)
        {
            var proType = property.PropertyType.GetRealType();
            return new CellRender
            {
                ComPath = "/src/components/grid/column/ListSelectRender.vue",
                CanEdit = true,
                Props = new Dictionary<string, object>
                {
                    {
                        "dataSourceApi", new ApiCallSchema
                        {
                            ApiUrl = "/api/sys/ListValueDisplay",
                            Cacheable = true,
                            PostParams = new ValueDisplayQuery
                            {
                                Type = attr.ValueListType,
                                EnumTypeName = proType.IsEnum ? proType.FullName : null,
                            }
                        }
                    }
                },
            };
        }

        return null;
    }

    private static ModuleVm? GetModule(PropertyInfo property, FieldEditorAttribute attr)
    {
        var proType = property.PropertyType.GetRealType();
        ModuleVm re;
        if (attr.EditorType == FieldEditorType.IconSelectInput)
        {
            re = new ModuleVm
            {
                ComPath = "/src/components/edit/tool/IconSelectInput.vue",
            };
        }
        else if (attr.EditorType == FieldEditorType.ModuleEditor)
        {
            re = new ModuleVm
            {
                ComPath = "/src/components/edit/tool/ModuleEditor.vue",
            };
        }
        else if (attr.EditorType == FieldEditorType.VueComSelectInput)
        {
            re = new ModuleVm
            {
                ComPath = "/src/components/edit/complex/VueComSelectInput.vue",
            };
        }
        else if (proType.IsEnum || attr.ValueListType != ValueDisplayType.Undefined)
        {
            re = new ModuleVm
            {
                ComPath = "/src/components/edit/complex/DataSelectInput.vue",
                Props = new Dictionary<string, object>()
                {
                    {
                        "dataSourceApi", new ApiCallSchema
                        {
                            ApiUrl = "/api/sys/ListValueDisplay",
                            Cacheable = true,
                            PostParams = new ValueDisplayQuery
                            {
                                Type = attr.ValueListType,
                                EnumTypeName = proType.IsEnum ? proType.FullName : null,
                            }
                        }
                    }
                }
            };
        }
        else if (proType == typeof(string))
        {
            re = new ModuleVm
            {
                ComPath = "/src/components/edit/simple/StringInput.vue",
                Props = new Dictionary<string, object>()
                {
                    { "rows", attr.Rows }
                }
            };
        }
        else if (proType == typeof(int))
        {
            re = new ModuleVm
            {
                ComPath = "/src/components/edit/simple/NumberInput.vue",
            };
        }
        else if (proType == typeof(bool))
        {
            re = new ModuleVm
            {
                ComPath = "/src/components/edit/simple/BoolInput.vue",
            };
        }
        else if (proType == typeof(DateTime))
        {
            re = new ModuleVm
            {
                ComPath = "/src/components/edit/simple/DateInput.vue",
            };
        }
        else if (proType == typeof(DateQuery))
        {
            re = new ModuleVm
            {
                ComPath = "/src/components/edit/complex/DateSearch.vue",
            };
        }
        else if (proType == typeof(StringQuery))
        {
            re = new ModuleVm
            {
                ComPath = "/src/components/edit/complex/StringSearch.vue",
            };
        }
        else if (proType == typeof(DecimalQuery))
        {
            re = new ModuleVm
            {
                ComPath = "/src/components/edit/complex/NumberSearch.vue",
                Props = new Dictionary<string, object>()
                {
                    { "precision", -1 }
                }
            };
        }
        else if (proType == typeof(IntQuery))
        {
            re = new ModuleVm
            {
                ComPath = "/src/components/edit/complex/NumberSearch.vue",
                Props = new Dictionary<string, object>()
                {
                    { "precision", 0 }
                }
            };
        }
        else
        {
            re = new ModuleVm()
            {
                ComPath = "/src/components/edit/simple/StringInput.vue",
            };
        }

        if (attr.PropJson.Has())
        {
            var addProps = JsonConvert.DeserializeObject<Dictionary<string, Object>>(attr.PropJson);
            if (re.Props == null)
            {
                re.Props = addProps;
            }
            else
            {
                addProps.ForEach(pair => { re.Props[pair.Key] = pair.Value; });
            }
        }

        return re;
    }
}