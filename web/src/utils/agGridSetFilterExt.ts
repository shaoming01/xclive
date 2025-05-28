import {IRowNode, ValueGetterFunc, type ValueGetterParams} from "ag-grid-community";
import {IFullTableSchema, IGridApiObj} from "@/types/schema";
import _ from "lodash";

export class agGridSetFilterExt {
    private readonly api: IGridApiObj;
    private readonly rowData: any[] | [] | undefined;
    private lastPopFilterField: any = undefined; //最后一次弹出的过滤器
    private excludeLastFilterDataList: any[] = []; //排除最后激活过滤器以后的过滤结果
    constructor(api: IGridApiObj, tableSchema: IFullTableSchema) {
        this.api = api;
        this.rowData = tableSchema.rowData;

    }

    public createFilterParams() {
        return {
            newRowsAction: 'keep',
            buttons: ['reset'],
            "valueFormatter": (param: any) => this.filterValueFormatter(param),
            cellRenderer: (param: any) => this.filterCellRenderer(param),
        };
    }

    public filterValueFormatter(param: any) {
        if (this.lastPopFilterField != param.colDef.field) {
            this.lastPopFilterField = param.colDef.field;
            this.iniHasFilterList();
        }
        if (_.isBoolean(param.value)) {
            return param.value ? '是' : '否';
        }
        return param.value;
    }

    firstIni() {
        if (!this.api.gridApi) return;
        const w = window as any;
        w.agGridExt = w.agGridExt ?? {};
        const funName = 'clickOnlyFilterThisValue_' + this.api.gridApi.getGridId();
        if (!w.agGridExt[funName])
            w.agGridExt[funName] = (event: any, el: any, value: any) => this.clickOnlyFilterThisValue(event, el, value);
    }

    public filterCellRenderer(param: any) {
        this.firstIni();
        if (!this.api.gridApi) return;
        let text = param.value;
        if (_.isBoolean(param.value)) {
            text = param.value ? '是' : '否';
        }
        console.log('Render:', param.value);
        if (this.isNull(param.value))
            text = "(空)";
        if (param.value == '(全选)') return text;
        if (!this.lastPopFilterField) return text;

        let count = this.calcCountFromFilterRows(this.lastPopFilterField, param.value);
        let textValue = '\'' + param.value + '\'';

        if (param.value == undefined)
            textValue = '';
        const funName = 'window.agGridExt.clickOnlyFilterThisValue_' + this.api.gridApi.getGridId();

        let backCall = funName + '(event,this,' + textValue + ')';
        return text +
            '&nbsp;(' +
            count +
            ')<span class="ag-filter-ext-filter-one" onclick="event.stopPropagation();' +
            backCall +
            '" style="right: 0;z-index: 999;position: absolute;background: #7ab950;color: #fff;cursor: pointer;height: 24px;line-height: 24px;top: 0; padding-left:6px; display: none;">仅筛选此项</span>\n';
    }

    public clickOnlyFilterThisValue(event: any, el: any, value: any) {
        if (!this.api.gridApi) return;
//event.stopBubble(event); //阻止点击事件冒泡到上层
        let models = this.api.gridApi.getFilterModel();
        models[this.lastPopFilterField] = {filterType: 'set', values: [value]};
        this.api.gridApi.setFilterModel(models);
    }

    /**
     * 从已过滤的行里计算符合条件的数量
     * 用于弹出过滤器时显示每个项目的数量
     * @param fieldName
     * @param value
     * @returns {number}
     */
    public calcCountFromFilterRows(fieldName: string, value: any) {
        let count = 0;
        let list = this.excludeLastFilterDataList || this.rowData;
        list.forEach(item => {
            if (!(fieldName in item)) { //undefined
                if (value == null) //null和undefined相等
                    count++;
                return;
            }
            let targetVal = this.getCellValue(item, fieldName);
            if (this.isEqualVal(targetVal, value)) {
                count++;
            } else if (targetVal === '' && value == null) {
                count++;
            }
        });
        return count;
    }

    public isNull(o: any) {
        return o === undefined || o === null;
    }

    public isEqualVal(a: any, b: any) {
        if (this.isNull(a) && this.isNull(b)) return true; //同为null相等
        if (this.isNull(a) || this.isNull(b)) return false;
        return a.toString() === b.toString();
    }

    /**
     * 获取单元格数据，兼容valueGetter
     */
    public getCellValue(data: any, fieldName: string) {
        if (!this.api.gridApi) return undefined;
        const column = this.api.gridApi.getColumn(fieldName);
        const colDef = this.api.gridApi.getColumnDef(fieldName);
        const valueGetter = colDef?.valueGetter as ValueGetterFunc;
        if (!valueGetter || !column || !colDef) {
            return data[fieldName];
        }
        let param: ValueGetterParams = {
            colDef: colDef,
            column: column,
            data: data,
            api: this.api.gridApi,
            node: {data: data,} as IRowNode,
            context: undefined,
            'getValue': () => {
                return data[fieldName];
            }
        };
        return valueGetter(param);
    }

    /**
     * set类型的过滤器打开以后，记录下来除最后过滤器之外的筛选结果
     */
    public iniHasFilterList() {
        if (!this.api.gridApi) return;
        if (!this.lastPopFilterField) {
            console.log('当前过滤字段未找到，无法初始化中间过滤结果');
            return;
        }
        let models = this.api.gridApi.getFilterModel();
        let thisModel = null;
//排除当前过滤项
        if (models.hasOwnProperty(this.lastPopFilterField)) {
            thisModel = models[this.lastPopFilterField];
            delete models[this.lastPopFilterField];
            this.api.gridApi.setFilterModel(models);
        }
        this.excludeLastFilterDataList.length = 0;
        this.api.gridApi.forEachNodeAfterFilter(r => {
            if (!r.data) return; //GroupRow
            this.excludeLastFilterDataList.push(r.data);
        });
        console.log(this.excludeLastFilterDataList.length);
        if (thisModel) {
            models[this.lastPopFilterField] = thisModel;
//恢复所有过滤项
            this.api.gridApi.setFilterModel(models);
        }
    }


}
