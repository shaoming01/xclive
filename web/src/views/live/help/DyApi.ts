import {R} from "@/utils/R";
import {CefHelp, ICookie, IDyAccountAuthVm} from "@/views/live/help/LiveInterface";
import {accountHelper} from "@/views/live/help/accountHelper";
import {IByProduct} from "@/views/live/help/ByApi";

export class DyApi {
    public static async getAccountInfo(cookies: ICookie[]): Promise<R<IDyLiveAccount>> {
        const rHtml = await CefHelp.getHtml('https://leads.cluerich.com/aweme/v1/bluev/user/info', undefined, cookies);
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取登录账号失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IDyLiveAccount;
        if (obj.status_code > 0) return R.error(obj.status_message);
        return R.ok(obj);
    }

    /**
     * 封装好的操作上下架方法
     * @param accountId
     * @param cardIds
     * @param option
     */
    public static async shelfOpt(accountId: string, cardIds: string[], option: 'shelf' | 'unShelf'): Promise<R> {
        const accRe = await accountHelper.getAccount(accountId);
        if (!accRe.success) {
            return R.error(accRe.message)
        }
        const auth = JSON.parse(accRe.data!.authJson!) as IDyAccountAuthVm;

        const cookies = this.parseCookie(auth.cookie);
        //直播间状态
        const statusRe = await this.getLiveStatus(cookies);
        if (!statusRe.success) {
            return R.error(statusRe.message)
        }
        const status = statusRe.data!.data.status;
        const isLive = status != 2;
        const roomId = statusRe.data!.data.room_id;
        //直播间卡片类型
        let agg_card_id = '0';//如果没有选择过转换类型，这里默认就是0，选择过了1次以后，下次就能获取到值了
        let tranformType = 7;//线索服务类型就是7
        let shelfedIds: string[] = [];//默认已上架Id为空的
        //获取已经上架的卡片
        const seletedCardTypeRe = await this.getSelectedCardList(cookies, isLive ? roomId : '0');
        if (!seletedCardTypeRe.success) return R.error(seletedCardTypeRe.message);
        if (seletedCardTypeRe.data?.card_list.length) {
            agg_card_id = seletedCardTypeRe.data!.card_list[0].card_id;
            tranformType = seletedCardTypeRe.data!.card_list[0].TransformType;
            //已经上架的卡片
            const shelfListRe = await this.getShelfCardList(cookies, roomId, agg_card_id, auth.douyin_unique_id);
            if (!shelfListRe.success) {
                return R.error(shelfListRe.message)
            }
            shelfedIds = shelfListRe.data!.card_list.map(c => c.card_id);//已经上架的Id列表
        }

        let finalIds = [];//最终需要保存的所有Id
        if (option == 'shelf') {//上架，去重后加进去
            finalIds = [...cardIds.filter(id => !shelfedIds.includes(id)), ...shelfedIds];
        } else {
            finalIds = shelfedIds.filter(id => !cardIds.includes(id));

        }
        //保存状态
        const saveRe = await this.saveAggCard(cookies, isLive ? roomId : '0', agg_card_id, tranformType, finalIds);
        if (!saveRe.success) {
            return R.error(saveRe.message)
        }
        return R.ok();

    }

    public static async popOpt(accountId: string, cardId: string, option: 'pop' | 'unPop'): Promise<R> {
        const accRe = await accountHelper.getAccount(accountId);
        if (!accRe.success) {
            return R.error(accRe.message)
        }
        const auth = JSON.parse(accRe.data!.authJson!) as IDyAccountAuthVm;

        const cookies = this.parseCookie(auth.cookie);
        //直播间状态
        const statusRe = await this.getLiveStatus(cookies);
        if (!statusRe.success) {
            return R.error(statusRe.message)
        }
        const status = statusRe.data!.data.status;
        const isLive = status != 2;
        const roomId = statusRe.data!.data.room_id;

        const switchRe = await this.switchCard(cookies, roomId, cardId, option == 'pop' ? 3 : 4)
        if (!switchRe.success) {
            return R.error(switchRe.message)
        }
        return R.ok();

    }

    public static async getLiveStatus(cookies: ICookie[]): Promise<R<IDyLiveStatus>> {
        const rHtml = await CefHelp.getHtml('https://leads.cluerich.com/live_console/status', undefined, cookies);
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取直播状态失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IDyLiveStatus;
        if (obj.code > 0) {
            return R.error(obj.msg);
        }
        return R.ok(obj);
    }

    /**
     * 获取卡片类型
     * @param cookies
     * @param roomId 如果未在播，传'0'才行
     */
    public static async getSelectedCardList(cookies: ICookie[], roomId: string): Promise<R<IDyLiveCardSelected>> {
        const rHtml = await CefHelp.getHtml('https://leads.cluerich.com/aweme/v1/saiyan/live/card/selected/?room_id=' + roomId, undefined, cookies);
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取上架卡片失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IDyLiveCardSelected;
        if (obj.status_code > 0) {
            return R.error(obj.status_msg);
        }
        return R.ok(obj);
    }

