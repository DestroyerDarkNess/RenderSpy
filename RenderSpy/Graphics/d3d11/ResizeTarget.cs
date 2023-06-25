using MinHook;
using RenderSpy.Interfaces;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Graphics.d3d11
{
    public class ResizeTarget : IHook
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int ResizeTargetDelegate(IntPtr swapChainPtr, ref ModeDescription newTargetParameters);

        IntPtr OrigAddr = IntPtr.Zero;
        HookEngine Engine;
        ResizeTargetDelegate ResizeTarget_orig;

        public event ResizeTargetDelegate OnPreResizeTarget;
        public event ResizeTargetDelegate OnPostResizeTarget;

        public void Install()
        {

            OrigAddr = Globals.GetFunctionPtr(dxgi.DXGISwapChainVTbl.ResizeTarget);

            if (OrigAddr != IntPtr.Zero)
            {

                Engine = new HookEngine();
                ResizeTarget_orig = Engine.CreateHook(OrigAddr, new ResizeTargetDelegate(ResizeTarget_Detour));
                Engine.EnableHooks();

            }
            else { throw new Exception("The corresponding Address is not found in the vTable"); }


        }

        public virtual int ResizeTarget_Detour(IntPtr swapChainPtr, ref ModeDescription newTargetParameters)
        {
            OnPreResizeTarget?.Invoke(swapChainPtr, ref newTargetParameters);
            int Result = ResizeTarget_orig(swapChainPtr, ref newTargetParameters);
            OnPostResizeTarget?.Invoke(swapChainPtr, ref newTargetParameters);

            return Result;
        }

        public void Uninstall()
        {
            Engine?.Dispose();
        }

    }
}
