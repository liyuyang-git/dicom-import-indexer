using FellowOakDicom;
using Learning_conding;

if (args.Length != 1 || !Directory.Exists(args[0]))
{
    Console.Error.WriteLine("请传入一个有效的 DICOM 文件夹。");
    Console.Error.WriteLine("""用法：dotnet run -- "./samples" """.TrimEnd());
    return 1;
}

new DicomSetupBuilder().Build();

DicomImportResult result;
try
{
    result = DicomDirectoryImporter.Import(args[0]);
}
catch (Exception error) when (error is IOException or UnauthorizedAccessException)
{
    Console.Error.WriteLine($"无法扫描目录：{error.Message}");
    return 1;
}

foreach (var study in result.Instances.Values.GroupBy(instance => instance.StudyUid))
{
    Console.WriteLine($"Study {study.Key}");
    foreach (var series in study.GroupBy(instance => instance.SeriesUid))
        Console.WriteLine($"  Series {series.Key}（{series.Count()} 个实例）");
}

foreach (var conflict in result.Conflicts)
{
    Console.Error.WriteLine(
        $"实例 {conflict.Existing.InstanceUid} 归属冲突。保留 {Describe(conflict.Existing)}，待核查 {Describe(conflict.Incoming)}。");
}

foreach (var file in result.RejectedFiles)
    Console.Error.WriteLine($"跳过 {file.FileName}：{file.Reason}");

Console.WriteLine(
    $"扫描 {result.CandidateCount} 个文件，已登记 {result.Instances.Count}，重复 {result.DuplicateCount}，冲突 {result.Conflicts.Count}，无法导入 {result.RejectedFiles.Count}。");
return 0;

static string Describe(DicomInstance instance) =>
    $"{instance.FileName}（Study {instance.StudyUid}，Series {instance.SeriesUid}）";