    /**
     * 获取已经上架卡片
     * @param cookies
     * @param roomId
     * @param shelfId 和arr_card_id是同个值
     * @param anchorId
     */
    public static async getShelfCardList(cookies: ICookie[], roomId: string, shelfId: string, anchorId: string): Promise<R<IDyLiveShelfCard>> {
        const req = {
            shelf_id: shelfId,
            room_id: roomId,
            anchor_id: anchorId,
        }
        const rHtml = await CefHelp.getHtml('https://leads.cluerich.com/aweme/v1/saiyan/live/shelf/card/detail', undefined, cookies, JSON.stringify(req));
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取上架卡片失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IDyLiveShelfCard;
        if (obj.status_code > 0) {
            return R.error(obj.status_msg);
        }
        return R.ok(obj);
    }

    /**
     * 保存已上架的Id
     * @param cookies
     * @param roomId 未开播传0
     */
    public static async saveAggCard(cookies: ICookie[], roomId: string, agg_card_id: string, agg_card_type: number, cardIds: string[]): Promise<R> {
        const dataList = cardIds.map(cardId => {
            return {
                card_id: cardId,
                auth_type: 0,
            }
        })
        const postObj = {
            source_type: 1,
            room_id: roomId,
            agg_card_id: agg_card_id,
            agg_card_type: agg_card_type,
            card_data: JSON.stringify(dataList),
        }
        const rHtml = await CefHelp.getHtml('https://leads.cluerich.com/aweme/v1/saiyan/live/agg/card/save', undefined, cookies, JSON.stringify(postObj));
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('保存上架状态失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IDyAggCardSaveResp;
        if (obj.status_code > 0) {
            return R.error(obj.status_msg);
        }
        return R.ok(obj);
    }

    /**
     * 切换卡片讲解状态
     * @param cookies
     * @param roomId
     * @param cardId
     * @param operation 3讲解，4取消讲解
     */
    public static async switchCard(cookies: ICookie[], roomId: string, cardId: string, operation: number): Promise<R> {
        const postObj = {
            room_id: roomId,
            operation: operation,
            source: 0,
            card_id: cardId,
        }
        const rHtml = await CefHelp.getHtml('https://leads.cluerich.com/aweme/v1/saiyan/live/card/switch', undefined, cookies, JSON.stringify(postObj));
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('操作讲解失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IDyAggCardSaveResp;
        if (obj.status_code > 0) {
            return R.error(obj.status_msg);
        }
        return R.ok(obj);
    }

    /**
     * 获取全部风车卡片
     * @param cookies
     */
    public static async getServiceCard(cookies: ICookie[]): Promise<R<IDyServiceCard[]>> {
        const rHtml = await CefHelp.getHtml('https://leads.cluerich.com/aweme/v1/saiyan/live/card/list/?page=1&room_id=&limit=20&live_card_type=21,14&transform_type=7&source=0', undefined, cookies);
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取数据失败' + rHtml.message);
        }
        const resp = JSON.parse(rHtml.data!);
        if (resp['status_code']) return R.error(resp['status_msg']);
        const reList = resp['card_list'] as IDyServiceCard[];
        return R.ok(reList);
    }

    public static cookieToString(cookies: ICookie[]): string {
        const arr = cookies.map(c => c.name + "=" + c.value);
        return arr.join(';');
    }

    public static parseCookie(cookieStr: string | undefined): ICookie[] {
        if (!cookieStr || typeof cookieStr !== "string") return [];

        return cookieStr
            .split(";")
            .map(part => part.trim())
            .filter(part => part.includes("="))
            .map(part => {
                const eqIndex = part.indexOf("=");
                const name = part.slice(0, eqIndex).trim();
                const value = part.slice(eqIndex + 1).trim();
                return {
                    name,
                    value,
                    domain: undefined, // Cookie 头没有 Domain 信息
                } as ICookie;
            });
    }

}

export interface IDyLiveAccount {
    status_code: number,
    status_message: string,
    douyin_unique_id: string,
    nick_name: string,
    douyin_uid: string,
    company_name: string,
}

export interface IDyLiveStatus {
    code: number,
    msg: string,
    data: {
        status: number,
        room_id: string,
        shop_first_up: boolean,
    }
}


export interface IDyLiveCardSelected {
    status_code: number,
    status_msg: string,
    card_list: [{
        TransformType: number,
        card_id: string,
        is_agg_card: boolean,
    }],

}

export interface IDyLiveShelfCard {
    status_code: number,
    status_msg: string,
    card_list: [{
        card_id: string,
        service_info: {
            service_title: string,
        }
    }],

}

export interface IDyAggCardSaveResp {
    status_code: number,
    status_msg: string,
    agg_card_id: string,

}

export interface IDyServiceCard {
    component_title: string,
    card_id: string,
    service_info: {
        service_title: string,
        service_id: string,
        service_banner_url: string,
    }
}

export interface IShelfTask {
    id: string,
    type: IShelfTaskType,
    title: string,
    taskData: ISleepTaskData | IFengcheTaskData | IHuangcheTaskData
}


export type IShelfTaskType = '等待停顿' | '风车上架' | '风车下架' | '风车讲解' | '黄车上架' | '黄车下架' | '黄车讲解'


export interface ISleepTaskData {
    begin: number;
    end: number;

}

export interface IFengcheTaskData {
    accountId: string,
    accountName: string,
    cards: IDyServiceCard[],
}


export interface IHuangcheTaskData {
    accountId: string,
    accountName: string,
    products: IByProduct[],
}


