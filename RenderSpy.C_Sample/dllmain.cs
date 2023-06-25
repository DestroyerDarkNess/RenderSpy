using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.C_Sample
{
    public class dllmain
    {
        static IntPtr GameHandle;
        public static void EntryPoint()
        {
            while (GameHandle.ToInt32() == 0)
            {
                GameHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;  // Get Main Game Window Handle
            }

            WinApi.AllocConsole();

            Console.WriteLine("Game WindowHandle  -->> " + GameHandle.ToString());

            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                //Call Costura.AssemblyLoader.Attach(); Via Reflection.

                string nspace = "Costura";

                var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                        where t.IsClass && t.Namespace == nspace
                        select t;

                Type AssemblyLoader = null;

                foreach (Type t in q.ToList())
                {
                    if (t.Name == "AssemblyLoader")
                    {
                        AssemblyLoader = t;
                    }
                }

                if (AssemblyLoader == null)
                {
                    throw new Exception("AssemblyLoader Error");
                }
                else
                {
                    MethodInfo theMethod = AssemblyLoader.GetMethod("Attach");
                    theMethod.Invoke(null, null);
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("All Embed Libs Loaded!");
                }


            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Costura Error: " + ex.Message);
                Console.ReadKey();
            }

            try
            {

                if (WinApi.GetModuleHandle("d3d9.dll") != IntPtr.Zero)
                {
                    Graphics.d3d9.EndScene NewEndScene = new Graphics.d3d9.EndScene();
                    NewEndScene.Install();

                    NewEndScene.EndSceneEvent += (device) =>
                    {
                        Console.WriteLine("d3d9 EndScene Hooked! Device Address: " + device.ToString());
                        return 0;
                    };
                }
                else if (WinApi.GetModuleHandle("d3d10.dll") != IntPtr.Zero)
                {
                    Graphics.d3d10.Present NewPresent = new Graphics.d3d10.Present();
                    NewPresent.Install();

                    NewPresent.PresentEvent += (swapChainPtr, syncInterval, flags) =>
                    {
                        Console.WriteLine("d3d10 Present Hooked! swapChainPtr Address: " + swapChainPtr.ToString());
                        return 0;
                    };
                }
                else if (WinApi.GetModuleHandle("d3d11.dll") != IntPtr.Zero)
                {
                    Graphics.d3d11.Present NewPresent = new Graphics.d3d11.Present();
                    NewPresent.Install();

                    NewPresent.PresentEvent += (swapChainPtr, syncInterval, flags) =>
                    {
                        Console.WriteLine("d3d11 Present Hooked! swapChainPtr Address: " + swapChainPtr.ToString());
                        return 0;
                    };
                }
                else if (WinApi.GetModuleHandle("d3d12.dll") != IntPtr.Zero)
                {
                    // No Implement!!
                }
                else if (WinApi.GetModuleHandle("opengl32.dll") != IntPtr.Zero)
                {
                    Graphics.opengl.wglSwapBuffers NewSwapBuffers = new Graphics.opengl.wglSwapBuffers();
                    NewSwapBuffers.Install();

                    NewSwapBuffers.wglSwapBuffersEvent += (hdc) =>
                    {
                        Console.WriteLine("OpenGL wglSwapBuffers Hooked! SwapBuffers Address: " + hdc.ToString());
                        return true;
                    };
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            while (true) { }


        }


    }
}
