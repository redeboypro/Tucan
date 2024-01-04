using System.Runtime.InteropServices;

namespace Tucan.External.HiddenFeatures;

[StructLayout(LayoutKind.Explicit)]
internal sealed class PixelFormatDescriptor
{
    [FieldOffset(0)]
    internal ushort Size;
        
    [FieldOffset(2)]
    internal ushort Version;
        
    [FieldOffset(4)]
    internal PixelFormatDescriptorFlags Flags;
        
    [FieldOffset(8)]
    internal PixelFormatDescriptorType PixelType;
        
    [FieldOffset(9)]
    internal byte ColorBits;
        
    [FieldOffset(10)]
    internal byte RedBits;
        
    [FieldOffset(11)]
    internal byte RedShift;
        
    [FieldOffset(12)]
    internal byte GreenBits;
        
    [FieldOffset(13)]
    internal byte GreenShift;
        
    [FieldOffset(14)]
    internal byte BlueBits;
        
    [FieldOffset(15)]
    internal byte BlueShift;
        
    [FieldOffset(16)]
    internal byte AlphaBits;
        
    [FieldOffset(17)]
    internal byte AlphaShift;
        
    [FieldOffset(18)]
    internal byte AccumBits;
        
    [FieldOffset(19)]
    internal byte AccumRedBits;
        
    [FieldOffset(20)]
    internal byte AccumGreenBits;
        
    [FieldOffset(21)]
    internal byte AccumBlueBits;
        
    [FieldOffset(22)]
    internal byte AccumAlphaBits;
        
    [FieldOffset(23)]
    internal byte DepthBits;
        
    [FieldOffset(24)]
    internal byte StencilBits;
        
    [FieldOffset(25)]
    internal byte AuxBuffers;
        
    [FieldOffset(26)]
    internal PixelFormatDescriptorPlanes LayerType;
        
    [FieldOffset(27)]
    internal byte Reserved;
        
    [FieldOffset(28)]
    internal uint LayerMask;
        
    [FieldOffset(32)]
    internal uint VisibleMask;
        
    [FieldOffset(36)]
    internal uint DamageMask;
}