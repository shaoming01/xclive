export class R<T = any> {
    code?: number;
    message?: string | undefined;
    data?: T | undefined;

    public get success(): boolean {
        return this.code == 0;
    }

    public set success(val: boolean) {
        this.code = val ? 0 : 1;
    }

    public static ok<T>(data?: T): R<T> {
        const r = new R<T>();
        r.success = true
        r.data = data;
        return r;
    }

    public static error<T>(message: string | undefined, code = 1): R<T> {
        const r = new R<T>();
        r.success = false
        r.code = code;
        r.message = message;
        return r;
    }
}