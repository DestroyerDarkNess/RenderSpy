using RenderSpy.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Inputs
{
    public class SetWindowSubclass : IHook
    {

        public delegate int SUBCLASSPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", EntryPoint = "SetWindowSubclass", SetLastError = true)]
        public static extern bool SetWindowSubclassEx(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern bool RemoveWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern int DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        private SUBCLASSPROC SubClassDelegate = null;

        public event Globals.WindowProcDelegate WindowProc;

        public bool BlockInput = false;

        public IntPtr Handle;

        public IntPtr WindowHandle   // property
        {
            get { return Handle; }   // get Handle
            set { Handle = value; }  // set Handle
        }

        public void Install()
        {
            if (SubClassDelegate == null)
            {
                SubClassDelegate = new SUBCLASSPROC(WindowSubClass);
                bool bReturn = SetWindowSubclassEx(Handle, SubClassDelegate, 0, 0);
                if (bReturn == false)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }

        }


        public virtual int WindowSubClass(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData)
        {
            WindowProc?.Invoke(hWnd, uMsg, wParam, lParam);
            if (BlockInput == true) { return 0; } else { return DefSubclassProc(hWnd, uMsg, wParam, lParam); }
        }

        public void Uninstall()
        {
            if (SubClassDelegate != null)
            {
                RemoveWindowSubclass(Handle, SubClassDelegate, 0);
                SubClassDelegate = null;
            }
        }


    }
}
