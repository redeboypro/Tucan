using System.Runtime.InteropServices;
using Tucan.External.HiddenFeatures;

namespace Tucan.External;

internal static class GDI32
{
    internal const string GDI32DLL = "gdi32.dll";

    [DllImport(GDI32DLL, SetLastError = true)]
    internal static extern int ChoosePixelFormat(IntPtr hDeviceContext,
        [In, MarshalAs(UnmanagedType.LPStruct)] PIXELFORMATDESCRIPTOR pixelFormatDescriptor);

    [DllImport(GDI32DLL, SetLastError = true)]
    internal static extern int SetPixelFormat(IntPtr hDeviceContext, int iPixelFormat,
        [In, MarshalAs(UnmanagedType.LPStruct)] PIXELFORMATDESCRIPTOR pixelFormatDescriptor);

    [DllImport(GDI32DLL)]
    internal static extern int SwapBuffers(IntPtr hDeviceContext);
}