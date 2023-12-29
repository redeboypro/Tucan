﻿using System.Runtime.InteropServices;
using Tucan.External.HiddenFeatures;

namespace Tucan.External;

internal static class User32
{
    internal const string USR32DLL = "user32.dll";
    
    [DllImport(USR32DLL)]
    public static extern int ReleaseDC(IntPtr hWindow, IntPtr hDeviceContext);
    
    [DllImport(USR32DLL)]
    internal static extern IntPtr GetDC(IntPtr hWindow);
    
    [DllImport(USR32DLL)]
    internal static extern bool UpdateWindow(IntPtr hWindow);

    [DllImport(USR32DLL)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ShowWindow(IntPtr hWindow, int nCmdShow);

    [DllImport(USR32DLL, SetLastError = true)]
    internal static extern bool DestroyWindow(IntPtr hWindow);
    
    [DllImport(USR32DLL, SetLastError = true, EntryPoint = "CreateWindowEx")]
    internal static extern IntPtr CreateWindowEx(
        int dwExStyle,
        ushort regResult,
        string lpWindowName,
        uint dwStyle,
        int x, int y,
        int nWidth, int nHeight,
        IntPtr hWindowParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    [DllImport(USR32DLL, SetLastError = true, EntryPoint = "RegisterClassEx")]
    internal static extern ushort RegisterClass([In] ref WNDCLASSEX lpWndClass);
    
    [DllImport(USR32DLL, SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

    [DllImport(USR32DLL)]
    internal static extern IntPtr DefWindowProc(IntPtr hWindow, uint uMsg, IntPtr wParam, IntPtr lParam);

    [DllImport(USR32DLL)]
    internal static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);
    
    [DllImport(USR32DLL)]
    internal static extern bool PeekMessage(
        out MSG lpMessage,
        IntPtr hWindow,
        uint wMessageFilterMin,
        uint wMessageFilterMax,
        PeekMessageOptions wRemoveMessage);
    
    [DllImport(USR32DLL)]
    internal static extern bool TranslateMessage([In] ref MSG lpMessage);

    [DllImport(USR32DLL)]
    internal static extern IntPtr DispatchMessage([In] ref MSG lpMessage); 
}