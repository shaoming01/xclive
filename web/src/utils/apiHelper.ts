import axios, {AxiosInstance, AxiosRequestConfig, AxiosResponse} from "axios";
import {R} from "@/utils/R";

export class apiHelper {
    static async request<T>(urlPath: string, params?: object | undefined, postObj?: any, inputHeaders?: any): Promise<R<T>> {
        let axObj = this.createCommonAxObj();
        let fullUrl = urlPath;
        let method = postObj ? 'POST' : 'GET';
        let headers = {
            "Content-Type": "application/json",
            "Accept": "application/json, */*"
        };
        headers = {...inputHeaders, ...headers};
        /*if (inputHeaders) {
            headers = Ex.appendObj(headers, inputHeaders);
        }*/
        const axPar: AxiosRequestConfig = {
            method: method == 'GET' ? 'GET' : 'POST',
            data: postObj,
            url: fullUrl,
            params: params,
            headers: headers,
        };
        try {
            const _response = await axObj.request<R<T>>(axPar);
            return this.processResponse(_response);
        } catch (error) {
            // 处理请求错误
            const r = new R<T>();
            r.success = false;
            if (error instanceof Error) {
                r.message = error.message;
            } else {
                r.message = error + '';
            }
            return r;
        }
    }

    static createCommonAxObj(): AxiosInstance {
        let ins = axios.create();
        //响应时的拦截
        ins.interceptors.response.use(
            response => {
                return response;
                //return Promise.resolve(response);
            },
            err => {
                let resp = err.response;
                if (!resp) {
                    let resp1: AxiosResponse<string> = {
                        data: '',
                        status: 500,
                        statusText: '服务器无返回',
                        headers: err.response.headers,
                        config: err.response.config,
                    }
                    return Promise.resolve(resp1);

                }
                let resp1: AxiosResponse<string> = {
                    data: resp.data,
                    status: resp.status,
                    statusText: resp.statusText,
                    headers: resp.headers,
                    config: resp.config,
                }
                return Promise.resolve(resp1);

            });
        return ins;
    }

    static processResponse<T>(response: AxiosResponse<R<T>>): R<T> {//对返回进行加工
        if (!response) {
            return R.error<T>('无返回值，可能是网络异常', 500);
        }
        const status = response.status;
        if (status !== 200) {//2xx都是成功的
            return R.error<T>('请求异常：URL：' + response.config.url + '\tERR：' + response.statusText + JSON.stringify(response.data ?? {}), status);
        }
        const data = response.data;
        if (!this.isObject(data)) {
            return R.error<T>('返回内容解析出错：需要检查接口返回内容');
        }
        const re = new R<T>();
        re.success = data.success;
        re.code = data.code;
        re.message = data.message;
        re.data = data.data;
        return re;
    }

    static isObject(value: unknown): boolean {
        return value !== null && typeof value === 'object';
    }

}