using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace SpriteTextRenderer
{
    /// <summary>
    /// Defines how a text is aligned in a rectangle. Use OR-combinations of vertical and horizontal alignment.
    /// </summary>
    /// <example>
    /// This example aligns the textblock on the top edge of the rectangle horizontally centered:
    /// <code lang="cs">var textAlignment = TextAlignment.Top | TextAlignment.HorizontalCenter</code>
    /// <code lang="vb">Dim textAlignment = TextAlignment.Top Or TextAlignment.HorizontalCenter</code>
    /// </example>
    [Flags]
    public enum TextAlignment
    {
        /// <summary>
        /// The top edge of the text is aligned at the top edge of the rectangle.
        /// </summary>
        Top = 1,
        /// <summary>
        /// The vertical center of the text is aligned at the vertical center of the rectangle.
        /// </summary>
        VerticalCenter = 2,
        /// <summary>
        /// The bottom edge of the text is aligned at the bottom edge of the rectangle.
        /// </summary>
        Bottom = 4,

        /// <summary>
        /// The left edge of the text is aligned at the left edge of the rectangle.
        /// </summary>
        Left = 8,
        /// <summary>
        /// The horizontal center of the text is aligned at the horizontal center of the rectangle. Each line is aligned independently.
        /// </summary>
        HorizontalCenter = 16,
        /// <summary>
        /// The right edge of the text is aligned at the right edge of the rectangle. Each line is aligned independently.
        /// </summary>
        Right = 32
    }

    /// <summary>
    /// Holds references to all library-specific devices and factories that are used for rendering text.
    /// </summary>
    public class DeviceDescriptor
    {
        /// <summary>
        /// The DirectX10 device to use to render fonts. This device is shared across all TextBlockRenderer instances
        /// </summary>
        public  IDisposable D3DDevice10;
        /// <summary>
        /// The DirectWrite factory to use. This factory is shared across all TextBlockRenderer instances.
        /// </summary>
        public IDisposable WriteFactory;

        /// <summary>
        /// The Direct2D factory to use. This factory is shared across all TextBlockRenderer instances.
        /// </summary>
        public IDisposable D2DFactory;

        /// <summary>
        /// Holds the number of active TextBlockRenderer instances
        /// </summary>
        public int referenceCount;

        /// <summary>
        /// Disposes of all devices and factories of this description.
        /// </summary>
        public void DisposeAll()
        {
            D3DDevice10.Dispose();
            WriteFactory.Dispose();
            D2DFactory.Dispose();
        }
}

    /// <summary>
    /// This class is responsible for rendering arbitrary text. Every TextRenderer is specialized for a specific font and relies on
    /// a SpriteRenderer for rendering the text.
    /// </summary>
    public abstract class TextBlockRenderer : IDisposable
    {
        static Dictionary<Type,DeviceDescriptor> _deviceDescriptors;

        private DeviceDescriptor _desc;
        private static object lockObject;
        
        protected SpriteRenderer Sprite;
        protected IDisposable font;
        private float _FontSize;

        /// <summary>
        /// Returns the font size that this TextRenderer was created for.
        /// </summary>
        public float FontSize { get { return _FontSize; } }

        /// <summary>
        /// Gets or sets whether this TextRenderer should behave PIX compatibly.
        /// </summary>
        /// <remarks>
        /// PIX compatibility means that no shared resource is used.
        /// However, this will result in no visible text being drawn. 
        /// The geometry itself will be visible in PIX.
        /// </remarks>
        public static bool PixCompatible { get; set; }

        static TextBlockRenderer()
        {
            PixCompatible = false;
            _deviceDescriptors = new Dictionary<Type,DeviceDescriptor>();
             lockObject = new object();
        }

        /// <summary>
        /// Contains information about every char table that has been created.
        /// </summary>
        private Dictionary<byte, CharTableDescription> CharTables = new Dictionary<byte, CharTableDescription>();

        /// <summary>
        /// Creates a new text renderer for a specific font.
        /// </summary>
        /// <param name="sprite">The sprite renderer that is used for rendering</param>
        /// <param name="fontWeight">Font weight parameter</param>
        public TextBlockRenderer(SpriteRenderer sprite, float fontSize)
        {
            this.Sprite = sprite;
            this._FontSize = fontSize;
            AssertDevice();
            IncRefCount();        
        }

        /// <summary>
        /// Calculates the text layout for a given string.
        /// </summary>
        /// <param name="s">The string to layout</param>
        /// <returns>The string's layout information</returns>
        protected abstract STRLayout GetTextLayout(string s);

        /// <summary>
        /// Creates a new shared texture and performs the specified draw calls on it.
        /// </summary>
        /// <param name="width">The texture's width</param>
        /// <param name="height">The texture's height</param>
        /// <param name="drawCalls">The draw calls to perform</param>
        protected abstract IDisposable CreateFontMapTexture(int width, int height, CharRenderCall[] drawCalls);

        /// <summary>
        /// Creates a DirectX11 texture from the DirectX10 texture by opening the shared resource (or by creating a new one in PIX compatibility mode).
        /// </summary>
        /// <param name="width">The texture's width for PIX compatibility mode</param>
        /// <param name="height">The texture's height for PIX compatibility mode</param>
        /// <param name="texture10">The DirectX10 texture</param>
        /// <param name="texture11">Output parameter, the DirectX11 texture</param>
        /// <param name="srv11">Output parameter, the texture's ShaderResourceView</param>
        protected abstract void CreateDeviceCompatibleTexture(int width, int height, IDisposable texture10, out IDisposable texture11, out IDisposable srv11);

        /// <summary>
        /// Creates the texture and necessary structures for 256 chars whose unicode number starts with the given byte.
        /// The table containing ASCII has a prefix of 0 (0x00/00 - 0x00/FF).
        /// </summary>
        /// <param name="bytePrefix">The byte prefix of characters.</param>
        protected void CreateCharTable(byte bytePrefix)
        {
            int sizeX;
            int sizeY;
            STRLayout[] tl;
            //Get appropriate texture width height and layout accoring to 'Font' field member
            GenerateTextLayout(bytePrefix, out sizeX,out sizeY, out tl);

            CharTableDescription TableDesc;
            CharRenderCall[] drawCalls;
            
            //Get Draw calls and table description
            GenerateDrawCalls(sizeX, sizeY, tl, out TableDesc,out drawCalls);

            //Create font map texture from previously created draw calls
            var fontMapTexture = CreateFontMapTexture(sizeX, sizeY, drawCalls);

            //Create a texture to be used by the associated sprite renderer's graphics device from the font map texture.
            CreateDeviceCompatibleTexture(sizeX, sizeY, fontMapTexture, out TableDesc.Texture, out TableDesc.SRV);

            fontMapTexture.Dispose();

            foreach (var layout in tl)
            {
                layout.TextLayout.Dispose();
            }
            #if DEBUG 
                System.Diagnostics.Debug.WriteLine("Created Char Table " + bytePrefix + " in " + sizeX + " x " + sizeY);
            #endif

            //System.Threading.Monitor.Enter(D3DDevice11);
            //SlimDX.Direct3D11.Texture2D.SaveTextureToFile(Sprite.Device.ImmediateContext, Texture11, SlimDX.Direct3D11.ImageFileFormat.Png, Font.FontFamilyName + "Table" + BytePrefix + ".png");
            //System.Threading.Monitor.Exit(D3DDevice11);

            CharTables.Add(bytePrefix, TableDesc);           
        }

        private void GenerateTextLayout(byte bytePrefix, out int sizeX, out int sizeY, out STRLayout[] tl)
        {
            sizeX = (int)(FontSize * 12);
            sizeX = (int)Math.Pow(2, Math.Ceiling(Math.Log(sizeX, 2)));
            //Try how many lines are needed:
            tl = new STRLayout[256];
            int line = 0, xPos = 0, yPos = 0;
            for (int i = 0; i < 256; ++i)
            {
                tl[i] = GetTextLayout(Convert.ToChar(i + (bytePrefix << 8)).ToString());
                int charWidth = 2 + (int)Math.Ceiling(tl[i].LayoutSize.X + tl[i].OverhangLeft + tl[i].OverhangRight);
                int charHeight = 2 + (int)Math.Ceiling(tl[i].LayoutSize.X + tl[i].OverhangTop + tl[i].OverhangBottom);
                line = Math.Max(line, charHeight);
                if (xPos + charWidth >= sizeX)
                {
                    xPos = 0;
                    yPos += line;
                    line = 0;
                }
                xPos += charWidth;
            }

            sizeY = (int)(line + yPos);
            sizeY = (int)Math.Pow(2, Math.Ceiling(Math.Log(sizeY, 2)));
        }

        private static void GenerateDrawCalls(int sizeX, int sizeY, STRLayout[] tl, out CharTableDescription TableDesc, out CharRenderCall[] drawCalls)
        {
            drawCalls = new CharRenderCall[256];
            TableDesc = new CharTableDescription();
            int line = 0, xPos = 0, yPos = 0;            
            for (int i = 0; i < 256; ++i)
            {
                //1 additional pixel on each side
                int charWidth = 2 + (int)Math.Ceiling(tl[i].LayoutSize.X + tl[i].OverhangLeft + tl[i].OverhangRight);
                int charHeight = 2 + (int)Math.Ceiling(tl[i].LayoutSize.Y + tl[i].OverhangTop + tl[i].OverhangBottom);
                line = Math.Max(line, charHeight);
                if (xPos + charWidth >= sizeX)
                {
                    xPos = 0;
                    yPos += line;
                    line = 0;
                }
                var charDesc = new CharDescription();

                charDesc.CharSize = new STRVector(tl[i].WidthIncludingTrailingWhitespaces, tl[i].Size.Y);
                charDesc.OverhangLeft = tl[i].OverhangLeft + 1;
                charDesc.OverhangTop = tl[i].OverhangTop + 1;
                //Make XPos + CD.Overhang.Left an integer number in order to draw at integer positions
                charDesc.OverhangLeft += (float)Math.Ceiling(xPos + charDesc.OverhangLeft) - (xPos + charDesc.OverhangLeft);
                //Make YPos + CD.Overhang.Top an integer number in order to draw at integer positions
                charDesc.OverhangTop += (float)Math.Ceiling(yPos + charDesc.OverhangTop) - (yPos + charDesc.OverhangTop);

                charDesc.OverhangRight = charWidth - charDesc.CharSize.X - charDesc.OverhangLeft;
                charDesc.OverhangBottom = charHeight - charDesc.CharSize.Y - charDesc.OverhangTop;

                charDesc.TexCoordsStart = new STRVector(((float)xPos / sizeX), ((float)yPos / sizeY));
                charDesc.TexCoordsSize = new STRVector((float)charWidth / sizeX, (float)charHeight / sizeY);

                charDesc.TableDescription = TableDesc;

                TableDesc.Chars[i] = charDesc;

                drawCalls[i] = new CharRenderCall() { Position = new Vector2(xPos + charDesc.OverhangLeft, yPos + charDesc.OverhangTop), TextLayout = tl[i].TextLayout};
                
                xPos += charWidth;
            }
    }
        
        /// <summary>
        /// Draws the string in the specified coordinate system.
        /// </summary>
        /// <param name="text">The text to draw</param>
        /// <param name="position">A position in the chosen coordinate system where the top left corner of the first character will be</param>
        /// <param name="realFontSize">The real font size in the chosen coordinate system</param>
        /// <param name="color">The color in which to draw the text</param>
        /// <param name="coordinateType">The chosen coordinate system</param>
        /// <returns>The StringMetrics for the rendered text</returns>
        protected internal StringMetrics DrawString(string text, STRVector position, float realFontSize, STRColor color, CoordinateType coordinateType)
        {
            StringMetrics sm;
            IterateStringEm(text, position, true, realFontSize, color, coordinateType, out sm);
            return sm;
        }

        /// <summary>
        /// Draws the string untransformed in absolute coordinate system.
        /// </summary>
        /// <param name="text">The text to draw</param>
        /// <param name="position">A position in absolute coordinates where the top left corner of the first character will be</param>
        /// <param name="color">The color in which to draw the text</param>
        /// <returns>The StringMetrics for the rendered text</returns>
        protected internal StringMetrics DrawString(string text, STRVector position, STRColor color)
        {
            return DrawString(text, position, FontSize, color, CoordinateType.Absolute);
        }

        /// <summary>
        /// Measures the untransformed string in absolute coordinate system.
        /// </summary>
        /// <param name="text">The text to measure</param>
        /// <returns>The StringMetrics for the text</returns>
        public StringMetrics MeasureString(string text)
        {
            StringMetrics sm;
            IterateString(text, STRVector.Zero, false, 1, new STRColor(), CoordinateType.Absolute, out sm);
            return sm;
        }

        /// <summary>
        /// Measures the string in the specified coordinate system.
        /// </summary>
        /// <param name="text">The text to measure</param>
        /// <param name="realFontSize">The real font size in the chosen coordinate system</param>
        /// <param name="coordinateType">The chosen coordinate system</param>
        /// <returns>The StringMetrics for the text</returns>
        public StringMetrics MeasureString(string text, float realFontSize, CoordinateType coordinateType)
        {
            StringMetrics sm;
            IterateStringEm(text, STRVector.Zero, false, realFontSize, new STRColor(), coordinateType, out sm);
            return sm;
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
        protected internal StringMetrics DrawString(string text, RectangleF rect, TextAlignment align, float realFontSize, STRColor color, CoordinateType coordinateType)
        {
            //If text is aligned top and left, no adjustment has to be made
            if (align.HasFlag(TextAlignment.Top) && align.HasFlag(TextAlignment.Left))
            {
                return DrawString(text, new STRVector(rect.X, rect.Y), realFontSize, color, coordinateType);
            }

            text = text.Replace("\r", "");
            var rawTextMetrics = MeasureString(text, realFontSize, coordinateType);
            var mMetrics = MeasureString("m", realFontSize, coordinateType);
            float startY;
            if (align.HasFlag(TextAlignment.Top))
                startY = rect.Top;
            else if (align.HasFlag(TextAlignment.VerticalCenter))
                startY = rect.Top + rect.Height / 2 - rawTextMetrics.Size.Y / 2;
            else //Bottom
                startY = rect.Bottom - rawTextMetrics.Size.Y;

            var totalMetrics = new StringMetrics();

            //break text into lines
            var lines = text.Split('\n');

            foreach (var line in lines)
            {
                float startX;
                if (align.HasFlag(TextAlignment.Left))
                    startX = rect.X;
                else
                {
                    var lineMetrics = MeasureString(line, realFontSize, coordinateType);
                    if (align.HasFlag(TextAlignment.HorizontalCenter))
                        startX = rect.X + rect.Width / 2 - lineMetrics.Size.X / 2;
                    else //Right
                        startX = rect.Right - lineMetrics.Size.X;
                }

                var lineMetrics2 = DrawString(line, new STRVector(startX, startY), realFontSize, color, coordinateType);
                float lineHeight;
                if (mMetrics.Size.Y < 0)
                    lineHeight = Math.Min(lineMetrics2.Size.Y, mMetrics.Size.Y);
                else
                    lineHeight = Math.Max(lineMetrics2.Size.Y, mMetrics.Size.Y);
                startY += lineHeight;
                totalMetrics.Merge(lineMetrics2);
            }

            return totalMetrics;
        }

        /// <summary>
        /// Draws the string unscaled in absolute coordinate system aligned in the given rectangle. The text is not clipped or wrapped.
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="rect">A position in absolute coordinates where the top left corner of the first character will be</param>
        /// <param name="align">Alignment in rectangle</param>
        /// <param name="color">Color in which to draw the text</param>
        /// <returns>The StringMetrics for the rendered text</returns>
        protected internal StringMetrics DrawString(string text, RectangleF rect, TextAlignment align, STRColor color)
        {
            return DrawString(text, rect, align, FontSize, color, CoordinateType.Absolute);
        }

        private void IterateStringEm(string text, STRVector position, bool Draw, float realFontSize, STRColor color, CoordinateType coordinateType, out StringMetrics metrics)
        {
            float scale = realFontSize / _FontSize;
            IterateString(text, position, Draw, scale, color, coordinateType, out metrics);
        }

        private void IterateString(string text, STRVector position, bool draw, float scale, STRColor color, CoordinateType coordinateType, out StringMetrics metrics)
        {
            try {
                //.WriteLine("1a");
            metrics = new StringMetrics();
                //.WriteLine("2a");
                STRVector startPosition = position;
                //.WriteLine("3a");
                float scalY = coordinateType == SpriteTextRenderer.CoordinateType.SNorm ? -1 : 1;
                //.WriteLine("4a");
                string visualText =  NBidi.NBidi.LogicalToVisual(text);
                //.WriteLine("5a Visual: " + visualText);
                int[] codePoints = Helpers.ConvertToCodePointArray(visualText);
                //.WriteLine("6a Count: " + codePoints.Length);
                foreach (int c in codePoints)
            {
                    //.WriteLine("intval : " + c);
                    var charDesc = GetCharDescription(c);
                    //.WriteLine("GetCharDescription : " + c);
                    var charMetrics = charDesc.ToStringMetrics(position, scale, scale * scalY);
                    //.WriteLine("charMetrics : " + c);
                    if (draw)
                {
                    if (charMetrics.FullRectSize.X != 0 && charMetrics.FullRectSize.Y != 0)
                    {
                        float posY = position.Y - scalY * charMetrics.OverhangTop;
                        float posX = position.X - charMetrics.OverhangLeft;
                            //.WriteLine("Sprite : " + c);
                            //.WriteLine("Sprite IsNull : " + (Sprite == null));
                            Sprite.Draw(charDesc.TableDescription.SRV, new STRVector(posX, posY), charMetrics.FullRectSize, STRVector.Zero, 0, charDesc.TexCoordsStart, charDesc.TexCoordsSize, color, coordinateType);
                            //.WriteLine("Draw : " + c);
                        }
                }
                    //.WriteLine("metrics : " + c);
                    metrics.Merge(charMetrics);
                    //.WriteLine("Merge : " + c);

                    position.X += charMetrics.Size.X;

                //Break newlines
                if (c == '\r')
                    position.X = metrics.TopLeft.X;

                if (c == '\n')
                    position.Y = metrics.BottomRight.Y - charMetrics.Size.Y / 2;
            }
            }
            catch (Exception e) { throw new Exception("IterateString Error: " + e.Message); }
        }

        private CharDescription GetCharDescription(int c)
        {
            byte b = (byte)(c & 0x000000FF);
            byte bytePrefix = (byte)((c & 0x0000FF00) >> 8);
            if (!CharTables.ContainsKey(bytePrefix))
                CreateCharTable(bytePrefix);
            return CharTables[bytePrefix].Chars[b];
        }

        
        protected  abstract DeviceDescriptor CreateDevicesAndFactories();

        void AssertDevice()
        {
            DeviceDescriptor desc = null;
            lock (lockObject)

                if (!_deviceDescriptors.TryGetValue(this.GetType(),out desc))
                    _deviceDescriptors[this.GetType()] = desc = CreateDevicesAndFactories();

            _desc = desc;
        }

        private void DecRefCount()
        {
            DeviceDescriptor desc = null;
            if (_deviceDescriptors.TryGetValue(this.GetType(), out desc))
                desc.referenceCount--;

            if (desc != null && desc.referenceCount == 0)
            {
                desc.DisposeAll();
                _deviceDescriptors.Remove(this.GetType());

            }
        }
        private void IncRefCount()
        {
            DeviceDescriptor desc = null;
            if (_deviceDescriptors.TryGetValue(this.GetType(), out desc))
                desc.referenceCount++;
        }

        protected IDisposable D3DDevice10 { get { return _desc.D3DDevice10; } }
        protected IDisposable WriteFactory { get { return _desc.WriteFactory; } }
        protected IDisposable D2DFactory { get { return _desc.D2DFactory; } }

        #region IDisposable Support
        private bool disposed = false;
        /// <summary>
        /// Disposes of the SpriteRenderer.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                font.Dispose();

                foreach (var Table in CharTables)
                {
                    Table.Value.SRV.Dispose();
                    Table.Value.Texture.Dispose();
                }

                DecRefCount();
                this.disposed = true;
            }
        }

        #endregion
      }

    
    
}
