using System;
using System.Threading;
using System.Windows.Forms;
using SharpDX.Direct3D9;
using SharpDX;
using SharpDX.Windows;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RenderSpy.Overlay
{
    public class D3D9Window : RenderForm
    {

        #region "Properties"

        public SharpDX.Color ClearColor = SharpDX.Color.Black;
        public bool AutoIni = true;
        public bool EnableDrag = false;

        public delegate void OnDXCreated(object sender, bool Status);
        public event OnDXCreated OnD3DReady = null;

        #endregion

        public D3D9Window()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.Text = "RenderSpy D3D9";
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.ForeColor = System.Drawing.Color.White;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            this.Load += OnLoad;
            this.Resize += OnResize;
            SystemEvents.DisplaySettingsChanging += OnDisplaySettingsChanging;

            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.TransparencyKey = System.Drawing.Color.Black;
            this.TopMost = true;
        }

        public D3D9Window(Device Dx9Device)
        {
            D3DDevice = Dx9Device;

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.Text = "RenderSpy D3D9";
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.ForeColor = System.Drawing.Color.White;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            this.Load += OnLoad;
            this.Resize += OnResize;
            SystemEvents.DisplaySettingsChanging += OnDisplaySettingsChanging;

            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.TransparencyKey = System.Drawing.Color.Black;
            this.TopMost = true;
        }


        private void OnLoad(object sender, EventArgs e)
        {
            if (AutoIni == true)
            {
                Initialized();
            }

        }

        #region " Initialized D3D "

        public void Initialized()
        {
            try
            {
                bool InitializeD3D = Ini();

                if (InitializeD3D == true)
                {
                    bool D3DReadyEventCalled = false;

                    RenderLoop.Run(this, () =>
                    {
                        D3DDevice.Clear(ClearFlags.Target, this.ClearColor, 1.0f, 0);
                        D3DDevice.BeginScene();

                        Trademark();

                        D3DDevice.EndScene();
                        D3DDevice.Present();

                        if (D3DReadyEventCalled == false) { D3DReadyEventCalled = true; OnD3DReady.Invoke(this, InitializeD3D); }

                    });

                }
            }
            catch (SharpDXException ex)
            {
                bool IsPathAvailable = this.Patch(ex);
                if (IsPathAvailable == false)
                {
                    throw ex;
                }
            }

        }

        public bool AutoPresentParams = true;
        public PresentParameters presentParams;

        public Device D3DDevice = null;

        public SharpDXException D3dError = null;

        private bool Ini()
        {
            if (D3DDevice == null)
            {
                if(AutoPresentParams) { presentParams = GetPresentParameters(); }
               
                if (CreateDevice() == false) { D3dError = new SharpDXException("CreateDevice Error"); return false; }
            }
            return (D3dError == null);
        }

        public PresentParameters GetPresentParameters()
        {
            PresentParameters presentParameters = new PresentParameters();
            presentParameters.Windowed = true;
            presentParameters.SwapEffect = SwapEffect.Discard;
            presentParameters.BackBufferFormat = Format.A8R8G8B8;

            return presentParams;
        }

        public bool CreateDevice()
        {
            try
            {

                D3DDevice = new Device(new Direct3D(), 0, DeviceType.Hardware, this.Handle, CreateFlags.MixedVertexProcessing, presentParams);

                return true;

            }
            catch (SharpDXException Ex) { D3dError = Ex; return false; }

        }

        #endregion

        #region " Reset Device "

        public bool Patch(SharpDXException DXException)
        {
            if (DXException.ResultCode == ResultCode.DeviceLost)
            {
                try
                {
                    Reset();
                }
                catch (SharpDXException Ex)
                {

                    if (DXException.ResultCode == ResultCode.InvalidCall)
                    {
                        D3dError = Ex; return false;
                    }
                }

                return true;
            }

            Thread.Sleep(1000);
            return false;

        }

        protected override void OnActivated(EventArgs e)
        {
            Reset();
            base.OnActivated(e);
        }

        private void OnResize(object sender, EventArgs e)
        {
            Reset();
        }

        void OnDisplaySettingsChanging(object sender, EventArgs e)
        {
            Reset();
        }

        public bool Reset()
        {
            if (D3DDevice != null)
            {

                foreach (SharpDX.Direct3D9.Font Fonts in FontSurfaces)
                {
                    Fonts.Dispose();
                }

                FontSurfaces.Clear();

                foreach (SharpDX.Direct3D9.Line Lines in LineSurfaces)
                {
                    Lines.Dispose();
                }

                LineSurfaces.Clear();

                presentParams.BackBufferHeight = this.ClientSize.Height;
                presentParams.BackBufferWidth = this.ClientSize.Width;
                D3DDevice.Reset(presentParams);
            }

            return false;
        }

        #endregion

        #region " Trademark "

        public List<SharpDX.Direct3D9.Font> FontSurfaces = new List<SharpDX.Direct3D9.Font>();
       
        public List<SharpDX.Direct3D9.Line>  LineSurfaces = new List<SharpDX.Direct3D9.Line>();

        private bool Trademark()
        {
         
            if (D3DDevice != null && FontSurfaces.Count == 0)
            {

                 FontDescription fontDescription = new FontDescription()
                 {
                    Height = 14,
                    Italic = false,
                    CharacterSet = FontCharacterSet.Ansi,
                    FaceName = "Arial",
                    MipLevels = 0,
                    OutputPrecision = FontPrecision.TrueType,
                    PitchAndFamily = FontPitchAndFamily.Default,
                    Quality = FontQuality.ClearType,
                    Weight = FontWeight.Regular
                 };

                   SharpDX.Direct3D9.Font TradeMarkFont = new SharpDX.Direct3D9.Font(D3DDevice, fontDescription);
                   FontSurfaces.Add(TradeMarkFont);
            }

            if (D3DDevice != null && LineSurfaces.Count == 0)
            {

                LineSurfaces.Add(new SharpDX.Direct3D9.Line(D3DDevice));

            }

            FontSurfaces[0].DrawText(null, "https://github.com/DestroyerDarkNess/RenderSpy" + Environment.NewLine + "Discord: Destroyer#8328", 0, 0, ClearColor);
            
            return true;
        }

        #endregion

        #region " Overlay "

        public bool NoActiveWindow = false;
        public int WindowStyles = 0;

        protected override bool ShowWithoutActivation
        {
            get
            {
                return NoActiveWindow;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParamsA = base.CreateParams;
               
                if (WindowStyles != 0)
                {
                    createParamsA.ExStyle = createParamsA.ExStyle | WindowStyles;
                }
                return createParamsA;
            }
        }

        #endregion

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
                Reset();
            }
        }



        #endregion

    }
}
