using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Graphics.d3d9
{
    internal class Globals
    {
       
        public const int MaxIndexSize = 119;

        public static IntPtr GetFunctionPtr(Direct3DDevice9FunctionOrdinals FunctionPointer , Device GlobalDevice = null)
        {
            IntPtr OrigAddrPtr = IntPtr.Zero;

            using (Direct3D d3d = new Direct3D())
            {
                if (GlobalDevice == null) { GlobalDevice = new Device(d3d, 0, DeviceType.NullReference, IntPtr.Zero, CreateFlags.SoftwareVertexProcessing, new PresentParameters()); }

                IntPtr vTablePtr = GlobalDevice.NativePointer;
                OrigAddrPtr = Helpers.GetVTblAddresses(vTablePtr, MaxIndexSize)[(int)FunctionPointer];
            }

            return OrigAddrPtr;
        }

    }
}
