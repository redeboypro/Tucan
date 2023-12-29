namespace Tucan.External;

public enum WindowMessage : uint
{
    Quit = 0x0012,
    Destroy = 2,
    Paint = 0x0f,
    LeftButtonDown = 0x0201,
    LeftButtonUp = 0x0202,
    LeftButtonDoubleClick = 0x0203,
    RightButtonDown = 0x0204,
    RightButtonUp = 0x0205
}