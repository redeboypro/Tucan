using Tucan.External.HiddenFeatures;

namespace Tucan.Input;

public static class InputManager
{
    private const int KeyCount = 256;
    
    private static SortedList<int, bool>? _currentKeyStates;
    private static SortedList<int, bool>? _lastKeyStates;

    private static int _lastMousePositionX;
    private static int _lastMousePositionY;

    public static int MouseX { get; private set; }
    public static int MouseY { get; private set; }
    
    public static int MouseDeltaX { get; private set; }
    public static int MouseDeltaY { get; private set; }

    public static bool GetKeyDown(int key)
    {
        return GetKey(key) && !_lastKeyStates![key];
    }

    public static bool GetKeyDown(char key)
    {
        return GetKey(key) && !_lastKeyStates![key];
    }
    
    public static bool GetKeyDown(KeyCode key)
    {
        return GetKey(key) && !_lastKeyStates![(int) key];
    }
    
    public static bool GetKeyUp(int key)
    {
        return !GetKey(key) && _lastKeyStates![key];
    }

    public static bool GetKeyUp(char key)
    {
        return !GetKey(key) && _lastKeyStates![key];
    }
    
    public static bool GetKeyUp(KeyCode key)
    {
        return !GetKey(key) && _lastKeyStates![(int) key];
    }
    
    public static bool GetKey(int key)
    {
        return _currentKeyStates![key];
    }

    public static bool GetKey(char key)
    {
        return _currentKeyStates![key];
    }
    
    public static bool GetKey(KeyCode key)
    {
        return _currentKeyStates![(int) key];
    }

    internal static void InitializeKeyStates()
    {
        _currentKeyStates = new SortedList<int, bool>();
        _lastKeyStates = new SortedList<int, bool>();
        for (var i = 0; i < 256; i++)
        {
            _currentKeyStates[i] = _lastKeyStates[i] = false;
        }
    }

    internal static void BeginFrame(ref IntPtr window)
    {
        for (var i = 0; i < 256; i++)
        {
            if (_lastKeyStates != null)
            {
                _lastKeyStates[i] = (User32.GetAsyncKeyState(i) & 0x8000) != 0;
            }
        }
        
        var cursorPoint = new Point();
        User32.GetCursorPos(ref cursorPoint);
        User32.ScreenToClient(window, ref cursorPoint);
        MouseX = cursorPoint.X;
        MouseY = cursorPoint.Y;
        MouseDeltaX = MouseX - _lastMousePositionX;
        MouseDeltaY = MouseY - _lastMousePositionY;
    }
    
    internal static void EndFrame()
    {
        for (var i = 0; i < KeyCount; i++)
        {
            if (_lastKeyStates != null && _currentKeyStates != null)
            {
                _lastKeyStates[i] = _currentKeyStates[i];
            }
        }
        
        _lastMousePositionX = MouseX;
        _lastMousePositionY = MouseY;
    }
}