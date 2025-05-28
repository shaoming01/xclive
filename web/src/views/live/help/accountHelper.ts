import {R} from "@/utils/R";
import {IDyAccountAuthVm, ILiveAccount} from "@/views/live/help/LiveInterface";

export class accountHelper {
    static async getAccount(accountId: string): Promise<R<ILiveAccount>> {
        const accountRe = await apiHelper.request<ILiveAccount>('api/LiveAccount/LiveAccountGetEditVm', {id: accountId});
        if (!accountRe.success) {
            return R.error('获取观察员信息失败：' + accountRe.message);
        }
        return R.ok(accountRe.data);
    }

    static getCookieString(account: ILiveAccount | undefined) {
        if (!account) {
            return ""
        }
        const auth = JSON.parse(account.authJson!) as IDyAccountAuthVm;
        return auth.cookie;
    }
}