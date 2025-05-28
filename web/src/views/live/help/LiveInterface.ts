import {R} from "@/utils/R";
import {IValueDisplay} from "@/types/dto";
import {IAppInfo} from "@/types/interfaces";

export class CefHelp {

    static async closeWindow(): Promise<R> {
        return await this.exeFun('closeWindow') as R;
    }

    static async switchWindow(): Promise<R> {
        return await this.exeFun('switchWindow') as R;
    }

    static async minWindow(): Promise<R> {
        return await this.exeFun('minWindow') as R;
    }

    static async startWatchBarrage(roomId: string, liveObserverCookie: string, loginToken: string): Promise<R> {
        return await this.exeFun('startWatchBarrage', roomId, liveObserverCookie, loginToken) as R;
    }

    static async stopWatchBarrage(): Promise<R> {
        return await this.exeFun('stopWatchBarrage') as R;
    }

    static async startBrowser(url: string, dataPath: string): Promise<R> {
        return await this.exeFun('startBrowser', url, dataPath) as R;
    }

    static async downloadFile(url: string, fileName: string): Promise<R> {
        return await this.exeFun('downloadFile', url, fileName) as R;
    }

    static async fileExists(fileName: string): Promise<R<boolean>> {
        return await this.exeFun('fileExists', fileName) as R<boolean>;
    }

    static async updateNewVersion(zipFilePath: string): Promise<R> {
        return await this.exeFun('updateNewVersion', zipFilePath) as R;
    }

    static async hideBorder(noBorder?: boolean): Promise<R> {
        return await this.exeFun('hideBorder', noBorder) as R;
    }

    static async closeBrowser(): Promise<R> {
        return await this.exeFun('closeBrowser') as R;
    }

    static async getAppInfo(): Promise<R<IAppInfo>> {
        return await this.exeFun('getAppInfo') as R<IAppInfo>;
    }

    static async getDeviceId(): Promise<R<string>> {
        return await this.exeFun('getDeviceId') as R<string>;
    }

    static async getBrowserCookie(): Promise<R<ICookie[]>> {
        return await this.exeFun('getBrowserCookie') as R<ICookie[]>;
    }

    static async getHtml(url: string, headers: Record<string, string> | undefined, cookies: ICookie[], postJson?: string | undefined): Promise<R<string>> {
        return await this.exeFun('getHtml', url, headers, cookies, postJson) as R<string>;
    }

    static async getVoiceDeviceList(): Promise<R<IValueDisplay[]>> {
        return await this.exeFun('getVoiceDeviceList') as R<IValueDisplay[]>;
    }

    static async getSoundCardVolume(id: string): Promise<R<number>> {
        return await this.exeFun('getSoundCardVolume', id) as R<number>;
    }

    static async setSoundCardVolume(id: string, volume: number): Promise<R> {
        return await this.exeFun('setSoundCardVolume', id, volume) as R;
    }

    static async setupVirtualSoundCard(): Promise<R> {
        return await this.exeFun('setupVirtualSoundCard') as R;
    }

    static async getPlayStatus(): Promise<R<IVoicePlayStatus>> {
        return await this.exeFun('getPlayStatus') as R<IVoicePlayStatus>;
    }

    static async playVoice(data: IVoicePlayData): Promise<R<IVoicePlayStatus>> {
        return await this.exeFun('playVoice', data) as R<IVoicePlayStatus>;
    }

    static async isIdInQueue(id: string): Promise<R<boolean>> {
        return await this.exeFun('isIdInQueue', id) as R<boolean>;
    }

    static async pausePlay(): Promise<R> {
        return await this.exeFun('pausePlay') as R;
    }

    static async stopPlay(): Promise<R> {
        return await this.exeFun('stopPlay') as R;
    }

    static async resumePlay(): Promise<R> {
        return await this.exeFun('resumePlay') as R;
    }

