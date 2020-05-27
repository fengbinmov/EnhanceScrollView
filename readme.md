---
typora-root-url: src
---
滑动效果  

![滑动](https://github.com/fengbinmov/EnhanceScrollView/tree/master/src/滑动.gif)

点击效果（事件的响应在目标Item滚动到指定位置后调用,如果直接点击已处于目标位置的Item将会按照按钮响应规则触发）

![点击事件 响应](/点击事件 响应.gif)

使用外部按钮翻动

![外部翻动调用](/外部翻动调用.gif)



Item 宽度与间距调整

![间距](/间距.gif)

每个Item的X位置偏移

![x偏移](/x偏移.gif)



每个Item的Y位置偏移

![y偏移](/y偏移.gif)

每个Item的大小控制

![缩放](/缩放.gif)



奇偶Item总数的显示

![奇偶处理效果](/奇偶处理效果.gif)





根据以上控制参数可调出(左对齐与右对齐等多种效果)

本组件工包含两个脚本

EnhanceScrollView.cs		负责控制卷轴的基础样式设置与构型

ItemDrag.cs						负责每个Item的触发控制
