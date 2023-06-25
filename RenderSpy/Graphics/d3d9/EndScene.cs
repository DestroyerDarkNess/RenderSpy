using MinHook;
using RenderSpy.Interfaces;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Graphics.d3d9
{
    public class EndScene : IHook
    {

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int EndSceneDelegate(IntPtr device);

        IntPtr OrigAddr = IntPtr.Zero;
        HookEngine Engine;
        EndSceneDelegate EndScene_orig;
      
        public Device GlobalDevice = null;

        public event EndSceneDelegate EndSceneEvent;

        public void Install() {

          OrigAddr = Globals.GetFunctionPtr(Direct3DDevice9FunctionOrdinals.EndScene, GlobalDevice);

          if (OrigAddr != IntPtr.Zero)  {

                Engine = new HookEngine();
                EndScene_orig = Engine.CreateHook(OrigAddr, new EndSceneDelegate(EndScene_Detour));
                Engine.EnableHooks();
          
          } else { throw new Exception("The corresponding Address is not found in the vTable"); }

          
        }

        public virtual int EndScene_Detour(IntPtr device)
        {
            EndSceneEvent?.Invoke(device);

            return EndScene_orig(device);
        }

        public void Uninstall()
        {
            Engine?.Dispose();
        }

       
    }
}
