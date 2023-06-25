using SharpDX.Direct3D10;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Graphics.d3d10
{
    internal class Globals
    {

        public const int D3D10_DEVICE_METHOD_COUNT = 98;

        public static IntPtr GetFunctionPtr(dxgi.DXGISwapChainVTbl FunctionPointer , SharpDX.Direct3D10.Device GlobalDevice = null)
        {
            IntPtr OrigAddrPtr = IntPtr.Zero;

            using (var factory = new Factory(IntPtr.Zero))
            {
                if (GlobalDevice == null) { GlobalDevice = new SharpDX.Direct3D10.Device(factory.GetAdapter(0), DeviceCreationFlags.None); }

                    using (var renderForm = new System.Windows.Forms.Form())
                    {
                        using (SharpDX.DXGI.SwapChain SwapChainEx = new SharpDX.DXGI.SwapChain(factory, GlobalDevice, dxgi.Globals.CreateSwapChainDescription(renderForm.Handle)))
                        {
                        IntPtr vTablePtr = SwapChainEx.NativePointer;
                        OrigAddrPtr = Helpers.GetVTblAddresses(SwapChainEx.NativePointer, dxgi.Globals.DXGI_SWAPCHAIN_METHOD_COUNT)[(int)FunctionPointer];
                        }
                    }
                
            }


            return OrigAddrPtr;
        }

       

    }
}
