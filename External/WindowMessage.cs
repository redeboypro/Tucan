namespace Tucan.External;

public enum WindowMessage : uint
{
    Quit = 0x0012,
    Destroy = 2,
    Paint = 0x0f,
    SetFocus = 0x0007,
    AppCommand = 0x0319,
    Activate = 0x0006,
    KeyDown = 0x0100,
    KeyUp = 0x0101,
    HotKey = 0x0312,
    Char = 0x0102,
    DeadChar = 0x0103,
    LeftButtonDown = 0x0201,
    LeftButtonUp = 0x0202,
    LeftButtonDoubleClick = 0x0203,
    RightButtonDown = 0x0204,
    RightButtonUp = 0x0205
}