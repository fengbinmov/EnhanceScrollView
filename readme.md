滑动效果  

![滑动](src/滑动.gif)

点击效果（事件的响应在目标Item滚动到指定位置后调用,如果直接点击已处于目标位置的Item将会按照按钮响应规则触发）

![点击事件 响应](src/点击事件响应.gif)

使用外部按钮翻动

![外部翻动调用](src/外部翻动调用.gif)

当滑动时，将触发滑动事件（例子中驱动了 Silder）

![改变事件](src/改变事件.gif)



Item 宽度与间距调整

![间距](src/间距.gif)

每个Item的X位置偏移

![x偏移](src/x偏移.gif)



每个Item的Y位置偏移

![y偏移](src/y偏移.gif)

每个Item的大小控制

![缩放](src/缩放.gif)



奇偶Item总数的显示

![奇偶处理效果](src/奇偶处理效果.gif)

Item宽度显示Item之间的间隔大小显示

![提示](src/提示.gif)

Item之间的排列方向,可选择 ‘水平’ 或 ‘垂直，默认我水平排列’

![排序方向](src/排列方向.gif)

Inspector界面，可在编辑状态中实时改动与预览

![Inspector界面](src/Inspector界面.jpg)

根据以上控制参数可调出(左对齐与右对齐等多种效果)

本组件工包含两个脚本

EnhanceScrollView.cs		负责控制卷轴的基础样式设置与构型

ItemDrag.cs						负责每个Item的触发控制
