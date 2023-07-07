using System.Linq;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.Direct3D;

namespace SpriteTextRenderer.SharpDX
{
    /// <summary>
    /// This class is responsible for rendering 2D sprites using SharpDX. Typically, only one instance of this class is necessary.
    /// </summary>
    public class SpriteRenderer : SpriteTextRenderer.SpriteRenderer
    {



        private Device device;
        /// <summary>
        /// Returns the Direct3D device that this SpriteRenderer was created for.
        /// </summary>
        public Device Device { get { return device; } }
        private DeviceContext context;

        //public override object DeviceRef { get { return device; } }

        protected internal override object DeviceRef => device;

        protected override object CurrentBlendState
        {
            get { return Device.ImmediateContext.OutputMerger.BlendState; }
            set { Device.ImmediateContext.OutputMerger.BlendState = (BlendState)value; }
        }

        protected override object CurrentDepthStencilState
        {
            get { return Device.ImmediateContext.OutputMerger.DepthStencilState; }
            set { Device.ImmediateContext.OutputMerger.DepthStencilState = (DepthStencilState)value; }
        }

        public SpriteRenderer(Device device, int bufferSize = 128)
            : base(bufferSize)
        {
            this.device = device;
            this.context = device.ImmediateContext;

            Initialize();
        }

        public SpriteRenderer(Device device, string SpriteShaderSource, int bufferSize = 128)
         : base(bufferSize)
        {
            this.device = device;
            this.context = device.ImmediateContext;

            Initialize(SpriteShaderSource);
        }

        #region ### Private SharpDX members
        Effect fx;
        EffectPass pass;
        EffectShaderResourceVariable textureVariable;
        InputLayout inputLayout;

        Buffer vb;
        VertexBufferBinding vbBinding;
        #endregion

        #region ### Public draw interface ###

