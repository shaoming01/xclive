<script setup lang="ts">
import {IFullTableSchema} from "@/types/schema";
import {useLivePlay} from "@/views/live/useLivePlay";
import {useBarrage} from "@/views/live/useBarrage";
import {nextTick, ref} from "vue";
import InteractSetting from "@/views/live/com/InteractSetting.vue";
import ReplySetting from "@/views/live/com/ReplySetting.vue";

const observerId = ref('');
const {
  wsStatus,
  barrageTableRef,
  startWatchBarrage,
  roomNo,
  roomInfo,
  disConnectWss,
  getChatContent,
  getInteractContent,
} = useBarrage(observerId);


const {
  aiVerticalAnchor,
  aiVerticalAnchorKey,
  scriptTableRef,
  manualGetNewScript,
  anchorModel1,
  anchorModel2,
  anchorVolume1,
  anchorVolume2,
  anchorSpeed1,
  anchorSpeed2,
  startLive,
  stop,
  operateId,
  currentLine,
  needSetupVirtualSoundCard,
  soundCardList, soundCardKey, currentTime, totalTime,
  setupVirtualSoundCard, soundCard, playing, refreshSoundCards,
  currentIsInsert, manualPlay, manualPlayText, manualPlayTextRef, addTag,
  chkInteract, interactBegin, interactEnd,
  interactLeftSecond, interactSettingVisible,
  chkReply, replyBegin, replyEnd,
  replyLeftSecond, replySettingVisible,
  insertLeftSecond, insertEnd, insertBegin,
  chkInsert, insertSettingVisible,
  listenTts,
} = useLivePlay(observerId, roomInfo, onLogMsg, getInteractContent, getChatContent);


const logTable: IFullTableSchema = {
  columns: [{
    field: 'content', headerName: '运行日志', width: 200, cellRender: {
      comPath: '/src/components/grid/column/LongStringRender.vue',
    }
  }],
  rowData: [],
}
const logTableRef = ref(logTable)

function onLogMsg(content: string) {
  logTableRef.value.options?.addNewRows([{content: content}]);
}

async function clickInteractSetting() {
  interactSettingVisible.value = false;
  await nextTick();
  interactSettingVisible.value = true;
}

async function insertSettingClicked() {
  insertSettingVisible.value = false;
  await nextTick();
  insertSettingVisible.value = true;
}

async function clickReplySetting() {
  replySettingVisible.value = false;
  await nextTick();
  replySettingVisible.value = true;
}

function canRefresh() {
  if (playing.value) {
    msg.error('当前正在直播不能刷新，请先停止直播');
    return false;
  }
  if (wsStatus.value?.status == 1 || wsStatus.value?.status == 3) {
    msg.error('当前公屏在连接中，请先中断公屏连接');
    return false;
  }
  return true;
}

function clearLog() {
  logTableRef.value && (logTableRef.value.rowData = []);
}

//暴露一个方法出去
defineExpose({canRefresh});
</script>

