using MinHook;
using RenderSpy.Interfaces;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Graphics.d3d10
{
    public class Present : IHook
    {

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int PresentDelegate(IntPtr swapChainPtr, int syncInterval, PresentFlags flags);

        IntPtr OrigAddr = IntPtr.Zero;
        HookEngine Engine;
        public PresentDelegate Present_orig;

        public SharpDX.Direct3D10.Device GlobalDevice = null;

        public event PresentDelegate PresentEvent;

        public void Install()
        {

            OrigAddr = Globals.GetFunctionPtr(dxgi.DXGISwapChainVTbl.Present, GlobalDevice);

            if (OrigAddr != IntPtr.Zero)
            {

                Engine = new HookEngine();
                Present_orig = Engine.CreateHook(OrigAddr, new PresentDelegate(Present_Detour));
                Engine.EnableHooks();

            }
            else { throw new Exception("The corresponding Address is not found in the vTable"); }


        }

        public virtual int Present_Detour(IntPtr swapChainPtr, int syncInterval, PresentFlags flags)
        {

            if (PresentEvent != null)
            {
                int result = PresentEvent.Invoke(swapChainPtr, syncInterval, flags);
                return result;
            }
            else { return Present_orig(swapChainPtr, syncInterval, flags); }

        }

        public void Uninstall()
        {
            Engine?.Dispose();
        }


    }
}
