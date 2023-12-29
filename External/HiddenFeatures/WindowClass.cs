using System.Runtime.InteropServices;

namespace Tucan.External.HiddenFeatures;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct WindowClass
{
    [MarshalAs(UnmanagedType.U4)]
    internal int Size;
    
    [MarshalAs(UnmanagedType.U4)]
    internal ClassStyle Style;
    
    internal IntPtr MessageCallbackPtr;
    internal int ClassExtra;
    internal int WindowExtra;
    internal IntPtr Instance;
    internal IntPtr Icon;
    internal IntPtr Cursor;
    internal IntPtr Background;
    internal string MenuName;
    internal string ClassName;
    internal IntPtr SmallIcon;
}