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

        public static IntPtr GetFunctionPtr(dxgi.DXGISwapChainVTbl FunctionPointer , SharpDX.Direct3D11.Device deviceEx = null, SharpDX.DXGI.SwapChain swapChainEx = null)
        {
            IntPtr OrigAddrPtr = IntPtr.Zero;

            SharpDX.Direct3D11.Device device = deviceEx;
            SharpDX.DXGI.SwapChain swapChain = swapChainEx;
            
            if (device == null && swapChain == null)
            {
                using (SharpDX.Windows.RenderForm renderForm = new SharpDX.Windows.RenderForm())
                {

                    SharpDX.Direct3D11.Device.CreateWithSwapChain(
                        DriverType.Hardware,
                        DeviceCreationFlags.None,
                        dxgi.Globals.CreateSwapChainDescription(renderForm.Handle),
                        out device,
                        out swapChain);

                    using (swapChain)
                    {
                        OrigAddrPtr = RenderSpy.Globals.Helpers.GetVTblAddresses(swapChain.NativePointer, dxgi.Globals.DXGI_SWAPCHAIN_METHOD_COUNT)[(int)FunctionPointer];
                    }

                }
            }
            else
            {
                using (swapChain)
                {
                    OrigAddrPtr = RenderSpy.Globals.Helpers.GetVTblAddresses(swapChain.NativePointer, dxgi.Globals.DXGI_SWAPCHAIN_METHOD_COUNT)[(int)FunctionPointer];
                }
            }

            return OrigAddrPtr;
        }

       


    }
}
