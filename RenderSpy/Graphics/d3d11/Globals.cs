using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Graphics.d3d11
{
    public class Globals
    {
        const int D3D11_DEVICE_METHOD_COUNT = 43;

        public static IntPtr GetFunctionPtr(dxgi.DXGISwapChainVTbl FunctionPointer )
        {
            IntPtr OrigAddrPtr = IntPtr.Zero;

            SharpDX.Direct3D11.Device device;
            using (SharpDX.Windows.RenderForm renderForm = new SharpDX.Windows.RenderForm())
            {

                SharpDX.Direct3D11.Device.CreateWithSwapChain(
                    DriverType.Hardware,
                    DeviceCreationFlags.None,
                    dxgi.Globals.CreateSwapChainDescription(renderForm.Handle),
                    out device,
                    out SwapChain swapChain);

                if (device != null && swapChain != null)
                {
                    using (swapChain)
                    {
                        OrigAddrPtr =   RenderSpy.Globals.Helpers.GetVTblAddresses(swapChain.NativePointer, dxgi.Globals.DXGI_SWAPCHAIN_METHOD_COUNT)[(int)FunctionPointer];
                    }
                }
                else
                {
                    throw new Exception("Hook: Device creation failed");
                }

            }

            return OrigAddrPtr;
        }

       


    }
}
