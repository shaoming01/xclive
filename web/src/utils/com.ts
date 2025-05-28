/**
 * 系统全局控件映射
 * 数据库里或文档里存储的都是VUE组件的路径并不是JS路径，此处存储映射关系，异步加载组件的时候通过这里得到实际映射关系
 */

export class comUtil {
    private static allComponents = import.meta.glob('/src/components/**/*.vue');
    /**
     * 这里而缓存的是类型，不是实例
     * @private
     */
    private static componentCache = new Map<string, ReturnType<typeof defineAsyncComponent>>();
    static {
        this.allComponents = import.meta.glob('/src/components/**/*.vue');
    }

    public static getCom(comPath: string | undefined) {
        if (!comPath) {
            console.error('组件路径不能为空');
            return undefined;
        }
        if (this.componentCache.has(comPath)) {
            return this.componentCache.get(comPath);
        }
        const com = this.allComponents[comPath] as () => Promise<any>;
        if (!com) {
            console.error('找不到组件：', comPath);
            return;
        }

        const asyncComponent = defineAsyncComponent(com);
        asyncComponent.__asyncLoader();
        this.componentCache.set(comPath, asyncComponent);
        return asyncComponent;
    }
}