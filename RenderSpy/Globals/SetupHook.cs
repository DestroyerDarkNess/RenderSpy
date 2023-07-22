using MinHook;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RenderSpy.Globals
{
    public class SetupHook : IDisposable
    {
        private HookEngine HookEngine;

        public HookEngine Hook 
        {
            get { return HookEngine; }  
        }

        public Delegate DelegateOrig_Func;


        public SetupHook(IntPtr OrigAddr, Delegate FunctionTarget ) {
            if (OrigAddr != IntPtr.Zero)
            {

                HookEngine = new HookEngine();
                DelegateOrig_Func  = HookEngine.CreateHook(OrigAddr, FunctionTarget);
                HookEngine.EnableHooks();

            }
            else { throw new Exception("Address not found: " + OrigAddr.ToString()); }
        }

        #region IDisposable implementation with finalizer
        private bool isDisposed = false;
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    if (HookEngine != null) HookEngine.Dispose();
                }
            }
            isDisposed = true;
        }
        #endregion

    }
}