<template>
  <FlexLayout style="padding-bottom: 5px;">
    <FlexLayoutContent>
      <ARow style="height: 100%">
        <ACol span="8">
          <FlexLayout style="padding: 5px;">
            <FlexLayoutHeader>
              <ARow :align="'middle'" :wrap="false" :gutter="[10,10]">
                <ACol flex="auto">
                  <AInputGroup compact>
                    <AInput placeholder="请输入房间号码" style="width: calc(100% - 110px);" v-model:value="roomNo">
                    </AInput>
                    <AButton :icon="getIcon('icon-a-014_fangdajing')" @click="startWatchBarrage">获取公屏</AButton>
                  </AInputGroup>
                </ACol>
                <ACol style="text-align: center;">
                  <ATooltip>
                    <template #title>{{ wsStatus.message }}</template>
                    <Icon :name="wsStatus.icon" :style="{color:wsStatus.color}" @click="disConnectWss"></Icon>
                  </ATooltip>
                </ACol>
              </ARow>
              <ARow :align="'middle'" style="margin-top:5px;" v-if="roomInfo?.roomUserCount" :wrap="false"
                    :gutter="[10,10]">
                <ACol span="3"><img alt="主播头像" style="width: 28px;border-radius: 4px;" :src="roomInfo?.avatar"/>
                </ACol>
                <ACol flex="1" style="color: #1890ff">{{ roomInfo?.roomTitle }}</ACol>
                <ACol span="7" v-if="roomInfo?.status=='2'">在线：<span
                    style="font-weight: bold;color: #07c160">{{ roomInfo?.roomUserCount }}</span></ACol>
                <ACol span="7" v-if="roomInfo?.status!='2'">直播结束</ACol>
              </ARow>
            </FlexLayoutHeader>
            <FlexLayoutContent style="padding-top:5px;">
              <FullTable :schema="barrageTableRef"></FullTable>
            </FlexLayoutContent>
            <FlexLayoutFooter>
              <ARow :align="'middle'" style="margin-top:5px;" :gutter="5">
                <ACol v-if="false">
                  <AButton type="text" :icon="getIcon('icon-shezhi')"
                           @click="clickInteractSetting">
                  </AButton>
                  <InteractSetting v-if="interactSettingVisible"></InteractSetting>
                </ACol>
                <ACol>
                  <ACheckbox v-model:checked="chkInteract">AI互动</ACheckbox>
                </ACol>
                <ACol>
                  <AInputNumber style="width:60px;" size="small" min="0" max="300"
                                v-model:value="interactBegin"></AInputNumber>
                </ACol>
                <ACol style="text-align: center">
                  至
                </ACol>
                <ACol>
                  <AInputNumber style="width:60px;" size="small" v-model:value="interactEnd"></AInputNumber>
                </ACol>
                <ACol v-if="chkInteract" flex="auto" style="text-align: center">
                  <span style="color: #4e69fd">{{ interactLeftSecond }}</span>
                </ACol>
              </ARow>
              <ARow :align="'middle'" style="margin-top:5px;" :gutter="5">
                <ACol v-if="false">
                  <AButton type="text" :icon="getIcon('icon-shezhi')" @click="clickReplySetting">
                  </AButton>
                  <ReplySetting v-if="replySettingVisible"></ReplySetting>

                </ACol>
                <ACol>
                  <ACheckbox v-model:checked="chkReply">AI回复</ACheckbox>
                </ACol>
                <ACol>
                  <AInputNumber style="width:60px;" size="small" v-model:value="replyBegin"></AInputNumber>

                </ACol>
                <ACol style="text-align: center">
                  至
                </ACol>
                <ACol>
                  <AInputNumber style="width:60px;" size="small" v-model:value="replyEnd"></AInputNumber>
                </ACol>
                <ACol v-show="chkReply" flex="auto" style="text-align: center">
                  <span style="color: #4e69fd">{{ replyLeftSecond }}</span>
                </ACol>
              </ARow>

              <ARow style="margin-top:5px;">
                <ACol flex="auto">
                  <ATextarea ref="manualPlayTextRef" :rows="3" v-model:value="manualPlayText">
                  </ATextarea>
                </ACol>
              </ARow>
              <ARow style="margin-top:5px;" :gutter="5">
                <ACol>
                  <AButton size="small" type="default" @click="addTag('{时间}')">+时间</AButton>
                </ACol>
                <ACol>
                  <AButton size="small" type="default" @click="addTag('{人数}')">+人数</AButton>
                </ACol>
                <ACol flex="1" style="text-align: right">
                  <ATooltip title="以助播的语音播放一段自定义语音">
                    <LoadingBtn size="small" type="primary" :icon="getIcon('icon-play2')" @click="manualPlay">试听
                    </LoadingBtn>
                  </ATooltip>

                </ACol>
              </ARow>
            </FlexLayoutFooter>
          </FlexLayout>

        </ACol>
        <ACol span="9">
          <FlexLayout style="padding: 5px;">
            <FlexLayoutHeader>
              <ARow :align="'middle'">
                <ACol flex="auto">
                  <AInputGroup compact>
                    <DataSelectInput placeholder="选择AI主播"
                                     :dataSourceApi="{apiUrl:'api/sys/ListValueDisplay',postParams:{type:11}}"
                                     v-model:value="aiVerticalAnchor" :key="aiVerticalAnchorKey"
                                     style="width: calc(100% - 32px);"></DataSelectInput>
                    <AButton :icon="getIcon('icon-shuaxin')" @click="aiVerticalAnchorKey++"></AButton>
                  </AInputGroup>
                </ACol>
                <ACol>
                  <ATooltip title="开始直接时会自动生成话术，点此按钮可提前生成话术预览">
                    <LoadingBtn @click="manualGetNewScript">生成话术</LoadingBtn>
                  </ATooltip>
                </ACol>
              </ARow>
              <ARow :align="'middle'" style="padding-top:5px;">
                <ACol span="24"><span style="color:rgb(35,35,35)">【话术列表】</span></ACol>
              </ARow>
            </FlexLayoutHeader>
            <FlexLayoutContent style="padding-top:5px;">
              <FullTable :schema="scriptTableRef"></FullTable>
            </FlexLayoutContent>
            <FlexLayoutFooter>
              <ARow :align="'middle'" style="margin-top:2px; ">
                <ACol>
                  <AButton type="text" :icon="getIcon('icon-play2')" @click="listenTts(anchorModel1,anchorVolume1)">
                  </AButton>
                </ACol>
                <ACol span="9">
                  <DataSelectInput size="small" placeholder="选择主播声音" v-model:value="anchorModel1"
                                   :dataSourceApi="{apiUrl:'api/sys/ListValueDisplay',postParams:{type:8}}"/>
                </ACol>
                <ACol>
                  <AButton type="text" :icon="getIcon('icon-play2')" @click="listenTts(anchorModel2,anchorVolume2)">
                  </AButton>
                </ACol>
                <ACol span="9">
                  <DataSelectInput size="small" placeholder="选择助播声音" v-model:value="anchorModel2"
                                   :dataSourceApi="{apiUrl:'api/sys/ListValueDisplay',postParams:{type:8}}"/>
                </ACol>

              </ARow>
              <ARow :align="'middle'" style="margin-top:2px;">
                <ACol>
                  <AButton type="text" :icon="getIcon('icon-shengyin_shiti')">
                  </AButton>
                </ACol>
                <ACol span="9">
                  <ASlider v-model:value="anchorVolume1"/>
                </ACol>
                <ACol>
                  <AButton type="text" :icon="getIcon('icon-shengyin_shiti')">
                  </AButton>
                </ACol>
                <ACol span="9">
                  <ASlider v-model:value="anchorVolume2"/>
                </ACol>

              </ARow>
              <ARow v-if="false" :align="'middle'" style="margin-top:2px;">
                <ACol>
                  <AButton type="text" :icon="getIcon('icon-zuoxiyusu')">
                  </AButton>
                </ACol>
                <ACol span="9">
                  <ASlider v-model:value="anchorSpeed1"/>
                </ACol>
                <ACol>
                  <AButton type="text" :icon="getIcon('icon-zuoxiyusu')">
                  </AButton>
                </ACol>
                <ACol span="9">
                  <ASlider v-model:value="anchorSpeed2"/>
                </ACol>

              </ARow>
              <ARow v-if="false" :align="'middle'" style="margin-top:2px;">
                <ACol>
                  <AButton type="text" :icon="getIcon('icon-shezhi')"
                           @click="insertSettingClicked"
                  >
                  </AButton>
                  <InteractSetting v-if="insertSettingVisible"></InteractSetting>
                </ACol>
                <ACol>
                  <ACheckbox v-model:checked="chkInsert">插入</ACheckbox>
                </ACol>
                <ACol span="4">
                  <AInput v-model:value="insertBegin" size="small"></AInput>

                </ACol>
                <ACol span="4" style="text-align: center">
                  至
                </ACol>
                <ACol span="4">
                  <AInput v-model:value="insertEnd" size="small"></AInput>
                </ACol>
                <ACol span="4" style="text-align: center">
                  <span style="color: #4e69fd">{{ insertLeftSecond }}</span>
                </ACol>
              </ARow>
              <ARow v-if="false" :align="'middle'">
                <ACol>
                  <AButton type="text" :icon="getIcon('icon-shezhi')">
                  </AButton>
                </ACol>
                <ACol>
                  <ACheckbox>场控</ACheckbox>
                </ACol>
                <ACol span="4">
                  <AInput value="5" size="small"></AInput>
                </ACol>
                <ACol span="4" style="text-align: center">
                  至
                </ACol>
                <ACol span="4">
                  <AInput value="10" size="small"></AInput>
                </ACol>
                <ACol span="4" style="text-align: center">
                  <span style="color: #4e69fd">6</span>
                </ACol>
              </ARow>
              <ARow :align="'middle'" style="margin-top:2px;" :gutter="10">
                <ACol v-if="false">
                  <AButton \type="default">场控评论</AButton>
                </ACol>
                <ACol v-if="false">
                  <AButton type="default">音画同步</AButton>
                </ACol>
                <ACol flex="1" style="text-align: right">
                  <AButtonGroup>
                    <AButton type="primary" @click="startLive" :icon="getIcon(playing?'icon-pause':'icon-zhibo3')">
                      {{ playing ? '暂停' : '开始直播' }}
                    </AButton>
                    <ATooltip title="停止当前播放，清空话术">
                      <AButton type="primary" danger :icon="getIcon('icon-stop')" @click="stop">
                      </AButton>
                    </ATooltip>
                  </AButtonGroup>


                </ACol>

              </ARow>

            </FlexLayoutFooter>
          </FlexLayout>
        </ACol>
        <ACol span="7">
          <FlexLayout style="padding: 5px;">
            <FlexLayoutHeader>
              <ARow :align="'middle'">
                <ACol span="12">
                  <DataSelectInput placeholder="选择观察员" v-model:value="observerId"
                                   :dataSourceApi="{apiUrl:'api/sys/ListValueDisplay',postParams:{type:6}}"></DataSelectInput>
                </ACol>
                <ACol span="12">
                  <DataSelectInput placeholder="选择操作员" v-model:value="operateId"
                                   :dataSourceApi="{apiUrl:'api/sys/ListValueDisplay',postParams:{type:7}}"></DataSelectInput>
                </ACol>

              </ARow>
              <ARow :align="'middle'" style="padding-top:5px;">
                <ACol flex="1"><span style="color:rgb(35,35,35)">【运行日志】</span></ACol>
                <ACol>
                  <AButton size="small" @click="clearLog">清空日志</AButton>
                </ACol>
              </ARow>
            </FlexLayoutHeader>
            <FlexLayoutContent style="padding-top:5px;">
              <FullTable :schema="logTableRef"></FullTable>
            </FlexLayoutContent>

          </FlexLayout>
        </ACol>
      </ARow>
    </FlexLayoutContent>
    <FlexLayoutFooter style="padding: 0 5px;">
      <ADivider style="margin: 2px 0;"></ADivider>
      <ARow :align="'middle'" :wrap="false" :gutter="10" style="padding-right: 20px;">
        <ACol v-if="needSetupVirtualSoundCard">
          <AButton @click="setupVirtualSoundCard">安装虚拟声卡</AButton>
        </ACol>
        <ACol span="4">
          <AInputGroup compact>
            <DataSelectInput placeholder="选择声卡" :data-source="soundCardList" v-model:value="soundCard"
                             :key="soundCardKey" style="width: calc(100% - 32px);"></DataSelectInput>
            <AButton :icon="getIcon('icon-shuaxin')" @click="refreshSoundCards"></AButton>
          </AInputGroup>
        </ACol>
        <ACol span="3">
          <ASlider v-model:value="currentTime" :max="totalTime" disabled/>
        </ACol>
        <ACol flex="1" style="white-space: nowrap;overflow: hidden;text-overflow: ellipsis;"
              :class="currentIsInsert?'insertStyle':''">
          {{
            currentLine
          }}
        </ACol>
      </ARow>
    </FlexLayoutFooter>
  </FlexLayout>

</template>

<style scoped>
.insertStyle {
  color: #3351f3;
}
</style>
