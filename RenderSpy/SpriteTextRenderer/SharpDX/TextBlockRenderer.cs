using System;
using SharpDX.DirectWrite;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX;
using SharpDX.Direct3D;

namespace SpriteTextRenderer.SharpDX
{
    /// <summary>
    /// This class is responsible for rendering arbitrary text using SharpDX. Every TextRenderer is specialized for a specific font and relies on
    /// a SpriteRenderer for rendering the text.
    /// </summary>
    public class TextBlockRenderer : SpriteTextRenderer.TextBlockRenderer
    {
        private RenderTargetProperties rtp;

        /// <summary>
        /// Creates a new text renderer for a specific font.
        /// </summary>
        /// <param name="sprite">The sprite renderer that is used for rendering</param>
        /// <param name="fontName">Name of font. The font has to be installed on the system. 
        /// If no font can be found, a default one is used.</param>
        /// <param name="fontSize">Size in which to prerender the text. FontSize should be equal to render size for best results.</param>
        /// <param name="fontStretch">Font stretch parameter</param>
        /// <param name="fontStyle">Font style parameter</param>
        /// <param name="fontWeight">Font weight parameter</param>
        public TextBlockRenderer(SpriteRenderer sprite, String fontName, global::SharpDX.DirectWrite.FontWeight fontWeight,
            global::SharpDX.DirectWrite.FontStyle fontStyle, FontStretch fontStretch, float fontSize)
            : base(sprite, fontSize)
        {
            System.Threading.Monitor.Enter(sprite.Device);
            try
            {
                rtp = new RenderTargetProperties()
                {

                    DpiX = 96,
                    DpiY = 96,
                    Type = RenderTargetType.Default,
                    PixelFormat = new PixelFormat(Format.R8G8B8A8_UNorm, global::SharpDX.Direct2D1.AlphaMode.Premultiplied),
                    MinLevel = global::SharpDX.Direct2D1.FeatureLevel.Level_10
                };

                font = new TextFormat((global::SharpDX.DirectWrite.Factory)WriteFactory, fontName, fontWeight, fontStyle, fontStretch, fontSize);
            }
            finally
            {
                System.Threading.Monitor.Exit(sprite.Device);
            }

            CreateCharTable(0);
        }

        // ### Public draw interface ###

        /// <summary>
        /// Draws the string in the specified coordinate system.
        /// </summary>
        /// <param name="text">The text to draw</param>
        /// <param name="position">A position in the chosen coordinate system where the top left corner of the first character will be</param>
        /// <param name="realFontSize">The real font size in the chosen coordinate system</param>
        /// <param name="color">The color in which to draw the text</param>
        /// <param name="coordinateType">The chosen coordinate system</param>
        /// <returns>The StringMetrics for the rendered text</returns>
        public StringMetrics DrawString(string text, Vector2 position, float realFontSize, Color4 color, CoordinateType coordinateType)
        {
            return base.DrawString(text, position.ToSTRVector(), realFontSize, color.ToSTRColor(), coordinateType);
        }

        /// <summary>
        /// Draws the string untransformed in absolute coordinate system.
        /// </summary>
        /// <param name="text">The text to draw</param>
        /// <param name="position">A position in absolute coordinates where the top left corner of the first character will be</param>
        /// <param name="color">The color in which to draw the text</param>
        /// <returns>The StringMetrics for the rendered text</returns>
        public StringMetrics DrawString(string text, Vector2 position, Color4 color)
        {
            return base.DrawString(text, position.ToSTRVector(), color.ToSTRColor());
        }

        /// <summary>
        /// Draws the string in the specified coordinate system aligned in the given rectangle. The text is not clipped or wrapped.
        /// </summary>
        /// <param name="text">The text to draw</param>
        /// <param name="rect">The rectangle in which to align the text</param>
        /// <param name="align">Alignment of text in rectangle</param>
        /// <param name="realFontSize">The real font size in the chosen coordinate system</param>
        /// <param name="color">The color in which to draw the text</param>
        /// <param name="coordinateType">The chosen coordinate system</param>
        /// <returns>The StringMetrics for the rendered text</returns>
        public StringMetrics DrawString(string text, System.Drawing.RectangleF rect, TextAlignment align, float realFontSize, Color4 color, CoordinateType coordinateType)
        {
            return base.DrawString(text, rect, align, realFontSize, color.ToSTRColor(), coordinateType);
        }

        /// <summary>
        /// Draws the string unscaled in absolute coordinate system aligned in the given rectangle. The text is not clipped or wrapped.
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="rect">A position in absolute coordinates where the top left corner of the first character will be</param>
        /// <param name="align">Alignment in rectangle</param>
        /// <param name="color">Color in which to draw the text</param>
        /// <returns>The StringMetrics for the rendered text</returns>
        public StringMetrics DrawString(string text, System.Drawing.RectangleF rect, TextAlignment align, Color4 color)
        {
            return base.DrawString(text, rect, align, color.ToSTRColor());
        }

