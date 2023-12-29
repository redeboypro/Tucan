using System.Runtime.InteropServices;

namespace Tucan.External.HiddenFeatures;

internal static class GDI32
{
    internal const string GDI32DLL = "gdi32.dll";

    [DllImport(GDI32DLL, SetLastError = true)]
    internal static extern int ChoosePixelFormat(IntPtr hDeviceContext,
        [In, MarshalAs(UnmanagedType.LPStruct)] PixelFormatDescriptor pixelFormatDescriptor);

    [DllImport(GDI32DLL, SetLastError = true)]
    internal static extern int SetPixelFormat(IntPtr hDeviceContext, int iPixelFormat,
        [In, MarshalAs(UnmanagedType.LPStruct)] PixelFormatDescriptor pixelFormatDescriptor);

    [DllImport(GDI32DLL)]
    internal static extern int SwapBuffers(IntPtr hDeviceContext);
}