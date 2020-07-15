前提：PointerEventData eventData
//自己测试的
eventData:返回鼠标的屏幕坐标

eventData.button：触发此方法时按下的鼠标按键，返回值为：PointerEventData.InputButton.Left（左键），PointerEventData.InputButton.Right（右键），PointerEventData.InputButton.Middle（滚轮）三种

eventData.clickcount：连续点击次数,返回值类型：int

eventData.clickTime：最近一次完成点击（click）的时间,比Time.time要快1.25s到1.27s,返回值类型：float

eventData.currentInputModule：未知

eventData.delta：