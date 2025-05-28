import {R} from "@/utils/R";
import {CefHelp, ICookie} from "@/views/live/help/LiveInterface";

/**
 * 坑1：一定要保持签名UA和真实UA一致，否则会被拦截
 * 坑2：Referer，Accept-Encoding必须要有
 * 坑3：后台签名的时候一定要注意参数的URLEnCode不能随便用
 */
export class ByApi {

    public static async getAccountInfo(cookies: ICookie[]): Promise<R<IByLiveAccountResp>> {
        const rHtml = await CefHelp.getHtml('https://buyin.jinritemai.com/index/getUser', undefined, cookies);
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取登录账号失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IByLiveAccountResp;
        if (obj.code != 0) return R.error(obj.msg);
        return R.ok(obj);
    }

    public static async getUrl(baseUrl: string, cookies: ICookie[], queryParams: Record<string, string>, postString: string): Promise<R<string>> {
        let verify = cookies.find(c => c.name == 's_v_web_id')?.value;

        const urlRe = await apiHelper.request<string>('/api/Sign/GetByReqUrl', undefined, {
            baseUrl: baseUrl,
            s_v_web_id: verify,
            userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36 Edg/118.0.2088.122',
            postJson: postString,
            queryParams: queryParams,
        });
        return urlRe

    }

    public static async getPromotionList(cookies: ICookie[], page: number): Promise<R<IByPromotionResp>> {
        const req = {
            title: '',
            page_size: 20,
            page: page,
            pool_type: '2',
            filter_type: 1,
            filter_bound: true,
            request_from: 0,
        }
        const urlGetRe = await this.getUrl('https://buyin.jinritemai.com/pc/selection_tool/promotion/list', cookies, {}, JSON.stringify(req));
        if (!urlGetRe.success) {
            return R.error(urlGetRe.message);
        }
        const url = urlGetRe.data!;

        const rHtml = await CefHelp.getHtml(url, undefined, cookies, JSON.stringify(req));
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取活动商品失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IByPromotionResp;
        if (obj.code != 0) return R.error(obj.msg);
        return R.ok(obj);
    }

    /**
     * 获取正在直播的商品列表
     * @param cookies
     */
    public static async getLiveList(cookies: ICookie[]): Promise<R<IByLiveListResp>> {

        const params: Record<string, string> = {
            source_type: 'force',
        }

        const urlGetRe = await this.getUrl('https://buyin.jinritemai.com/api/anchor/livepc/basic_list', cookies, params, '');
        if (!urlGetRe.success) {
            return R.error(urlGetRe.message);
        }
        const url = urlGetRe.data!;

        const rHtml = await CefHelp.getHtml(url, undefined, cookies);
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取活动商品失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IByLiveListResp;
        if (obj.code != 0) return R.error(obj.msg);
        return R.ok(obj);
    }

    public static async listPromotions(cookies: ICookie[], promotion_ids: string): Promise<R<IByPromotionLiveResp>> {

        const params: Record<string, string> = {
            list_type: '1',
            source_type: 'force',
            promotion_ids: promotion_ids,
            extra: 'hit_marketing_ab',
            promotion_info_fields: 'all',
            room_info_fields: 'all',
        }

        const urlGetRe = await this.getUrl('https://buyin.jinritemai.com/api/anchor/livepc/promotions_v2', cookies, params, '');
        if (!urlGetRe.success) {
            return R.error(urlGetRe.message);
        }
        const url = urlGetRe.data!;

        const rHtml = await CefHelp.getHtml(url, undefined, cookies);
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取活动商品失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IByPromotionLiveResp;
        if (obj.code != 0) return R.error(obj.msg);
        return R.ok(obj);
    }

    /**
     * 上架商品到直播间
     * @param cookies
     * @param products
     */
    public static async bindToLive(cookies: ICookie[], products: IByProduct[]): Promise<R<IByBindToLiveResp>> {
        const data = {
            "promotions": products.map(p => {
                return {
                    promotion_id: p.promotion_id,
                    bind_source: "1",
                    product_id: p.product_id,
                    item_type: 4,
                }
            }),
            "auction_campaign_id": ""
        }
        const urlGetRe = await this.getUrl('https://buyin.jinritemai.com/pc/live/bind', cookies, {}, JSON.stringify(data));
        if (!urlGetRe.success) {
            return R.error(urlGetRe.message);
        }
        const url = urlGetRe.data!;

        const rHtml = await CefHelp.getHtml(url, undefined, cookies, JSON.stringify(data));
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取活动商品失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IByBindToLiveResp;
        if (obj.code != 0) return R.error(obj.msg);
        return R.ok(obj);
    }

