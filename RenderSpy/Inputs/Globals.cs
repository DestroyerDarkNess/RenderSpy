using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Inputs
{
    public class Globals
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        public delegate IntPtr WindowProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public Int32 X;
            public Int32 Y;
        }
    }
}
