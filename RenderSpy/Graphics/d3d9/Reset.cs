using MinHook;
using RenderSpy.Interfaces;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static RenderSpy.Graphics.d3d9.Present;

namespace RenderSpy.Graphics.d3d9
{
    public class Reset : IHook
    {

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int ResetDelegate(IntPtr device, ref PresentParameters presentParameters);

        IntPtr OrigAddr = IntPtr.Zero;
        HookEngine Engine;
        public ResetDelegate Reset_orig;

        public Device GlobalDevice = null;

        public event ResetDelegate Reset_Event;


        public void Install()
        {

            OrigAddr = Globals.GetFunctionPtr(Direct3DDevice9FunctionOrdinals.Reset, GlobalDevice);

            if (OrigAddr != IntPtr.Zero)
            {

                Engine = new HookEngine();
                Reset_orig = Engine.CreateHook(OrigAddr, new ResetDelegate(Reset_Detour));
                Engine.EnableHooks();

            }
            else { throw new Exception("The corresponding Address is not found in the vTable"); }


        }

        public virtual int Reset_Detour(IntPtr device, ref PresentParameters presentParameters)
        {

            if (Reset_Event != null)
            {
                int Result = Reset_Event.Invoke(device, ref presentParameters);
                return Result;
            }
            else { return Reset_orig(device, ref presentParameters); }

        }

        public void Uninstall()
        {
            Engine?.Dispose();
        }


    }
}
