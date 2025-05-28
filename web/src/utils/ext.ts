import _ from "lodash";

type ComplexObject = Record<string, any>;

export class ext {
    public static isJson(str: string | undefined): boolean {
        if (!str) return false;
        try {
            JSON.parse(str);
            return true;
        } catch {
            return false;
        }
    }

    public static asyncLoop(fn: () => Promise<any>, interval = 500): { stop: () => void } {
        let stopped = false;
        const execute = async () => {
            if (stopped) return;
            try {
                await fn();  // 执行异步操作，并等待其完成
            } catch (error) {
                console.error('异步操作出错:', error);
            }

            // 等待指定的间隔时间再执行下一次
            setTimeout(execute, interval);
        };

        execute();  // 启动第一次执行
        return {
            stop: () => {
                stopped = true;
            }
        };
    }

    public static deepCopy(input: any | undefined) {
        if (input == undefined) return undefined;
        const str = JSON.stringify(input);
        if (str == undefined) return undefined;
        return JSON.parse(str);
    }

    /**
     * 比较两个对象，返回 obj2 中修改过的字段（不包括新增字段）。
     * @param obj1 对象 1
     * @param obj2 对象 2
     * @param keyFields 需要保留的关键字段
     * @returns obj2 中修改过的字段
     */
    public static getExtraFields(
        obj1: ComplexObject | undefined,
        obj2: ComplexObject | undefined,
        keyFields: string[]
    ): ComplexObject {
        obj1 = obj1 || {};
        obj2 = obj2 || {};
        const result: ComplexObject = {};
        for (const key in obj2) {
            if (!obj2.hasOwnProperty(key)) continue;

            const value1 = obj1[key];
            const value2 = obj2[key];

            // 如果 obj1 中没有这个字段，则跳过（不返回新增字段）
            if (!(key in obj1)) continue;

            if (
                typeof value1 === 'object' &&
                value1 !== null &&
                typeof value2 === 'object' &&
                value2 !== null &&
                !Array.isArray(value1) &&
                !Array.isArray(value2)
            ) {
                // 如果是子对象，递归比较
                const extraSubFields = this.getExtraFields(value1, value2, keyFields);
                if (Object.keys(extraSubFields).length > 0) {
                    result[key] = extraSubFields;
                }
            } else if (Array.isArray(value2) && Array.isArray(value1)) {
                // 如果两者都是数组，按 keyFields 匹配并比较元素
                const extraArrayFields = this.compareArrayElements(value1, value2, keyFields);
                if (extraArrayFields.length > 0) {
                    result[key] = extraArrayFields;
                }
            } else if (value1 !== value2) {
                // 如果是普通字段且值不同，记录差异
                result[key] = value2;
            }
        }

        return result;
    }

    /**
     * 比较两个数组中的元素，返回 obj2 中修改过的字段。
     * @param arr1 数组 1
     * @param arr2 数组 2
     * @param keyFields 用于匹配元素的方法
     * @returns 数组中元素的差异
     */
    public static compareArrayElements(
        arr1: ComplexObject[],
        arr2: ComplexObject[],
        keyFields: string[]
    ): ComplexObject[] {
        const result: ComplexObject[] = [];

        function compare(item1: ComplexObject, item2: ComplexObject, keyFields: string[]) {
            if (!item1 || !item2) return false;

            // 检查是否有任意一个关键字段相同
            return keyFields.some((keyField) => item1[keyField] !== undefined && item1[keyField] === item2[keyField]);
        }

        arr2.forEach((item2) => {
            // 找到 arr1 中 keyFields 匹配的元素
            const matchedItem = arr1.find((item1) => compare(item1, item2, keyFields));

            if (matchedItem) {
                // 如果有匹配的元素，则递归比较字段差异
                const diff = this.getExtraFields(matchedItem, item2, keyFields);
                if (Object.keys(diff).length > 0) {
                    // 包含关键字段与差异字段
                    const keyFieldsData = Object.fromEntries(
                        keyFields.map((field) => [field, item2[field]])
                    );
                    result.push({
                        ...keyFieldsData,
                        ...diff,
                    });
                }
            } else {
                result.push(item2);
            }
        });

        return result;
    }

