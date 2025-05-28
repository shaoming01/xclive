import type {GridApi} from "ag-grid-community/dist/types/core/api/gridApi";
import {InputState} from "@/utils/inputState";
import {CellContextMenuEvent, CellRange, RowSelectionOptions} from "ag-grid-community";

export class agGridSelectionUtil {

    private inputState: InputState;
    private readonly _multiple: boolean = false;
    private _api: GridApi;

    constructor(api: GridApi, rowSelection: RowSelectionOptions<any> | "single" | "multiple" | undefined) {
        if (rowSelection == 'multiple') {
            this._multiple = true;
        }
        this._api = api ?? {};
        const gridId = api.getGridId();

        /**
         *
         * @type {InputState}
         */
        this.inputState = new InputState(gridId);

        this.iniEvents();

    }

    iniEvents() {
        this._api.addEventListener('rangeSelectionChanged', () => this.onRangeSelectionChanged());
        this._api.addEventListener('cellContextMenu', (params) => this.onCellContextMenu(params));

        this.inputState.spaceClickHandle = () => this.onSpaceClicked();
        this.inputState.doubleCtrlHandle = () => this.onDoubleCtrlKey();

    }

    async onSpaceClicked() { //键盘弹起事件
        if (!this._multiple) {
            return;
        }
        // 当前行会被系统的空格键执行反选操作，所以在运行前，先恢复空格的反选
        let focusedCell = this._api.getFocusedCell();
        let selected = this._api.getDisplayedRowAtIndex(focusedCell?.rowIndex ?? -1)?.isSelected();
        this._api.getDisplayedRowAtIndex(focusedCell?.rowIndex ?? -1)?.setSelected(!selected);

        //对区域执行全选或反选
        let ranges = this._api.getCellRanges();
        let allRangeSelected = this.isAllRangeRowSelected(ranges);
        await this.setRangeRowSelect(ranges, !allRangeSelected);
    }

    onDoubleCtrlKey() {
        if (!this._multiple) {
            return;
        }
        this._api.deselectAll();
        this._api.clearRangeSelection();
    }


    async onRangeSelectionChanged() {
        this.inputState.logStatus();
        let ctrlKey = this.inputState.isCtrlPressing();
        let shiftKey = this.inputState.isShiftPressing();
        let rightPressed = this.inputState.checkRightPressing();

        if (!this._multiple) {
            return;
        }
        //没有ctrl、shift和右键终止
        if (!ctrlKey && !shiftKey && !rightPressed)
            return;
        //根据单元格选择行
        let sortedRanges = this._api.getCellRanges();
        if (rightPressed) {
            if (this.isAllRangeRowSelected(sortedRanges)) {
                return; //右键在已选择的行中，行不需要重新选择
            } else {
                this._api.deselectAll();
            }
        }
        if (shiftKey && !ctrlKey)
            this._api.deselectAll();
        //存在sortedRanges
        if (sortedRanges && sortedRanges.length > 0) {
            let lastRanges = [sortedRanges[sortedRanges.length - 1]];
            await this.setRangeRowSelect(lastRanges, true);

        }


    }

    onCellContextMenu(params: CellContextMenuEvent<any, any>) {
        if (!params.node.isSelected()) {
            this._api.deselectAll();
            params.node.setSelected(true);
        }
    }

    isAllRangeRowSelected(ranges: CellRange[] | null) {
        let allSelected = true
        ranges?.forEach((selRange) => {
            let minRowIndex = Math.min(selRange.startRow?.rowIndex ?? -1, selRange.endRow?.rowIndex ?? -1)
            let maxRowIndex = Math.max(selRange.startRow?.rowIndex ?? -1, selRange.endRow?.rowIndex ?? -1)
            for (let i = minRowIndex; i <= maxRowIndex; i++) {
                let selected = this._api.getDisplayedRowAtIndex(i)?.isSelected()
                if (!selected) {
                    allSelected = false
                }
            }
        })
        return allSelected
    }

    async setRangeRowSelect(ranges: CellRange[] | null, selected: boolean) {
        ranges?.forEach((selRange) => {
            let minRowIndex = Math.min(selRange.startRow?.rowIndex ?? -1, selRange.endRow?.rowIndex ?? -1)
            let maxRowIndex = Math.max(selRange.startRow?.rowIndex ?? -1, selRange.endRow?.rowIndex ?? -1)
            for (let i = minRowIndex; i <= maxRowIndex; i++) {
                this._api.getDisplayedRowAtIndex(i)?.setSelected(selected)
            }
        })

    }

}

