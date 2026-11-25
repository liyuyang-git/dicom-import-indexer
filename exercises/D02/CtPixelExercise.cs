var cases = new (string Name, ushort Word, int Bits, bool Signed, double Slope, double Intercept, double Expected)[]
{
    ("Signed 12-bit minimum", 0x0800, 12, true, 1, 0, -2048),
    ("Unsigned 12-bit maximum", 0x0FFF, 12, false, 1, 0, 4095),
    ("Ignore bits outside the sample", 0xE002, 12, true, 2, -100, -96),
    ("Fractional rescale slope", 0x0123, 12, false, 0.5, -100, 45.5),
    ("Negative 16-bit sample", 0xFFFF, 16, true, 1, 0, -1),
};

int passed = 0;
try
{
    foreach (var testCase in cases)
    {
        double actual = CtPixelExercise.ConvertToHu(
            testCase.Word, testCase.Bits, testCase.Signed, testCase.Slope, testCase.Intercept);
        if (actual == testCase.Expected)
        {
            Console.WriteLine($"PASS {testCase.Name}");
            passed++;
        }
        else
        {
            Console.Error.WriteLine($"FAIL {testCase.Name}: expected {testCase.Expected}, got {actual}");
        }
    }
}
catch (NotImplementedException error)
{
    Console.Error.WriteLine(error.Message);
    return 2;
}

Console.WriteLine($"{passed}/{cases.Length} examples passed.");
return passed == cases.Length ? 0 : 1;

static class CtPixelExercise
{
    internal static double ConvertToHu(
        ushort pixelWord, int bitsStored, bool signed, double slope, double intercept)
    {
        throw new NotImplementedException("Complete ConvertToHu, then run the exercise again.");
    }
}
