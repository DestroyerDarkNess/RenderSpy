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
        public  EndSceneDelegate EndScene_orig;
      
        public Device GlobalDevice = null;

        public event EndSceneDelegate EndSceneEvent;

        public void Install() {

            // On Windows 7 64-bit w/ 32-bit app and d3d9 dll version 6.1.7600.16385, the address is equiv to:
            // (IntPtr)(GetModuleHandle("d3d9").ToInt32() + 0x1ce09),
            // A 64-bit app would use 0xff18
            // Note: GetFunctionPtr will output these addresses to a log file
            OrigAddr = Globals.GetFunctionPtr(Direct3DDevice9FunctionOrdinals.EndScene, GlobalDevice);

          if (OrigAddr != IntPtr.Zero)  {

                Engine = new HookEngine();
                EndScene_orig = Engine.CreateHook(OrigAddr, new EndSceneDelegate(EndScene_Detour));
                Engine.EnableHooks();
          
          } else { throw new Exception("The corresponding Address is not found in the vTable"); }

          
        }

        public virtual int EndScene_Detour(IntPtr device)
        {
            if (EndSceneEvent != null)
            {
                int Result = EndSceneEvent.Invoke(device);
                return Result;
            }
            else { return EndScene_orig(device); }

        }

        public void Uninstall()
        {
            Engine?.Dispose();
        }

       
    }
}
