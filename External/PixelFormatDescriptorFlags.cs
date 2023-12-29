namespace Tucan.External;

[Flags]
public enum PixelFormatDescriptorFlags : uint
{
    DoubleBuffer = 1,
    Stereo = 2,
    DrawToWindow = 4,
    DrawToBitmap = 8,
    SupportGDI = 16,
    SupportOpenGL = 32,
    GenericFormat = 64,
    NeedPalette = 128,
    NeedSystemPalette = 256,
    SwapExchange = 512,
    SwapCopy = 1024,
    SwapLayerBuffers = 2048,
    GenericAccelerated = 4096,
    SupportDirectDraw = 8192
}