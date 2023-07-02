using MinHook;
using RenderSpy.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Inputs
{
    public class SetWindowLongPtr : IHook
    {

        private Globals.WindowProcDelegate _newCallback = null;
        private IntPtr _oldCallback;

        public event Globals.WindowProcDelegate WindowProc;

        public bool BlockInput = false;

        public IntPtr Handle;

        public IntPtr WindowHandle   // property
        {
            get { return Handle; }   // get Handle
            set { Handle = value; }  // set Handle
        }

        public void Install()
        {
            if (_newCallback == null)
            {
                _newCallback = WindowProc_Detour;
                _oldCallback = RenderSpy.Globals.WinApi.SetWindowLongPtr(Handle, (int)RenderSpy.Globals.WinApi.GWL.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(_newCallback));
                if (_oldCallback == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }

        }

        public virtual IntPtr WindowProc_Detour(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            WindowProc?.Invoke(hWnd, msg, wParam, lParam);
          
            if (BlockInput == true) { return IntPtr.Zero; } else { return RenderSpy.Globals.WinApi.CallWindowProc(_oldCallback, hWnd, (int)msg, wParam, lParam); }
        }

        public void Uninstall()
        {
            if (_newCallback != null)
            {
                RenderSpy.Globals.WinApi.SetWindowLongPtr(Handle, (int)RenderSpy.Globals.WinApi.GWL.GWL_WNDPROC, _oldCallback);
                _newCallback = null;
            }
        }

    }
}
