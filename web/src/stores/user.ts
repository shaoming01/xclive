import {message} from "ant-design-vue"
import {userApi} from "@/api/userApi";
import {R} from "@/utils/R";
import {IUserLoginInfo} from "@/types/interfaces";

export const userStore = defineStore('user', {
    state: (): IUserLoginInfo => ({}),
    actions: {
        async login(username: string, password: string): Promise<R> {
            const re = await userApi.login(username, password);
            if (!re.success)
                return re;
            Object.assign(this, re.data);
            ext.setCookie('token', this.token ?? '')
            return R.ok();
        },
        async logout(token: string): Promise<string> {
            const re = await userApi.logout(token);
            if (!re.success)
                return re.message ?? '';
            ext.removeCookie('token')
            message.success('退出成功');
            return re.message ?? '';
        },
        async getUserInfo(token: string): Promise<R> {
            const re = await userApi.info(token);
            if (!re.success) {
                if (re.code == 1) {
                    //业务错误
                    ext.removeCookie('token')
                }
                return re;
            }
            Object.assign(this, re.data);
            return R.ok();
        }
    }
})