using System.Runtime.InteropServices;
using Tucan.External;
using Tucan.External.HiddenFeatures;
using Tucan.External.OpenGL;
using Tucan.External.OpenGL.ModernGL;

namespace Tucan.Windowing;

public delegate void WindowMessageCallbackDel(WindowMessage message);

public sealed class Display
{
    private readonly IntPtr _hInstance;
    private readonly DisplaySettings _settings;
    private readonly IntPtr _hWindow;
    private IntPtr _hDeviceContext;
    private IntPtr _hRenderContext;

    public Display(DisplaySettings settings, int coreMajorVersion, int coreMinorVersion)
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

        _hRenderContext = InitializeGL(_hDeviceContext, coreMajorVersion, coreMinorVersion);

        if (_hRenderContext == IntPtr.Zero) 
        {
            User32.ReleaseDC(_hWindow, _hDeviceContext); 
            _hDeviceContext = IntPtr.Zero;
        }

        GL.MakeCurrent(_hDeviceContext, _hRenderContext);
    }

    private IntPtr InitializeGL(IntPtr hDeviceContext, int coreMajorVersion, int coreMinorVersion)
    {
        InitializeExtensions();

        var pixelFormatAttribs = new []
        {
            (int) PixelFormatAttribute.DrawToWindowArb, 1,
            (int) PixelFormatAttribute.SupportOpenGLArb, 1,
            (int) PixelFormatAttribute.DoubleBufferArb, 1,
            (int) PixelFormatAttribute.AccelerationArb, (int) PixelFormatAttribute.FullAccelerationArb,
            (int) PixelFormatAttribute.PixelTypeArb, (int) PixelFormatAttribute.TypeRgbaArb,
            (int) PixelFormatAttribute.ColorBitsArb, 32,
            (int) PixelFormatAttribute.DepthBitsArb, 24,
            (int) PixelFormatAttribute.StencilBitsArb, 8
        };

        MGL.ChoosePixelFormat(hDeviceContext, pixelFormatAttribs, 0, 1, out var pixelFormat, out var numFormats);
        if (numFormats == 0)
        {
            throw new Exception("Failed to set the modern OpenGL pixel format.");
        }

        GDI32.DescribePixelFormat(hDeviceContext, pixelFormat, Marshal.SizeOf<PixelFormatDescriptor>(),
            out var pixelFormatDescriptor);

        if (GDI32.SetPixelFormat(hDeviceContext, pixelFormat, ref pixelFormatDescriptor) == 0)
        {
            throw new Exception("Failed to set the modern OpenGL pixel format.");
        }

        var glCoreAttribs = new[]
        {
            (int) ContextAttribute.MajorVersion, coreMajorVersion,
            (int) ContextAttribute.MinorVersion, coreMinorVersion,
            (int) ContextAttribute.ProfileMaskArb,  (int) ContextAttribute.CoreProfileBitArb,
            0
        };

        var glContext = MGL.CreateContextAttribs(hDeviceContext, IntPtr.Zero, glCoreAttribs);
        if (glContext == IntPtr.Zero)
        {
            throw new Exception("Failed to activate modern OpenGL pixel format.");
        }

        if (GL.MakeCurrent(hDeviceContext, glContext) == 0)
        {
            throw new Exception("Failed to create modern OpenGL pixel format.");
        }

        return glContext;
    }

    private void InitializeExtensions()
    {
        var dummyWindowClass = new WindowClass
        {
            Size = Marshal.SizeOf(typeof(WindowClass)),
            Style = ClassStyle.HorizontalRedraw | ClassStyle.VerticalRedraw,
            Background = (IntPtr) 2,
            ClassExtra = 0,
            WindowExtra = 0,
            Icon = IntPtr.Zero,
            Cursor = User32.LoadCursor(IntPtr.Zero, (int) CursorType.No),
            MenuName = null!,
            MessageCallbackPtr = Marshal.GetFunctionPointerForDelegate(new WndProc(User32.DefaultMessageCallback)),
            Instance = _hInstance,
            ClassName = "DummyClass"
        };
        
        var registerResult = User32.RegisterClass(ref dummyWindowClass);
        if (registerResult == 0)
        {
            throw new Exception("Failed to register dummy.");
        }
        
        const int useDefault = unchecked((int) 0x80000000);

        var dummyWindowInstance = User32.CreateWindowEx(
            0,
            registerResult,
            "Dummy",
            0,
            useDefault, useDefault, 
            useDefault, useDefault,
            IntPtr.Zero, 
            IntPtr.Zero,
            _hInstance, 
            IntPtr.Zero);
        
        if (dummyWindowInstance == IntPtr.Zero) 
        {
            throw new Exception("Failed to create dummy.");
        }

        var dummyDeviceContext = User32.GetDC(dummyWindowInstance);

        if (SetPixelFormat(ref dummyWindowInstance, ref dummyDeviceContext))
        {
            var dummyGLContext = GL.CreateContext(dummyDeviceContext);

            if (dummyDeviceContext == IntPtr.Zero)
            {
                throw new Exception("Failed to create dummy OpenGL context");
            }

            if (GL.MakeCurrent(dummyDeviceContext, dummyGLContext) == 0)
            {
                throw new Exception("Failed to activate dummy OpenGL context");
            }
            
            MGL.LoadFunctions();

            GL.MakeCurrent(dummyDeviceContext, IntPtr.Zero);
            GL.DeleteContext(dummyGLContext);
            User32.ReleaseDC(dummyWindowInstance, dummyDeviceContext);
            User32.DestroyWindow(dummyWindowInstance);
        }
        else
        {
            throw new Exception("Failed to set dummy pixel format");
        }
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
            if (message.Message is WindowMessage.Quit)
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

    public void Release()
    {
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

    ~Display()
    {
        Destroy();
        Release();
    }
    
    private ushort RegisterDisplayClass(
        IntPtr hInstance,
        ClassStyle windowClassStyle,
        CursorType cursorType,
        string className)
    {
        var windowClassInstance = new WindowClass
        {
            Size = Marshal.SizeOf<WindowClass>(),
            Style = windowClassStyle,
            Background = (IntPtr) 2,
            ClassExtra = 0,
            WindowExtra = 0,
            Instance = hInstance,
            Icon = IntPtr.Zero,
            Cursor = User32.LoadCursor(IntPtr.Zero, (int) cursorType),
            MenuName = null!,
            ClassName = className,
            MessageCallbackPtr = Marshal.GetFunctionPointerForDelegate(new WndProc(MessageCallbackPtr)),
            SmallIcon = IntPtr.Zero
        };

        return User32.RegisterClass(ref windowClassInstance);
    }

    private bool SetPixelFormat(ref IntPtr hWindow, ref IntPtr hDeviceContext)
    {
        var pixelFormat = new PixelFormatDescriptor
        {
            Size = (ushort) Marshal.SizeOf<PixelFormatDescriptor>(),
            Version = 1,
            Flags = _settings.PixelFormatDescriptorFlags,
            PixelType = _settings.PixelFormatDescriptorType,
            ColorBits = 32,
            RedBits = 0, RedShift = 0,
            GreenBits = 0, GreenShift = 0,
            BlueBits = 0, BlueShift = 0,
            AlphaBits = 8, AlphaShift = 0,
            AccumBits = 0, 
            AccumRedBits = 0, 
            AccumGreenBits = 0,
            AccumBlueBits = 0, 
            AccumAlphaBits = 0,
            DepthBits = 32, 
            StencilBits = 8,
            AuxBuffers = 0,
            LayerType = _settings.PixelFormatDescriptorPlane,
            Reserved = 0,
            LayerMask = 0,
            VisibleMask = 0,
            DamageMask = 0
        };
        
        var iPixelFormat = GDI32.ChoosePixelFormat(hDeviceContext, ref pixelFormat);

        if (iPixelFormat == 0) 
        {
            pixelFormat.DepthBits = 16;
            iPixelFormat = GDI32.ChoosePixelFormat(hDeviceContext, ref pixelFormat);
        }
        
        if (iPixelFormat == 0) 
        {
            User32.ReleaseDC(hWindow, hDeviceContext);
            hDeviceContext = IntPtr.Zero;
            return false;
        }

        if (GDI32.SetPixelFormat(hDeviceContext, iPixelFormat, ref pixelFormat) != 0)
        {
            return true;
        }
        
        User32.ReleaseDC(hWindow, hDeviceContext);
        hDeviceContext = IntPtr.Zero;
        return false;
    }
    
    private IntPtr MessageCallbackPtr(IntPtr hWindow, uint message, IntPtr wParam, IntPtr lParam)
    {
        MessageCallback?.Invoke((WindowMessage) message);
        return User32.DefWindowProc(hWindow, message, wParam, lParam);
    }
}