<script setup lang="ts">
import {IFullTableSchema, ITableToolBarItemSchema} from "@/types/schema";

const props = defineProps<{
  /**
   * @ignore
   */
  tableSchema: IFullTableSchema,
  /**
   * @ignore
   */
  item: ITableToolBarItemSchema,
  moduleId: string,
  sysModuleId: string,
}>();


const audioSrc = ref();
const audioRef = ref<HTMLAudioElement>();
const isPlaying = ref(false);
const currentSeconds = ref(0);
const durationSeconds = ref(0);

const currentTime = computed(() => formatTime(currentSeconds.value));
const duration = computed(() => formatTime(durationSeconds.value));

const modelId = ref('');
const loadingData = ref(false);

async function togglePlay() {
  const audio = audioRef.value!;
  if (isPlaying.value) {
    audio.pause();
    isPlaying.value = !isPlaying.value;
    return;
  }

  if (!props.tableSchema.currentRow) return msg.error('未选择行');
  if (!modelId.value) return msg.error('请选择声音');

  const rowId = props.tableSchema.currentRow['id'];
  const text = props.tableSchema.currentRow['text'];
  loadingData.value = true;
  const getRe = await apiHelper.request<IVoiceVm>('api/LiveScript/BuildVoice', undefined, {
    content: text,
    ttsModelId: modelId.value,
  })
  loadingData.value = false;
  if (!getRe.success) {
    return msg.error(getRe.message);
  }
  audioSrc.value = 'data:audio/mp3;base64,' + getRe.data?.voice;
  audio.onloadeddata = () => {
    audio.play();
    isPlaying.value = !isPlaying.value;
  };

}

function updateTime() {
  currentSeconds.value = Math.floor(audioRef.value?.currentTime || 0);
}

function onLoaded() {
  durationSeconds.value = Math.floor(audioRef.value?.duration || 0);
}

function onEnded() {
  isPlaying.value = false;
}

function onSliderChange(val: any) {
  if (audioRef.value) {
    audioRef.value.currentTime = val;
    currentSeconds.value = val;
  }
}

function formatTime(seconds: number) {
  const m = String(Math.floor(seconds / 60)).padStart(2, '0');
  const s = String(seconds % 60).padStart(2, '0');
  return `${m}:${s}`;
}

interface IVoiceVm {
  voice: string;
}


</script>

<template>
  <div class="audio-player">
    <ARow align="middle" :gutter="[5,0]">
      <ACol span="8">
        <DataSelectInput placeholder="选择主播声音" v-model:value="modelId"
                         :dataSourceApi="{apiUrl:'api/sys/ListValueDisplay',postParams:{type:8}}"/>
      </ACol>
      <ACol>
        <AButton :loading="loadingData" @click="togglePlay">{{ isPlaying ? '暂停' : '播放' }}</AButton>
      </ACol>
      <ACol><span>{{ currentTime }} / {{ duration }}</span></ACol>
      <ACol span="6">
        <a-slider
            :min="0"
            :max="durationSeconds"
            :value="currentSeconds"
            @change="onSliderChange"
        />
      </ACol>
      <ACol>
        <audio ref="audioRef" :src="audioSrc" @timeupdate="updateTime" @loadedmetadata="onLoaded" @ended="onEnded"/>
      </ACol>
    </ARow>

  </div>
</template>
<style scoped>
.audio-player {
  min-width: 400px;
}
</style>