    /**
     * 将对象所有key按.拆分
     * @param obj
     */
    public static expandObj(obj: any): any {
        if (Array.isArray(obj)) {
            const newArr = [];
            for (let i = 0; i < obj.length; i++) {
                const val = this.expandObj(obj[i]);
                newArr.push(val);
            }
            return newArr;
        } else if (typeof obj === 'object') {
            const newObj = {};
            for (const key in obj) {
                const val = this.expandObj(obj[key]);
                setToValue(newObj, key, val);
            }
            return newObj;

        } else {
            return obj;

        }

        function setToValue(target: any, key: string, val: any) {
            target = target ?? {};
            const arr = key.split('.');
            let nextTar = target;
            for (let i = 0; i < arr.length; i++) {
                if (i + 1 == arr.length) {
                    if (Array.isArray(val) && Array.isArray(nextTar[arr[i]]))//都是数组，则合并
                        nextTar[arr[i]] = [...nextTar[arr[i]], ...val];
                    else if (typeof obj === 'object' && typeof nextTar[arr[i]] === 'object')//都是对象，则合并
                        nextTar[arr[i]] = {...nextTar[arr[i]], ...val};
                    else
                        nextTar[arr[i]] = val;
                    break;
                }
                if (!nextTar.hasOwnProperty(arr[i])) {
                    nextTar[arr[i]] = {};
                }
                nextTar = nextTar[arr[i]];
            }
            return target;
        }
    }

    /**
     * 将追加的结构合并到现有结构中，不改变原有结构的对象地址
     * @param oldSchema
     * @param appendSchema
     */
    public static appendSchema(oldSchema: any, appendSchema: any) {
        return this.deepAppend(oldSchema, appendSchema, item => {
            if (!item) return item;
            if (item.hasOwnProperty('field')) return item['field'];
            if (item.hasOwnProperty('name')) return item['name'];
            return item;
        });
    }

    public static deepAppend(obj1: any, obj2: any, keyFn: (obj: any) => any): any {
        if (Array.isArray(obj1) && Array.isArray(obj2)) {
            // 规则4：处理对象构成的数组
            for (const item2 of obj2) {
                const key2 = keyFn(item2);
                const index = obj1.findIndex(item1 => keyFn(item1) === key2);
                if (index > -1) {
                    obj1[index] = this.deepMerge(obj1[index], item2, keyFn);
                } else {
                    obj1.push(item2);
                }
            }
            return obj1;
        } else if (typeof obj1 === 'object' && typeof obj2 === 'object' && obj1 !== null && obj2 !== null) {
            // 规则3：处理子对象
            for (const key in obj2) {
                obj1[key] = this.deepAppend(obj1[key], obj2[key], keyFn);
            }
            return obj1;
        } else if (typeof obj1 === 'function' && typeof obj2 === 'function') {
            return obj2;//方法类型的如果合并起来很容易出问题，还难追踪
            // 处理 function 类型字段
            return function (this: any, ...args: any[]) {
                obj1.apply(this, args);
                return obj2.apply(this, args);
            };
        } else {
            // 规则1和2：处理普通值类型和普通值类型数组
            return obj2 !== undefined ? obj2 : obj1;
        }
    }

    /**
     * 合并两个结构，产生新的结构
     * @param obj1
     * @param obj2
     */
    public static mergeSchema(obj1: any, obj2: any) {
        return this.deepMerge(obj1, obj2, item => {
            if (!item) return item;
            if (item.hasOwnProperty('field')) return item['field'];
            if (item.hasOwnProperty('name')) return item['name'];
            return item;
        });
    }

    public static isNullOrEmpty(value: string | null | undefined): boolean {
        return value === null || value === undefined || value.trim() === '';
    }