        /// <summary>
        /// Draws a complete texture on the screen.
        /// </summary>
        /// <param name="texture">The shader resource view of the texture to draw</param>
        /// <param name="position">Position of the top left corner of the texture in the chosen coordinate system</param>
        /// <param name="size">Size of the texture in the chosen coordinate system. The size is specified in the screen's coordinate system.</param>
        /// <param name="coordinateType">A custom coordinate system in which to draw the texture</param>
        public void Draw(ShaderResourceView texture, Vector2 position, Vector2 size, CoordinateType coordinateType)
        {
            base.Draw(texture, position.ToSTRVector(), size.ToSTRVector(), coordinateType);
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
        public void Draw(ShaderResourceView texture, Vector2 position, Vector2 size, Vector2 center, double rotationAngle, CoordinateType coordinateType)
        {
            base.Draw(texture, position.ToSTRVector(), size.ToSTRVector(), center.ToSTRVector(), rotationAngle, coordinateType);
        }

        /// <summary>
        /// Draws a complete texture on the screen.
        /// </summary>
        /// <param name="texture">The shader resource view of the texture to draw</param>
        /// <param name="position">Position of the top left corner of the texture in the chosen coordinate system</param>
        /// <param name="size">Size of the texture in the chosen coordinate system. The size is specified in the screen's coordinate system.</param>
        /// <param name="coordinateType">A custom coordinate system in which to draw the texture</param>
        /// <param name="color">The color with which to multiply the texture</param>
        public void Draw(ShaderResourceView texture, Vector2 position, Vector2 size, Color4 color, CoordinateType coordinateType)
        {
            base.Draw(texture, position.ToSTRVector(), size.ToSTRVector(), color.ToSTRColor(), coordinateType);
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
        public void Draw(ShaderResourceView texture, Vector2 position, Vector2 size, Vector2 center, double rotationAngle, Color4 color, CoordinateType coordinateType)
        {
            base.Draw(texture, position.ToSTRVector(), size.ToSTRVector(), center.ToSTRVector(), rotationAngle, color.ToSTRColor(), coordinateType);
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
        public void Draw(ShaderResourceView texture, Vector2 position, Vector2 size, Vector2 center, double rotationAngle, Vector2 texCoords, Vector2 texCoordsSize, Color4 color, CoordinateType coordinateType)
        {
            base.Draw(texture, position.ToSTRVector(), size.ToSTRVector(), center.ToSTRVector(), rotationAngle, texCoords.ToSTRVector(), texCoordsSize.ToSTRVector(), color.ToSTRColor(), coordinateType);
        }
        
        /// <summary>
        /// Draws a region of a texture on the screen.
        /// </summary>
        /// <param name="texture">The shader resource view of the texture to draw</param>
        /// <param name="position">Position of the center of the texture in the chosen coordinate system</param>
        /// <param name="size">Size of the texture in the chosen coordinate system. The size is specified in the screen's coordinate system.</param>
        /// <param name="coordinateType">A custom coordinate system in which to draw the texture</param>
        /// <param name="color">The color with which to multiply the texture</param>
        /// <param name="texCoords">Texture coordinates for the top left corner</param>
        /// <param name="texCoordsSize">Size of the region in texture coordinates</param>
        public void Draw(ShaderResourceView texture, Vector2 position, Vector2 size, Vector2 texCoords, Vector2 texCoordsSize, Color4 color, CoordinateType coordinateType)
        {
            base.Draw(texture, position.ToSTRVector(), size.ToSTRVector(), STRVector.Zero, 0.0, texCoords.ToSTRVector(), texCoordsSize.ToSTRVector(), color.ToSTRColor(), coordinateType);
        }
        #endregion

        #region ### Template method hooks ###

        protected override STRViewport QueryViewport()
        {
            return device.ImmediateContext.Rasterizer.GetViewports<ViewportF>()[0].ToSTRViewport();
        }

        protected override void CompileEffectAndGetVariable(string hlslSource, string variableName)
        {

            ShaderFlags flags = ShaderFlags.None;

            #if DEBUG
                flags |= ShaderFlags.Debug | ShaderFlags.SkipOptimization;
            #else
                flags |= ShaderFlags.OptimizationLevel3;
            #endif

            using (var code = ShaderBytecode.Compile(hlslSource, "fx_5_0", flags, EffectFlags.None))
            {
                this.fx = new Effect(device, code);
            }
            
            this.pass = fx.GetTechniqueByIndex(0).GetPassByIndex(0);
            textureVariable = fx.GetVariableByName(variableName).AsShaderResource();
        }

        protected override void CreateInputLayout(STRInputElement[] elements)
        {
            var specificElements = elements.Select(e => e.ToSharpDXInputElement()).ToArray();
            inputLayout = new InputLayout(device, pass.Description.Signature, specificElements);
            inputLayout.DebugName = "Input Layout for Sprites";
        }

        protected override void CreateVertexBuffer(int elementByteSize, int elements)
        {
            vb = new Buffer(device, elementByteSize * elements, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, elementByteSize);
            vb.DebugName = "Sprites Vertexbuffer";
            vbBinding = new VertexBufferBinding(vb, elementByteSize, 0);
        }

        protected override void CreateDepthStencilAndBlendState()
        {
            var dssd = new DepthStencilStateDescription()
            {
                IsDepthEnabled = false,
                DepthWriteMask = DepthWriteMask.Zero
            };
            depthStencilState = new DepthStencilState(Device, dssd);

            var blendDesc = new BlendStateDescription();
            blendDesc.AlphaToCoverageEnable = false;
            blendDesc.IndependentBlendEnable = false;
            blendDesc.RenderTarget[0]. BlendOperation = BlendOperation.Add;
            blendDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            blendDesc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            blendDesc.RenderTarget[0].IsBlendEnabled= true;
            blendDesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            blendDesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
            blendDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
            blendDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
            blendState = new BlendState(device, blendDesc);
        }

        protected override void UpdateVertexBufferData<T>(T[] vertices)
        {

            DataStream ds;
            var data = context.MapSubresource(vb, MapMode.WriteDiscard, MapFlags.None, out ds);
            ds.WriteRange(vertices);
            ds.Dispose();
            context.UnmapSubresource(vb, 0);
        }

        protected override void InitRendering()
        {
            device.ImmediateContext.InputAssembler.InputLayout = inputLayout;
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0, vbBinding);
        }

        protected override void Draw(object texture, int count, int offset)
        {
            textureVariable.SetResource((ShaderResourceView)texture);
            pass.Apply(context);
            device.ImmediateContext.Draw(count, offset);
        }

        protected override void DisposeOfResources()
        {
            fx.Dispose();
            inputLayout.Dispose();
            vb.Dispose();
        }
        #endregion

        protected override void UpdateAlphaBlend()
        {
            this.pass = fx.GetTechniqueByIndex((int)AlphaBlendMode).GetPassByIndex(0);
        }
    }
}
