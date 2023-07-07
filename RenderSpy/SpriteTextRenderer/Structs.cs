using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpriteTextRenderer
{
    internal class SpriteVertexLayout
    {
        internal static STRInputElement[] Description = {
            new STRInputElement("TEXCOORD", STRFormat.R32G32_Float, 0),
            new STRInputElement("TEXCOORDSIZE", STRFormat.R32G32_Float, 8),
            new STRInputElement("COLOR", STRFormat.B8G8R8A8_UNorm, 16),
            new STRInputElement("TOPLEFT", STRFormat.R32G32_Float, 20),
            new STRInputElement("TOPRIGHT", STRFormat.R32G32_Float, 28),
            new STRInputElement("BOTTOMLEFT", STRFormat.R32G32_Float, 36),
            new STRInputElement("BOTTOMRIGHT", STRFormat.R32G32_Float, 44)};

        internal struct Struct
        {
            internal STRVector TexCoord;
            internal STRVector TexCoordSize;
            internal int Color;
            internal STRVector TopLeft;
            internal STRVector TopRight;
            internal STRVector BottomLeft;
            internal STRVector BottomRight;

            internal static int SizeInBytes { get { return Marshal.SizeOf(typeof(Struct)); } }
        }
    }

    /// <summary>
    /// This structure holds data for sprites with a specific texture
    /// </summary>
    internal class SpriteSegment
    {
        /// <summary>
        /// The ShaderResourceView
        /// </summary>
        internal object Texture;
        internal List<SpriteVertexLayout.Struct> Sprites = new List<SpriteVertexLayout.Struct>();
    }

    internal class CharTableDescription
    {
        /// <summary>
        /// A Texture2D
        /// </summary>
        internal IDisposable Texture = null;
        internal IDisposable SRV;
        internal CharDescription[] Chars = new CharDescription[256];
    }

    internal class CharDescription
    {
        /// <summary>
        /// Size of the char excluding overhangs
        /// </summary>
        internal STRVector CharSize;
        internal float OverhangLeft, OverhangRight, OverhangTop, OverhangBottom;

        internal STRVector TexCoordsStart;
        internal STRVector TexCoordsSize;

        internal CharTableDescription TableDescription;

        internal StringMetrics ToStringMetrics(STRVector position, float scalX, float scalY)
        {
            return new StringMetrics
            {
                TopLeft = position,
                Size = new STRVector(CharSize.X * scalX, CharSize.Y * scalY),
                OverhangTop = Math.Abs(scalY) * OverhangTop,
                OverhangBottom = Math.Abs(scalY) * OverhangBottom,
                OverhangLeft = Math.Abs(scalX) * OverhangLeft,
                OverhangRight = Math.Abs(scalX) * OverhangRight,
            };
        }
    }    
}
