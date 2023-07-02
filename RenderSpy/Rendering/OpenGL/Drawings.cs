
using RenderSpy.Graphics.opengl;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using OpenGL;
using PixelFormat = OpenGL.PixelFormat;

namespace RenderSpy.Rendering.OpenGL
{
   
    public static class Drawings
    {
        [Obsolete]
        public static Size glViewport()
        {
            int[] viewport = new int[4];
            OldSDK.GetInteger(Target.VIEWPORT, viewport);
            return new Size(viewport[2], viewport[3]);
        }
        [Obsolete]
        public static void glLine(Vector2 src, Vector2 dst, Color color)
        {
            glLine((int)src.X, (int)src.X, (int)dst.X, (int)dst.Y, color);
        }
        [Obsolete]
        public static void glLine( int x1, int y1, int x2, int y2, Color color)
        {
            OldSDK.Begin(BeginMode.Lines);
            OldSDK.Color(color.R, color.G, color.B, color.A);
            OldSDK.Vertex(x1, y1);
            OldSDK.Vertex(x2, y2);
            OldSDK.End();
            OldSDK.Color(255f, 255f, 255f, 255f);
        }

        [Obsolete]
        public static void glRectangle( int x1, int y1, int width, int height, Color color, bool filled = false)
        {
            OldSDK.Begin(filled ? BeginMode.Quads : BeginMode.LineLoop);
            OldSDK.Color(color.R, color.G, color.B, color.A);
            OldSDK.Vertex(x1, y1 + height);
            OldSDK.Vertex(x1 + width, y1 + height);
            OldSDK.Vertex(x1 + width, y1);
            OldSDK.Vertex(x1, y1);
            OldSDK.End();
            OldSDK.Color(255f, 255f, 255f, 255f);
        }
        [Obsolete]
        public static bool glWorldToScreen( Vector3 src, out Vector2 dst, float[] viewMatrix)
        {
            dst = Vector2.Zero;

            Vector4 ClipCoords = new Vector4();
            ClipCoords.X = src.X * viewMatrix[0] + src.Y * viewMatrix[4] + src.Z * viewMatrix[8] + viewMatrix[12];
            ClipCoords.Y = src.X * viewMatrix[1] + src.Y * viewMatrix[5] + src.Z * viewMatrix[9] + viewMatrix[13];
            ClipCoords.Z = src.X * viewMatrix[2] + src.Y * viewMatrix[6] + src.Z * viewMatrix[10] + viewMatrix[14];
            ClipCoords.W = src.X * viewMatrix[3] + src.Y * viewMatrix[7] + src.Z * viewMatrix[11] + viewMatrix[15];

            if (ClipCoords.W < 0.1f)
                return false;

            Vector3 NDC;
            NDC.X = ClipCoords.X / ClipCoords.W;
            NDC.Y = ClipCoords.Y / ClipCoords.W;
            NDC.Z = ClipCoords.Z / ClipCoords.W;

            Size viewport = glViewport();
            dst.X = (viewport.Width / 2 * NDC.X) + (NDC.X + viewport.Width / 2);
            dst.Y = -(viewport.Height / 2 * NDC.Y) + (NDC.Y + viewport.Height / 2);
            return true;
        }

        [Obsolete]
        public static void glText(string text, Font font, Color FontColor, PointF position)
        {
            System.Drawing.Bitmap TextBitmap = Drawings.RenderTextToBitmap(text, font, FontColor, Color.Transparent);
            Drawings.glBitmap(TextBitmap, position);
        }

        public static byte[] GetImageData(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging. PixelFormat.Format32bppArgb);
            var length = bitmapData.Stride * bitmapData.Height;
            var data = new byte[length];
            Marshal.Copy(bitmapData.Scan0, data, 0, length);
            bitmap.UnlockBits(bitmapData);
            return data;
        }
        [Obsolete]
        public static bool LoadImageToTexture(Bitmap bitmap, Texture textureBind)
        {
            try {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                                       ImageLockMode.ReadOnly,
                                                       System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                // Activa y enlaza la textura
                textureBind.Bind();

                // Carga los datos de la imagen en la textura
                textureBind.LoadImageData(bitmapData.Scan0, bitmap.Width, bitmap.Height,
                                          Graphics.opengl.PixelFormat.RGBA, Graphics.opengl.DataType.UnsignedByte);

                OpenGLInterops.TexParameterI(OpenGLInterops.GL_TEXTURE_2D, OpenGLInterops.GL_TEXTURE_MAG_FILTER, (int)OpenGLInterops.GL_LINEAR);
                OpenGLInterops.TexParameterI(OpenGLInterops.GL_TEXTURE_2D, OpenGLInterops.GL_TEXTURE_MIN_FILTER, (int)OpenGLInterops.GL_LINEAR);

                bitmap.UnlockBits(bitmapData);
                bitmap.Dispose();
                return true;
            } catch { return false; }
           
        }

