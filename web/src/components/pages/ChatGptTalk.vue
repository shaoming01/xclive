<template>
  <ALayout style="height: 100%">
    <ALayoutSider style="background-color: white">
      <AMenu v-model:selectedKeys="selectedKeys"
             class="menu">
        <template v-for="item in menu">
          <AMenuDivider v-if="item.type=='divider'"></AMenuDivider>
          <AMenuItem v-else :icon="item.icon" :key="item.key" class="menu-item" @click="switchSession">
            {{ item?.label }}
            <ADropdown :trigger="['click']" v-if="item.key!='0'">
              <EllipsisOutlined @click.stop class="menu-icon"/>
              <template #overlay>
                <AMenu>
                  <APopconfirm :title="'确认删除？【'+item.label+'】'" placement="right"
                               @confirm="()=>deleteSession(item.key)">
                    <AMenuItem @click="">删除该会话</AMenuItem>
                  </APopconfirm>
                </AMenu>
              </template>

            </ADropdown>
          </AMenuItem>
        </template>

      </AMenu>

    </ALayoutSider>
    <FlexLayout>
      <FlexLayoutContent style="padding: 15px;">
        <AResult title="请在下方文本框输入问题，按回车即可开启新的会话" v-if="!chatContent">
          <template #icon>
            <SmileTwoTone/>
          </template>
        </AResult>
        <Markdown :value="chatContent" v-if="chatContent"></Markdown>
      </FlexLayoutContent>
      <FlexLayoutFooter style="padding: 15px;">
        <ARow justify="center">
          <ACol :span="24">
            <ASpin :spinning="spinning">
              <ATextarea size="large" :auto-size="true" v-model:value="msgInput"
                         @keydown="handleKeydown"
              ></ATextarea>
            </ASpin>

          </ACol>

        </ARow>
      </FlexLayoutFooter>
    </FlexLayout>

  </ALayout>


</template>

<script lang="ts" setup>

import {nextTick, ref} from "vue";
import {pageApi} from "@/api/pageApi";
import {getIcon} from "@/composables/useFontIcon";
import {R} from "@/utils/R";

const sessionList = ref<IGptSession[] | undefined>();
const historyList = ref<IGptSessionMessage[]>([]);
const selectedKeys = ref<string[]>(['0']);
const msgInput = ref('');
const spinning = ref(false);

interface IGptSessionMessage {
  id?: string;
  headerId?: string;
  message: string;
  role: string;
}

interface IGptSession {
  id: string,
  title: string
}

interface ICustomMenuItem {
  type?: string | undefined,
  label?: string | undefined,
  key?: string | undefined,
  icon?: any | undefined,
}

const menu: ComputedRef<ICustomMenuItem[]> = computed(() => {
  const newSessionItem = {
    label: '新的会话',
    key: '0',
    icon: () => getIcon('icon-a-014_gengduo-27'),
  }
  const dividerItem = {
    type: 'divider', // Must have
  };
  const list = [];
  list.push(newSessionItem)
  list.push(dividerItem)

  if (!sessionList.value)
    return [newSessionItem, dividerItem];
  const tmpMenu = sessionList.value.map(s => {
    return {label: s.title, key: s.id};
  })
  list.push(...tmpMenu);
  return list;
})
const chatContent = computed(() => {
  if (!historyList.value) return '';
  const list = historyList.value.map(l => {
    if (l.role == 'user')
      return `<h3 style="text-align: right">` + l.message + `</h3>`;
    return l.message;
  });
  return list.join('');

});

async function handleKeydown(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    event.preventDefault(); // 阻止默认行为（如换行）
    spinning.value = true;
    await onSendMessage();
    spinning.value = false;
  }
}

async function deleteSession(sessionId: string | undefined) {
  const re = await pageApi.deleteIds('/api/ChatGpt/DeleteSession', [sessionId ?? '']);
  if (!re.success) {
    msg.error(re.message);
    return;
  }
  if (sessionList.value) {
    const index = sessionList.value.findIndex(s => s.id == sessionId);
    if (index > -1)
      sessionList.value.splice(index, 1);
  }
  if (selectedKeys.value && selectedKeys.value[0] == sessionId) {
    selectedKeys.value = ['0'];
    await switchSession();
  }
  msg.success('删除成功:');
}

async function onSendMessage() {
  if (!msgInput.value) return;
  if (!selectedKeys.value || selectedKeys.value.length == 0) {
    msg.error('选择正常的会话');
    return;
  }
  const inputMsg = msgInput.value;
  let sessionId = selectedKeys.value[0];
  if (sessionId == '0') {
    const createRe = await createNewSession(inputMsg);
    if (!createRe.success || !createRe.data) {
      msg.error(createRe.message);
      return;
    }
    sessionId = createRe.data.id;
  }
  await nextTick();
  historyList.value.push({message: inputMsg, role: 'user'})
  msgInput.value = '';

  const reSend = await pageApi.save('/api/ChatGpt/SendMessage', {sessionId: sessionId, message: inputMsg});
  if (!reSend.success) {
    msg.error(reSend.message);
    return;
  }
  const respMsg = reSend.data as IGptSessionMessage;
  historyList.value.push(respMsg)

}


async function createNewSession(inputMsg: string): Promise<R<IGptSession | undefined>> {
  const newSessionRe = await pageApi.save('/api/ChatGpt/CreateNewSession', {sessionId: '0', message: inputMsg});
  if (!newSessionRe.success) {
    return R.error(newSessionRe.message);
  }
  const newSession = newSessionRe.data as IGptSession;
  if (!newSession)
    return R.error('返回内容为空');
  if (!sessionList.value || sessionList.value?.findIndex(l => l.id === newSession.id) < 0) {
    sessionList.value?.unshift(newSession);

  }
  selectedKeys.value = [newSession.id];
  await nextTick();
  return R.ok(newSession)
}

async function ini() {
  const historyRe = await pageApi.queryList('/api/ChatGpt/QueryList', {
    pageSize: 100, page: 1, queryObject: {}, orderBy: 'id Desc',
  });
  if (!historyRe.success) {
    msg.error(historyRe.message);
    return;
  }
  sessionList.value = historyRe.data;
}

async function switchSession() {
  await nextTick();
  if (!selectedKeys.value || selectedKeys.value.length == 0) {
    historyList.value = [];
    return;
  }
  const sessionId = selectedKeys.value[0];
  const historyRe = await pageApi.queryList('/api/ChatGpt/LoadSession', {
    pageSize: 100,
    page: 1,
    queryObject: {headerId: sessionId},
  });
  if (!historyRe.success) {
    msg.error(historyRe.message);
  }
  historyList.value = historyRe.data as IGptSessionMessage[];

}


ini();

</script>

<style scoped lang="scss">
.menu {
  height: 100%;
  background-color: white;
}

.menu-icon {
  position: absolute;
  right: 10px;
  top: 50%;
  transform: translateY(-50%);
  visibility: hidden;
}

.menu-item:hover .menu-icon {
  opacity: 1;
  visibility: visible;
}

</style>