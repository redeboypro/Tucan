using Tucan.External;

namespace Tucan.Windowing;

public struct DisplaySettings
{
    public static readonly DisplaySettings Default = new ()
    {
        Title = "Sample Window",
        Width = 800, Height = 600,
        WindowStyle = WindowStyle.OverlappedWindow,
        ClassStyle = ClassStyle.DoubleClicks | ClassStyle.HorizontalRedraw | ClassStyle.VerticalRedraw,
        ClassName = "SampleClass",
        CursorType = CursorType.Arrow,
        PeekMessageOptions = PeekMessageOptions.Remove,
        PixelFormatDescriptorFlags = PixelFormatDescriptorFlags.DrawToWindow | PixelFormatDescriptorFlags.SupportOpenGL | PixelFormatDescriptorFlags.DoubleBuffer,
        PixelFormatDescriptorType = PixelFormatDescriptorType.Rgba,
        PixelFormatDescriptorPlane = PixelFormatDescriptorPlanes.MainPlane
    };
    
    public string Title;
    public int Width, Height;
    public WindowStyle WindowStyle;
    public ClassStyle ClassStyle;
    public string ClassName;
    public CursorType CursorType;
    public PeekMessageOptions PeekMessageOptions;
    public PixelFormatDescriptorFlags PixelFormatDescriptorFlags;
    public PixelFormatDescriptorType PixelFormatDescriptorType;
    public PixelFormatDescriptorPlanes PixelFormatDescriptorPlane;
}