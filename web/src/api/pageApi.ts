import {R} from "@/utils/R";
import {apiHelper} from "@/utils/apiHelper";
import {IQueryParam, ISearchGroup, IValueDisplay} from "@/types/dto";
import {IApiCall, IMenuVm, IModuleVm} from "@/types/schema";

export class pageApi {
    static _cache = new memCache()

    static async exeApiCall(api: IApiCall): Promise<R> {
        if (api.cacheable) {
            const key = JSON.stringify(api);
            const cacheVal = this._cache.getKey(key);
            if (cacheVal) {
                return R.ok(cacheVal);
            }
        }

        const apiRe = await apiHelper.request(api.apiUrl, api.queryParams, api.postParams);
        if (!apiRe.success) return R.error(apiRe.message);
        if (api.cacheable) {
            const key = JSON.stringify(api);
            this._cache.setKey(key, apiRe.data);
        }
        return R.ok(apiRe.data);
    }

    static async getPageSearchGroups(path: string | undefined): Promise<R<ISearchGroup[]>> {
        let url = "/api/user/QuerySearchGroups";
        const query: IQueryParam = {
            page: -1, pageSize: -1, queryObject: {
                path: path,
            }
        }
        return apiHelper.request(url, undefined, query);
    }

    static async saveUserSearchGroup(group: ISearchGroup): Promise<R<ISearchGroup>> {
        let url = "/api/user/SaveSearchGroup";
        return apiHelper.request(url, undefined, group);
    }

    static async deleteSearchGroup(groupId: string): Promise<R<ISearchGroup>> {
        let url = "/api/user/DeleteSearchGroup";
        let par = {
            id: groupId,
        };
        return apiHelper.request(url, par);
    }

    static async getGroupConditions(groupId: string | undefined): Promise<R<Record<string, any>>> {
        let url = "/api/user/QuerySearchGroups";
        const query: IQueryParam = {
            page: -1, pageSize: -1, queryObject: {
                id: groupId,
            }
        }
        const re = await apiHelper.request(url, undefined, query);
        if (!re.success || !Array.isArray(re.data) || re.data.length == 0) {
            return {
                message: re.message,
                success: re.success,
            }
        }
        const group = re.data[0] as ISearchGroup;
        return R.ok(group.conditions);
    }


    static async queryList(url: string, postObj: IQueryParam | undefined): Promise<R<any[]>> {
        return apiHelper.request(url, undefined, postObj);
    }

    static async getById(url: string, id: string): Promise<R<Record<string, any>>> {
        let par = {
            id: id,
        };

        return apiHelper.request(url, par);
    }

    static async queryCount(url: string, postObj: IQueryParam | undefined): Promise<R<number>> {
        return apiHelper.request(url, undefined, postObj);
    }

    /**
     * 获取原始Module
     * @param moduleId
     * @param sysModuleId
     */
    static async getModule(moduleId?: string, sysModuleId?: string): Promise<R<IModuleVm>> {
        let url = "/api/Module/GetModule";
        let par = {
            moduleId: moduleId,
            sysModuleId: sysModuleId,
        };

        return apiHelper.request(url, par);
    }

    /**
     * 获取完整Module，主要将SysProps内容合并进Props字段中去
     * @param moduleId
     * @param sysModuleId
     */
    static async getFullModule(moduleId?: string, sysModuleId?: string): Promise<R<IModuleVm>> {
        const cacheKey = 'FullModule:' + moduleId + sysModuleId;
        let module = ext.deepCopy(this._cache.getKey(cacheKey));
        if (module) {
            return R.ok(module);
        }
        const moduleRe = await this.getModule(moduleId, sysModuleId);
        if (!moduleRe.success) {
            return moduleRe;
        }
        module = moduleRe.data;
        if (module && module.sysModuleName) {
            const sysModuleRe = await pageApi.getSysModuleSchema(module.sysModuleName);
            if (!sysModuleRe.success) {
                return sysModuleRe;
            }
            module.props = ext.mergeSchema(sysModuleRe.data, module.props);
        }
        this._cache.setKey(cacheKey, module);
        return R.ok(ext.deepCopy(module));
    }

    static async deleteIds(url: string, ids: string[]): Promise<R> {
        let par = {
            ids: ids.join(','),
        };

        return apiHelper.request(url, par);
    }

    static async saveModule(data: Record<string, any> | undefined): Promise<R<IModuleVm>> {
        let url = "/api/Module/Save";
        return apiHelper.request(url, undefined, data);
    }


    static async saveMenu(data: Record<string, any> | undefined): Promise<R<IMenuVm>> {
        let url = "/api/Menu/MenuSaveEditVm";
        return apiHelper.request(url, undefined, data);
    }

    static async save(url: string, data: Record<string, any> | undefined): Promise<R<Record<string, any>>> {
        return apiHelper.request(url, undefined, data);
    }

    static async queryUser(query: IQueryParam): Promise<R<[]>> {
        let url = "/api/user/query";
        return apiHelper.request(url, undefined, query);
    }

    static async listValueDisplay(type: number | undefined, enumType: string | undefined): Promise<R<IValueDisplay[]>> {
        const url = "/api/Sys/ListValueDisplay";
        const query = {type: type, enumTypeName: enumType};
        return apiHelper.request(url, undefined, query);
    }

    static async getSysModuleSchema(moduleName: string): Promise<R<Record<string, any>>> {
        let url = "/api/Sys/GetSysModuleSchema";
        let par = {
            moduleName: moduleName,
        };
        return apiHelper.request(url, par);
    }


}