    static isInFrame(): boolean {
        return !!this.getFun('resumePlay');
    }


    static async exeFun(funName: string, ...args: any[]): Promise<R> {
        const fun = this.getFun(funName);
        if (!fun) return R.error('浏览器中无法使用此功能，请在专用客户端中运行');
        const r = await fun.call(null, ...args);
        if (!r) {
            return R.ok();
        }
        return r as R;
    }

    static getFun(funName: string): Function | undefined {
        const w = window as any;
        if (!w['dotnetObject']) {
            //msg.error('此功能需要在专用客户端中运行才能使用');
            console.error('dotnetObject为空');
            return;
        }
        return w['dotnetObject'][funName];
    }

}

export interface ILiveAccount {
    id: string | undefined;
    platform: string | undefined;
    roleType: string | undefined;
    name: string | undefined;
    platformUserId: string | undefined;
    authJson: string | undefined;

}

export interface LiveAccountSaveDto {
    accountId: string | undefined;
    platformAccountId: string;
    platformAccountName: string;
    platform: number;
    roleType: number;
    authJson: string;
}

export interface IDyAccountAuthVm {
    douyin_unique_id: string,
    nick_name: string,
    douyin_uid: string,
    company_name: string,
    cookie: string,
}

/**
 * 抖音百应
 */
export interface IByAccountAuthVm {
    buyin_account_id: string,
    user_name: string,
    user_id: string,
    origin_uid: string,
    shop_id: string,
    cookie: string,
}

export interface ICookie {
    name: string;
    value: string;
    domain: string | undefined;
}

export interface IHeaderItem {
    key: string;
    value: string;
}

export interface IWebRoomInfo {
    roomId?: string,
    roomTitle?: string,
    roomUserCount?: string,
    /**
     * 2正在直播，4停止
     */
    status?: string,
    uniqueId?: string,
    avatar?: string,

}

export interface IBarrageMessage {
    Type: number,
    Data: any,
}

export interface DouyinMsgBase {
    /** 弹幕ID */
    MsgId: string;

    /** 用户数据 */
    User?: IDouyinUser;

    /** 消息内容 */
    Content?: string;

    /** 房间号 */
    RoomId: number;

    /** web直播间ID（如果需要，可取消注释） */
    // webRoomId: number;
}

export interface IDouyinUser {
    /** 真实ID */
    id: string;

    /** ShortId */
    ShortId: number;

    /** 自定义ID */
    DisplayId?: string;

    /** 昵称 */
    NickName?: string;

    /** 未知 */
    Level: number;

    /** 支付等级 */
    PayLevel: number;

    /** 性别 1男 2女 */
    Gender: number;

    /** 生日 */
    Birthday: number;

    /** 手机 */
    Telephone?: string;

    /** 头像地址 */
    Avatar?: string;

    /** 用户主页地址 */
    SecUid?: string;

    /** 粉丝团信息 */
    FansClub?: IDouyinFansClub;

    /** 粉丝数 */
    FollowerCount: number;

    /** 关注状态 0 未关注 1 已关注 2 不明 */
    FollowStatus: number;

    /** 关注数 */
    FollowingCount: number;
}

export interface IDouyinFansClub {
    /** 粉丝团名称 */
    ClubName?: string;

    /** 粉丝团等级，没加入则0 */
    Level: number;
}

export interface DouyinMsgFansClub extends DouyinMsgBase {
    /** 粉丝团消息类型, 升级1，加入2 */
    Type: number;

    /** 粉丝团等级 */
    Level: number;
}

export interface DouyinMsgGift extends DouyinMsgBase {
    /** 礼物ID */
    GiftId: number;

    /** 礼物名称 */
    GiftName?: string;

    /** 礼物分组ID */
    GroupId: number;

    /** 本次(增量)礼物数量 */
    ComboCount: number;

    /** 组内礼物数量 */
    GroupCount: number;

    /** 总礼物数量 */
    TotalCount: number;

