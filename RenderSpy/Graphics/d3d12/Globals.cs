
using System;
using System.Diagnostics;
using System.Windows.Forms;
using SharpDX.Direct3D12;
using SharpDX.DXGI;

namespace RenderSpy.Graphics.d3d12
{
    public class Globals
    {
    
        public static IntPtr GetFunctionPtr(dxgi.DXGISwapChainVTbl FunctionPointer)
        {
            IntPtr OrigAddrPtr = IntPtr.Zero;

                var device = new SharpDX.Direct3D12.Device(null, SharpDX.Direct3D.FeatureLevel.Level_12_0);
                var commandQueue = device.CreateCommandQueue(new CommandQueueDescription(CommandListType.Direct));
                var swapChainDesc = new SwapChainDescription()
                {
                    BufferCount = 2,
                    ModeDescription = new ModeDescription(100, 100, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    Usage = Usage.RenderTargetOutput,
                    SwapEffect = SwapEffect.FlipDiscard,
                    OutputHandle = Process.GetCurrentProcess().MainWindowHandle,
                    //Flags = SwapChainFlags.None,
                    SampleDescription = new SampleDescription(1, 0),
                    IsWindowed = true
                };
                using (var factory = new Factory4())
                using (var swapChain = new SwapChain(factory, commandQueue, swapChainDesc))
                {
                    OrigAddrPtr = RenderSpy.Globals.Helpers.GetVTblAddresses(swapChain.NativePointer, dxgi.Globals.DXGI_SWAPCHAIN_METHOD_COUNT)[(int)FunctionPointer];
                }

            return OrigAddrPtr;
        }


    }
}
