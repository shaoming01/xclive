import {createFromIconfontCN} from '@ant-design/icons-vue';
import {IconUrl} from "@/views/app/appConfig";

const IconFont = createFromIconfontCN({
    scriptUrl: IconUrl, // 替换成你的 Iconfont 链接
    extraCommonProps: {
        viewBox: '0 0 1024 1024',
    }
});

function getIcon(iconName?: string | undefined) {
    if (!iconName) {
        return undefined
    }

    return h(IconFont, {type: iconName, viewBox: '0 0 1024 1024'});
}

function getAllIcon(): string[] {
    const icons: string[] = [];
    // 这里假设 iconfont.js 已经被加载，并且 symbol 元素已注册到 DOM 中
    document.querySelectorAll('svg symbol').forEach((symbol) => {
        icons.push(symbol.id);
    });
    return icons;
}

export {getIcon, IconFont, getAllIcon};