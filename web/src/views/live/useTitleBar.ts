import {
    CefHelp,
} from "@/views/live/help/LiveInterface";
import _ from "lodash";

export function useTitleBar() {
    let downFlag = 0;

    function mouseDown(event: MouseEvent) {
        if (!CefHelp.isInFrame()) return;
        //需要用随机数来表达，否则第一次弹起后再按下去即使在150内也会触发拖动
        downFlag = _.random(1, 9999);
        const tmpVal = downFlag;
        // 设置一个延迟，如果在此期间未发生 mouseup，则认为是拖动
        setTimeout(() => {
            if (tmpVal != downFlag) return;
            const w = window as any;
            w.CefSharp.PostMessage('dragWindow');
        }, 100); // 延迟时间可以根据需要调整
    }

    // 鼠标抬起事件
    function mouseUp(event: MouseEvent) {
        downFlag = 0;
    }

    // 双击事件
    function doubleClick(event: MouseEvent) {
        if (!CefHelp.isInFrame()) return;
        CefHelp.switchWindow();
    }

    return {mouseDown, mouseUp, doubleClick};
}