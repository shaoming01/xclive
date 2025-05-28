export class memCache<T = any> {
    private readonly lifeSeconds: number; // 缓存有效时间（单位秒）
    private cacheObj: Record<string, { createDate: number; data: T }>; // 缓存存储对象

    constructor(lifeSeconds: number = 300) {
        // 如果传入的有效时间小于等于 0，设置默认值 300 秒
        this.lifeSeconds = lifeSeconds > 0 ? lifeSeconds : 300;
        this.cacheObj = {};
    }

    /**
     * 获取缓存值
     * @param key 缓存键
     * @returns 缓存值或 null（如果不存在或过期）
     */
    getKey(key: string): T | null {
        const val = this.cacheObj[key];
        if (!val || !val.createDate) {
            return null;
        }

        // 检查是否过期
        if (val.createDate + this.lifeSeconds * 1000 < Date.now()) {
            delete this.cacheObj[key]; // 过期后清除缓存
            return null;
        }

        return val.data;
    }

    /**
     * 设置缓存值
     * @param key 缓存键
     * @param val 缓存值
     */
    setKey(key: string, val: T): void {
        this.cacheObj[key] = {
            createDate: Date.now(),
            data: val,
        };
    }

    /**
     * 清除指定缓存
     * @param key 缓存键
     */
    clearKey(key: string): void {
        delete this.cacheObj[key];
    }

    /**
     * 清除所有缓存
     */
    clearAll(): void {
        this.cacheObj = {};
    }
}