    public static async getGptDesc(cookies: ICookie[], productId: string): Promise<R<IBYGptProductDescResp>> {

        const data = `origin_type=pc_buyin_selection_decision&mode=1&product_id=` + productId
        const rHtml = await CefHelp.getHtml('https://buyin.jinritemai.com/ai/getGptGeneratedProductDesc', undefined, cookies, data);
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取活动商品失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IBYGptProductDescResp;
        if (obj.code != 0) return R.error(obj.msg);
        return R.ok(obj);
    }

    public static async packH5(cookies: ICookie[], productId: string, gptDesc: string): Promise<R<IByPackH5Resp>> {
        const params = {
            is_h5: '1',
            is_native_h5: '1',
            origin_type: 'pc_buyin_selection_decision',
        }
        const data = `ui_params=%7B%22from_live%22%3Afalse%2C%22from_video%22%3Anull%2C%22three_d_log_data%22%3Anull%2C%22follow_status%22%3Anull%2C%22which_account%22%3Anull%2C%22ad_log_extra%22%3Anull%2C%22from_group_id%22%3Anull%2C%22bolt_param%22%3Anull%2C%22transition_tracker_data%22%3Anull%2C%22selected_ids%22%3Anull%2C%22window_reposition%22%3Anull%2C%22is_short_screen%22%3Anull%2C%22full_mode%22%3Atrue%7D&use_new_price=1&is_h5=1&bff_type=2&is_in_app=0&origin_type=pc_buyin_selection_decision&promotion_ids=${productId}&meta_param=&source_page=&request_additions=&isFromVideo=false&enable_timing=true&gtp_generated_product_desc=${gptDesc}&is_new_h5_bff=1`
        const urlGetRe = await this.getUrl('https://haohuo.jinritemai.com/aweme/v2/shop/promotion/pack/h5/', cookies, params, data);
        if (!urlGetRe.success) {
            return R.error(urlGetRe.message);
        }
        const url = urlGetRe.data!;

        const rHtml = await CefHelp.getHtml(url, undefined, cookies, data);
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取商品详情失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IByPackH5Resp;
        if (obj.status_code != 0) return R.error('获取商品详情失败');
        return R.ok(obj);
    }

    public static async packDetail(cookies: ICookie[], promotionId: string): Promise<R<IByPackDetailResp>> {
        const params = {
            is_h5: '1',
            origin_type: 'pc_buyin_selection_decision',
        }
        const data = `promotion_id=${promotionId}&enter_from=&meta_param=&is_h5=1`
        const urlGetRe = await this.getUrl('https://haohuo.jinritemai.com/aweme/v2/shop/promotion/pack/detail/', cookies, params, data);
        if (!urlGetRe.success) {
            return R.error(urlGetRe.message);
        }
        const url = urlGetRe.data!;

        const rHtml = await CefHelp.getHtml(url, undefined, cookies, data);
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取商品图片失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IByPackDetailResp;
        if (obj.status_code != 0) return R.error('获取商品图片失败');
        return R.ok(obj);
    }

    public static async unBindToLive(cookies: ICookie[], products: IByProduct[]): Promise<R<IByUnBindToLiveResp>> {
        const data = {
            "promotions": products.map(p => {
                return {promotion_id: p.promotion_id}
            })
        }
        const urlGetRe = await this.getUrl('https://buyin.jinritemai.com/pc/live/unbind', cookies, {}, JSON.stringify(data));
        if (!urlGetRe.success) {
            return R.error(urlGetRe.message);
        }
        const url = urlGetRe.data!;

        const rHtml = await CefHelp.getHtml(url, undefined, cookies, JSON.stringify(data));
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取活动商品失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IByUnBindToLiveResp;
        if (obj.code != 0) return R.error(obj.msg);
        return R.ok(obj);
    }

