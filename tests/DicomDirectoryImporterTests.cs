using FellowOakDicom;
using Learning_conding;
using Xunit;

public sealed class DicomDirectoryImporterTests : IDisposable
{
    private const string Study = "2.25.10";
    private const string OtherStudy = "2.25.11";
    private const string Series = "2.25.20";
    private const string OtherSeries = "2.25.21";
    private const string Instance = "2.25.30";
    private const string OtherInstance = "2.25.31";

    private readonly string directory = Directory.CreateTempSubdirectory("dicom-import-tests-").FullName;

    [Fact]
    public void AddsDistinctInstancesAndCountsCopiesOfTheSameInstance()
    {
        Sample("01-first.dcm", Study, Series, Instance);
        Sample("02-copy.dcm", Study, Series, Instance);
        Sample("03-distinct.DCM", Study, Series, OtherInstance);
        File.WriteAllText(Path.Combine(directory, "notes.txt"), "Ignored");

        var result = DicomDirectoryImporter.Import(directory);

        Assert.Equal(3, result.CandidateCount);
        Assert.Equal(2, result.Instances.Count);
        Assert.Equal("01-first.dcm", result.Instances[Instance].FileName);
        Assert.Equal(1, result.DuplicateCount);
        Assert.Empty(result.Conflicts);
        Assert.Empty(result.RejectedFiles);
    }

    [Theory]
    [InlineData(Study, OtherSeries)]
    [InlineData(OtherStudy, Series)]
    [InlineData(OtherStudy, OtherSeries)]
    public void KeepsFirstFileWhenTheSameInstanceHasAnotherOwner(string study, string series)
    {
        Sample("02-conflict.dcm", study, series, Instance);
        Sample("01-first.dcm", Study, Series, Instance);
        Sample("03-copy.dcm", Study, Series, Instance);

        var result = DicomDirectoryImporter.Import(directory);

        var retained = Assert.Single(result.Instances).Value;
        var conflict = Assert.Single(result.Conflicts);
        Assert.Equal(Instance, conflict.Incoming.InstanceUid);
        Assert.Equal("01-first.dcm", retained.FileName);
        Assert.Equal(retained, conflict.Existing);
        Assert.Equal("02-conflict.dcm", conflict.Incoming.FileName);
        Assert.Equal(study, conflict.Incoming.StudyUid);
        Assert.Equal(series, conflict.Incoming.SeriesUid);
        Assert.Equal(1, result.DuplicateCount);
    }

    [Fact]
    public void RejectsUnreadableFilesAndMissingUidsThenContinues()
    {
        File.WriteAllText(Path.Combine(directory, "01-invalid.dcm"), "Not a DICOM file");
        Sample("02-no-study.dcm", null, Series, Instance);
        Sample("03-valid.dcm", Study, Series, Instance);

        var result = DicomDirectoryImporter.Import(directory);

        Assert.Equal("03-valid.dcm", Assert.Single(result.Instances).Value.FileName);
        Assert.Collection(result.RejectedFiles,
            file => Assert.Equal("01-invalid.dcm", file.FileName),
            file => Assert.Contains("StudyInstanceUID", file.Reason));
        Assert.Empty(result.Conflicts);
    }

    private void Sample(string fileName, string? study, string series, string instance)
    {
        var dataset = new DicomDataset
        {
            { DicomTag.SOPClassUID, DicomUID.CTImageStorage },
            { DicomTag.SOPInstanceUID, instance },
            { DicomTag.SeriesInstanceUID, series },
            { DicomTag.Modality, "CT" }
        };
        if (study is not null)
            dataset.Add(DicomTag.StudyInstanceUID, study);

        new DicomFile(dataset).Save(Path.Combine(directory, fileName));
    }

    public void Dispose() => Directory.Delete(directory, recursive: true);
}
