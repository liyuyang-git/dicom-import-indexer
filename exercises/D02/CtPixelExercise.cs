var cases = new (string Name, ushort Word, int Bits, bool Signed, double Slope, double Intercept, double Expected)[]
{
    ("Signed 12-bit minimum", 0x0800, 12, true, 1, 0, -2048),
    ("Unsigned 12-bit maximum", 0x0FFF, 12, false, 1, 0, 4095),
    ("Ignore bits outside the sample", 0xE002, 12, true, 2, -100, -96),
    ("Fractional rescale slope", 0x0123, 12, false, 0.5, -100, 45.5),
    ("Negative 16-bit sample", 0xFFFF, 16, true, 1, 0, -1),
    ("Negative 1-bit sample", 0x0001, 1, true, 1, 0, -1),
};
int[] invalidBitWidths = [0, 17];

int passed = 0;
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

foreach (int bits in invalidBitWidths)
{
    try
    {
        CtPixelExercise.ConvertToHu(0, bits, false, 1, 0);
        Console.Error.WriteLine($"FAIL invalid bit width {bits}: expected ArgumentOutOfRangeException");
    }
    catch (ArgumentOutOfRangeException error) when (error.ParamName == "bitsStored")
    {
        Console.WriteLine($"PASS rejected invalid bit width {bits}");
        passed++;
    }
}

int total = cases.Length + invalidBitWidths.Length;
Console.WriteLine($"{passed}/{total} checks passed.");
return passed == total ? 0 : 1;

static class CtPixelExercise
{
    internal static double ConvertToHu(
        ushort pixelWord, int bitsStored, bool signed, double slope, double intercept)
    {
        if (bitsStored is < 1 or > 16)
        {
            throw new ArgumentOutOfRangeException(nameof(bitsStored), bitsStored, "Expected 1–16 bits.");
        }

        int wrapValue = 1 << bitsStored;
        int validBitsMask = wrapValue - 1;
        int storedValue = pixelWord & validBitsMask;

        int signBitMask = 1 << (bitsStored - 1);
        if (signed && (storedValue & signBitMask) != 0)
        {
            storedValue -= wrapValue;
        }

        return storedValue * slope + intercept;
    }
}
