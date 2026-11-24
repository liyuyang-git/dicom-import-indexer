using FellowOakDicom;

namespace Learning_conding;

public static class DicomDirectoryImporter
{
    public static DicomImportResult Import(string directory)
    {
        var instances = new Dictionary<string, DicomInstance>(StringComparer.Ordinal);
        var conflicts = new List<DicomConflict>();
        var rejectedFiles = new List<RejectedDicomFile>();
        int candidateCount = 0;
        int duplicateCount = 0;

        var paths = Directory.EnumerateFiles(directory)
            .Where(path => Path.GetExtension(path).Equals(".dcm", StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path, StringComparer.Ordinal);

        foreach (string path in paths)
        {
            candidateCount++;
            string fileName = Path.GetFileName(path);
            DicomInstance instance;
            try
            {
                instance = ReadInstance(path, fileName);
            }
            catch (Exception error) when (error is DicomFileException or DicomDataException
                or InvalidDataException or IOException or UnauthorizedAccessException)
            {
                rejectedFiles.Add(new RejectedDicomFile(fileName, error.Message));
                continue;
            }

            if (instances.TryAdd(instance.InstanceUid, instance))
                continue;

            DicomInstance existing = instances[instance.InstanceUid];
            if (existing.StudyUid == instance.StudyUid && existing.SeriesUid == instance.SeriesUid)
                duplicateCount++;
            else
                conflicts.Add(new DicomConflict(existing, instance));
        }

        return new DicomImportResult(
            candidateCount,
            instances.AsReadOnly(),
            duplicateCount,
            conflicts.AsReadOnly(),
            rejectedFiles.AsReadOnly());
    }

    private static DicomInstance ReadInstance(string path, string fileName)
    {
        var file = DicomFile.Open(
            path,
            DicomEncoding.Default,
            state => state.SequenceDepth == 0 && state.Tag == DicomTag.PixelData,
            FileReadOption.SkipLargeTags);

        if (file is null)
            throw new InvalidDataException("无法识别 DICOM 文件。");

        return new DicomInstance(
            fileName,
            ReadRequiredUid(file.Dataset, DicomTag.StudyInstanceUID),
            ReadRequiredUid(file.Dataset, DicomTag.SeriesInstanceUID),
            ReadRequiredUid(file.Dataset, DicomTag.SOPInstanceUID));
    }

    private static string ReadRequiredUid(DicomDataset dataset, DicomTag tag)
    {
        if (dataset.TryGetSingleValue<string>(tag, out var uid) && !string.IsNullOrWhiteSpace(uid))
            return uid;

        throw new InvalidDataException($"{tag.DictionaryEntry.Keyword} 缺失或为空。");
    }
}

public sealed record DicomInstance(string FileName, string StudyUid, string SeriesUid, string InstanceUid);
public sealed record DicomConflict(DicomInstance Existing, DicomInstance Incoming);
public sealed record RejectedDicomFile(string FileName, string Reason);

public sealed record DicomImportResult(
    int CandidateCount,
    IReadOnlyDictionary<string, DicomInstance> Instances,
    int DuplicateCount,
    IReadOnlyList<DicomConflict> Conflicts,
    IReadOnlyList<RejectedDicomFile> RejectedFiles);
