using SharpDX.Direct3D9;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RenderSpy.Graphics.d3d9
{
    public class Globals
    {
        public const int D3D9_DEVICE_METHOD_COUNT = 119;
        public const int D3D9Ex_DEVICE_METHOD_COUNT = 15;

        public static IntPtr GetFunctionPtr(Direct3DDevice9FunctionOrdinals FunctionPointer , Device GlobalDevice = null)
        {
            IntPtr OrigAddrPtr = IntPtr.Zero;

            using (Direct3D d3d = new Direct3D())
            {
                using (System.Windows.Forms.Form tempRender = new System.Windows.Forms.Form()) {

                    if (GlobalDevice == null) { GlobalDevice = new Device(d3d, 0, DeviceType.NullReference, tempRender.Handle, CreateFlags.SoftwareVertexProcessing | CreateFlags.DisableDriverManagement, GetPresentParameters(tempRender.Handle)); }
                    IntPtr vTablePtr = GlobalDevice.NativePointer;
                    OrigAddrPtr = RenderSpy.Globals.Helpers.GetVTblAddresses(vTablePtr, D3D9_DEVICE_METHOD_COUNT)[(int)FunctionPointer];

                }  
            }

            return OrigAddrPtr;
        }

        public static IntPtr GetFunctionPtrEx(Direct3DDevice9ExFunctionOrdinals FunctionPointer, Device GlobalDevice = null)
        {
            IntPtr OrigAddrPtr = IntPtr.Zero;

            using (Direct3DEx d3d = new Direct3DEx())
            {
                using (System.Windows.Forms.Form tempRender = new System.Windows.Forms.Form())
                {

                    if (GlobalDevice == null) { GlobalDevice = new DeviceEx(d3d, 0, DeviceType.NullReference, IntPtr.Zero, CreateFlags.SoftwareVertexProcessing | CreateFlags.DisableDriverManagement, GetPresentParameters(tempRender.Handle), new DisplayModeEx() { Width = 800, Height = 600 }); }
                    IntPtr vTablePtr = GlobalDevice.NativePointer;
                    OrigAddrPtr = RenderSpy.Globals.Helpers.GetVTblAddresses(vTablePtr, D3D9_DEVICE_METHOD_COUNT, D3D9Ex_DEVICE_METHOD_COUNT)[(int)FunctionPointer];

                }
            }

            return OrigAddrPtr;
        }

        public static PresentParameters GetPresentParameters(IntPtr window) {
            // from : https://github.com/Rebzzel/kiero/commit/35d4e4f7510f9123646f759beede172d22a85650
            PresentParameters D3DPRESENT_PARAMETERS = new PresentParameters();
            D3DPRESENT_PARAMETERS.BackBufferWidth = 0;
            D3DPRESENT_PARAMETERS.BackBufferHeight = 0;
            D3DPRESENT_PARAMETERS.BackBufferFormat = Format.Unknown;
            D3DPRESENT_PARAMETERS.BackBufferCount = 0;
            D3DPRESENT_PARAMETERS.MultiSampleType = MultisampleType.None;
            D3DPRESENT_PARAMETERS.SwapEffect = SwapEffect.Discard;
            D3DPRESENT_PARAMETERS.DeviceWindowHandle = window;
            D3DPRESENT_PARAMETERS.Windowed = true;
            D3DPRESENT_PARAMETERS.EnableAutoDepthStencil = false;
            D3DPRESENT_PARAMETERS.AutoDepthStencilFormat = Format.Unknown;
            D3DPRESENT_PARAMETERS.FullScreenRefreshRateInHz = 0;
            D3DPRESENT_PARAMETERS.PresentationInterval = 0;

            return D3DPRESENT_PARAMETERS;
        }

    }
}
