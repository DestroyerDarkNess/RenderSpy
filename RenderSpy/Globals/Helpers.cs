using RenderSpy.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Globals
{
    public class Helpers
    {
        public static bool IsValidAPI(string dllname) { return (WinApi.GetModuleHandle(dllname) != IntPtr.Zero); }

        public static IntPtr[] GetVTblAddresses(IntPtr pointer, int numberOfMethods)
        {
            return GetVTblAddresses(pointer, 0, numberOfMethods);
        }

        public static IntPtr[] GetVTblAddresses(IntPtr pointer, int startIndex, int numberOfMethods)
        {
            List<IntPtr> vtblAddresses = new List<IntPtr>();

            IntPtr vTable = Marshal.ReadIntPtr(pointer);
            for (int i = startIndex; i < startIndex + numberOfMethods; i++)
                vtblAddresses.Add(Marshal.ReadIntPtr(vTable, i * IntPtr.Size));

            return vtblAddresses.ToArray();
        }

        public static SharpDXSprite GetSharpDXSprite(ArchSpriteLib arq) {

            SharpDXSprite Result = null;

            switch (arq)
            {
                case ArchSpriteLib.x84:
                    Result = new SharpDXSprite("sharpdx_direct3d11_1_effects_x86.dll", RenderSpy.Properties.Resources.sharpdx_direct3d11_1_effects_x86);
                    break;
                case ArchSpriteLib.x64:
                    Result = new SharpDXSprite("sharpdx_direct3d11_1_effects_x64.dll", RenderSpy.Properties.Resources.sharpdx_direct3d11_1_effects_x64);
                    break;
                case ArchSpriteLib.arm:
                    Result = new SharpDXSprite("sharpdx_direct3d11_1_effects_arm.dll", RenderSpy.Properties.Resources.sharpdx_direct3d11_1_effects_arm);
                    break;
                case ArchSpriteLib.auto:

                    if (IntPtr.Size != 4)
                    {
                        Result = new SharpDXSprite("sharpdx_direct3d11_1_effects_x64.dll", RenderSpy.Properties.Resources.sharpdx_direct3d11_1_effects_x64);
                    }
                    else
                    {
                        Result = new SharpDXSprite("sharpdx_direct3d11_1_effects_x86.dll", RenderSpy.Properties.Resources.sharpdx_direct3d11_1_effects_x86);
                    }

                    break;
                default:

                    break;
            }

            return Result;

        }

    }

    public enum ArchSpriteLib { 
         x84 = 0, x64 = 1, arm = 2, auto = 3
    }

    public class SharpDXSprite {

        public SharpDXSprite(string name, Byte[] DataArray) { LibName = name; LibBytes = DataArray; }

        public Byte[] LibBytes;
        public string LibName;


    
    }



}
