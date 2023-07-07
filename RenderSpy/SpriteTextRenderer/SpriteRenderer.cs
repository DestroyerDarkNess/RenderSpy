using RenderSpy.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SpriteTextRenderer
{
    /// <summary>
    /// Specifies, how coordinates are interpreted.
    /// </summary>
    /// <remarks>
    /// <para>Sprites (and with that text) can be drawn in several coordinate systems. The user can choose, which system
    /// fits his needs best. There are basically two types of coordinate system:</para>
    /// <para><b>Type 1 systems</b><br/>
    /// <img src="../Coordinate1.jpg" alt="Type 1 coordinate system"/><br/>
    /// The origin of T1 systems is located at the top left corner of the screen. The x-axis points to the right,
    /// the y-axis points downwards. All T1 systems differ in the axes' scaling. <see cref="CoordinateType.UNorm"/>
    /// uses unsigned normalized coordinates. <see cref="CoordinateType.Absolute"/> uses the screen's pixel coordinates.
    /// Therefore, the SpriteRenderer needs the D3DDevice's viewport. For performance reasons the viewport will not be
    /// queried repeatedly, but only once at the construction of the <see cref="SpriteRenderer"/> or on a call to 
    /// <see cref="SpriteRenderer.RefreshViewport"/>. <see cref="CoordinateType.Relative"/> uses a T1 coordinate 
    /// system of custom size.
    /// </para>
    /// <para><b>Type 2 systems</b><br/>
    /// <img src="../Coordinate2.jpg" alt="Type 2 coordinate system"/><br/>
    /// The origin of T2 systems is at the screen center. The x-axis points to the right, the y-axis points upwards.
    /// I.e. this coordinate system uses a flipped y-axis. Although the bottom coordinates usually have values less than
    /// the top coordinates, you can safely specify positive size values. <see cref="CoordinateType.SNorm"/> uses a T2 system.
    /// </para>
    /// 
    /// </remarks>
    public enum CoordinateType
    {
        /// <summary>
        /// Coordinates are in the range from 0 to 1. (0, 0) is the top left corner; (1, 1) is the bottom right corner.
        /// </summary>
        UNorm,
        /// <summary>
        /// Coordinates are in the range from -1 to 1. (-1, -1) is the bottom left corner; (1, 1) is the top right corner. This is the DirectX standard interpretation.
        /// </summary>
        SNorm,
        /// <summary>
        /// Coordinates are in the range of the relative screen size. (0, 0) is the top left corner; (ScreenSize.X, ScreenSize.Y) is the bottom right corner. A variable screen size is used. Use <see cref="SpriteRenderer.ScreenSize"/>.
        /// </summary>
        Relative,
        /// <summary>
        /// Coordinates are in the range of the actual screen size. (0, 0) is the top left corner; (Viewport.Width, Viewport.Height) is the bottom right corner. Use <see cref="SpriteRenderer.RefreshViewport"/> for updates to the used viewport.
        /// </summary>
        Absolute
    }
    /// <summary>
    /// Specify the alpha blending mode of the texture being drawn 
    /// </summary>
    public enum AlphaBlendModeType
    {
        /// <summary>
        /// Alpha blending is postmultiplied (straight)
        /// </summary>
        PostMultiplied,
        /// <summary>
        /// The sprite renderer assumes alpha channel is premultiplied
        /// </summary>
        PreMultiplied
    }

    /// <summary>
    /// This class is responsible for rendering 2D sprites. Typically, only one instance of this class is necessary.
    /// </summary>
    public abstract class SpriteRenderer : IDisposable
    {
        private int bufferSize;
        public STRViewport viewport;

        /// <summary>
        /// The blend state to use for rendering sprites
        /// </summary>
        protected IDisposable blendState;

        /// <summary>
        /// The depth stencil state to use for rendering sprites
        /// </summary>
        protected IDisposable depthStencilState;

        /// <summary>
        /// Returns the device.
        /// </summary>
        protected internal abstract object DeviceRef
        {
            get;
        }

        /// <summary>
        /// Gets or sets, if this SpriteRenderer handles DepthStencilState
        /// </summary>
        /// <remarks>
        /// <para>
        /// Sprites have to be drawn with depth test disabled. If HandleDepthStencilState is set to true, the
        /// SpriteRenderer sets the DepthStencilState to a predefined state before drawing and resets it to
        /// the previous state after that. Set this value to false, if you want to handle states yourself.
        /// </para>
        /// <para>
        /// The default value is true.
        /// </para>
        /// </remarks>
        public bool HandleDepthStencilState { get; set; }

        /// <summary>
        /// Gets or sets, if this SpriteRenderer handles BlendState
        /// </summary>
        /// <remarks>
        /// <para>
        /// Sprites have to be drawn with simple alpha blending. If HandleBlendState is set to true, the
        /// SpriteRenderer sets the BlendState to a predefined state before drawing and resets it to
        /// the previous state after that. Set this value to false, if you want to handle states yourself.
        /// </para>
        /// <para>
        /// The default value is true.
        /// </para>
        /// </remarks>
        public bool HandleBlendState { get; set; }

        /// <summary>
        /// Gets or sets the alpha blending mode of this sprite renderer
        /// <para>
        /// If AlphaBlendMode is changed a flush operation immedietly occurs.
        /// </para>
        /// </summary>
        public AlphaBlendModeType AlphaBlendMode
        { 
            get 
            { 
                return alphaBlendMode; 
            } 

            set
            {
                if (alphaBlendMode != value)
                {
                    Flush();                    
                    alphaBlendMode = value;
                    UpdateAlphaBlend();
                }
            }
        }

        /// <summary>
        /// Called every time Alpha blending mode has changed.
        /// </summary>
        abstract protected  void UpdateAlphaBlend();
        

        //This variable has to be protected from cross-thread access.
        private bool lockDeviceOnDraw;
        /// <summary>
        /// Gets or sets whether to lock the device when rendering sprites. This can be used for multi-threaded rendering.
        /// However, locking comes with performance penalties.
        /// <remarks>The default value is false.</remarks>
        /// </summary>
        public bool LockDeviceOnDraw {
            get { return lockDeviceOnDraw; }
            set
            {
                lock (DeviceRef)
                {
                    lockDeviceOnDraw = value;
                }
            }
        }

        /// <summary>
        /// Set to true, if the order of draw calls can be rearranged for better performance.
        /// </summary>
        /// <remarks>
        /// Sprites are not drawn immediately, but only on a call to <see cref="SpriteRenderer.Flush"/>.
        /// Rendering performance can be improved, if the order of sprites can be changed, so that sprites
        /// with the same texture can be drawn with one draw call. However, this will not preserve the z-order.
        /// Use <see cref="SpriteRenderer.ClearReorderBuffer"/> to force a set of sprites to be drawn before another set.
        /// </remarks>
        /// <example>
        /// Consider the following pseudo code:
        /// <code>
        /// Draw left intense red circle
        /// Draw middle light red circle
        /// Draw right intense red circle
        /// </code>
        /// <para>With AllowReorder set to true, this will result in the following image:<br/>
        /// <img src="../Reorder1.jpg" alt=""/><br/>
        /// That is because the last circle is reordered to be drawn together with the first circle.
        /// </para>
        /// <para>With AllowReorder set to false, this will result in the following image:<br/>
        /// <img src="../Reorder2.jpg" alt=""/><br/>
        /// No optimization is applied. Performance may be slightly worse than with reordering.
        /// </para>
        /// </example>
        public bool AllowReorder { get; set; }

        /// <summary>
        /// When using relative coordinates, the screen size has to be set. Typically the screen size in pixels is used. However, other values are possible as well.
        /// </summary>
        public STRVector ScreenSize { get; set; }

        /// <summary>
        /// A list of all sprites to draw. Sprites are drawn in the order in this list.
        /// </summary>
        private List<SpriteSegment> sprites = new List<SpriteSegment>();
        /// <summary>
        /// Allows direct access to the according SpriteSegments based on the texture
        /// </summary>
        private Dictionary<object, List<SpriteSegment>> textureSprites = new Dictionary<object,List<SpriteSegment>>();

        /// <summary>
        /// The number of currently buffered sprites
        /// </summary>
        private int spriteCount = 0;

        /// <summary>
        /// The active Alpha blending mode
        /// </summary>
        private AlphaBlendModeType alphaBlendMode;

        /// <summary>
        /// Create a new SpriteRenderer instance.
        /// </summary>
        /// <param name="bufferSize">The number of elements that can be stored in the sprite buffer.</param>
        /// <remarks>
        /// Sprites are not drawn immediately, but buffered instead. The buffer size defines, how much sprites can be buffered.
        /// If the buffer is full, according draw calls will be issued on the GPU clearing the buffer. Its size should be as big as
        /// possible without wasting empty space.
        /// </remarks>
        public SpriteRenderer(int bufferSize = 128)
        {
            this.bufferSize = bufferSize;

            AllowReorder = true;
            HandleDepthStencilState = true;
            HandleBlendState = true;
            lockDeviceOnDraw = false;
        }

        /// <summary>
        /// Compiles the effect and saves it as the renderer's FX. Furthermore, one effect resource variable is saved.
        /// </summary>
        /// <param name="hlslSource">The source code to compile</param>
        /// <param name="variableName">The variable's name</param>
        protected abstract void CompileEffectAndGetVariable(string hlslSource, string variableName);

        /// <summary>
        /// Creates a new input layout and saves it as the renderer's layout.
        /// </summary>
        /// <param name="elements">The input elements</param>
        protected abstract void CreateInputLayout(STRInputElement[] elements);

        /// <summary>
        /// Creates a depth stencil state with IsDepthEnabled=false and DepthWriteMask=ZERO and a blend state with alpha blending.
        /// </summary>
        protected abstract void CreateDepthStencilAndBlendState();

        /// <summary>
        /// Creates a new vertex buffer and saves it as the renderer's buffer.
        /// </summary>
        /// <param name="elementByteSize">The byte size of one vertex</param>
        /// <param name="elements">The number of elements to hold in this buffer</param>
        protected abstract void CreateVertexBuffer(int elementByteSize, int elements);

        public bool FailedViewPort = false;

        /// <summary>
        /// Initializes the sprite renderer so it is set up for use.
        /// </summary>
        protected void Initialize()
        {
         
            try
            {
                CompileEffectAndGetVariable(Helpers.GetShaderSource(), "Tex");
                CreateInputLayout(SpriteVertexLayout.Description);
                CreateVertexBuffer(SpriteVertexLayout.Struct.SizeInBytes, bufferSize);
                CreateDepthStencilAndBlendState();
                RefreshViewport();
            }
            catch  {  }

        }

        /// <summary>
        /// Initializes the sprite renderer so it is set up for use.
        /// </summary>
        protected void Initialize(string SpriteShaderSource)
        {

            try
            {
                CompileEffectAndGetVariable(SpriteShaderSource, "Tex");
                CreateInputLayout(SpriteVertexLayout.Description);
                CreateVertexBuffer(SpriteVertexLayout.Struct.SizeInBytes, bufferSize);
                CreateDepthStencilAndBlendState();
                RefreshViewport();
            }
            catch { }

        }

        /// <summary>
        /// Queries the current viewport from the device.
        /// </summary>
        /// <returns>The current viewport</returns>
        protected abstract STRViewport QueryViewport();

        /// <summary>
        /// Updates the viewport used for absolute positioning. The first current viewport of the device's rasterizer will be used.
        /// </summary>
        public void RefreshViewport()
        {
            try { viewport = QueryViewport(); } catch { FailedViewPort = true; }
        }

        /// <summary>
        /// Closes a reorder session. Further draw calls will not be drawn together with previous draw calls.
        /// </summary>
        public void ClearReorderBuffer()
        {
            textureSprites.Clear();
        }

        private STRVector ConvertCoordinate(STRVector coordinate, CoordinateType coordinateType)
        {
            switch (coordinateType)
            {
                case CoordinateType.SNorm:
                    return coordinate;
                case CoordinateType.UNorm:
			        coordinate.X = (coordinate.X - 0.5f) * 2;
                    coordinate.Y = -(coordinate.Y - 0.5f) * 2;
                    return coordinate;
                case CoordinateType.Relative:
                    coordinate.X = coordinate.X / ScreenSize.X * 2 - 1;
                    coordinate.Y = -(coordinate.Y / ScreenSize.Y * 2 - 1);
                    return coordinate;
                case SpriteTextRenderer.CoordinateType.Absolute:                   
                    coordinate.X = coordinate.X / viewport.Width * 2 - 1;
                    coordinate.Y = -(coordinate.Y / viewport.Height * 2 - 1);
                    return coordinate;
            }
            return STRVector.Zero;
        }

        /// <summary>
        /// Draws a complete texture on the screen.
        /// </summary>
        /// <param name="texture">The shader resource view of the texture to draw</param>
        /// <param name="position">Position of the top left corner of the texture in the chosen coordinate system</param>
        /// <param name="size">Size of the texture in the chosen coordinate system. The size is specified in the screen's coordinate system.</param>
        /// <param name="coordinateType">A custom coordinate system in which to draw the texture</param>
        protected internal void Draw(object texture, STRVector position, STRVector size, CoordinateType coordinateType)
        {
            Draw(texture, position, size, STRVector.Zero, 0, coordinateType);
        }

        /// <summary>
        /// Draws a complete texture on the screen.
        /// </summary>
        /// <param name="texture">The shader resource view of the texture to draw</param>
        /// <param name="position">Position of the top left corner of the texture in the chosen coordinate system</param>
        /// <param name="size">Size of the texture in the chosen coordinate system. The size is specified in the screen's coordinate system.</param>
        /// <param name="center">Specify the texture's center in the chosen coordinate system. The center is specified in the texture's local coordinate system. E.g. for <paramref name="coordinateType"/>=CoordinateType.SNorm, the texture's center is defined by (0, 0).</param>
        /// <param name="rotationAngle">The angle in radians to rotate the texture. Positive values mean counter-clockwise rotation. Rotations can only be applied for relative or absolute coordinates. Consider using the Degrees or Radians helper structs.</param>
        /// <param name="coordinateType">A custom coordinate system in which to draw the texture</param>
        protected internal void Draw(object texture, STRVector position, STRVector size, STRVector center, double rotationAngle, CoordinateType coordinateType)
        {
            Draw(texture, position, size, center, rotationAngle, new STRColor(1, 1, 1, 1), coordinateType);
        }

        /// <summary>
        /// Draws a complete texture on the screen.
        /// </summary>
        /// <param name="texture">The shader resource view of the texture to draw</param>
        /// <param name="position">Position of the top left corner of the texture in the chosen coordinate system</param>
        /// <param name="size">Size of the texture in the chosen coordinate system. The size is specified in the screen's coordinate system.</param>
        /// <param name="coordinateType">A custom coordinate system in which to draw the texture</param>
        /// <param name="color">The color with which to multiply the texture</param>
        protected internal void Draw(object texture, STRVector position, STRVector size, STRColor color, CoordinateType coordinateType)
        {
            Draw(texture, position, size, STRVector.Zero, 0, STRVector.Zero, new STRVector(1, 1), color, coordinateType);
        }

        /// <summary>
        /// Draws a complete texture on the screen.
        /// </summary>
        /// <param name="texture">The shader resource view of the texture to draw</param>
        /// <param name="position">Position of the top left corner of the texture in the chosen coordinate system</param>
        /// <param name="size">Size of the texture in the chosen coordinate system. The size is specified in the screen's coordinate system.</param>
        /// <param name="center">Specify the texture's center in the chosen coordinate system. The center is specified in the texture's local coordinate system. E.g. for <paramref name="coordinateType"/>=CoordinateType.SNorm, the texture's center is defined by (0, 0).</param>
        /// <param name="rotationAngle">The angle in radians to rotate the texture. Positive values mean counter-clockwise rotation. Rotations can only be applied for relative or absolute coordinates. Consider using the Degrees or Radians helper structs.</param>
        /// <param name="coordinateType">A custom coordinate system in which to draw the texture</param>
        /// <param name="color">The color with which to multiply the texture</param>
        protected internal void Draw(object texture, STRVector position, STRVector size, STRVector center, double rotationAngle, STRColor color, CoordinateType coordinateType)
        {
            Draw(texture, position, size, center, rotationAngle, STRVector.Zero, new STRVector(1, 1), color, coordinateType);
        }

        private STRVector Rotate(STRVector v, float sine, float cosine)
        {
            return new STRVector(cosine * v.X + sine * v.Y, -sine * v.X + cosine * v.Y);
        }

        /// <summary>
        /// Draws a region of a texture on the screen.
        /// </summary>
        /// <param name="texture">The shader resource view of the texture to draw</param>
        /// <param name="position">Position of the center of the texture in the chosen coordinate system</param>
        /// <param name="size">Size of the texture in the chosen coordinate system. The size is specified in the screen's coordinate system.</param>
        /// <param name="center">Specify the texture's center in the chosen coordinate system. The center is specified in the texture's local coordinate system. E.g. for <paramref name="coordinateType"/>=CoordinateType.SNorm, the texture's center is defined by (0, 0).</param>
        /// <param name="rotationAngle">The angle in radians to rotate the texture. Positive values mean counter-clockwise rotation. Rotations can only be applied for relative or absolute coordinates. Consider using the Degrees or Radians helper structs.</param>
        /// <param name="coordinateType">A custom coordinate system in which to draw the texture</param>
        /// <param name="color">The color with which to multiply the texture</param>
        /// <param name="texCoords">Texture coordinates for the top left corner</param>
        /// <param name="texCoordsSize">Size of the region in texture coordinates</param>
        protected internal void Draw(object texture, STRVector position, STRVector size, STRVector center, double rotationAngle, STRVector texCoords, STRVector texCoordsSize, STRColor color, CoordinateType coordinateType)
        {
            if (texture == null) { return;  }

            size.X = Math.Abs(size.X);
            size.Y = Math.Abs(size.Y);

            //Difference vectors from the center to the texture edges (in screen coordinates).
            STRVector left, up, right, down;
            if (coordinateType == CoordinateType.UNorm)
            {
                left = new STRVector(0 - center.X * size.X, 0);
                up = new STRVector(0, 0 - center.Y * size.Y);
                right = new STRVector((1 - center.X) * size.X, 0);
                down = new STRVector(0, (1 - center.Y) * size.Y);
            }
            else if (coordinateType == CoordinateType.SNorm)
            {
                left = new STRVector((-1 - center.X) * size.X / 2, 0);
                up = new STRVector(0, (1 - center.Y) * size.Y / 2);
                right = new STRVector((1 - center.X) * size.X / 2, 0);
                down = new STRVector(0, (-1 - center.Y) * size.Y / 2);
            }
            else
            {
                left = new STRVector(-center.X, 0);
                up = new STRVector(0, -center.Y);
                right = new STRVector(size.X - center.X, 0);
                down = new STRVector(0, size.Y - center.Y);
            }
            //.WriteLine("Vector Setup");
            if (rotationAngle != 0)
            {
                if (coordinateType != CoordinateType.Absolute && coordinateType != CoordinateType.Relative)
                {
                    //Normalized coordinates tend to be skewed when applying rotation
                    throw new ArgumentException("Rotation is only allowed for relative or absolute coordinates", "rotationAngle");
                }
                float sine = (float)Math.Sin(rotationAngle);
                float cosine = (float)Math.Cos(rotationAngle);
                left = Rotate(left, sine, cosine);
                right = Rotate(right, sine, cosine);
                up = Rotate(up, sine, cosine);
                down = Rotate(down, sine, cosine);
            }
            //.WriteLine("SpriteVertexLayout Setup");
            var data = new SpriteVertexLayout.Struct();
            //.WriteLine("data Setup");
            data.TexCoord = texCoords;
            //.WriteLine("texCoords Setup");
            data.TexCoordSize = texCoordsSize;
            //.WriteLine("texCoordsSize Setup");
            data.Color = color.ToArgb();
            //.WriteLine("color Setup");

            //.WriteLine("position " + position.X);
            //.WriteLine("up " + up.X);
            //.WriteLine("left " + left.X);

            if (viewport == null) {
                if (ScreenSize.X == 0 && ScreenSize.Y == 0) {
                    coordinateType = CoordinateType.UNorm;
                        } else { 
                    coordinateType = CoordinateType.Relative; 
                }
            }

            //.WriteLine("coordinateType " + coordinateType.ToString());

            data.TopLeft = ConvertCoordinate(position + up + left, coordinateType);
            //.WriteLine("ConvertCoordinate 1");
            data.TopRight = ConvertCoordinate(position + up + right, coordinateType);
            //.WriteLine("ConvertCoordinate 2");
            data.BottomLeft = ConvertCoordinate(position + down + left, coordinateType);
            //.WriteLine("ConvertCoordinate 3");
            data.BottomRight = ConvertCoordinate(position + down + right, coordinateType);
            //.WriteLine("AllowReorder Setup");
            if (AllowReorder)
            {
                //Is there already a sprite for this texture?
                if (textureSprites.ContainsKey(texture))
                {
                    //Add the sprite to the last segment for this texture
                    var Segment = textureSprites[texture].Last();
                    AddIn(Segment, data);
                }
                else
                    //Add a new segment for this texture
                    AddNew(texture, data);
            }
            else
                //Add a new segment for this texture
                AddNew(texture, data);
        }

        private void AddNew(object texture, SpriteVertexLayout.Struct data)
        {
            //Create new segment with initial values
            var newSegment = new SpriteSegment();
            newSegment.Texture = texture;
            newSegment.Sprites.Add(data);
            sprites.Add(newSegment);

            //Create reference for segment in dictionary
            if (!textureSprites.ContainsKey(texture))
                textureSprites.Add(texture, new List<SpriteSegment>());

            textureSprites[texture].Add(newSegment);
            spriteCount++;
            CheckForFullBuffer();
        }

        /// <summary>
        /// If the buffer is full, then draw all sprites and clear it.
        /// </summary>
        private void CheckForFullBuffer()
        {            
            if (spriteCount >= bufferSize)
                Flush();
        }

        private void AddIn(SpriteSegment segment, SpriteVertexLayout.Struct data)
        {
            segment.Sprites.Add(data);
            spriteCount++;
            CheckForFullBuffer();
        }

        /// <summary>
        /// Copies the vertices to the vertex buffer.
        /// </summary>
        /// <typeparam name="T">The vertices' type</typeparam>
        /// <param name="vertices">The vertices to copy</param>
        protected abstract void UpdateVertexBufferData<T>(T[] vertices) where T : struct;

        /// <summary>
        /// Initializes the rendering by setting the input layout, primitive topology and vertex buffer.
        /// </summary>
        protected abstract void InitRendering();

        /// <summary>
        /// Draws the specified range of points with the specified texture.
        /// </summary>
        /// <param name="texture">The texture to draw</param>
        /// <param name="count">The number of vertices to draw</param>
        /// <param name="offset">The offset of the first vertex from the vertex buffer start</param>
        protected abstract void Draw(object texture, int count, int offset);

        /// <summary>
        /// Gets or sets the device's current blend state. The type is the specific library's type.
        /// </summary>
        protected abstract object CurrentBlendState { get; set; }

        /// <summary>
        /// Gets or sets the device's current depth stencil state. The type is the specific library's type.
        /// </summary>
        protected abstract object CurrentDepthStencilState { get; set; }

        /// <summary>
        /// This method causes the SpriteRenderer to immediately draw all buffered sprites.
        /// </summary>
        /// <remarks>
        /// This method should be called at the end of a frame in order to draw the last sprites that are in the buffer.
        /// </remarks>
        public void Flush()
        {
            if (spriteCount == 0)
                return;

            if(LockDeviceOnDraw)
                Monitor.Enter(DeviceRef);

            try
            {
                //Update DepthStencilState if necessary
                object oldDSState = null;
                object oldBlendState = null;
                if (HandleDepthStencilState)
                {
                    oldDSState = CurrentDepthStencilState;
                    CurrentDepthStencilState = depthStencilState;
                }
                if (HandleBlendState)
                {
                    oldBlendState = CurrentBlendState;
                    CurrentBlendState = blendState;
                }

                //Construct vertexbuffer
                UpdateVertexBufferData(sprites.SelectMany(s => s.Sprites).ToArray());


                //Initialize render calls
                InitRendering();

                //Draw
                int offset = 0;
                foreach (var segment in sprites)
                {
                    int count = segment.Sprites.Count;
                    Draw(segment.Texture, count, offset);
                    offset += count;
                }

                if (HandleDepthStencilState)
                {
                    CurrentDepthStencilState = oldDSState;
                }
                if (HandleBlendState)
                {
                    CurrentBlendState = oldBlendState;
                }
            }
            finally
            {
                if(LockDeviceOnDraw)
                    Monitor.Exit(DeviceRef);
            }
            
            //Reset buffers
            spriteCount = 0;
            sprites.Clear();
            textureSprites.Clear();
        }

        #region IDisposable Support
        /// <summary>
        /// Dispose of the resources that have been created by the specific renderer, except for blend and depth stencil state.
        /// </summary>
        protected abstract void DisposeOfResources();

        private bool disposed = false;
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //There are no managed resources to dispose
                }

                DisposeOfResources();
                depthStencilState.Dispose();
                blendState.Dispose();
            }
            this.disposed = true;
        }

        /// <summary>
        /// Disposes of the SpriteRenderer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
