import {ColDef} from "ag-grid-community";
import {ITableColumnSchema} from "@/types/dto";
import {IFullTableSchema, IGridApiObj} from "@/types/schema";
import {agGridValueGetterExt} from "@/utils/agGridValueGetterExt";
import {comUtil} from "@/utils/com";
import {agGridSetFilterExt} from "@/utils/agGridSetFilterExt";


export class agGridColumnUtil {
    private _setFilterExt: agGridSetFilterExt;
    private _api: IGridApiObj;
    private _tableSchema: IFullTableSchema;

    constructor(api: IGridApiObj, tableSchema: IFullTableSchema) {
        this._setFilterExt = new agGridSetFilterExt(api, tableSchema);
        this._api = api;
        this._tableSchema = tableSchema;
    }

    createCheckCol(): ColDef {
        return {
            field: 'ck',
            headerName: "",
            width: 40,
            headerCheckboxSelection: true,
            headerCheckboxSelectionFilteredOnly: true,
            checkboxSelection: true,
            cellClass: 'checkbox-show ag-center-cell',
            filter: false,
            suppressMenu: true,
            lockPosition: true,
            pinned: 'left',
            resizable: false,
            sortable: false,
            suppressNavigable: true,
            lockVisible: true,
            suppressSizeToFit: true,
            enableValue: false,
            enablePivot: false,
            enableRowGroup: false,
            chartDataType: 'excluded',
        };
    }

    createRowNoCol(): ColDef {
        return {
            field: "rowNo",
            headerName: "",
            width: 40,
            cellClass: 'number-cols ag-center-cell',
            filter: false,
            suppressMenu: true,
            lockPosition: true,
            pinned: 'left',
            resizable: false,
            valueGetter: 'node.childIndex + 1',
            sortable: false,
            cellStyle: {
                'padding': '0',
                'background': '#f7f7f7',
                'text-align': 'center',
                'justify-content': 'center',
                'color': '#000',
            },
            suppressNavigable: true,
            lockVisible: true,
            suppressSizeToFit: true,
            enableValue: false,
            enablePivot: false,
            enableRowGroup: false,
            chartDataType: 'excluded',
        }
    }

    public async convertToAgCol(c: ITableColumnSchema): Promise<ColDef> {
        const valueGetter = await agGridValueGetterExt.getValueGetter(c.valueGetter);
        const cellRender = c.cellRender ? comUtil.getCom(c.cellRender.comPath) : undefined;
        const emptyEditor = comUtil.getCom('/src/components/grid/column/EmptyEditor.vue');

        const editor = c.cellRender?.canEdit ? emptyEditor : undefined;//渲染器支持编辑，不需要再启用编辑器
        const dataType = this.calcColumnDataType(c.propertyType);
        const filterInfo = this.calcFilter(dataType, c);
        const col: ColDef = {
            field: c.field,
            headerName: c.headerName,
            width: c.width,
            editable: c.editable,
            cellEditor: editor,
            cellRenderer: cellRender,
            cellRendererParams: c.cellRender?.props,
            //tooltipField: c.field,
            valueGetter: valueGetter,
            filter: filterInfo.filter,
            filterParams: filterInfo.filterParams,
            cellDataType: dataType,
            headerTooltip: c.tip,
            headerClass: c.editable ? 'editable-header' : undefined,
            suppressHeaderMenuButton: c.suppressHeaderMenuButton,
            sortable: !c.suppressSort,
            autoHeight: c.autoRowHeight,
            wrapText: c.autoRowHeight,
        };
        return col;
    }

    calcColumnDataType(propertyType: string | undefined): string {
        switch (propertyType) {
            case 'Int32':
            case 'Decimal':
                return 'number';
            case 'Boolean':
                return 'boolean';
            case 'Int64':
            case 'String':
            case 'DateTime'://                return 'dateString';
            default:
                return 'text'
        }
    }

    public async createColumns(): Promise<ColDef[]> {
        const cols: ColDef[] = [];
        for (const col of this._tableSchema.columns ?? []) {
            const agCol = await this.convertToAgCol(col);
            cols.push(agCol)
        }
        if (this._tableSchema.rowSelection == 'multiple')
            cols.unshift(this.createCheckCol());
        cols.unshift(this.createRowNoCol());
        return cols;

    }


    private calcFilter(dataType: string, col: ITableColumnSchema): { filter: any, filterParams: any } {
        if (dataType == 'text' || dataType == 'boolean')
            return {
                filter: 'agSetColumnFilter',
                filterParams: this._setFilterExt.createFilterParams(),
            };
        else if (dataType == 'number') {
            return {
                filter: 'agNumberColumnFilter',
                filterParams: {
                    buttons: ['reset'],
                    defaultOption: 'greaterThanOrEqual'
                },

            };

        }

        return {filter: undefined, filterParams: undefined};
    }
}