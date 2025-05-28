import {message, Modal} from "ant-design-vue";


/**
 *
 * @param arr Array<any>
 * @returns 数组随机项
 */
export function randomPick(arr: Array<any>) {
    return arr[Math.floor(Math.random() * arr.length)]
}

/**
 *
 * @returns guid
 */
export const guid = () => {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
            v = c == 'x' ? r : (r & 0x3 | 0x8)
        return v.toString(16)
    })
}

/**
 * @summary 验证字段是不是手机号码
 * @param {string} str
 * @returns {Boolean}
 */
export const isPhone = (str: string) => {
    const reg = /^1[3456789]\d{9}$/
    return reg.test(str)
}

/**
 * @summary 验证字段是不是邮箱
 * @param {string} str
 * @returns {Boolean}
 */
export const isEmail = (str: string) => {
    const reg = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/
    return reg.test(str)
}

export function launchFullscreen(element: any = document.documentElement) {
    if (element.requestFullscreen) {
        element.requestFullscreen()
    } else if (element.mozRequestFullScreen) {
        element.mozRequestFullScreen()
    } else if (element.webkitRequestFullscreen) {
        element.webkitRequestFullscreen()
    } else if (element.msRequestFullscreen) {
        element.msRequestFullscreen()
    }
}

export function exitFullscreen(element: any = document) {
    if (element.exitFullscreen) {
        element.exitFullscreen()
    } else if (element.mozCancelFullScreen) {
        element.mozCancelFullScreen()
    } else if (element.msExitFullscreen) {
        element.msExiFullscreen()
    } else if (element.webkitCancelFullScreen) {
        element.webkitCancelFullScreen()
    }
}




