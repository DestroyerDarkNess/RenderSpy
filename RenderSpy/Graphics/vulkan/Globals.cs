using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Graphics.vulkan
{
    public class Globals
    {

        [DllImport("vulkan-1.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr vkGetInstanceProcAddr(IntPtr instance, string pName);




    }
}