    /** 是否结束重复 1: 结束，0: 未结束 */
    RepeatEnd: number;

    /** 礼物数量(连续的) */
    RepeatCount: number;

    /** 抖币价格 */
    DiamondCount: number;

    /** 送礼目标(连麦直播间有用) */
    ToUser?: IDouyinUser;
}

/** 抖音点赞消息 */
export interface DouyinMsgLike extends DouyinMsgBase {
    /** 点赞数量 */
    Count: number;

    /** 总共点赞数量 */
    Total: number;
}

/** 抖音加入直播间消息 */
export interface DouyinMsgMember extends DouyinMsgBase {
    /** 当前直播间人数 */
    MemberCount: number;
}

/** 抖音直播间统计消息 */
export interface DouyinMsgRoomUserSeq extends DouyinMsgBase {
    /** 当前直播间用户数量 */
    OnlineUserCount: number;

    /** 累计直播间用户数量 */
    TotalUserCount: number;

    /** 累计直播间用户数量 显示文本 */
    TotalUserCountStr?: string;

    /** 当前直播间用户数量 显示文本 */
    OnlineUserCountStr?: string;
}

/** 抖音分享消息 */
export interface DouyinMsgShare extends DouyinMsgBase {
    /** 分享目标 */
    ShareType: ShareTypeEnum;

    ShareTarget: string;
}

/** 直播间分享目标 */
export enum ShareTypeEnum {
    /** 微信 */
    Wechat = 1,

    /** 朋友圈 */
    CircleOfFriends = 2,

    /** 微博 */
    Weibo = 3,

    /** QQ空间 */
    Qzone = 4,

    /** QQ */
    QQ = 5,

    /** 抖音好友 */
    Douyin = 112,
}

/** 弹幕 */
export interface DouyinMsgChat extends DouyinMsgBase {
}

export interface DouyinMsgControl extends DouyinMsgBase {
}

export interface DouyinMsgSocial extends DouyinMsgBase {
}

export interface ILiveScriptVoiceDetailVm {
    id: string;
    text: string;
}

export interface IVoicePlayStatus {
    /**
     * 0,stoped,1,Playing,2,Paused
     */
    playState: number;
    isInsert: boolean;
    mainAudioTotal: number;
    mainAudioCurrent: number;
    insertAudioTotal: number;
    insertAudioCurrent: number;
    mainQueueCount: number;
    insertQueueCount: number;
    mainId: string;
    insertId: string;
    mainText: string;
    insertText: string;

}

export interface IVoicePlayData {
    id?: string;
    soundCardId: string;
    /**
     * 1,追加；2插入；3清空后再播放
     */
    playType: number;
    voice: string;
    text: string;
    volume: number;

}

export interface ILiveScriptRow {
    id: string;
    content: string;
    type: string;//插入，生成，回复
    voiceBase64?: string;
}

export interface IAiVerticalAnchorEditVm {
    id: number;

    /** 名称 */
    name?: string;

    /** 主播语音 */
    primaryTtsModelId?: number;

    /** 助播语音 */
    secondaryTtsModelId?: number;

    /** 生成话术模板（支持多个） */
    scriptTemplateIds?: string;

    /** 聊天回复模板（支持多个） */
    chatTemplateIds?: string;
}

export interface ILiveSettingVm {
    socialReply?: string,
    fansClubReply?: string,
    likeReply?: string,
    memberReply?: string,
    giftReply?: string,
    insertVoice?: string,
    interactMsgCount?: number,
    replyMsgCount?: number,
    replySetting?: IReplySettingItem[],
}

export interface IReplySettingItem {
    name: string,
    content: string,
    matchRules?: ReplyMatchRule[],
}

export interface ReplyMatchRule {
    keyword: string,
    isFuzzy: boolean,
}

export interface IShelfTaskConfigEditVm {
    id: string,
    name: string,
    dataJson: string,
}
