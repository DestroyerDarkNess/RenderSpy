using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Graphics.opengl
{
    [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
    public class OldSDK
    {
        public const uint GL_COLOR_BUFFER_BIT = 0x00004000;
        public const uint GL_DEPTH_BUFFER_BIT = 0x00000100;

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [System.Runtime.InteropServices.DllImport("opengl32.dll", SetLastError = true)]
        public static extern void glClear(uint mask);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [System.Runtime.InteropServices.DllImport("opengl32.dll")]
        public static extern bool wglSwapBuffers(IntPtr hdc);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "wglGetCurrentContext")]
        public static extern IntPtr GetCurrentContext();

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glDrawArrays")]
        public static extern void DrawArrays(BeginMode mode, int first, int count);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glGetIntegerv")]
        public static extern void GetInteger(Target target, int[] parameters);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("opengl32.dll")]
        public static extern void glGetFloatv(Target target, float[] parameters);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glScissor")]
        public static extern void Scissor(int x, int y, int width, int height);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glGetString")]
        private static extern IntPtr GetStringInner(uint name);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        public static string GetString(uint name)
        {
            ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

            string output = Marshal.PtrToStringAnsi(GetStringInner(name));

            return output;
        }

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glPixelStorei")]
        public static extern void PixelStore(uint pname, int param);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glDeleteTextures")]
        public static extern void DeleteTextures(int n, int[] textures);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glBlendFunc")]
        public static extern void BlendFunc(BlendingSourceFactor sfactor, BlendingDestinationFactor dfactor);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glViewport")]
        public static extern void Viewport(int x, int y, int width, int height);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glMultMatrixf")]
        public static extern void MultMatrix(float[] matrix);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glLoadMatrixf")]
        public static extern void LoadMatrix(float[] matrix);

        //[DllImport("Opengl32.dll", EntryPoint = "glLoadMatrixf")]
        //public static extern void LoadMatrix(ref Matrix4x4 matrix);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glColorMaterial")]
        public static extern void ColorMaterial(uint face, uint mode);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glTexCoordPointer")]
        public static extern void TexCoordPointer(int size, DataType type, int stride, float[] vertexData);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glVertexPointer")]
        public static extern void VertexPointer(int size, DataType type, int stride, float[] vertexData);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glNormalPointer")]
        public static extern void NormalPointer(DataType type, int stride, float[] normalData);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glDrawElements")]
        public static extern void DrawElements(uint mode, int count, DataType type, uint[] indices);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glDisableClientState")]
        public static extern void DisableClientState(uint array);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glEnableClientState")]
        public static extern void EnableClientState(uint array);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glTranslatef")]
        public static extern void Translate(float x, float y, float z);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glLightfv")]
        public static extern void Lightfv(uint light, uint pname, float[] parameters);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glHint")]
        public static extern void Hint(uint target, uint mode);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glMatrixMode")]
        public static extern void MatrixMode(MatrixMode mode);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glDepthFunc")]
        public static extern void DepthFunc(uint mode);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glShadeModel")]
        public static extern void ShadeModel(ShadingModel func);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glClearDepth")]
        public static extern void ClearDepth(double depth);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glPopMatrix")]
        public static extern void PopMatrix();

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glPushMatrix")]
        public static extern void PushMatrix();

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glRotated")]
        public static extern void Rotate(double angle, double x, double y, double z);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glRotatef")]
        public static extern void Rotate(float angle, float x, float y, float z);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glScaled")]
        public static extern void Scale(double x, double y, double z);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glScalef")]
        public static extern void Scale(float x, float y, float z);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glLoadIdentity")]
        public static extern void LoadIdentity();

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glClear")]
        public static extern void Clear(AttribueMask mask);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glBegin")]
        public static extern void Begin(BeginMode mode);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glEnd")]
        public static extern void End();

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glVertex2i")]
        public static extern void Vertex(int x, int y);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glVertex2f")]
        public static extern void Vertexf(float x, float y);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glVertex3f")]
        public static extern void Vertex(float x, float y, float z);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glColor3f")]
        public static extern void Color(float red, float green, float blue);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glColor4f")]
        public static extern void Color(float red, float green, float blue, float alpha);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glClearColor")]
        public static extern void ClearColor(float red, float green, float blue, float alpha);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glTexImage2D")]
        public static extern void TexImage2D(Target target, int level, uint internalformat, int width, int height, int border, uint format, DataType type, IntPtr pixels);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glTexImage2D")]
        public static extern void TexImage2D(Target target, int level, uint internalformat, int width, int height, int border, uint format, DataType type, byte[] pixels);

            [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glGenTextures")]
        public static extern void GenTextures(int size, ref int textures);
            
        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glBindTexture")]
        public static extern void BindTexture(Target target, int texture);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glEnable")]
        public static extern void Enable(Target cap);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("opengl32.dll")]
        public static extern void glRasterPos2f(float x, float y);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("opengl32.dll")]
        public static extern bool wglUseFontBitmaps(IntPtr hDC, uint first, uint count, uint listBase);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("opengl32.dll")]
        public static extern void glCallLists(int count, uint type, IntPtr lists);


        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glDisable")]
        public static extern void Disable(Target cap);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glTexCoord2f")]
        public static extern void TexCoord(float s, float t);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glTexParameteri")]
        public static extern void TexParameteri(Target target, TextureParameterName pname, int param);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glGetError")]
        public static extern uint GetError();

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glFlush")]
        public static extern void Flush();

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll", EntryPoint = "glFinish")]
        public static extern void Finish();

        #region Windows Specific

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll")]
        public static extern IntPtr wglCreateContext(IntPtr hdc);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("Opengl32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool wglMakeCurrent(IntPtr hdc, IntPtr hglrc);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("opengl32.dll")]
        public static extern IntPtr wglGetProcAddress(string name);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("opengl32.dll")]
        public static extern bool wglDeleteContext(IntPtr hglrc);

        [Obsolete("Please use the class: RenderSpy.Rendering.OpenGL.OpenGLInterops")]
        [DllImport("opengl32.dll")]
        public static extern void glOrtho(double left, double right, double bottom, double top, double zNear, double zFar);

        #endregion
    }

    public enum MatrixMode : uint
    {
        ModelView = 0x1700,
        Projection = 0x1701,
        Texture = 0x1702
    }

    public enum TextureParameterName : uint
    {
        TextureMagFilter = 0x2800,
        TextureMinFilter = 0x2801,
        TextureWrapS = 0x2802,
        TextureWrapT = 0x2803
    }

    public enum TextureMagFilter : int
    {
        Nearest = 0x2600,
        Linear = 0x2601
    }

    public enum TextureWrapParameter : int
    {
        ClampToEdge = 0x812F,
        MirroredRepeat = 0x8370,
        Repeat = 0x2901
    }

    public enum Target : uint
    {
        CurrentColor = 0x0B00,
        CurrentIndex = 0x0B01,
        CurrentNormal = 0x0B02,
        CURRENT_TEXTURE_COORDS = 0x0B03,
        CURRENT_RASTER_COLOR = 0x0B04,
        CURRENT_RASTER_INDEX = 0x0B05,
        CURRENT_RASTER_TEXTURE_COORDS = 0x0B06,
        CURRENT_RASTER_POSITION = 0x0B07,
        CURRENT_RASTER_POSITION_VALID = 0x0B08,
        CURRENT_RASTER_DISTANCE = 0x0B09,
        POINT_SMOOTH = 0x0B10,
        POINT_SIZE = 0x0B11,
        POINT_SIZE_RANGE = 0x0B12,
        POINT_SIZE_GRANULARITY = 0x0B13,
        LINE_SMOOTH = 0x0B20,
        LINE_WIDTH = 0x0B21,
        LINE_WIDTH_RANGE = 0x0B22,
        LINE_WIDTH_GRANULARITY = 0x0B23,
        LINE_STIPPLE = 0x0B24,
        LINE_STIPPLE_PATTERN = 0x0B25,
        LINE_STIPPLE_REPEAT = 0x0B26,
        LIST_MODE = 0x0B30,
        MAX_LIST_NESTING = 0x0B31,
        LIST_BASE = 0x0B32,
        LIST_INDEX = 0x0B33,
        POLYGON_MODE = 0x0B40,
        POLYGON_SMOOTH = 0x0B41,
        POLYGON_STIPPLE = 0x0B42,
        EDGE_FLAG = 0x0B43,
        CULL_FACE = 0x0B44,
        CULL_FACE_MODE = 0x0B45,
        FRONT_FACE = 0x0B46,
        Lighting = 0x0B50,
        LIGHT_MODEL_LOCAL_VIEWER = 0x0B51,
        LIGHT_MODEL_TWO_SIDE = 0x0B52,
        LIGHT_MODEL_AMBIENT = 0x0B53,
        ShadeModel = 0x0B54,
        COLOR_MATERIAL_FACE = 0x0B55,
        COLOR_MATERIAL_PARAMETER = 0x0B56,
        COLOR_MATERIAL = 0x0B57,
        Fog = 0x0B60,
        FOG_INDEX = 0x0B61,
        FOG_DENSITY = 0x0B62,
        FOG_START = 0x0B63,
        FOG_END = 0x0B64,
        FOG_MODE = 0x0B65,
        FOG_COLOR = 0x0B66,
        DEPTH_RANGE = 0x0B70,
        DepthTest = 0x0B71,
        DEPTH_WRITEMASK = 0x0B72,
        DEPTH_CLEAR_VALUE = 0x0B73,
        DEPTH_FUNC = 0x0B74,
        ACCUM_CLEAR_VALUE = 0x0B80,
        STENCIL_TEST = 0x0B90,
        STENCIL_CLEAR_VALUE = 0x0B91,
        STENCIL_FUNC = 0x0B92,
        STENCIL_VALUE_MASK = 0x0B93,
        STENCIL_FAIL = 0x0B94,
        STENCIL_PASS_DEPTH_FAIL = 0x0B95,
        STENCIL_PASS_DEPTH_PASS = 0x0B96,
        STENCIL_REF = 0x0B97,
        STENCIL_WRITEMASK = 0x0B98,
        MatrixMode = 0x0BA0,
        Normalize = 0x0BA1,
        VIEWPORT = 0x0BA2,
        MODELVIEW_STACK_DEPTH = 0x0BA3,
        PROJECTION_STACK_DEPTH = 0x0BA4,
        TEXTURE_STACK_DEPTH = 0x0BA5,
        MODELVIEW_MATRIX = 0x0BA6,
        PROJECTION_MATRIX = 0x0BA7,
        TEXTURE_MATRIX = 0x0BA8,
        ATTRIB_STACK_DEPTH = 0x0BB0,
        CLIENT_ATTRIB_STACK_DEPTH = 0x0BB1,
        AlphaTest = 0x0BC0,
        ALPHA_TEST_FUNC = 0x0BC1,
        ALPHA_TEST_REF = 0x0BC2,
        DITHER = 0x0BD0,
        BLEND_DST = 0x0BE0,
        BLEND_SRC = 0x0BE1,
        Blend = 0x0BE2,
        LOGIC_OP_MODE = 0x0BF0,
        INDEX_LOGIC_OP = 0x0BF1,
        COLOR_LOGIC_OP = 0x0BF2,
        AUX_BUFFERS = 0x0C00,
        DRAW_BUFFER = 0x0C01,
        READ_BUFFER = 0x0C02,
        SCISSOR_BOX = 0x0C10,
        SCISSOR_TEST = 0x0C11,
        INDEX_CLEAR_VALUE = 0x0C20,
        INDEX_WRITEMASK = 0x0C21,
        COLOR_CLEAR_VALUE = 0x0C22,
        COLOR_WRITEMASK = 0x0C23,
        INDEX_MODE = 0x0C30,
        RGBA_MODE = 0x0C31,
        DOUBLEBUFFER = 0x0C32,
        STEREO = 0x0C33,
        RENDER_MODE = 0x0C40,
        PERSPECTIVE_CORRECTION_HINT = 0x0C50,
        POINT_SMOOTH_HINT = 0x0C51,
        LINE_SMOOTH_HINT = 0x0C52,
        POLYGON_SMOOTH_HINT = 0x0C53,
        FOG_HINT = 0x0C54,
        TEXTURE_GEN_S = 0x0C60,
        TEXTURE_GEN_T = 0x0C61,
        TEXTURE_GEN_R = 0x0C62,
        TEXTURE_GEN_Q = 0x0C63,
        PIXEL_MAP_I_TO_I = 0x0C70,
        PIXEL_MAP_S_TO_S = 0x0C71,
        PIXEL_MAP_I_TO_R = 0x0C72,
        PIXEL_MAP_I_TO_G = 0x0C73,
        PIXEL_MAP_I_TO_B = 0x0C74,
        PIXEL_MAP_I_TO_A = 0x0C75,
        PIXEL_MAP_R_TO_R = 0x0C76,
        PIXEL_MAP_G_TO_G = 0x0C77,
        PIXEL_MAP_B_TO_B = 0x0C78,
        PIXEL_MAP_A_TO_A = 0x0C79,
        PIXEL_MAP_I_TO_I_SIZE = 0x0CB0,
        PIXEL_MAP_S_TO_S_SIZE = 0x0CB1,
        PIXEL_MAP_I_TO_R_SIZE = 0x0CB2,
        PIXEL_MAP_I_TO_G_SIZE = 0x0CB3,
        PIXEL_MAP_I_TO_B_SIZE = 0x0CB4,
        PIXEL_MAP_I_TO_A_SIZE = 0x0CB5,
        PIXEL_MAP_R_TO_R_SIZE = 0x0CB6,
        PIXEL_MAP_G_TO_G_SIZE = 0x0CB7,
        PIXEL_MAP_B_TO_B_SIZE = 0x0CB8,
        PIXEL_MAP_A_TO_A_SIZE = 0x0CB9,
        UNPACK_SWAP_BYTES = 0x0CF0,
        UNPACK_LSB_FIRST = 0x0CF1,
        UNPACK_ROW_LENGTH = 0x0CF2,
        UNPACK_SKIP_ROWS = 0x0CF3,
        UNPACK_SKIP_PIXELS = 0x0CF4,
        UNPACK_ALIGNMENT = 0x0CF5,
        PACK_SWAP_BYTES = 0x0D00,
        PACK_LSB_FIRST = 0x0D01,
        PACK_ROW_LENGTH = 0x0D02,
        PACK_SKIP_ROWS = 0x0D03,
        PACK_SKIP_PIXELS = 0x0D04,
        PACK_ALIGNMENT = 0x0D05,
        MAP_COLOR = 0x0D10,
        MAP_STENCIL = 0x0D11,
        INDEX_SHIFT = 0x0D12,
        INDEX_OFFSET = 0x0D13,
        RED_SCALE = 0x0D14,
        RED_BIAS = 0x0D15,
        ZOOM_X = 0x0D16,
        ZOOM_Y = 0x0D17,
        GREEN_SCALE = 0x0D18,
        GREEN_BIAS = 0x0D19,
        BLUE_SCALE = 0x0D1A,
        BLUE_BIAS = 0x0D1B,
        ALPHA_SCALE = 0x0D1C,
        ALPHA_BIAS = 0x0D1D,
        DEPTH_SCALE = 0x0D1E,
        DEPTH_BIAS = 0x0D1F,
        MAX_EVAL_ORDER = 0x0D30,
        MAX_LIGHTS = 0x0D31,
        MAX_CLIP_PLANES = 0x0D32,
        MAX_TEXTURE_SIZE = 0x0D33,
        MAX_PIXEL_MAP_TABLE = 0x0D34,
        MAX_ATTRIB_STACK_DEPTH = 0x0D35,
        MAX_MODELVIEW_STACK_DEPTH = 0x0D36,
        MAX_NAME_STACK_DEPTH = 0x0D37,
        MAX_PROJECTION_STACK_DEPTH = 0x0D38,
        MAX_TEXTURE_STACK_DEPTH = 0x0D39,
        MAX_VIEWPORT_DIMS = 0x0D3A,
        MAX_CLIENT_ATTRIB_STACK_DEPTH = 0x0D3B,
        SUBPIXEL_BITS = 0x0D50,
        INDEX_BITS = 0x0D51,
        RED_BITS = 0x0D52,
        GREEN_BITS = 0x0D53,
        BLUE_BITS = 0x0D54,
        ALPHA_BITS = 0x0D55,
        DEPTH_BITS = 0x0D56,
        STENCIL_BITS = 0x0D57,
        ACCUM_RED_BITS = 0x0D58,
        ACCUM_GREEN_BITS = 0x0D59,
        ACCUM_BLUE_BITS = 0x0D5A,
        ACCUM_ALPHA_BITS = 0x0D5B,
        NAME_STACK_DEPTH = 0x0D70,
        AUTO_NORMAL = 0x0D80,
        MAP1_COLOR_4 = 0x0D90,
        MAP1_INDEX = 0x0D91,
        MAP1_NORMAL = 0x0D92,
        MAP1_TEXTURE_COORD_1 = 0x0D93,
        MAP1_TEXTURE_COORD_2 = 0x0D94,
        MAP1_TEXTURE_COORD_3 = 0x0D95,
        MAP1_TEXTURE_COORD_4 = 0x0D96,
        MAP1_VERTEX_3 = 0x0D97,
        MAP1_VERTEX_4 = 0x0D98,
        MAP2_COLOR_4 = 0x0DB0,
        MAP2_INDEX = 0x0DB1,
        MAP2_NORMAL = 0x0DB2,
        MAP2_TEXTURE_COORD_1 = 0x0DB3,
        MAP2_TEXTURE_COORD_2 = 0x0DB4,
        MAP2_TEXTURE_COORD_3 = 0x0DB5,
        MAP2_TEXTURE_COORD_4 = 0x0DB6,
        MAP2_VERTEX_3 = 0x0DB7,
        MAP2_VERTEX_4 = 0x0DB8,
        MAP1_GRID_DOMAIN = 0x0DD0,
        MAP1_GRID_SEGMENTS = 0x0DD1,
        MAP2_GRID_DOMAIN = 0x0DD2,
        MAP2_GRID_SEGMENTS = 0x0DD3,
        Texture1D = 0x0DE0,
        Texture2D = 0x0DE1,
        FEEDBACK_BUFFER_POINTER = 0x0DF0,
        FEEDBACK_BUFFER_SIZE = 0x0DF1,
        FEEDBACK_BUFFER_TYPE = 0x0DF2,
        SELECTION_BUFFER_POINTER = 0x0DF3,
        SELECTION_BUFFER_SIZE = 0x0DF4
    }

    public enum DataType : uint
    {
        Byte = 0x1400,
        UnsignedByte = 0x1401,
        Short = 0x1402,
        UnsignedShor = 0x1403,
        Int = 0x1404,
        UnsignedInt = 0x1405,
        Float = 0x1406,
        TwoBytes = 0x1407,
        ThreeBytes = 0x1408,
        FourBytes = 0x1409,
        Double = 0x140A
    }

    public enum TextureInternalFormat : uint
    {
        Alpha4 = 0x803B,
        Alpha8 = 0x803C,
        Alpha12 = 0x803D,
        Alpha16 = 0x803E,
        Luminance4 = 0x803F,
        Luminance8 = 0x8040,
        Luminance12 = 0x8041,
        Luminance6 = 0x8042,
        Luminance4Alpha4 = 0x8043,
        Luminance6Alpha2 = 0x8044,
        Luminance8Alpha8 = 0x8045,
        Luminance12Alpha4 = 0x8046,
        Luminance12Alpha12 = 0x8047,
        Luminance16Alpha16 = 0x8048,
        Intensity = 0x8049,
        Intensity4 = 0x804A,
        Intensity8 = 0x804B,
        Intensity12 = 0x804C,
        Intensity16 = 0x804D,
        R3G3B2 = 0x2A10,
        RGB4 = 0x804F,
        RGB5 = 0x8050,
        RGB8 = 0x8051,
        RGB10 = 0x8052,
        RGB12 = 0x8053,
        RGB16 = 0x8054,
        RGBA2 = 0x8055,
        RGBA4 = 0x8056,
        RGB5A1 = 0x8057,
        RGBA8 = 0x8058,
        RGB10A2 = 0x8059,
        RGBA12 = 0x805A,
        RGBA16 = 0x805B,
        TextureRedSize = 0x805C,
        TextureGreenSize = 0x805D,
        TextureBlueSize = 0x805E,
        TextureAlphaSize = 0x805F,
        TextureLuminanceSize = 0x8060,
        TextureIntensitySize = 0x8061,
        ProxyTexture1D = 0x8063,
        ProxyTexture2D = 0x8064
    }

    public enum PixelFormat : uint
    {
        ColorIndex = 0x1900,
        StencilIndex = 0x1901,
        DepthComponent = 0x1902,
        Red = 0x1903,
        Green = 0x1904,
        Blue = 0x1905,
        Alpha = 0x1906,
        RGB = 0x1907,
        RGBA = 0x1908,
        Luminance = 0x1909,
        LuminanceAlpha = 0x190A
    }

    public enum BeginMode : uint
    {
        Point = 0x0000,
        Lines = 0x0001,
        LineLoop = 0x0002,
        LineStrip = 0x0003,
        Triangles = 0x0004,
        TriangleStrip = 0x0005,
        TriangleFan = 0x0006,
        Quads = 0x0007,
        QuadStrip = 0x0008,
        Polygon = 0x0009
    }

    public enum ArrayType : uint
    {
        Vertex = 0x8074,
        Normal = 0x8075,
        Color = 0x8076,
        Index = 0x8077,
        TextureCoord = 0x8078
    }

    public enum BlendingSourceFactor : uint
    {
        Zero = 0,
        One = 1,
        DestinationColor = 0x0306,
        OneMinusDestinationColor = 0x0307,
        SourceAlphaSaturate = 0x0308,
        SourceAlpha = 0x0302,
        OneMinusSourceAlpha = 0x0303,
        DestinationAlpha = 0x0304,
        OneMinusDestinationAlpha = 0x0305
    }

    public enum BlendingDestinationFactor : uint
    {
        Zero = 0,
        One = 1,
        SourceColor = 0x0300,
        OneMinusSourceColor = 0x0301,
        SourceAlpha = 0x0302,
        OneMinusSourceAlpha = 0x0303,
        DestinationAlpha = 0x0304,
        OneMinusDestinationAlpha = 0x0305
    }

    public enum HintMode : uint
    {
        DontCare = 0x1100,
        Fastest = 0x1101,
        Nicest = 0x1102
    }

    public enum ShadingModel : uint
    {
        Flat = 0x1D00,
        Smooth = 0x1D01
    }

    public enum AttribueMask : uint
    {
        CurrentBit = 0x00000001,
        PointBit = 0x00000002,
        LineBit = 0x00000004,
        PolygonBit = 0x00000008,
        PolygonStippleBit = 0x00000010,
        PixelModeBit = 0x00000020,
        LightingBit = 0x00000040,
        FogBit = 0x00000080,
        DepthBufferBit = 0x00000100,
        AccumBufferBit = 0x00000200,
        StencilBufferBit = 0x00000400,
        ViewportBit = 0x00000800,
        TransformBit = 0x00001000,
        EnableBit = 0x00002000,
        ColorBufferBit = 0x00004000,
        HintBit = 0x00008000,
        EvalBit = 0x00010000,
        ListBit = 0x00020000,
        TextureBit = 0x00040000,
        ScissorBit = 0x00080000,
        AllAttribBits = 0x000fffff
    }

}

