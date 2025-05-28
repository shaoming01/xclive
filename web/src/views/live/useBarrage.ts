import {ref} from 'vue'
import {IFullTableSchema} from "@/types/schema";
import {accountHelper} from "@/views/live/help/accountHelper";
import {
    CefHelp,
    DouyinMsgChat, DouyinMsgControl, DouyinMsgFansClub, DouyinMsgGift, DouyinMsgLike,
    DouyinMsgMember, DouyinMsgRoomUserSeq,
    DouyinMsgShare, DouyinMsgSocial,
    IBarrageMessage, ILiveSettingVm, IWebRoomInfo
} from "@/views/live/help/LiveInterface";
import _ from "lodash";
import {userStore} from "@/stores/user";

export function useBarrage(observerId: Ref<string>) {
    const roomNo = ref('');

    const roomInfo = ref<IWebRoomInfo>();
    const wsStatus = ref({status: 0, icon: 'icon-icon--1', message: '未连接', color: '#747474'});
    const barrageTable: IFullTableSchema = {
        columns: [{field: 'name', headerName: '昵称', width: 80}, {field: 'content', headerName: '内容', width: 160}],
        rowData: [],
    }
    const barrageTableRef = ref<IFullTableSchema>(barrageTable);

    let replyMsgCount = 3;
    let interactMsgCount = 3;

    function wssOnMessage(statusType: number, arg: any) {
        if (statusType == 1) {
            roomInfo.value = arg as IWebRoomInfo;
        }
        processStatus(statusType, arg);
        if (statusType == 3)
            processMessage(arg)
        console.log(statusType, arg);
    }

    function onChatMsg(chat: DouyinMsgChat) {

    }

    async function disConnectWss() {
        if (wsStatus.value?.status != 3) return;
        if (!await msg.confirm('当前是连接状态，确认断开连接吗？')) {
            return;
        }
        await CefHelp.stopWatchBarrage();
    }

    /**
     * 未处理消息，处理完了就清空
     */
    const unProcessMessageList: IBarrageMessage[] = [];

    function getMessageNicks(msgType: number) {
        const nickList = unProcessMessageList.filter(m => m.Type == msgType)
            .map(m => (m.Data as DouyinMsgRoomUserSeq)?.User?.NickName)
            .filter(n => n);

        const distinctArr = [...new Set(nickList)];
        return distinctArr.slice(0, interactMsgCount).join('、');

    }

    async function getInteractContent() {
        const guanzhu = getMessageNicks(2);
        const jiatuan = getMessageNicks(9);
        const dianzan = getMessageNicks(4);
        const jinru = getMessageNicks(1);
        const liwu = getMessageNicks(5);

        //除了弹幕，其他的都处理完了
        _.remove(unProcessMessageList, l => l.Type != 3);
        let baseText = '';
        if (guanzhu) {
            baseText += guanzhu + '关注了直播间，';
        }
        if (jiatuan) {
            baseText += jiatuan + '加入粉丝团，';
        }
        if (dianzan) {
            baseText += dianzan + '点赞，';
        }
        if (jinru) {
            baseText += jinru + '进入直播间，';
        }
        if (liwu) {
            baseText += liwu + '送了礼物，';
        }
        return baseText;
    }

    async function getChatContent() {
        const msgList = unProcessMessageList.filter(m => m.Type == 3)
            .map(m => {
                const msg = (m.Data as DouyinMsgChat);
                if (!msg || !msg.User?.NickName) return '';
                return msg.User?.NickName + '说：' + msg.Content
            })
            .filter(n => n);
        const distinctMsg = [...new Set(msgList)];
        const content = distinctMsg.slice(0, replyMsgCount).join('；');
        _.remove(unProcessMessageList, l => l.Type == 3);
        return content;
    }

    function processMessage(arg: any) {
        const msg = arg as IBarrageMessage;
        unProcessMessageList.push(msg);
        if (msg.Type == 3) {//弹幕
            const content = msg.Data as DouyinMsgChat;
            pushBarrageMessage(content.User?.NickName, content.Content);
            onChatMsg?.call(null, content);
        } else if (msg.Type == 1) {//来了
            const content = msg.Data as DouyinMsgMember;
            pushBarrageMessage(content.User?.NickName, content.Content);
        } else if (msg.Type == 6) {//分享
            const content = msg.Data as DouyinMsgShare;
            pushBarrageMessage(content.User?.NickName, content.Content);
        } else if (msg.Type == 5) {//礼物
            const content = msg.Data as DouyinMsgGift;
            pushBarrageMessage(content.User?.NickName, `[礼物] 送出 ${content.RepeatCount} 个 ${content.GiftName}`);
        } else if (msg.Type == 9) {//粉丝
            const content = msg.Data as DouyinMsgFansClub;
            pushBarrageMessage(content.User?.NickName, `[粉丝团] 加入了主播粉丝团`);
        } else if (msg.Type == 8) {//直播状态
            const content = msg.Data as DouyinMsgControl;
            pushBarrageMessage(content.User?.NickName, `[系统]当前直播已结束`);
            wsStatus.value.message = "当前直播已结束";
            wsStatus.value.status = 4;

        } else if (msg.Type == 4) {//点赞
            const content = msg.Data as DouyinMsgLike;
            pushBarrageMessage(content.User?.NickName, `[点赞] 点了 ${content.Count} 个赞`);
        } else if (msg.Type == 2) {//关注
            const content = msg.Data as DouyinMsgSocial;
            pushBarrageMessage(content.User?.NickName, `[关注] 关注了主播`);
        } else if (msg.Type == 7) {//分享
            const content = msg.Data as DouyinMsgRoomUserSeq;
            if (roomInfo.value)
                roomInfo.value.roomUserCount = content.OnlineUserCount + '';
        } else if (msg.Type == 11) {//异常
            if (wsStatus.value)
                wsStatus.value.message = `系统连接出错`;
        } else {
            console.log('未处理消息', arg)
        }

    }

    /**
     * 互动消息
     * @param name
     * @param content
     */
    function pushBarrageMessage(name: string | undefined, content: string | undefined) {
        barrageTableRef.value.options?.addNewRows([{name: name + '', content: content}]);
    }

    function processStatus(statusType: number, arg: any) {
        wsStatus.value.status = statusType;
        if (statusType == 1 || statusType == 3) {//开始，正常消息
            wsStatus.value.icon = 'icon-icon--';
            wsStatus.value.color = 'green';
            wsStatus.value.message = '连接中';
        } else if (statusType == 2) {
            wsStatus.value.icon = 'icon-icon--jinggao';
            wsStatus.value.color = '#FABC41';
            wsStatus.value.message = '错误' + arg;
        } else if (statusType == 4) {
            wsStatus.value.icon = 'icon-icon--1';
            wsStatus.value.color = '#747474';
            wsStatus.value.message = '未连接';
        }
    }

    async function startWatchBarrage() {
        let cookieStr = '';
        if (!observerId.value) {
            if (!await msg.confirm('当前没有选择观察员，点【确定】将在无观察员的模式下运行。\n点【取消】重新选择观察员')) return;
        } else {
            const accountRe = await accountHelper.getAccount(observerId.value);
            if (!accountRe.success) {
                return msg.error('获取观察员信息失败：' + accountRe.message);
            }
            cookieStr = accountHelper.getCookieString(accountRe.data);
        }
        if (!roomNo.value) {
            return msg.error('请输入房间号码，比如：直播间网址https://live.douyin.com/12345678，后面的数字12345678就是房间号')
        }
        const w = window as any;
        w['wssOnMessage'] = wssOnMessage;
        const user = userStore()
        const watchRe = await CefHelp.startWatchBarrage(roomNo.value, cookieStr, user.token!);
        if (!watchRe.success) {
            msg.error(watchRe.message)
            return;
        }
    }


    async function ini() {
        const re1 = await apiHelper.request<ILiveSettingVm>('/api/sys/GetSetting', {typeName: 'LiveSettingVm'});
        if (!re1.success) return msg.error(re1.message);
        replyMsgCount = re1.data?.replyMsgCount ?? 3;
        interactMsgCount = re1.data?.interactMsgCount ?? 3;
    }

    onMounted(() => {
        ini();
    })

    return {
        wsStatus,
        barrageTableRef,
        startWatchBarrage,
        roomNo,
        roomInfo,
        disConnectWss,
        getChatContent,
        getInteractContent
    }
}