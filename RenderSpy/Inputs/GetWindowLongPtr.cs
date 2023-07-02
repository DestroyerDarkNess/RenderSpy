using MinHook;
using RenderSpy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Inputs
{
    public class GetWindowLongPtr : IHook
    {

        IntPtr OrigAddr = IntPtr.Zero;
        HookEngine Engine;
        Globals.WindowProcDelegate Hook_orig;

        public event Globals.WindowProcDelegate WindowProc;

        public bool BlockInput  = false;

        private IntPtr Handle;

        public IntPtr WindowHandle   // property
        {
            get { return Handle; }   // get Handle
            set { Handle = value; }  // set Handle
        }

        public void Install()
        {
           
            OrigAddr = RenderSpy.Globals.WinApi.GetWindowLongPtr(Handle, (int)RenderSpy.Globals.WinApi.GWL.GWL_WNDPROC);

            if (OrigAddr != IntPtr.Zero)
            {

                Engine = new HookEngine();
                Hook_orig = Engine.CreateHook(OrigAddr, new Globals.WindowProcDelegate(WindowProc_Detour));
                Engine.EnableHooks();

            }
            else { throw new Exception("The corresponding Address is not found"); }


        }

        public virtual IntPtr WindowProc_Detour(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            WindowProc?.Invoke(hWnd, msg, wParam, lParam);

            if (BlockInput == true) { return IntPtr.Zero; } else { return Hook_orig(hWnd, msg, wParam, lParam); }
        }

        public void Uninstall()
        {
            Engine?.Dispose();
        }


    }

}
