using RenderSpy.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Graphics
{
    public enum GraphicsType
    {
        d3d9 = 0, d3d10 = 1, d3d11 = 2, d3d12 = 3, 
        opengl = 4, 
        vulkan = 5, 
        gdi = 6, gdiplus = 7,
        unknown = 8
    }

    public class Detector
    {

        public static bool IsValidAPI(GraphicsType GT) { return (WinApi.GetModuleHandle(GetLibByEnum(GT)) != IntPtr.Zero); }

        public static GraphicsType GetCurrentGraphicsType() {

            if (WinApi.GetModuleHandle(GetLibByEnum(GraphicsType.d3d9)) != IntPtr.Zero) { return GraphicsType.d3d9; }
            else if (WinApi.GetModuleHandle(GetLibByEnum(GraphicsType.d3d10)) != IntPtr.Zero) { return GraphicsType.d3d10; }
            else if (WinApi.GetModuleHandle(GetLibByEnum(GraphicsType.d3d11)) != IntPtr.Zero) { return GraphicsType.d3d11; }
            else if (WinApi.GetModuleHandle(GetLibByEnum(GraphicsType.d3d12)) != IntPtr.Zero) { return GraphicsType.d3d12; }
            else if (WinApi.GetModuleHandle(GetLibByEnum(GraphicsType.opengl)) != IntPtr.Zero) { return GraphicsType.opengl; }
            else if (WinApi.GetModuleHandle(GetLibByEnum(GraphicsType.vulkan)) != IntPtr.Zero) { return GraphicsType.vulkan; }
            else if (WinApi.GetModuleHandle(GetLibByEnum(GraphicsType.gdi)) != IntPtr.Zero) { return GraphicsType.gdi; }
            else if (WinApi.GetModuleHandle(GetLibByEnum(GraphicsType.gdiplus)) != IntPtr.Zero) { return GraphicsType.gdiplus; }
            else { return GraphicsType.unknown; }
        }


        public static string GetLibByEnum(GraphicsType GT) {
          string Result = "";
            switch (GT)
            {
                case GraphicsType.d3d9:
                    Result =  "d3d9.dll";
                    break;
                case GraphicsType.d3d10:
                    Result = "d3d10.dll";
                    break;
                case GraphicsType.d3d11:
                    Result = "d3d11.dll";
                    break;
                case GraphicsType.d3d12:
                    Result = "d3d12.dll";
                    break;
                case GraphicsType.opengl:
                    Result = "opengl32.dll";
                    break;
                case GraphicsType.vulkan:
                    Result = "vulkan-1.dll";
                    break;
                case GraphicsType.gdi:
                    Result = "gdi32.dll";
                    break;
                case GraphicsType.gdiplus:
                    Result = "gdiplus.dll";
                    break;
                default:

                    break;
            }
            return Result;
        }



    }
}
