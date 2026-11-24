# DICOM 导入与实例索引

一个使用 C#、.NET 10 和 fo-dicom 的命令行练习。扫描目录中的 DICOM 文件，以 Study 和 Series 组织图像索引，并报告重复、归属冲突和读取失败。

## 运行

安装 [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)，将合成 DICOM 样本放在一个本地目录，然后运行：

```sh
dotnet run --project Learning-conding.csproj -- "./samples"
dotnet test Learning-conding.slnx
```

程序扫描所选目录这一层的 `.dcm` 文件。完成扫描返回退出码 0，目录参数无效或目录无法读取时返回 1。报告包含每次检查下的序列、已登记实例数、同归属副本数、冲突数和无法导入的文件数。

## 导入规则

- 按文件名顺序读取并提取 Study、Series 和 SOP Instance UID。读取到顶层 Pixel Data 时停止解析。
- 以 SOP Instance UID 查全局索引。首次出现时登记。再次出现且 Study、Series 均一致时计为同归属副本，保留首次登记。
- Study 或 Series 不一致时，报告两份文件名与各自的归属，保留首次登记。归属一致也只代表索引键一致，程序未比较像素内容。
- 无法读取或缺少必需 UID 的文件记入报告，其余文件继续导入。

`DicomDirectoryImporter` 负责解析和索引，`Program.cs` 负责命令行参数和显示。xUnit 测试在临时目录生成合成样本，验证重复、跨序列及跨检查冲突，还有无效文件后的继续导入。

本项目用于学习 DICOM 导入，不用于诊断或治疗。测试数据在运行时生成。
