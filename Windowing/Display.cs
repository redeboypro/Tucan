﻿using System.Runtime.InteropServices;
using Tucan.External;
using Tucan.External.HiddenFeatures;
using Tucan.External.OpenGL;
using Tucan.External.OpenGL.ModernGL;
using Tucan.Input;

namespace Tucan.Windowing;

public sealed class Display : IDisposable
{
    private const float TicksToMilliseconds = 0.0000001f;
    
    private readonly DisplaySettings _settings;
    private readonly WndProc _messageCallbackHolder;
    
    private IntPtr _hInstance;
    private IntPtr _hWindow;
    private IntPtr _hDeviceContext;
    private IntPtr _hRenderContext;
    
    private DateTime _lastTime;
    private DateTime _currentTime;
    
    private float _frameTime;
    private int _frameRecorder;

    private float _deltaTime;
    private int _framesPerSecond;

    private bool _vSync;

    public Display(DisplaySettings settings, int coreMajorVersion, int coreMinorVersion)
    {
        _lastTime = DateTime.Now;
        _currentTime = DateTime.Now;
        _frameTime = 0.0f;
        _frameRecorder = 0;
        
        _deltaTime = Single.Epsilon;
        _framesPerSecond = 0;

        _messageCallbackHolder = MessageCallbackPtr;
        
        InputManager.InitializeKeyStates();
        
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
    
    public float DeltaTime
    {
        get
        {
            return _deltaTime;
        }
    }

    public int FramesPerSecond
    {
        get
        {
            return _framesPerSecond;
        }
    }

    public bool VSync
    {
        get
        {
            return _vSync;
        }
        set
        {
            _vSync = value;
            MGL.SwapIntervalEXT(_vSync ? 1 : 0);
        }
    }

    public WindowMessageCallback? MessageCallback { get; set; }

    public void Show()
    {
        User32.ShowWindow(_hWindow, 1);
        User32.UpdateWindow(_hWindow);
    }

    public bool ShouldClose()
    {
        while (User32.PeekMessage(out var message, _hWindow, 0u, 0u, _settings.PeekMessageOptions))
        {
            if (message.Message is WindowMessage.Quit) return true;

            User32.TranslateMessage(ref message);
            User32.DispatchMessage(ref message);
        }
        
        _currentTime = DateTime.Now;
        _deltaTime = (_currentTime.Ticks - _lastTime.Ticks) * TicksToMilliseconds;
            
        _frameTime += _deltaTime;
        _frameRecorder++;

        if (_frameTime >= 1.0f)
        {
            _framesPerSecond = _frameRecorder;
            _frameTime = 0.0f;
            _frameRecorder = 0;
        }

        InputManager.BeginFrame(ref _hWindow);
        GL.MakeCurrent(_hDeviceContext, _hRenderContext);
        return false;
    }

    public void Destroy()
    {
        User32.DestroyWindow(_hWindow);
    }

    public void Update()
    {
        _lastTime = _currentTime;
        InputManager.EndFrame();
        SwapBuffers();
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

        if (_hDeviceContext == IntPtr.Zero) return;
        
        User32.ReleaseDC(_hWindow, _hDeviceContext);
        _hDeviceContext = IntPtr.Zero;
    }

    ~Display()
    {
        ReleaseUnmanagedResources();
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
            throw new Exception("Failed to set the modern OpenGL pixel format.");

        var pixelFormatDescriptor = new PixelFormatDescriptor();
        GDI32.DescribePixelFormat(hDeviceContext, pixelFormat, (uint) Marshal.SizeOf<PixelFormatDescriptor>(),
            ref pixelFormatDescriptor);

        if (GDI32.SetPixelFormat(hDeviceContext, pixelFormat, ref pixelFormatDescriptor) == 0)
            throw new Exception("Failed to set the modern OpenGL pixel format.");

        var glCoreAttribs = new[]
        {
            (int) ContextAttribute.MajorVersion, coreMajorVersion,
            (int) ContextAttribute.MinorVersion, coreMinorVersion,
            (int) ContextAttribute.ProfileMaskArb,  (int) ContextAttribute.CoreProfileBitArb,
            0
        };

        var glContext = MGL.CreateContextAttribs(hDeviceContext, IntPtr.Zero, glCoreAttribs);
        if (glContext == IntPtr.Zero)
            throw new Exception("Failed to activate modern OpenGL pixel format.");

        if (GL.MakeCurrent(hDeviceContext, glContext) == 0)
            throw new Exception("Failed to create modern OpenGL pixel format.");

        return glContext;
    }

    private void InitializeExtensions()
    {
        var dummyWindowClass = new WindowClass
        {
            Size = Marshal.SizeOf<WindowClass>(),
            Style = ClassStyle.HorizontalRedraw | ClassStyle.VerticalRedraw,
            Background = (IntPtr) 2,
            ClassExtra = 0,
            WindowExtra = 0,
            Icon = IntPtr.Zero,
            Cursor = User32.LoadCursor(IntPtr.Zero, 0),
            MenuName = null!,
            MessageCallbackPtr = Marshal.GetFunctionPointerForDelegate(_messageCallbackHolder),
            Instance = _hInstance,
            ClassName = "DummyClass"
        };
        
        var registerResult = User32.RegisterClass(ref dummyWindowClass);
        if (registerResult == 0)
            throw new Exception("Failed to register dummy.");
        
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
            throw new Exception("Failed to create dummy.");

        var dummyDeviceContext = User32.GetDC(dummyWindowInstance);

        if (SetPixelFormat(ref dummyWindowInstance, ref dummyDeviceContext))
        {
            var dummyGLContext = GL.CreateContext(dummyDeviceContext);

            if (dummyDeviceContext == IntPtr.Zero)
                throw new Exception("Failed to create dummy OpenGL context");

            if (GL.MakeCurrent(dummyDeviceContext, dummyGLContext) == 0)
                throw new Exception("Failed to activate dummy OpenGL context");
            
            MGL.LoadFunctions();

            GL.MakeCurrent(dummyDeviceContext, IntPtr.Zero);
            GL.DeleteContext(dummyGLContext);
            User32.ReleaseDC(dummyWindowInstance, dummyDeviceContext);
            User32.DestroyWindow(dummyWindowInstance);
            return;
        }
        
        throw new Exception("Failed to set dummy pixel format");
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
            MenuName = null,
            ClassName = className,
            MessageCallbackPtr = Marshal.GetFunctionPointerForDelegate(_messageCallbackHolder),
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

        if (GDI32.SetPixelFormat(hDeviceContext, iPixelFormat, ref pixelFormat) != 0) return true;
        
        User32.ReleaseDC(hWindow, hDeviceContext);
        hDeviceContext = IntPtr.Zero;
        return false;
    }
    
    private IntPtr MessageCallbackPtr(IntPtr hWindow, WindowMessage message, IntPtr wParam, IntPtr lParam)
    {
        MessageCallback?.Invoke(message);
        return User32.DefWindowProc(hWindow, message, wParam, lParam);
    }
    
    private void SwapBuffers()
    {
        GL.Flush();
        GDI32.SwapBuffers(_hDeviceContext);
    }

    private void ReleaseUnmanagedResources()
    {
        Release();
        Destroy();
        _hInstance = IntPtr.Zero;
        _hWindow = IntPtr.Zero;
        _hDeviceContext = IntPtr.Zero;
        _hRenderContext = IntPtr.Zero;
        _frameTime = 0;
        _frameRecorder = 0;
        _deltaTime = 0;
        _framesPerSecond = 0;
        _vSync = false;
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
}