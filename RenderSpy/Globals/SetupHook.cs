using MinHook;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Globals
{
    public class SetupHook
    {
       public HookEngine HookEngine;

        public SetupHook() {

        }

        public Delegate Install(IntPtr OrigAddr, Delegate FunctionTarget) {

            if (OrigAddr != IntPtr.Zero)
            {

                HookEngine = new HookEngine();
                Delegate DelegateFunc = HookEngine.CreateHook(OrigAddr, FunctionTarget);
                HookEngine.EnableHooks();
                return DelegateFunc;

            }
            else { throw new Exception("Address not found: " + OrigAddr.ToString()); }

        }

        public void Uninstall()
        {
            HookEngine?.Dispose();
        }

    }
}
