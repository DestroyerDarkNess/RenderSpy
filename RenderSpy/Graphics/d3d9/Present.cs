using MinHook;
using RenderSpy.Interfaces;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Graphics.d3d9
{
    public class Present : IHook
    {

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int PresentDelegate(IntPtr device, IntPtr sourceRect, IntPtr destRect, IntPtr hDestWindowOverride, IntPtr dirtyRegion);


        IntPtr OrigAddr = IntPtr.Zero;
        HookEngine Engine;
        PresentDelegate Present_orig;

        public Device GlobalDevice = null;

        public event PresentDelegate PresentEvent;

        public void Install()
        {

            OrigAddr = Globals.GetFunctionPtr(Direct3DDevice9FunctionOrdinals.Present, GlobalDevice);

            if (OrigAddr != IntPtr.Zero)
            {

                Engine = new HookEngine();
                Present_orig = Engine.CreateHook(OrigAddr, new PresentDelegate(Present_Detour));
                Engine.EnableHooks();

            }
            else { throw new Exception("The corresponding Address is not found in the vTable"); }


        }

        public virtual int Present_Detour(IntPtr device, IntPtr sourceRect, IntPtr destRect, IntPtr hDestWindowOverride, IntPtr dirtyRegion)
        {
            PresentEvent?.Invoke(device, sourceRect, destRect, hDestWindowOverride, dirtyRegion);

            return Present_orig(device, sourceRect, destRect, hDestWindowOverride, dirtyRegion);
        }

        public void Uninstall()
        {
            Engine?.Dispose();
        }


    }
}
