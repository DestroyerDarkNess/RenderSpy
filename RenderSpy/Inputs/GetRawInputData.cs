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
    public class GetRawInputData : IHook
    {
        
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int GetRawInputDataDelegate(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        HookEngine Engine;
        GetRawInputDataDelegate Hook_orig;

        public event GetRawInputDataDelegate WindowProc;

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
            Hook_orig = Engine.CreateHook("user32.dll", "GetRawInputData", new GetRawInputDataDelegate(GetRawInputData_Detour));
            Engine.EnableHooks();

        }

        public virtual int GetRawInputData_Detour(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader)
        {
           
            WindowProc?.Invoke(hRawInput, uiCommand, pData, ref pcbSize, cbSizeHeader);
            
            if (BlockInput == true) { return 0; } else { return Hook_orig(hRawInput, uiCommand, pData, ref pcbSize, cbSizeHeader); }
        }

        public System.Windows.Forms.Message GetLastMessage() {
            System.Windows.Forms.Message message;
            WinApi.GetMessage(out message, Handle, 0, 0);
            return message;
        }

        public void Uninstall()
        {
            Engine?.Dispose();
        }

    }
}
