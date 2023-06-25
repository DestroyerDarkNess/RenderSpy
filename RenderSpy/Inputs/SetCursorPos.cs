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
    public class SetCursorPos : IHook
    {
      
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SetCursorPosDelegate(int x, int y);

        HookEngine Engine;

        SetCursorPosDelegate Hook_orig;

        public event SetCursorPosDelegate SetCursorPos_Event;

        public bool BlockInput = false;

        private IntPtr Handle;

        public IntPtr WindowHandle   // property
        {
            get { return Handle; }   // get Handle
            set { Handle = value; }  // set Handle
        }

        public void Install()
        {

            Engine = new HookEngine();
            Hook_orig = Engine.CreateHook("user32.dll", "SetCursorPos", new SetCursorPosDelegate(SetCursorPos_Detour));
            Engine.EnableHooks();

        }
        public virtual bool SetCursorPos_Detour(int x, int y)
        {
            SetCursorPos_Event?.Invoke(x,y);

            if (BlockInput == true) { return false; } else { return Hook_orig(x,y); }
        }


        public void Uninstall()
        {
            Engine?.Dispose();
        }

    }
}
