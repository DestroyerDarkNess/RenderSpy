
using Microsoft.Win32;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace RenderSpy.Overlay
{
    public class D3D9Window : RenderForm
    {
        #region Properties

        // Public Properties
        public Color ClearColor { get; set; } = Color.Black;
        public bool AutoInitialize { get; set; } = true;
        public bool EnableDrag { get; set; } = false;
        private bool AutoPresentParams { get; set; } = true;

        private PresentParameters _presentParams;

        // Public property with getter and setter
        public PresentParameters PresentParams
        {
            get
            {
                if (AutoPresentParams) _presentParams = CreateDefaultPresentParameters();

                return _presentParams;
            }
            set { AutoPresentParams = false; _presentParams = value; }
        }

        private bool AutoCreateDevice { get; set; } = true;

        private Device _D3DDevice = null;


        public Device D3DDevice
        {
            get
            {
                return _D3DDevice;
            }
            set { AutoCreateDevice = false; _D3DDevice = value; }
        }

        public delegate void OnDXCreated(object sender, bool Status);
        public event OnDXCreated OnD3DReady = null;

        // Private Fields
        private bool _isInitialized = false;
        private bool _deviceReadyEventCalled = false;


        #endregion

        #region Constructor

        public D3D9Window()
        {
            InitializeWindow();
        }

        #endregion

        #region Window Initialization

        private void InitializeWindow()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            this.Text = "RenderSpy D3D9";
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.ForeColor = System.Drawing.Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.Black;
            this.TransparencyKey = System.Drawing.Color.Black;
            this.AllowTransparency = true;
            this.TopMost = true;

            this.Load += OnLoad;
            this.Resize += OnResize;
            SystemEvents.DisplaySettingsChanging += OnDisplaySettingsChanging;

        }

        private void OnLoad(object sender, EventArgs e)
        {
            if (AutoInitialize)
            {
                InitializeD3D();
            }
        }

        #endregion

        #region D3D Initialization

        public Thread overlayThread = null;

        public void InitializeD3D()
        {
            try
            {
                if (!_isInitialized)
                {
                    if (AutoCreateDevice) _D3DDevice = CreateDevice();
                    bool success = (_D3DDevice != null);
                    if (success)
                    {
                        _isInitialized = true;
                        RenderLoop.Run(this, RenderCallback);
                    }
                }
            }
            catch (SharpDXException ex)
            {
                if (!HandleDeviceLost(ex))
                {
                    throw;
                }
            }
        }

        private PresentParameters CreateDefaultPresentParameters()
        {
            return new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                BackBufferFormat = Format.A8R8G8B8,
                PresentationInterval = PresentInterval.Immediate // Prevent vsync for reducing lag
            };
        }

        private Device CreateDevice()
        {
            try
            {
                return new DeviceEx(new Direct3DEx(), 0, DeviceType.Hardware, this.Handle, CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded, PresentParams); // new Device(new Direct3D(), 0, DeviceType.Hardware, this.Handle, CreateFlags.HardwareVertexProcessing, PresentParams); ;
            }
            catch (SharpDXException ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return null;
            }
        }

        #endregion

        #region Rendering

        public int FPSlimit = 0; // unlimited by default

        private void RenderCallback()
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                D3DDevice.Clear(ClearFlags.Target, ClearColor, 1.0f, 0);
                D3DDevice.BeginScene();

                // Perform your drawing/rendering here
                RenderOverlay();

                D3DDevice.EndScene();
                D3DDevice.Present();

                if (!_deviceReadyEventCalled)
                {
                    _deviceReadyEventCalled = true;
                    OnD3DReady?.Invoke(this, true);
                }
            }
            catch (SharpDXException ex)
            {
                if (!HandleDeviceLost(ex))
                {
                    throw;
                }
            }

            if (FPSlimit > 0)
            {
                // Frame limiting
                int FrameDelay = 1000 / FPSlimit;
                stopwatch.Stop();
                int elapsedTime = (int)stopwatch.ElapsedMilliseconds;
                if (elapsedTime < FrameDelay)
                {
                    Thread.Sleep(FrameDelay - elapsedTime); // Sleep to limit frame rate
                }
            }

        }


        private void RenderOverlay()
        {
            // Example rendering method - customize with your actual rendering logic
            if (FontSurfaces.Count == 0)
            {
                InitializeFontsAndLines();
            }

            FontSurfaces[0].DrawText(null, "Overlay: RenderSpy", 10, 10, ClearColor);
        }

        #endregion

        #region Resource Management

        public List<SharpDX.Direct3D9.Font> FontSurfaces = new List<SharpDX.Direct3D9.Font>();
        public List<SharpDX.Direct3D9.Line> LineSurfaces = new List<SharpDX.Direct3D9.Line>();

        private void InitializeFontsAndLines()
        {
            if (D3DDevice == null) return;

            FontDescription fontDesc = new FontDescription
            {
                Height = 14,
                FaceName = "Arial",
                Weight = FontWeight.UltraLight,
                Quality = FontQuality.Antialiased
            };

            var font = new SharpDX.Direct3D9.Font(D3DDevice, fontDesc);
            FontSurfaces.Add(font);

            var line = new SharpDX.Direct3D9.Line(D3DDevice);
            LineSurfaces.Add(line);
        }

        private void DisposeResources()
        {
            foreach (var font in FontSurfaces)
            {
                font.Dispose();
            }
            FontSurfaces.Clear();

            foreach (var line in LineSurfaces)
            {
                line.Dispose();
            }
            LineSurfaces.Clear();
        }

        #endregion

        #region Device Reset and Error Handling

        private void OnResize(object sender, EventArgs e)
        {
            ResetDevice();
        }

        private void OnDisplaySettingsChanging(object sender, EventArgs e)
        {
            ResetDevice();
        }

        public void ResetDevice()
        {
            if (D3DDevice != null)
            {
                DisposeResources();

                // Now you can modify PresentParams (backed by the _presentParams field)
                _presentParams.BackBufferWidth = ClientSize.Width;
                _presentParams.BackBufferHeight = ClientSize.Height;

                try
                {
                    D3DDevice.Reset(PresentParams);
                }
                catch (SharpDXException ex)
                {
                    HandleDeviceLost(ex);
                }
            }
        }

        private bool HandleDeviceLost(SharpDXException ex)
        {
            if (ex.ResultCode == ResultCode.DeviceLost)
            {
                try
                {
                    Thread.Sleep(1000);
                    ResetDevice();
                    return true;
                }
                catch (SharpDXException resetEx)
                {
                    if (resetEx.ResultCode == ResultCode.InvalidCall)
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Overlay Window Settings

        public bool NoActivateWindow { get; set; } = false;

        protected override bool ShowWithoutActivation => NoActivateWindow;

        private int additionalExStyle = 0;

        public int AdditionalExStyle
        {
            get => additionalExStyle;
            set
            {
                additionalExStyle = value;
                UpdateStyles();
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var createParams = base.CreateParams;
                createParams.ExStyle |= 0x80000; // WS_EX_LAYERED (transparent window)
                createParams.ExStyle |= 0x20;    // WS_EX_TRANSPARENT (overlay doesn't interfere with mouse events)
                createParams.ExStyle |= additionalExStyle;

                return createParams;
            }
        }

        #endregion

        //#region Window Dragging and Resize

        //private const int WM_NCHITTEST = 0x84;
        //private const int HTCAPTION = 0x2;

        //protected override void WndProc(ref Message m)
        //{
        //    if (m.Msg == WM_NCHITTEST)
        //    {
        //        var cursorPos = this.PointToClient(Cursor.Position);
        //        if (EnableDrag && cursorPos.Y <= 30)
        //        {
        //            m.Result = (IntPtr)HTCAPTION;
        //            return;
        //        }
        //    }

        //    base.WndProc(ref m);
        //}

        //#endregion

        #region " Drag Form "

        private const int WM_NCHITTEST = 0x84;
        private const int HTCAPTION = 0x2;
        private const int HTCLIENT = 0x1;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;

        public bool Fix_WM_NCLBUTTONDBLCLK = false;

        public bool ResizableBorders = false;

        protected override void WndProc(ref Message message)
        {
            FormWindowState org = this.WindowState;


            if (message.Msg == WM_NCHITTEST)
            {
                System.Drawing.Point clientPoint = this.PointToClient(new System.Drawing.Point(message.LParam.ToInt32()));

                if (EnableDrag && IsCaptionArea(clientPoint))
                {
                    message.Result = (IntPtr)(HTCAPTION);
                }
                else
                {
                    base.WndProc(ref message);

                    if (ResizableBorders == true && message.Result.ToInt32() == HTCLIENT)
                    {
                        System.Drawing.Point p = this.PointToClient(Cursor.Position);

                        if (p.X <= 5 && p.Y <= 5)
                            message.Result = (IntPtr)HTTOPLEFT;
                        else if (p.X >= this.ClientRectangle.Width - 5 && p.Y <= 5)
                            message.Result = (IntPtr)HTTOPRIGHT;
                        else if (p.X <= 5 && p.Y >= this.ClientRectangle.Height - 5)
                            message.Result = (IntPtr)HTBOTTOMLEFT;
                        else if (p.X >= this.ClientRectangle.Width - 5 && p.Y >= this.ClientRectangle.Height - 5)
                            message.Result = (IntPtr)HTBOTTOMRIGHT;
                        else if (p.X <= 5)
                            message.Result = (IntPtr)HTLEFT;
                        else if (p.X >= this.ClientRectangle.Width - 5)
                            message.Result = (IntPtr)HTRIGHT;
                        else if (p.Y <= 5)
                            message.Result = (IntPtr)HTTOP;
                        else if (p.Y >= this.ClientRectangle.Height - 5)
                            message.Result = (IntPtr)HTBOTTOM;
                    }
                }
            }
            else
            {

                if (Fix_WM_NCLBUTTONDBLCLK = true && message.Msg == 0xA3)
                {
                    return;
                }
                else
                {

                    base.WndProc(ref message);
                }

            }

            if (this.WindowState != org)
                this.OnFormWindowStateChanged(org, this.WindowState);
        }

        public int borderSize = 5;

        private bool IsCaptionArea(System.Drawing.Point clientPoint)
        {
            System.Drawing.Rectangle DragZone = new System.Drawing.Rectangle(borderSize, borderSize,
                                                 this.ClientSize.Width - 2 * borderSize,
                                                 this.ClientSize.Height - 2 * borderSize);

            return DragZone.Contains(clientPoint);
        }

        protected virtual void OnFormWindowStateChanged(FormWindowState Old, FormWindowState New)
        {
            if (New == FormWindowState.Maximized | New == FormWindowState.Normal)
            {
                ResetDevice();
            }
        }

        #endregion

        #region " Other "

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        const int GWL_STYLE = -16;
        const int WS_CHILD = 0x40000000; // Estilo de ventana hija
        const int WS_POPUP = unchecked((int)0x80000000);  // Estilo de ventana popup (superposición)

        public void MakeOverlayChild(IntPtr overlayHandle, IntPtr gameHandle)
        {
            // Elimina el estilo WS_POPUP si está presente
            int style = GetWindowLong(overlayHandle, GWL_STYLE);
            style &= ~WS_POPUP;  // Remover WS_POPUP
            style |= WS_CHILD;   // Añadir WS_CHILD para hacerla una ventana hija

            // Aplica los nuevos estilos a la ventana
            SetWindowLong(overlayHandle, GWL_STYLE, style);

            // Ahora puedes usar SetParent para hacer que sea hija de la ventana del juego
            SetParent(overlayHandle, gameHandle);
        }


        #endregion

    }
}
