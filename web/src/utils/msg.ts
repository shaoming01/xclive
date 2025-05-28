import {message, Modal} from "ant-design-vue";

export class msg {
    static success(content: string | undefined, ms = 3000) {
        message.success(content, ms / 1000).then();
    }

    static error(content: string | undefined) {
        Modal.error({
            content: content,

        })
    }

    static confirm(content: string | undefined): Promise<boolean> {
        return new Promise(resolve => {
            Modal.confirm({
                content: content,
                onOk() {
                    resolve(true);
                },
                onCancel() {
                    resolve(false);
                }
            })
        })

    }
}