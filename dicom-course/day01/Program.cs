using FellowOakDicom;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: --samples <directory> | --demo <file> | --inspect <file>");
    return 1;
}

new DicomSetupBuilder().Build();

switch (args[0])
{
    case "--samples":
        Directory.CreateDirectory(args[1]);
        SaveSample("ct-empty-date.dcm", DicomUID.CTImageStorage, "2.25.101", includeStudyDate: true);
        SaveSample("sc-missing-date.dcm", DicomUID.SecondaryCaptureImageStorage, "2.25.102", includeStudyDate: false);
        return 0;
    case "--demo":
        var dataset = DicomFile.Open(args[1]).Dataset;
        Console.WriteLine($"SOP Class UID: {dataset.GetSingleValue<string>(DicomTag.SOPClassUID)}");
        Console.WriteLine($"SOP Instance UID: {dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID)}");
        return 0;
    case "--inspect":
        try
        {
            DayOneExercise.Inspect(DicomFile.Open(args[1]).Dataset);
            return 0;
        }
        catch (NotImplementedException error)
        {
            Console.Error.WriteLine(error.Message);
            return 2;
        }
    default:
        Console.Error.WriteLine($"Unknown option: {args[0]}");
        return 1;
}

void SaveSample(string fileName, DicomUID sopClass, string instanceUid, bool includeStudyDate)
{
    var sample = new DicomDataset(DicomTransferSyntax.ExplicitVRLittleEndian)
    {
        { DicomTag.SOPClassUID, sopClass },
        { DicomTag.SOPInstanceUID, instanceUid },
        { DicomTag.StudyInstanceUID, "2.25.100" },
        { DicomTag.SeriesInstanceUID, includeStudyDate ? "2.25.11" : "2.25.12" }
    };

    if (includeStudyDate)
        sample.Add<string>(DicomTag.StudyDate);

    var path = Path.Combine(args[1], fileName);
    if (File.Exists(path))
    {
        Console.WriteLine($"Already exists: {path}");
        return;
    }

    using var output = File.Open(path, FileMode.CreateNew, FileAccess.Write);
    new DicomFile(sample).Save(output);
    Console.WriteLine($"Created: {path}");
}
