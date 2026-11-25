// A decoded synthetic CT pixel word. Its high bit is Bits Stored - 1.
const int pixelWord = 0x0FFE;
const int bitsStored = 12;
const double slope = 2;
const double intercept = -1024;

int valueRange = 1 << bitsStored;
int validBitsMask = valueRange - 1;
int signMask = valueRange >> 1;
int storedValue = pixelWord & validBitsMask;

// Pixel Representation is 1 in this example.
if ((storedValue & signMask) != 0)
{
    storedValue -= valueRange;
}

double hu = storedValue * slope + intercept;
Console.WriteLine($"Stored value: {storedValue}");
Console.WriteLine($"HU: {hu}");
