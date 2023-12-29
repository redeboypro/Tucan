using System.Runtime.InteropServices;
using Tucan.External;
using Tucan.External.HiddenFeatures;
using Tucan.External.OpenGL;

namespace Tucan.Windowing;

public delegate void WindowMessageCallbackDel(WindowMessage message);

public sealed class Display
{
    private readonly IntPtr _hInstance;
    private readonly DisplaySettings _settings;
    private readonly IntPtr _hWindow;
    private IntPtr _hDeviceContext;
    private IntPtr _hRenderContext;

    public Display(DisplaySettings settings)
    {
        _hInstance = Marshal.GetHINSTANCE(GetType().Module);
        _settings = settings;

        var registerResult = RegisterDisplayClass(
            _hInstance,
            settings.ClassStyle,
            settings.CursorType,
            settings.ClassName);
        
        _hWindow = User32.CreateWindowEx(
            0,
            registerResult,
            settings.Title,
            (uint) settings.WindowStyle,
            0, 0, 
            settings.Width, settings.Height,
            IntPtr.Zero, 
            IntPtr.Zero,
            _hInstance, 
            IntPtr.Zero);

        _hDeviceContext = User32.GetDC(_hWindow);
        
        if (SetPixelFormat())
        {
            _hRenderContext = GL.CreateContext(_hDeviceContext);
            if (_hRenderContext == IntPtr.Zero)
            {
                User32.ReleaseDC(_hWindow, _hDeviceContext);
                _hDeviceContext = IntPtr.Zero;
            }
        }

        GL.MakeCurrent(_hDeviceContext, _hRenderContext);
    }
    
    public WindowMessageCallbackDel? MessageCallback { get; set; }

    public void Show()
    {
        User32.ShowWindow(_hWindow, 1);
        User32.UpdateWindow(_hWindow);
    }

    public bool ProcessMessages()
    {
        while (User32.PeekMessage(out var message, _hWindow, 0u, 0u, _settings.PeekMessageOptions))
        {
            if (message.message == (uint) WindowMessage.Quit)
            {
                return false;
            }

            User32.TranslateMessage(ref message);
            User32.DispatchMessage(ref message);
        }

        GL.MakeCurrent(_hDeviceContext, _hRenderContext);
        return true;
    }

    public void Destroy()
    {
        User32.DestroyWindow(_hWindow);
    }

    public void SwapBuffers()
    {
        GL.Flush();
        GDI32.SwapBuffers(_hDeviceContext);
    }

    ~Display()
    {
        Destroy();
        User32.UnregisterClass(_settings.ClassName, _hInstance);
        
        if (_hRenderContext != IntPtr.Zero) 
        {
            GL.MakeCurrent(_hDeviceContext, IntPtr.Zero);
            GL.DeleteContext(_hRenderContext);
            _hRenderContext = IntPtr.Zero;
        }

        if (_hDeviceContext == IntPtr.Zero)
        {
            return;
        }
        
        User32.ReleaseDC(_hWindow, _hDeviceContext);
        _hDeviceContext = IntPtr.Zero;
    }
    
    private ushort RegisterDisplayClass(
        IntPtr hInstance,
        ClassStyle windowClassStyle,
        CursorType cursorType,
        string className)
    {
        var windowClassInstance = new WNDCLASSEX
        {
            cbSize = Marshal.SizeOf(typeof(WNDCLASSEX)),
            style = (int) windowClassStyle,
            hbrBackground = (IntPtr) 2,
            cbClsExtra = 0,
            cbWndExtra = 0,
            hInstance = hInstance,
            hIcon = IntPtr.Zero,
            hCursor = User32.LoadCursor(IntPtr.Zero, (int) cursorType),
            lpszMenuName = null!,
            lpszClassName = className,
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(new WndProc(MessageCallbackPtr)),
            hIconSm = IntPtr.Zero
        };

        return User32.RegisterClass(ref windowClassInstance);
    }

    private bool SetPixelFormat()
    {
        var pixelFormat = new PIXELFORMATDESCRIPTOR
        {
            nSize = 40,
            nVersion = 1,
            dwFlags = (uint) _settings.PixelFormatDescriptorFlags,
            iPixelType = (byte) _settings.PixelFormatDescriptorType,
            cColorBits = 24,
            cRedBits = 0, cRedShift = 0,
            cGreenBits = 0, cGreenShift = 0,
            cBlueBits = 0, cBlueShift = 0,
            cAlphaBits = 0, cAlphaShift = 0,
            cAccumBits = 0, 
            cAccumRedBits = 0, 
            cAccumGreenBits = 0,
            cAccumBlueBits = 0, 
            cAccumAlphaBits = 0,
            cDepthBits = 24, 
            cStencilBits = 0,
            cAuxBuffers = 0,
            iLayerType = (sbyte) _settings.PixelFormatDescriptorPlanes,
            bReserved = 0,
            dwLayerMask = 0,
            dwVisibleMask = 0,
            dwDamageMask = 0
        };
        
        var iPixelFormat = GDI32.ChoosePixelFormat(_hDeviceContext, pixelFormat);
        if (iPixelFormat == 0) 
        {
            pixelFormat.cDepthBits = 32;
            iPixelFormat = GDI32.ChoosePixelFormat(_hDeviceContext, pixelFormat);
        }
        
        if (iPixelFormat == 0) 
        {
            pixelFormat.cDepthBits = 16;
            iPixelFormat = GDI32.ChoosePixelFormat(_hDeviceContext, pixelFormat);
        }
        
        if (iPixelFormat == 0) 
        {
            User32.ReleaseDC(_hWindow, _hDeviceContext);
            _hDeviceContext = IntPtr.Zero;
            return false;
        }

        if (GDI32.SetPixelFormat(_hDeviceContext, iPixelFormat, pixelFormat) != 0)
        {
            return true;
        }
        
        User32.ReleaseDC(_hWindow, _hDeviceContext);
        _hDeviceContext = IntPtr.Zero;
        return false;
    }
    
    private IntPtr MessageCallbackPtr(IntPtr hWindow, uint message, IntPtr wParam, IntPtr lParam)
    {
        MessageCallback?.Invoke((WindowMessage) message);
        return User32.DefWindowProc(hWindow, message, wParam, lParam);
    }
}