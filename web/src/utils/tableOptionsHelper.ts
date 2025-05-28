import {
    IFullTableSchema, IModalDataSelectSchema, IModalObjectEditSchema
} from "@/types/schema";
import {modalUtil} from "@/utils/modalUtil";
import {R} from "@/utils/R";

export class tableOptionsHelper {
    public static async add(tableSchema: IFullTableSchema, moduleId: string, sysModuleId: string, appendSchema?: IModalObjectEditSchema | undefined) {
        const editRe = await modalUtil.showModalEditorByModule(moduleId, sysModuleId, appendSchema);
        if (editRe.success && editRe.data) {
            const ids = [editRe.data.id];
            tableSchema.options?.refreshRows(ids);
        }
        return;
    }

    public static async select(moduleId: string, sysModuleId: string, appendSchema?: IModalDataSelectSchema | undefined): Promise<R<any[]>> {
        return await modalUtil.showDataSelectByModule(moduleId, sysModuleId, appendSchema);
    }

    public static async edit(tableSchema: IFullTableSchema, moduleId: string, sysModuleId: string, appendSchema?: IModalObjectEditSchema | undefined) {
        if (!tableSchema?.currentRow) {
            return msg.error('请选择1条数据后进行此操作')
        }
        const tmpSchema: IModalObjectEditSchema = appendSchema ?? {objectEditSchema: {}};
        tmpSchema.dataId = tableSchema.currentRow['id'];
        const editRe = await modalUtil.showModalEditorByModule(moduleId, sysModuleId, tmpSchema);
        if (editRe.success && editRe.data) {
            const ids = [editRe.data.id];
            tableSchema.options?.refreshRows(ids);
        }
    }

    public static async del(tableSchema: IFullTableSchema) {
        if (!tableSchema?.options?.getSelectedRows()?.length) {
            return msg.error('请选择要删除的数据')
        }
        if (!await msg.confirm('请确认删除选中数据?')) {
            return;
        }
        const delRe = await tableSchema?.options?.onlineDeleteSelectedRows();
        if (!delRe.success)
            return msg.error(delRe.message)
        msg.success('删除成功');
    }
}