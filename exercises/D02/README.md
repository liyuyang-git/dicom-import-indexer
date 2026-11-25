# D02 CT 像素与 HU

先跑 [固定样本演示](../../examples/D02/CtPixelExample.cs)。预期输出为 Stored value: -2 和 HU: -1028。

~~~sh
dotnet run --file examples/D02/CtPixelExample.cs
~~~

[CtPixelExercise.cs](CtPixelExercise.cs) 包含课堂参考实现。先手算一个无符号边界和一个有符号边界，再运行：

~~~sh
dotnet run --file exercises/D02/CtPixelExercise.cs
~~~

输入是已解码的 16 位合成像素字。有效位宽为 1–16 位，有效值右对齐，signed 对应 Pixel Representation。先用位掩码保留有效位，再按标记决定是否解释为二进制补码，最后用斜率和截距换算 HU。

预期通过 8 项检查，包括 6 个数值样例和位宽 0、17 两个异常样例。课堂复测时，请自己解释一个新样本，写清掩码、符号与 HU 的推导。
