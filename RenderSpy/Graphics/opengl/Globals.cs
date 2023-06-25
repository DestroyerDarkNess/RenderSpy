using OpenGL;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Graphics.opengl
{
    [StructLayout(LayoutKind.Sequential)]
    public class GLVersion
    {
       public int major = 0;
       public int minor = 0;
    }

    internal class Globals
    {
     
        public delegate void glGetIntegervDelegate(uint pname, out int data);

        public const uint GL_VERSION = 0x1F02;
        public const uint GL_MAJOR_VERSION = 0x821B;
        public const uint GL_MINOR_VERSION = 0x821C;

        [DllImport("opengl32.dll")]  public static extern IntPtr GetString(uint name);
        [DllImport("opengl32.dll")]  public static extern IntPtr wglGetProcAddress(string procName);

        public static bool IsModernOpenGL()
        {
            GLVersion OpenGLVer = GetGLversion();
          
            if (OpenGLVer.major >= 2) //&& OpenGLVer.minor >= 2
            {
                return true;
            }
            else if (OpenGLVer.major < 2)
            {
                return false;
            }
            else
            {
                IntPtr versionPtr = GetString(GL_VERSION);
                string versionString = Marshal.PtrToStringAnsi(versionPtr).ToLower();

                bool IsOldOpenGL = versionString.StartsWith("1.");

                return !IsOldOpenGL; //versionString.StartsWith("2.") || versionString.StartsWith("3.") || versionString.StartsWith("4.") || versionString.StartsWith("opengl es");
            }
        }

        public static GLVersion GetGLversion() {

            GLVersion value = new GLVersion();

            IntPtr hModule = WinApi.GetModuleHandle("opengl32.dll");

            if (hModule == IntPtr.Zero)
            {
                IntPtr glGetIntegervPtr = WinApi.GetProcAddress(hModule, "glGetIntegerv");

                glGetIntegervDelegate glGetIntegerv = Marshal.GetDelegateForFunctionPointer<glGetIntegervDelegate>(glGetIntegervPtr);

                glGetIntegerv(GL_MAJOR_VERSION, out value.major);
                glGetIntegerv(GL_MINOR_VERSION, out value.minor);

            }

            return value;
        }

    }
}
