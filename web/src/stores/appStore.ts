import {IAppInfo} from "@/types/interfaces";
import {CefHelp} from "@/views/live/help/LiveInterface";

export const appStore = defineStore('app', {
    state: (): {
        appInfo: IAppInfo | undefined,
        isCefBrowser?: boolean,
    } => ({
        appInfo: undefined,

    }),
    actions: {
        async ini() {
            if (this.appInfo) {
                return
            }
            const isCefBrowser = CefHelp.isInFrame();
            if (isCefBrowser) {
                const re1 = await CefHelp.getAppInfo();
                if (!re1.success) {
                    return msg.error('获取产品信息出错:' + re1.message)
                }
                this.appInfo = re1.data;
                this.isCefBrowser = true;
            } else {
                const re1 = await apiHelper.request<IAppInfo>('api/Product/Info');
                if (!re1.success) {
                    return msg.error('获取产品信息出错:' + re1.message)
                }
                this.appInfo = re1.data;
            }

            document.title = this.appInfo?.productName ?? '';
        },

    }
})