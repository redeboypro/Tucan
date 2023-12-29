using System.Runtime.InteropServices;

namespace Tucan.External.HiddenFeatures;

[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    public int X;
    public int Y;
}