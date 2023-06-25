using MinHook;
using RenderSpy.Graphics.d3d9;
using RenderSpy.Interfaces;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static RenderSpy.Graphics.d3d9.EndScene;

namespace RenderSpy.Graphics.opengl
{
    public class wglSwapBuffers : IHook
    {

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        public delegate bool WglSwapBuffersFunc(IntPtr hdc);

        HookEngine Engine;
        WglSwapBuffersFunc wglSwapBuffers_orig;

        public event WglSwapBuffersFunc wglSwapBuffersEvent;

        public void Install()
        {
           
            Engine = new HookEngine();

            if (Globals.IsModernOpenGL() == true)  {

                IntPtr wglSwapBuffersAddress = Globals.wglGetProcAddress("wglSwapBuffers");
                wglSwapBuffers_orig = Engine.CreateHook(wglSwapBuffersAddress, new WglSwapBuffersFunc(wglSwapBuffers_Detour));

            }
            else {

                wglSwapBuffers_orig = Engine.CreateHook("opengl32.dll", "wglSwapBuffers", new WglSwapBuffersFunc(wglSwapBuffers_Detour));

            }

            Engine.EnableHooks();
          
        }

        public virtual bool wglSwapBuffers_Detour(IntPtr hdc)
        {
            wglSwapBuffersEvent?.Invoke(hdc);

            return wglSwapBuffers_orig(hdc);
        }

        public void Uninstall()
        {
            Engine?.Dispose();
        }



    }
}
