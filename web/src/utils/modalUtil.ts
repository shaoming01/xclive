import {R} from "@/utils/R";
import {IModalDataSelectSchema, IModalObjectEditSchema, IObjectEditSchema} from "@/types/schema";
import {createApp} from "vue";
import {comUtil} from "@/utils/com";
import {pageApi} from "@/api/pageApi";
import _ from "lodash";


export class modalUtil {
    public static async showStringInput(title: string, value: string | undefined): Promise<R<string>> {
        const objectSchema: IObjectEditSchema = {
            data: {'field1': value},
            fields: [{
                label: '',
                field: 'field1',
                placeholder: title,
                fieldType: 'string',
                labelColSpan: 0,
                wrapperColSpan: 24,
                span: 24,
                require: true,
                module: {
                    comPath: '/src/components/edit/simple/StringInput.vue',
                }
            }],
        };
        const schema: IModalObjectEditSchema = {
            title: title,
            sizeMode: 1,
            objectEditSchema: objectSchema,
        }
        const r = await this.showModalEditor(schema);
        if (!r.success) {
            return R.error('');
        }
        const valueInput = r.data['field1'];
        return R.ok(valueInput);
    }

    public static async showTextArea(title: string, content: string): Promise<R<string>> {
        const objectSchema: IObjectEditSchema = {
            data: {'field1': content},
            fields: [{
                label: '',
                field: 'field1',
                placeholder: title,
                fieldType: 'string',
                labelColSpan: 0,
                wrapperColSpan: 24,
                span: 24,
                require: true,
                module: {
                    comPath: '/src/components/edit/simple/StringInput.vue',
                    props: {
                        rows: 12,
                    }
                }
            }],
        };
        const schema: IModalObjectEditSchema = {
            title: title,
            sizeMode: 4,
            centered: true,
            objectEditSchema: objectSchema,
        }
        const r = await this.showModalEditor(schema);
        if (!r.success) {
            return R.error('');
        }
        const valueInput = r.data['field1'];
        return R.ok(valueInput);
    }

    public static showModalEditor(schema: IModalObjectEditSchema): Promise<R> {
        return this.showModalEditorInternal(schema, '/src/components/base/ModalObjectEditor.vue');
    }

    public static async showModalEditorByModule(moduleId: string, sysModuleId: string, appendSchema?: IModalObjectEditSchema): Promise<R> {
        const schemaRe = await pageApi.getFullModule(moduleId, sysModuleId);
        if (!schemaRe.success) {
            msg.error(schemaRe.message)
            return R.error(schemaRe.message)
        }
        const schema = ref(ext.appendSchema(schemaRe.data?.props?.schema ?? {}, appendSchema));
        const comPath = schemaRe.data?.comPath ?? '';
        return this.showModalEditorInternal(schema.value, comPath);

    }

    public static showModalEditorInternal(schema: IModalObjectEditSchema, comPath: string): Promise<R> {
        return new Promise((resolve) => {
            const component = comUtil.getCom(comPath);
            const container = document.createElement('div');
            container.className = 'modalDiv';
            document.body.appendChild(container);  // 将容器添加到页面

            const addSchema = {
                afterClose: (r: R) => {
                    resolve(r)
                    console.log('移除Modal元素')
                    //dynamicApp.unmount();  // modal里面会完成的，有动画效果
                    document.body.removeChild(container);  // 从 DOM 中移除容器，删除 data-v-app div
                }
            };
            const dynamicApp = createApp(component, {
                schema: ext.appendSchema(schema, addSchema),
            })
            const app = dynamicApp.mount(container);
            console.log('显示对话框：', app);
        })
    }


    public static showDataSelect(schema: IModalDataSelectSchema): Promise<R<any[]>> {
        return this.showDataSelectInternal(schema, '/src/components/base/ModalDataSelect.vue');
    }

    public static async showDataSelectByModule(moduleId: string, sysModuleId: string, appendSchema?: IModalDataSelectSchema): Promise<R<any[]>> {
        const schemaRe = await pageApi.getFullModule(moduleId, sysModuleId);
        if (!schemaRe.success) {
            msg.error(schemaRe.message)
            return R.error<any[]>(schemaRe.message)
        }
        const schema = ref(ext.mergeSchema(schemaRe.data?.props?.schema ?? {}, appendSchema));
        const comPath = schemaRe.data?.comPath ?? '';
        return this.showDataSelectInternal(schema.value, comPath);
    }

    public static showDataSelectInternal(schema: IModalDataSelectSchema, comPath: string): Promise<R<any[]>> {
        return new Promise(resolve => {
            const component = comUtil.getCom(comPath);
            const container = document.createElement('div');
            container.className = 'modalDiv';
            document.body.appendChild(container);  // 将容器添加到页面
            schema.afterClose = (r: R<any[]>) => {
                resolve(r);
                console.log('移除Modal元素')
                document.body.removeChild(container);  // 从 DOM 中移除容器，删除 data-v-app div
            };

            const schemaRef = ref(schema);
            const dynamicApp = createApp(component, {
                schema: schemaRef.value,
            })
            dynamicApp.mount(container);
        })
    }

    public static calcModalSize(sizeMode: number): { width: string; height: string } {
        const sizeModeNum = _.toInteger(sizeMode);
        const width = 300 + (sizeModeNum * 100);
        const height = width * 0.7 - 116;//窗口本身有高度
        return {height: height + 'px', width: width + 'px'};
    }

}