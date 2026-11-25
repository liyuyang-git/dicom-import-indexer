# D02 CT 像素与 HU

先跑 [固定样本演示](../../examples/D02/CtPixelExample.cs)。预期输出为 Stored value: -2 和 HU: -1028。

~~~sh
dotnet run --file examples/D02/CtPixelExample.cs
~~~

课堂进行到实操题时，在 [CtPixelExercise.cs](CtPixelExercise.cs) 实现 ConvertToHu。先手算一个无符号边界和一个有符号边界，再运行：

~~~sh
dotnet run --file exercises/D02/CtPixelExercise.cs
~~~

输入是已解码的 16 位合成像素字。有效位宽为 1–16 位，有效值右对齐，signed 对应 Pixel Representation。有符号输入使用二进制补码。斜率和截距已知，输出单位为 HU。方法需拒绝有效范围之外的位宽，补一个相应检查。

起始代码会显示待实现提示，退出码为 2。实现后核对五个样例，另写一项非法位宽检查。把运行命令、实际输出和一个仍需处理的输入边界写入 Obsidian 的 D02 学习记录。
