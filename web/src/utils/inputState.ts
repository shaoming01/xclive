export class InputState {
    private lastCtrlUpTime = 0;
    private lastCtrlDownTime = 0;
    private lastShiftUpTime = 0;
    private lastShiftDownTime = 0;
    private ctrlKey = false;
    private shiftKey = false;
    private mouseDownCount = 0;
    private lastMouseUpEvent: MouseEvent | undefined;
    private lastMouseDownEvent: MouseEvent | undefined;
    private keyUpHandle: any;
    public doubleCtrlHandle?: (event: KeyboardEvent) => any;
    public spaceClickHandle?: (event: KeyboardEvent) => any;

    constructor(gridId: string) {
        const element = document.querySelector('div[grid-id="' + gridId + '"]');
        if (!element) {
            console.log('获取不到GridId：' + gridId);
            return;
        }
        element.addEventListener('mousedown', (event) => {
            this.onMouseDown(event as MouseEvent);
        });

        element.addEventListener('mouseup', (event) => {
            this.onMouseUp(event as MouseEvent);
        });

        element.addEventListener('keyup', (event) => {
            this.onKeyUp(event as KeyboardEvent);
        });

        element.addEventListener('keydown', (event) => {
            this.onKeyDown(event as KeyboardEvent);
        });

    }


    //判断右键是否按下中
    checkRightPressing() {
        if (!this.lastMouseDownEvent) {
            return false;
        }
        if (!this.lastMouseUpEvent) { //没有弹起事件
            return this.lastMouseDownEvent.button === 2;
        }
        if (this.lastMouseUpEvent.button === 2 &&
            this.lastMouseUpEvent.timeStamp >= this.lastMouseDownEvent.timeStamp) {
            return false; //右键弹起了
        }
        return this.lastMouseDownEvent.button === 2;
    }


    onKeyDown(event: KeyboardEvent) {
        if (event.keyCode === 17) { //Ctrl键
            this.lastCtrlDownTime = new Date().getTime();
            this.ctrlKey = true;
        }
        if (event.keyCode === 16) { //Shift键
            this.lastShiftDownTime = new Date().getTime();
            this.shiftKey = true;
        }
    }

    /**
     * 判断是否Ctrl键正在按下状态或者100MS以内是按下状态
     * @returns {boolean}
     */
    isCtrlPressing() {
        if (this.ctrlKey && new Date().getTime() - this.lastCtrlDownTime < 1000)//在本页面Down没在本页面Up，会漏掉UP事件
            return true;
        else if (new Date().getTime() - this.lastCtrlUpTime < 100)//键刚刚弹起100MS ，也视为按下状态
            return true
        return false;
    }

    logStatus() {
        console.debug(
            'Now:' + new Date().getTime()
            + '  ctrlKey:' + this.ctrlKey
            + '  shiftKey:' + this.shiftKey
            + '  isCtrlPressing:' + this.isCtrlPressing()
            + '  isShiftPressing:' + this.isShiftPressing()
            + '  lastCtrlDownTime:' + this.lastCtrlDownTime
            + '  lastCtrlUpTime:' + this.lastCtrlUpTime
            + '  lastShiftDownTime:' + this.lastShiftDownTime
            + '  lastShiftUpTime:' + this.lastShiftUpTime
        )
    }

    /**
     * 判断Shift键是否被按下状态或者100MS以内是按下状态
     * @returns {boolean}
     */
    isShiftPressing() {
        if (this.shiftKey && new Date().getTime() - this.lastShiftDownTime < 1000)//在本页面Down没在本页面Up，会漏掉UP事件
            return true;
        else if (new Date().getTime() - this.lastShiftUpTime < 100)//键刚刚弹起100MS ，也视为按下状态
            return true
        return false;
    }

    onKeyUp(event: KeyboardEvent) {
        if (this.keyUpHandle) {
            this.keyUpHandle(event);
        }
        if (event.keyCode === 32 && this.spaceClickHandle) {
            this.spaceClickHandle(event);
        }
        if (event.keyCode === 17 && new Date().getTime() - this.lastCtrlUpTime < 300 && this.doubleCtrlHandle) {
            this.doubleCtrlHandle(event);
        }
        if (event.keyCode === 17) { //Ctrl键
            this.ctrlKey = false;
            this.lastCtrlUpTime = new Date().getTime();
        }
        if (event.keyCode === 16) { //Shift键
            this.shiftKey = false;
            this.lastShiftUpTime = new Date().getTime();

        }
    }

    onMouseDown(event: MouseEvent) {
        this.mouseDownCount++;
        this.lastMouseDownEvent = event;
    }

    onMouseUp(event: MouseEvent) {
        this.lastMouseUpEvent = event;
    }


}