        [Obsolete]
        public static void glBitmap(Bitmap Image,  PointF position)
        {
            OldSDK.Enable(Target.Blend);
          
            OldSDK.BlendFunc( BlendingSourceFactor.SourceAlpha,  BlendingDestinationFactor.OneMinusSourceAlpha);


            int textureId = 0;
            OldSDK.GenTextures(1, ref textureId);
            OldSDK.BindTexture(Target.Texture2D, textureId);
            OldSDK.TexImage2D(Target.Texture2D, 0, (uint)PixelFormat.Rgba, Image.Width, Image.Height, 0, (uint)PixelFormat.Bgra, Graphics.opengl.DataType.UnsignedByte, GetImageData(Image));
            OldSDK.TexParameteri(Target.Texture2D, Graphics.opengl.TextureParameterName.TextureMagFilter, (int)Graphics.opengl.TextureMagFilter.Linear);
            OldSDK.TexParameteri(Target.Texture2D, Graphics.opengl.TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            OldSDK.BindTexture(Target.Texture2D, 0);

            OldSDK.Enable(Target.Texture2D);
            OldSDK.BindTexture(Target.Texture2D, textureId);
            OldSDK.Begin(BeginMode.Quads);
            OldSDK.TexCoord(0, 0); OldSDK.Vertexf(position.X, position.Y);
            OldSDK.TexCoord(1, 0); OldSDK.Vertexf(position.X + Image.Width, position.Y);
            OldSDK.TexCoord(1, 1); OldSDK.Vertexf(position.X + Image.Width, position.Y + Image.Height);
            OldSDK.TexCoord(0, 1); OldSDK.Vertexf(position.X, position.Y + Image.Height);
            OldSDK.End();
            OldSDK.Disable(Target.Texture2D);

            OldSDK.DeleteTextures(1, new int[] { textureId });

            OldSDK.Disable(Target.Blend);
        }

            public static Bitmap RenderTextToBitmap(string text, Font font, Color textColor, Color backColor)
        {
            // Crear un objeto de gráficos a partir de un bitmap en blanco
            Bitmap bitmap = new Bitmap(1, 1);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);

            // Calcular el tamaño necesario para el bitmap basado en el texto y la fuente
            SizeF textSize = graphics.MeasureString(text, font);
            int width = (int)Math.Ceiling(textSize.Width);
            int height = (int)Math.Ceiling(textSize.Height);

            // Crear un nuevo bitmap con el tamaño calculado
            bitmap = new Bitmap(width, height);
            graphics = System.Drawing.Graphics.FromImage(bitmap);

            // Configurar los colores y el fondo del bitmap
            graphics.Clear(backColor);
            Brush textBrush = new SolidBrush(textColor);

            // Dibujar el texto en el bitmap
            graphics.DrawString(text, font, textBrush, PointF.Empty);

            // Liberar los recursos de gráficos
            graphics.Dispose();

            return bitmap;
        }

    }

    public class Texture
    {
        private int _textureId;
        [Obsolete]
        public void Create()
        {
            OldSDK.GenTextures(1, ref _textureId);
        }
        [Obsolete]
        public void Bind()
        {
            OldSDK.BindTexture(Target.Texture2D, _textureId);
        }
        [Obsolete]
        public void Unbind()
        {
            OldSDK.BindTexture(Target.Texture2D, 0);
        }
        [Obsolete]
        public void SetParameter(Graphics.opengl.TextureParameterName parameterName, int parameterValue)
        {
            OldSDK.TexParameteri(Target.Texture2D, parameterName, parameterValue);
        }
        [Obsolete]
        public void LoadImageData(IntPtr data, int width, int height, Graphics.opengl.PixelFormat pixelFormat, Graphics.opengl.DataType pixelType)
        {
            OldSDK.TexImage2D(Target.Texture2D, 0, (uint)pixelFormat, width, height, 0, (uint)pixelFormat, pixelType, data);
        }
        [Obsolete]
        public void Dispose()
        {
            OldSDK.DeleteTextures(1, new int[] { _textureId });
        }
    }

}