    /**
     * 将目标对象中的字段清空，将目标对象中所有元素复制过去，源对象的对象地址不变
     * @param targetObj
     * @param sourceObj
     */
    public static coverObject(targetObj: Record<string, any> | undefined, sourceObj: Record<string, any> | undefined) {
        targetObj = targetObj ?? {};
        // 清空原来的对象
        for (const key in targetObj) {
            if (Object.prototype.hasOwnProperty.call(targetObj, key)) {
                delete targetObj[key];
            }
        }
        // 复制新对象的属性
        Object.assign(targetObj, sourceObj ?? {});
    }

    /**
     * 将源对象元素清空，将目标对象中的元素都复制过去，源对象地址不变
     * @param targetObj
     * @param sourceObj
     */
    public static coverArray(targetObj: [] | undefined, sourceObj: [] | undefined) {
        targetObj = targetObj ?? [];
        targetObj.splice(0, targetObj.length);
        targetObj.push(...sourceObj ?? []);
    }


    public static deepMerge(obj1: any, obj2: any, keyFn: (obj: any) => any): any {
        if (Array.isArray(obj1) && Array.isArray(obj2)) {
            // 规则4：处理对象构成的数组
            const resultArray: any[] = [...obj1];
            for (const item2 of obj2) {
                const key2 = keyFn(item2);
                const index = resultArray.findIndex(item1 => keyFn(item1) === key2);
                if (index > -1) {
                    resultArray[index] = this.deepMerge(resultArray[index], item2, keyFn);
                } else {
                    resultArray.push(item2);
                }
            }
            return resultArray;
        } else if (typeof obj1 === 'object' && typeof obj2 === 'object' && obj1 !== null && obj2 !== null) {
            // 规则3：处理子对象
            const result: any = {...obj1};
            for (const key in obj2) {
                if (obj2.hasOwnProperty(key)) {
                    result[key] = this.deepMerge(obj1[key], obj2[key], keyFn);
                }
            }
            return result;
        } else if (typeof obj1 === 'function' && typeof obj2 === 'function') {
            // 处理 function 类型字段
            return function (this: any, ...args: any[]) {
                obj1.apply(this, args);
                obj2.apply(this, args);
            };
        } else {
            // 规则1和2：处理普通值类型和普通值类型数组
            return obj2 !== undefined ? obj2 : obj1;
        }
    }

    /**
     * @param ms 睡眠时间（毫秒）
     * @returns Promise<unknown>
     */
    public static sleep(ms: number) {
        return new Promise(resolve => {
            setTimeout(() => {
                resolve('wake up')
            }, ms)
        })
    }

    public static randomString(length: number) {
        const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_';
        return _.times(length, () => _.sample(chars)).join('');
    }

    public static isRef(value: any): boolean {
        return isReactive(value) || isRef(value);
    }

    public static async waitFor(check: () => boolean, timeOut: number) {
        let round = 0;
        while (!check()) {
            await this.sleep(50);
            if ((round++) * 50 > timeOut)
                break;
        }
    }

    /**
     * @param key cookie的键
     * @param value cookie的值
     * @param expires cookie的过期时间（天数），不传则关闭会话后失效，传参为负数则清除该cookie
     * @param path cookie生效路径范围，默认"/"全局生效
     */
    public static setCookie(key: string, value: string | number, expires: number = 0, path: string = '/') {
        let cookie = `${key}=${encodeURIComponent(value)};path=${path};SameSite=strict;Secure}`
        if (expires !== 0) {
            const date = new Date()
            date.setDate(date.getDate() + expires)
            cookie += `;expires=${date.toUTCString()}`
        }
        document.cookie = cookie
    }

    /**
     * @param key cookie的键
     * @returns cookie的值
     */
    public static getCookie(key: string) {
        const reg = new RegExp("(^| )" + encodeURIComponent(key) + "=([^;]+)")
        const match = document.cookie.match(reg)
        return match ? decodeURIComponent(match[2]) : ""
    }

    /**
     *
     * @param key 移除的cookie的键名
     */
    public static removeCookie(key: string) {
        this.setCookie(key, '', -1)
    }

    public static random(min: number, max: number): number {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }

    public static test() {
        debugger
        return '';
    }
}