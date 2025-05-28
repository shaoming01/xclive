import {IEditFieldSchema, IStartEndDate} from "@/types/schema";
import dayjs, {Dayjs} from "dayjs";

export class editFiledHelper {
    static createDefault(fields: IEditFieldSchema[] | undefined): Record<string, any> {
        let defaultConditions: Record<string, any> = {};
        if (!fields) {
            return defaultConditions;
        }
        for (const field of fields) {
            if (!field.defaultValue) {
                continue
            }
            if (field.fieldType == 'DateQuery' && typeof field.defaultValue == 'string') {
                defaultConditions[field.field] = {
                    complexValue: field.defaultValue,
                };
                continue;
            }
            defaultConditions[field.field] = field.defaultValue;
        }
        return defaultConditions

    }

    /**
     *将复杂日期串转换成开始结束两个日期
     * @param value 格式：今天、昨天、2021-1-1~、`~2021-1-1
     */
    static getStartEndByComplexValue(value?: string): IStartEndDate {
        const re: IStartEndDate = {};
        if (!value)
            return re;
        if (value.includes('~')) {
            const arr = value.split("~");
            if (arr.length !== 2) {
                return re;
            }
            re.start = dayjs(arr[0]).isValid() ? dayjs(arr[0]) : undefined;
            re.end = dayjs(arr[1]).isValid() ? dayjs(arr[1]) : undefined;
            return re;
        }
        return this.getStartEndBySelect(value);

    }

    static getStartEndBySelect(selectName: string): { start: Dayjs | undefined, end: Dayjs | undefined } {
        let val1: Dayjs | undefined = undefined;
        let val2: Dayjs | undefined = undefined;
        const today = dayjs(dayjs().format('YYYY-MM-DD'));
        switch (selectName) {
            case DateSelectType[DateSelectType.昨天]:
                val1 = today.add(-1, 'd');
                val2 = today;
                break
            case DateSelectType[DateSelectType.今天]:
                val1 = today;
                break
            case DateSelectType[DateSelectType.近3天]:
                val1 = today.add(-2, 'd');
                break
            case DateSelectType[DateSelectType.近7天]:
                val1 = today.add(-6, 'd');
                break
            case DateSelectType[DateSelectType.近30天]:
                val1 = today.add(-29, 'd');
                break
            case DateSelectType[DateSelectType.本周]:
                val1 = today.startOf('week').add(1, 'day');
                break
            case DateSelectType[DateSelectType.本月]:
                val1 = today.startOf('month');
                break
            case DateSelectType[DateSelectType.上月]:
                val1 = today.add(-1, 'month').startOf('month');
                val2 = today.startOf('month');
                break
            case DateSelectType[DateSelectType.近3月]:
                val1 = today.add(-3, 'month').startOf('month')
                break
            case DateSelectType[DateSelectType.近12月]:
                val1 = today.add(-12, 'month').startOf('month');
                break
            case DateSelectType[DateSelectType.今年]:
                val1 = today.startOf('year');
                break
            case DateSelectType[DateSelectType.去年]:
                val1 = today.add(-1, 'year').startOf('year');
                val2 = today.startOf('year');
                break

        }
        return {start: val1, end: val2};

    }

}

export enum DateSelectType {
    昨天, 今天, 近3天, 近7天, 近30天, 本周, 本月, 上月, 近3月, 近12月, 今年, 去年
}