    public static async setCurrentToLive(cookies: ICookie[], promotionId: string, cancel: boolean): Promise<R<IByUnBindToLiveResp>> {
        const data = `promotion_id=${promotionId}&cancel=${cancel}`;
        const urlGetRe = await this.getUrl('https://buyin.jinritemai.com/api/anchor/livepc/setcurrent', cookies, {}, data);
        if (!urlGetRe.success) {
            return R.error(urlGetRe.message);
        }
        const url = urlGetRe.data!;

        const rHtml = await CefHelp.getHtml(url, undefined, cookies, data);
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取活动商品失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IByUnBindToLiveResp;
        if (obj.code != 0) return R.error(obj.msg);
        return R.ok(obj);
    }


    public static async shopProducts(cookies: ICookie[], page: number, pageSize: number, searchText: string): Promise<R<IByProductResp>> {
        const params: Record<string, string> = {
            page: page.toString(),
            page_size: pageSize.toString(),
            searchText: searchText ?? '',
            product_type: '0',
            filter: 'false',
        }
        const urlGetRe = await this.getUrl('https://buyin.jinritemai.com/api/author/shop/products', cookies, params, '');
        if (!urlGetRe.success) {
            return R.error(urlGetRe.message);
        }
        const url = urlGetRe.data!;
        const rHtml = await CefHelp.getHtml(url, undefined, cookies);
        if (!rHtml.success || !ext.isJson(rHtml.data)) {
            return R.error('获取登录账号失败' + rHtml.message);
        }
        const obj = JSON.parse(rHtml.data!) as IByProductResp;
        if (obj.code != 0) return R.error(obj.msg);
        return R.ok(obj);
    }


}


export interface IByLiveAccountResp {
    code: number,
    msg: string,
    data: {
        buyin_account_id: string,
        user_name: string,
        user_id: string,
        origin_uid: string,
        shop_id: string,
    },
}

export interface IByPromotionResp {
    code: number,
    msg: string,
    total: number,
    data: [{
        promotion_id: string,
        product_id: string,
        price: number,
        cos_fee: number,
        cos_ratio: number,
        title: string,
        new_cover: string,
        price_text: string,
        on_shelf_info: { can_show: boolean },
    }],
}

export interface IByLiveListResp {
    code: number,
    msg: string,
    data: {
        basic_list: [{ promotion_id: string, product_id: string }]
    },
}

export interface IByPromotionLiveResp {
    code: number,
    msg: string,
    data: {
        promotions: {
            promotion_id: string,
            product_id: string,
            title: string
            cover: string
            price_desc: {
                min_price: {
                    origin: number,
                    integer: string,
                    decimal: string,
                },
                price_text: string,
            }
            stock_num: string
        }[]
    },
}

export interface IByBindToLiveResp {
    code: number,
    msg: string,
    data?: {
        success_count: number,
        failure_count: number,
        partial_failure_count: number,
        failure_list?: [{
            bind_status: number,
            bind_reason: string
            product_id: string
            title: string
        }],
    },
}

export interface IByPackH5Resp {
    status_code: number,
    promotion_h5: {
        basic_info_data: {
            price_info: {
                price: {
                    min_price: number,
                    suffix: number,
                },
                discount_price: {
                    prefix: string,
                    min_price: number,
                    suffix: string,
                }
            },
            title_info: {
                title: string,
            }
        },
    }
}

export interface IBYGptProductDescResp {
    code: number,
    msg: string,
    data?: {
        gtp_generated_product_desc: string,
    },
}

export interface IByPackDetailResp {
    status_code: number,
    detail_info: {
        product_format: {
            format: {
                name: string,
                message: {
                    desc: string
                }[]
            }[]
        }[],
        detail_imgs_new: {
            image: {
                url_list: string[]
            }
        }[]
    },
}

export interface IByUnBindToLiveResp {
    code: number,
    msg: string

}

export interface IByProductResp {
    code: number,
    msg: string,
    data: {
        total: number,
        list: IByProduct[]
    },
}

export interface IByProduct {
    product_id: string,
    promotion_id: string,
    name: string,
    cover: string,
    price: number,
    bind_time: number,
    price_text: string,
    detail_url: string,
    add_source: string,
    commission: {
        cos_ratio: number,
        commission_type: number
    },
}


