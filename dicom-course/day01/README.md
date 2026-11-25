# 第 1 天 · 对象与属性检查

从仓库根目录运行。样本只包含本课所需的合成元数据，用于字段读取和校验练习。

~~~powershell
dotnet run --project dicom-course/day01 -- --samples samples/dicom-course-day01
dotnet run --project dicom-course/day01 -- --demo samples/dicom-course-day01/ct-empty-date.dcm
~~~

演示用 GetSingleValue 读取 SOP Class UID 和 SOP Instance UID。使用两个样本复核同一个 Study 下如何区分序列和实例。

在 DayOneExercise.cs 的 Inspect 中打印四个 UID 并检查 Study Date。首先用 Contains 检查元素是否存在，存在时再用 GetValueCount 或 GetSingleValueOrDefault 判断空值。遇到空日期应报告「存在，日期未知」。遇到缺失日期应报告「缺少 Type 2 元素」。

~~~powershell
dotnet run --project dicom-course/day01 -- --inspect samples/dicom-course-day01/ct-empty-date.dcm
dotnet run --project dicom-course/day01 -- --inspect samples/dicom-course-day01/sc-missing-date.dcm
~~~

先写出两个样本的预期，再对照运行输出。起始代码会提示待实现，退出码是 2。
