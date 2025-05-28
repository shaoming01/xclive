<script setup lang="ts">
import {userStore} from '@/stores/user'
import Logo from "@/assets/logo.png";
import loginpic from "@/assets/loginpic.png";
import {appStore} from "@/stores/appStore";
import Register from "@/views/account/register.vue";
import UseCardKey from "@/views/account/useCardKey.vue";
import ResetPassword from "@/views/account/resetPassword.vue";
import {CefHelp} from "@/views/live/help/LiveInterface";

interface LoginForm {
  username: string
  password: string
  remember: boolean
}

const form = ref<LoginForm>({
  username: import.meta.env.VITE_USERNAME || '',
  password: import.meta.env.VITE_PASSWORD || '',
  remember: true
})
const loading = ref(false);
const router = useRouter()
const user = userStore()
const route = useRoute();

async function login() {
  loading.value = true;
  const re = await user.login(form.value.username, form.value.password);
  if (re.success) {
    if (CefHelp.isInFrame()) {
      await router.replace('/liveIndex');
      return
    }
    // 获取 redirect 参数，如果没有，则跳转到首页
    const redirect = route.query.redirect as string || '/';
    await router.replace(decodeURIComponent(redirect));
  } else {
    loading.value = false;
    msg.error(re.message)
  }
}

const app = appStore();
app.ini();


const showRegister = ref(false);
const showUseCardKey = ref(false);
const showResetPassword = ref(false);

</script>
<template>
  <main class="main">
    <div class="left">
      <!-- 左上角的LOGO -->
      <div class="logo" style="color: white">
        <img :src="Logo" style="width: 100px;" :alt="app.appInfo?.productName"/>
        {{ app.appInfo?.productName }}
        V{{ app.appInfo?.version }}
      </div>
      <!-- 左侧中间的文字 -->
      <div class="left-content dragToMoveWindow">
        <img :src="loginpic" :alt="app.appInfo?.productName" style="width: 400px;">
        <div>借助AI的心智去超越</div>
      </div>
    </div>

    <!-- 右侧登录表单 -->
    <div class="login-wrapper dragToMoveWindow">
      <AForm name="loginForm" :model="form" layout="vertical" class="login-form shadow" @finish="login">
        <h2 class="title">{{ app.appInfo?.productName }} 登录</h2>
        <AFormItem name="username" :rules="[{ required: true, message: '用户名不能为空!' }]">
          <AInput v-model:value="form.username" placeholder="用户名" size="large">
            <template #prefix>
              <UserOutlined/>
            </template>
          </AInput>
        </AFormItem>
        <AFormItem name="password" :rules="[{ required: true, message: '密码不能为空!' }]">
          <AInputPassword v-model:value="form.password" placeholder="密码" size="large">
            <template #prefix>
              <LockOutlined/>
            </template>
          </AInputPassword>
        </AFormItem>
        <ARow align="middle" style="margin-top: -0.8em" :gutter="5">
          <ACol flex="auto">
            <AFormItem name="remember" no-style>
              <ACheckbox v-model:checked="form.remember">记住我</ACheckbox>
            </AFormItem>
          </ACol>

          <ACol>
            <AButton @click="()=>showRegister=true">注册</AButton>
            <register v-model:show="showRegister"></register>
          </ACol>
          <ACol>
            <AButton @click="()=>showUseCardKey=true">充值</AButton>
            <UseCardKey v-model:user-name="form.username" v-model:password="form.password"
                        v-model:show="showUseCardKey"></UseCardKey>
          </ACol>
          <ACol>
            <AButton>官网</AButton>
          </ACol>
        </ARow>
        <ARow style="margin-top: 20px;">
          <ACol flex="auto">
            <AButton type="primary" style="width: 100%;" size="large" html-type="submit" :loading="loading">
              登录
            </AButton>
          </ACol>
        </ARow>
        <ARow>
          <ACol span="24">
            <AButton type="link" style="width: 100%;padding-top: 0;" size="large" @click="()=>showResetPassword=true">
              忘记密码
            </AButton>
            <ResetPassword v-model:user-name="form.username" v-model:show="showResetPassword"></ResetPassword>

          </ACol>
        </ARow>


      </AForm>
    </div>
  </main>
</template>
<style scoped lang="scss">
.main {
  background-color: rgb(245, 246, 247);
  width: 100vw;
  height: 100vh;
  overflow: hidden;
  display: flex;
  flex-direction: row;
}

/* 左侧部分样式 */
.left {
  width: 60%;
  background-color: rgb(44, 63, 180);
  display: flex;
  justify-content: center;
  align-items: center;
  position: relative; /* 使logo的absolute定位基于left */
}

/* 左上角LOGO样式 */
.logo {
  position: absolute;
  top: 20px;
  left: 20px;
  display: flex;
  align-items: center;
  font-size: 1.2rem; /* 控制标题字体大小 */
  img {
    margin-right: 10px;
  }
}

.left-content {
  color: white;
  font-size: 2rem; /* 调整字体大小 */
  line-height: 1.5; /* 调整行高 */
  text-align: center;
}


/* 右侧登录表单 */
.login-wrapper {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;

  .title {
    text-align: center;
    color: #333;
    margin-bottom: 1.5rem;
  }

  .login-form {
    background-color: var(--white);
    padding: 2rem 1.5rem;
    width: 25rem;
    border-radius: 0.5rem;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  }

  .wrapper-remember {
    margin-bottom: 1rem;
  }
}

/* 响应式处理：当宽度小于972px时隐藏左侧部分 */
@media (max-width: 972px) {
  .left {
    display: none;
  }

  /* 右侧部分占据全部宽度 */
  .login-wrapper {
    width: 90%; /* 调整右侧表单宽度 */
    padding: 1.5rem; /* 减少内边距 */
  }
}
</style>
