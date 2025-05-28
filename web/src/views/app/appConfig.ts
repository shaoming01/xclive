import { EnvType } from "@/types/app"

/**
 * app标题
 */

/** 提供动态固定头部 */
export const fixedHeader = ref(true)


/**
 * mock代理指定环境
 * 只在开发环境且appConfig的mock字段为‘on’的情况启动mock
 */
export const mockEnv: EnvType[] = ['development', 'staging', 'production']

export const IconUrl = '//at.alicdn.com/t/c/font_3397413_0heoug9i58th.js';