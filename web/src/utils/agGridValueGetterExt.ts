import type {ValueGetterParams} from "ag-grid-community";
import {IValueDisplay, IValueGetterSchema} from "@/types/dto";
import {pageApi} from "@/api/pageApi";
import _ from "lodash";

export class agGridValueGetterExt {
    static async getValueGetter(schema: IValueGetterSchema | ((params: ValueGetterParams) => string) | undefined) {
        if (typeof schema === 'function')
            return schema;
        switch (schema?.funcName) {
            case 'listValueGetter':
                const valueList = await this.getValueList(schema);
                const valueMap = new Map<string, string>(
                    valueList.map(v => [v.value, v.label])
                );
                return (params: ValueGetterParams) => {
                    return this.listValueGetter(params, valueMap);
                }
        }

        return undefined;
    }

    static async getValueList(schema: IValueGetterSchema | undefined): Promise<IValueDisplay[]> {
        if (!schema?.params?.dataSourceApi) {
            console.error('缺少参数');
            return [];
        }
        const re = await pageApi.exeApiCall(schema.params.dataSourceApi);
        if (!re.success) {
            console.error('调用调用出错', re.message);
            return [];
        }
        return re.data as IValueDisplay[];

    }

    static listValueGetter(params: ValueGetterParams, valueMap: Map<string, string>) {
        const val = params.data[params.colDef.field ?? ''];
        const arr = _.split(val, ",");
        return arr.map((v) => {
            return valueMap.get(v)
        }).join(',');


    }

}