        // ### Template method hooks

        protected override STRLayout GetTextLayout(string s)
        {
            return new TextLayout((global::SharpDX.DirectWrite.Factory)WriteFactory, s, (TextFormat)font,1.0f,1.0f).ToSTRLayout();
        }

        protected override IDisposable CreateFontMapTexture(int width, int height, CharRenderCall[] drawCalls)
        {
            var TexDesc = new Texture2DDescription()
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                Height = height,
                Width = width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.Shared,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };

            var device10 = (global::SharpDX.Direct3D11.Device)D3DDevice10;
            var texture = new Texture2D(device10, TexDesc);

            var rtv = new RenderTargetView(device10, texture);
            device10.ImmediateContext.ClearRenderTargetView(rtv, new Color4(1, 1, 1, 0));


            Surface surface = texture.QueryInterface<Surface>();
            var target = new RenderTarget((global::SharpDX.Direct2D1.Factory)D2DFactory, surface, rtp);
            var color = new SolidColorBrush(target, new Color4(1, 1, 1, 1));

            target.BeginDraw();

            foreach (var drawCall in drawCalls)
            {
                target.DrawTextLayout(new Vector2(drawCall.Position.X,drawCall.Position.Y) , (TextLayout)drawCall.TextLayout, color);
            }

            target.EndDraw();

            color.Dispose();

            //This is a workaround for Windows 8.1 machines. 
            //If these lines would not be present, the shared resource would be empty.
            //TODO: find a nicer solution
            #region WorkAround
            var textureDescDummy = new Texture2DDescription()
            {
                ArraySize = 1,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                Format = Format.R8G8B8A8_UNorm,
                Height = height,
                Width = width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Staging
            };
            global::SharpDX.Direct3D11.Device device = (global::SharpDX.Direct3D11.Device) D3DDevice10;
            var textureDummy = new Texture2D(device, textureDescDummy);
            DataStream dataStreamDummy = null;
            device.ImmediateContext.CopyResource(texture, textureDummy);

            DataBox databox = device.ImmediateContext.MapSubresource(textureDummy, 0, 0, MapMode.Read, global::SharpDX.Direct3D11.MapFlags.None, out dataStreamDummy);
            dataStreamDummy.Dispose();
            textureDummy.Dispose();
            #endregion

            target.Dispose();
            surface.Dispose();
            rtv.Dispose();
            return texture;
        }

        protected override void CreateDeviceCompatibleTexture(int width, int height, IDisposable texture10, out IDisposable texture11, out IDisposable srv11)
        {
            var texture = (global::SharpDX.Direct3D11.Texture2D)texture10;
            var device11 = ((SpriteRenderer)Sprite).Device;

            lock (device11)
            {
                var dxgiResource = texture.QueryInterface<global::SharpDX.DXGI.Resource>();

                global::SharpDX.Direct3D11.Texture2D tex11;
                if (PixCompatible)
                {
                    tex11 = new global::SharpDX.Direct3D11.Texture2D(device11, new global::SharpDX.Direct3D11.Texture2DDescription()
                    {
                        ArraySize = 1,
                        BindFlags = global::SharpDX.Direct3D11.BindFlags.ShaderResource | global::SharpDX.Direct3D11.BindFlags.RenderTarget,
                        CpuAccessFlags = global::SharpDX.Direct3D11.CpuAccessFlags.None,
                        Format = Format.R8G8B8A8_UNorm,
                        Height = height,
                        Width = width,
                        MipLevels = 1,
                        OptionFlags = global::SharpDX.Direct3D11.ResourceOptionFlags.Shared,
                        SampleDescription = new SampleDescription(1, 0),
                        Usage = global::SharpDX.Direct3D11.ResourceUsage.Default
                    });
                }
                else
                {
                    tex11 = device11.OpenSharedResource<global::SharpDX.Direct3D11.Texture2D>(dxgiResource.SharedHandle);
                }
                srv11 = new global::SharpDX.Direct3D11.ShaderResourceView(device11, tex11);
                texture11 = tex11;
                dxgiResource.Dispose();
            }
        }

        protected override DeviceDescriptor CreateDevicesAndFactories()
        {
            DeviceDescriptor desc = new DeviceDescriptor();

            var creationFlags = global::SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport;
            var featureLevels = global::SharpDX.Direct3D.FeatureLevel.Level_10_0;
            using (var defaultDevice = new global::SharpDX.Direct3D11.Device(DriverType.Hardware, creationFlags, featureLevels))
                desc.D3DDevice10 = defaultDevice.QueryInterface<global::SharpDX.Direct3D11.Device1>();

            desc.WriteFactory = new global::SharpDX.DirectWrite.Factory(global::SharpDX.DirectWrite.FactoryType.Shared);
            desc.D2DFactory = new global::SharpDX.Direct2D1.Factory(global::SharpDX.Direct2D1.FactoryType.SingleThreaded);
            return desc;
        }
    }
}
