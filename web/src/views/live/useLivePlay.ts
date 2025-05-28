import {ref} from 'vue'
import {IFullTableSchema} from "@/types/schema";
import {CellDoubleClickedEvent, RowClassParams} from "ag-grid-community";
import {R} from "@/utils/R";
import {
    CefHelp, DouyinMsgChat,
    IAiVerticalAnchorEditVm,
    ILiveScriptRow,
    ILiveScriptVoiceDetailVm, IVoicePlayData,
    IVoicePlayStatus, IWebRoomInfo
} from "@/views/live/help/LiveInterface";
import {IValueDisplay} from "@/types/dto";
import _ from "lodash";

export function useLivePlay(observerId: Ref<string>, roomInfo: Ref<IWebRoomInfo | undefined>, onLogMsg: (msg: string) => any, getInteractContent: () => Promise<string>, getChatContent: () => Promise<string>) {
    const voiceCacheCount = 3;
    const scriptCachedCount = 6;
    const aiVerticalAnchor = ref('');
    const anchorDto = ref<IAiVerticalAnchorEditVm | undefined>(undefined);
    const aiVerticalAnchorKey = ref(0);
    const soundCardKey = ref(0);
    const currentLine = ref('');//当前台词
    const currentIsInsert = ref(false);//当前台词
    const anchorModel1 = ref('');//语音
    const anchorModel2 = ref('');//语音
    const anchorVolume1 = ref(50);//音量
    const anchorVolume2 = ref(50);//音量
    const anchorSpeed1 = ref(50);//速度
    const anchorSpeed2 = ref(50);//速度
    const currentPlayIndex = ref(0);//当前播放位置
    const manualPlayText = ref('');
    const manualPlayTextRef = ref();
    const operateId = ref('');
    const currentTime = ref(0);
    const totalTime = ref(100);

    const chkInteract = ref(false);
    const interactBegin = ref(10);
    const interactEnd = ref(30);
    const interactLeftSecond = ref('');
    const interactSettingVisible = ref(false);

    const chkReply = ref(false);
    const replyBegin = ref(10);
    const replyEnd = ref(30);
    const replyLeftSecond = ref('');
    const replySettingVisible = ref(false);

    const chkInsert = ref(false);
    const insertBegin = ref(10);
    const insertEnd = ref(30);
    const insertLeftSecond = ref('');
    const insertSettingVisible = ref(false);


    const playing = ref(false);
    let stopBuildVoice = true;
    const scriptTable: IFullTableSchema = {
        columns: [
            {field: 'type', headerName: '类型', width: 60, suppressSort: true, suppressHeaderMenuButton: true},
            {
                field: 'content',
                headerName: '话术',
                width: 200,
                suppressSort: true,
                suppressHeaderMenuButton: true,
                cellRender: {
                    comPath: '/src/components/grid/column/LongStringRender.vue',
                }
            }],
        rowData: [],
        gridOptions: {
            getRowStyle(params: RowClassParams<any, any>) {
                const style: Record<string, string> = {};
                if (params.rowIndex == currentPlayIndex.value) {
                    style.background = '#7df831';
                }
                if (params.data['voiceBase64']) {
                    style.color = '#0e2df3';
                }
                return style;
            },
            async onCellDoubleClicked(event: CellDoubleClickedEvent<any>) {
                if (event.rowIndex == null || event.rowIndex < 0)
                    return;
                await CefHelp.stopPlay();
                currentPlayIndex.value = event.rowIndex;
            },
        }
    }

    const scriptTableRef = ref(scriptTable)


    watch(() => playing.value, async () => {
        if (playing.value) {
            stopBuildVoice = false;
            await CefHelp.resumePlay();
        } else {
            await CefHelp.pausePlay();
        }
    })
    watch(() => currentPlayIndex.value, () => {
        scriptTableRef.value.options?.redrawRows();
    })
    watch(() => aiVerticalAnchor.value, async () => {
        const getRe = await apiHelper.request<IAiVerticalAnchorEditVm>('api/AiVerticalAnchor/AiVerticalAnchorGetEditVm', {id: aiVerticalAnchor.value})
        if (!getRe.success) return msg.error(getRe.message);
        anchorDto.value = getRe.data;

    })

    async function getNewScript(): Promise<R> {
        var req = {aiVerticalAnchorId: aiVerticalAnchor.value, chatText: ""};
        const newScriptRe = await apiHelper.request<ILiveScriptVoiceDetailVm[]>('api/LiveScript/BuildAiVerticalAnchorScript', req);
        if (!newScriptRe.success) return R.error(newScriptRe.message);
        const newArr: ILiveScriptVoiceDetailVm[] = newScriptRe.data ?? [];
        const newRows: ILiveScriptRow[] = newArr.map(d => {
            return {
                content: d.text,
                id: d.id,
                type: 'AI话术',
            }
        });
        scriptTableRef.value.options?.addNewRows(newRows);
        return R.ok();
    }

    function addTag(tag: string) {
        const textArea = manualPlayTextRef.value?.['resizableTextArea']?.['textArea'] as HTMLTextAreaElement | undefined;
        if (!textArea) return

        const start = textArea.selectionStart
        const end = textArea.selectionEnd
        const value = textArea.value

        textArea.value = value.slice(0, start) + tag + value.slice(end)

        nextTick(() => {
            textArea.focus()
            const newPos = start + tag.length
            textArea.setSelectionRange(newPos, newPos)
        })
    }

    async function loadScript(id: string): Promise<R> {
        const newScriptRe = await apiHelper.request<ILiveScriptVoiceDetailVm[]>('api/LiveScript/LiveScriptVoiceDetailQueryList', undefined, {QueryObject: {headerId: id}});
        if (!newScriptRe.success) return R.error(newScriptRe.message);
        const newArr: ILiveScriptVoiceDetailVm[] = newScriptRe.data ?? [];
        const newRows: ILiveScriptRow[] = newArr.map(d => {
            return {
                content: d.text,
                id: d.id,
                type: 'AI话术',
            }
        });
        scriptTableRef.value.options?.addNewRows(newRows);
        return R.ok();
    }

    async function listenTts(modelId: string, volume: number) {
        if (!modelId) return msg.error('请选择一个语音');
        const re = await apiHelper.request<IVoiceVm>('api/LiveScript/ListenVoice', {modelId: modelId});
        if (!re.success) return msg.error(re.message);
        const playRe = await CefHelp.playVoice({
            playType: 2,
            voice: re.data?.voice as string,
            text: '试听',
            volume: volume / 100,
            id: Date.now() + '',
            soundCardId: soundCard.value,
        })
        if (!playRe.success) {
            msg.error(playRe.message);
            return;
        }
        msg.success('播放成功')
    }

    async function manualPlay() {
        if (!manualPlayText.value) {
            return msg.error('请输入要播放的内容')
        }
        let text = manualPlayText.value;
        text = text.replace('{时间}', `${new Date().getHours()}点${new Date().getMinutes()}分`);
        text = text.replace('{人数}', `${roomInfo.value?.roomUserCount}`);
        const ttsId = anchorDto.value?.secondaryTtsModelId || anchorModel2.value;
        if (!ttsId) {
            return msg.error('请选择一个助手语音')
        }
        const getRe = await apiHelper.request<IVoiceVm>('api/LiveScript/BuildVoice', undefined, {
            content: text,
            ttsModelId: ttsId,
        });
        if (!getRe.success) {
            msg.error(getRe.message);
            return;
        }
        const playRe = await CefHelp.playVoice({
            playType: 2,
            voice: getRe.data?.voice as string,
            text: manualPlayText.value,
            volume: anchorVolume2.value / 100,
            id: Date.now() + '',
            soundCardId: soundCard.value,
        })
        if (!playRe.success) {
            msg.error(playRe.message);
            return;
        }
        msg.success('播放成功')
    }

    async function manualGetNewScript(): Promise<any> {
        if (!aiVerticalAnchor.value) return msg.error('请选择一个AI主播');
        const re = await getNewScript();
        if (!re.success) return msg.error(re.message);
        msg.success('加入新话术生成');
    }

    async function setupVirtualSoundCard() {
        const r = await CefHelp.setupVirtualSoundCard();
        if (!r.success) msg.error(r.message);
    }

    const needSetupVirtualSoundCard = computed(() => {
        if (!soundCardList.value) return;
        return !soundCardList.value.some(l => l.label.includes('VoiceMeeter'))
    });
    const soundCardList = ref<IValueDisplay[]>([]);
    const soundCard = ref('-1');

    async function refreshSoundCards() {
        const r = await CefHelp.getVoiceDeviceList();
        if (!r.success) return msg.error('获取声卡失败：' + r.message);
        soundCardList.value = r.data ?? [];
        soundCardList.value.unshift({value: '-1', label: '默认声卡'});
    }


    async function startLive() {
        if (!playing.value) {
            if (!aiVerticalAnchor.value) {
                return msg.error('请选择一个AI主播')
            }
            if (!anchorModel1.value) {
                return msg.error('请选择默认主播语音')
            }
            if (!anchorModel2.value) {
                return msg.error('请选择默认副播语音')
            }
        }
        playing.value = !playing.value;

    }

    async function stop() {
        playing.value = false;
        currentPlayIndex.value = 0;
        CefHelp.stopPlay();
        scriptTableRef.value && (scriptTableRef.value.rowData = []);
    }

    async function backgroundBuildVoice() {
        if (stopBuildVoice) return;
        const list = scriptTableRef.value.rowData ?? [];
        if (!list) return;
        let curr = currentPlayIndex.value;
        //释放使用过的声音文件，那个占内存
        for (let i = 0; i < list.length; i++) {
            if (i < currentPlayIndex.value) {
                list[i].voiceBase64 = undefined;
            }
        }
        for (let i = 0; i < voiceCacheCount; i++) {
            if (curr + i >= list.length) {
                break;
            }
            const row = list[curr + i] as ILiveScriptRow;
            if (row.voiceBase64) continue;
            console.log(`生成语音：${curr + i}`, row.content);
            const getRe = await apiHelper.request<IVoiceVm>('api/LiveScript/BuildVoice', undefined, {
                content: row.content,
                ttsModelId: anchorDto.value?.primaryTtsModelId || anchorModel1.value,
            });
            if (!getRe.success) {
                console.error('生成语音出错' + getRe.message);
                if (!await msg.confirm('生成语音出错，点【确定】重试，点【取消】停止。\n' + getRe.message)) {
                    stopBuildVoice = true;
                }
                return;
            }
            row.voiceBase64 = getRe.data?.voice;
            scriptTableRef.value.options?.redrawRows();
        }

    }

    async function backgroundSyncPlayStatus() {
        const playStatusRe = await CefHelp.getPlayStatus();
        if (!playStatusRe.success) {
            return
        }
        const playStatus = playStatusRe.data as IVoicePlayStatus;

        const current = playStatus.isInsert ? playStatus.insertAudioCurrent : playStatus.mainAudioCurrent;
        const total = playStatus.isInsert ? playStatus.insertAudioTotal : playStatus.mainAudioTotal;
        currentTime.value = current / 1000;
        totalTime.value = total / 1000;
        const currentText = playStatus.isInsert ? playStatus.insertText : playStatus.mainText;
        currentLine.value = `${currentText}`;
        currentIsInsert.value = playStatus.isInsert;

        //更新当前行号
        const list: ILiveScriptRow[] = scriptTableRef.value.rowData ?? [];
        if (!list) return;
        let curr = list.findIndex(l => l.id == playStatus.mainId);
        if (curr >= 0)
            currentPlayIndex.value = curr;
    }


    async function backgroundPlayVoice() {
        if (!playing.value) return;
        const playStatusRe = await CefHelp.getPlayStatus();
        if (!playStatusRe.success) {
            if (!await msg.confirm('播放器状态获取出错，点【确定】重试，点【取消】停止播放')) {
                playing.value = false;
            }
            return
        }
        const playStatus = playStatusRe.data as IVoicePlayStatus;

        if (playStatus.playState == 0) {//如果正在播放，就认为当前播放的就是本行，停止状态才需要推送当前行过去
            const succ = await playRow(currentPlayIndex.value);
            if (!succ) return;
        }
        //推送下一条语音
        await playRow(currentPlayIndex.value + 1);
    }

    async function playRow(rowIndex: number): Promise<boolean> {
        const list: ILiveScriptRow[] = scriptTableRef.value.rowData ?? [];
        const row = list.length > rowIndex ? list[rowIndex] : undefined;
        if (!row || !row.voiceBase64) {
            return false;
        }
        const isRe = await CefHelp.isIdInQueue(row.id);
        if (!isRe.success) {
            if (!await msg.confirm('播放语音出错，点【确定】重试，点【取消】停止播放。\n' + isRe.message)) {
                playing.value = false;
            }
            return false;
        }
        if (isRe.data) {
            return true;//此Id推送过了，不重复推送
        }

        const playRe = await CefHelp.playVoice({
            playType: 1,
            voice: row.voiceBase64,
            text: row.content,
            volume: anchorVolume1.value / 100,
            id: row.id,
            soundCardId: soundCard.value,
        })
        if (!playRe.success) {
            if (!await msg.confirm('播放语音出错，点【确定】重试，点【取消】停止播放。\n' + playRe.message)) {
                playing.value = false;
            }
            return false;
        }
        return true;


    }

    async function backgroundBuildScript() {
        if (!playing.value) return;
        const list = scriptTableRef.value.rowData ?? [];
        let curr = currentPlayIndex.value;
        if (curr + scriptCachedCount < list.length) {
            return;
        }
        console.log(`开始生成话术`);
        msg.success('开始生成新话术');
        const newScriptRe = await getNewScript();
        if (!newScriptRe.success) {
            console.error('生成新话术失败' + newScriptRe.message);
            if (!await msg.confirm('生成新话术失败，点【确定】重试，点【取消】停止。\n' + newScriptRe.message)) {
                playing.value = false;
            }
            return;
        }
        msg.success('生成新话术成功！')

    }

    async function backgroundReply() {
        //if (playing.value || stopBuildVoice) return;
        //处理倒计时
        if (chkReply.value && replyBegin.value >= 0 && replyEnd.value >= 0 && replyEnd.value >= replyBegin.value) {
            let left = _.toNumber(replyLeftSecond.value);
            if (left == 0) {
                await startReply();
                left = _.random(replyBegin.value, replyEnd.value);
            }
            replyLeftSecond.value = (--left).toString();

        }
        if (chkInteract.value && interactBegin.value >= 0 && interactEnd.value >= 0 && interactEnd.value >= interactBegin.value) {
            let left = _.toNumber(interactLeftSecond.value);
            if (left == 0) {
                await startInteract();
                left = _.random(interactBegin.value, interactEnd.value);
            }
            interactLeftSecond.value = (--left).toString();

        }
        if (chkInsert.value && insertBegin.value >= 0 && insertEnd.value >= 0 && insertEnd.value >= insertBegin.value) {
            let left = _.toNumber(insertLeftSecond.value);
            if (left == 0) {
                await startInsertVoice();
                left = _.random(insertBegin.value, insertEnd.value);
            }
            insertLeftSecond.value = (--left).toString();

        }


    }

    async function startInteract() {
        const msg = await getInteractContent();
        if (!msg) return;

        var req = {aiVerticalAnchorId: aiVerticalAnchor.value, interactText: msg};
        const newScriptRe = await apiHelper.request<ILiveScriptVoiceDetailVm[]>('api/LiveScript/BuildAiVerticalAnchorScript', req);
        if (!newScriptRe.success) {
            onLogMsg('互动出错：' + msg + ',' + newScriptRe.message);
            return R.error(newScriptRe.message);
        }
        const newArr: ILiveScriptVoiceDetailVm[] = newScriptRe.data ?? [];
        if (newArr.length == 0) {
            onLogMsg('互动未返回结果：' + msg)
            console.log(newArr);
        }
        const voiceList: IVoicePlayData[] = [];
        for (const row of newArr) {
            const ttsModelId = _.startsWith(row.text, '[助播]') ?
                (anchorDto.value?.secondaryTtsModelId || anchorModel2.value) :
                (anchorDto.value?.primaryTtsModelId || anchorModel1.value);
            const volume = _.startsWith(row.text, '[助播]') ? anchorVolume2.value / 100 : anchorVolume1.value / 100
            const getRe = await apiHelper.request<IVoiceVm>('api/LiveScript/BuildVoice', undefined, {
                content: row.text,
                ttsModelId: ttsModelId,
            });
            if (!getRe.success) {
                onLogMsg('互动语音出错：' + getRe.message);
                return;
            }
            const voiceData: IVoicePlayData = {
                playType: 2,
                voice: getRe.data?.voice as string,
                text: row.text,
                volume: volume,
                id: row.id,
                soundCardId: soundCard.value,
            };
            voiceList.push(voiceData);

        }
        for (const voiceData of voiceList) {
            const playRe = await CefHelp.playVoice(voiceData)
            if (!playRe.success) {
                onLogMsg('播放互动出错:' + playRe.message);
                return;
            }
            onLogMsg('互动成功:' + voiceData.text);
        }

    }

    async function startReply() {
        const content = await getChatContent();
        if (!content) return;

        var req = {aiVerticalAnchorId: aiVerticalAnchor.value, chatText: content};
        const newScriptRe = await apiHelper.request<ILiveScriptVoiceDetailVm[]>('api/LiveScript/BuildAiVerticalAnchorScript', req);
        if (!newScriptRe.success) {
            onLogMsg('回复出错：' + newScriptRe.message + content);
            return R.error(newScriptRe.message);
        }
        const newArr: ILiveScriptVoiceDetailVm[] = newScriptRe.data ?? [];
        if (newArr.length == 0) {
            onLogMsg('回复未返回结果：' + content)
            console.log(newArr);
        }
        for (const row of newArr) {
            const ttsModelId = _.startsWith(row.text, '[助播]') ?
                (anchorDto.value?.secondaryTtsModelId || anchorModel2.value) :
                (anchorDto.value?.primaryTtsModelId || anchorModel1.value);
            const volume = _.startsWith(row.text, '[助播]') ? anchorVolume2.value / 100 : anchorVolume1.value / 100
            const getRe = await apiHelper.request<IVoiceVm>('api/LiveScript/BuildVoice', undefined, {
                content: row.text,
                ttsModelId: ttsModelId,
            });
            if (!getRe.success) {
                onLogMsg('回复语音出错：' + getRe.message);
                return;
            }
            const playRe = await CefHelp.playVoice({
                playType: 2,
                voice: getRe.data?.voice as string,
                text: row.text,
                volume: volume,
                id: row.id,
                soundCardId: soundCard.value,
            })
            if (!playRe.success) {
                onLogMsg('播放回复出错:' + playRe.message);
                return;
            }
            onLogMsg('回复成功:' + row.text);
        }


    }

    async function startInsertVoice() {

    }

    const taskList: { stop: () => void }[] = [];
    onMounted(() => {
        refreshSoundCards();
        //提前生成语音数据
        taskList.push(ext.asyncLoop(backgroundBuildVoice, 500));
        //同步播放器状态
        taskList.push(ext.asyncLoop(backgroundSyncPlayStatus, 500));
        //播放声音
        taskList.push(ext.asyncLoop(backgroundPlayVoice, 1000));
        //生成话术
        taskList.push(ext.asyncLoop(backgroundBuildScript, 500));
        //互动
        taskList.push(ext.asyncLoop(backgroundReply, 1000));
    })
    onBeforeUnmount(() => {
        taskList.forEach((task) => {
            task.stop()
        })
    })
    return {
        aiVerticalAnchor,
        aiVerticalAnchorKey,
        scriptTable,
        scriptTableRef,
        currentLine,
        anchorModel1,
        anchorModel2,
        anchorVolume1,
        anchorVolume2,
        anchorSpeed1,
        anchorSpeed2,
        currentPlayIndex,
        manualGetNewScript,
        startLive,
        stop,
        operateId,
        refreshSoundCards,
        soundCard, playing, currentTime, totalTime, soundCardKey,
        needSetupVirtualSoundCard, soundCardList, setupVirtualSoundCard,
        currentIsInsert, manualPlay, manualPlayText, manualPlayTextRef,
        chkInteract, interactBegin, interactEnd,
        interactLeftSecond, interactSettingVisible,
        chkReply, replyBegin, replyEnd,
        replyLeftSecond, replySettingVisible,
        insertLeftSecond, insertEnd, insertBegin,
        chkInsert, insertSettingVisible, addTag,
        listenTts,
    }
}

interface IVoiceVm {
    voice: string;
}
