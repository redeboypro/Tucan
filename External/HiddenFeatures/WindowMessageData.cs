﻿using System.Runtime.InteropServices;

namespace Tucan.External.HiddenFeatures;

[StructLayout(LayoutKind.Sequential)]
internal struct WindowMessageData
{
    internal IntPtr HWindow;
    internal uint Message;
    internal IntPtr WParam;
    internal IntPtr LParam;
    internal uint Time;
    internal Point Point;
}