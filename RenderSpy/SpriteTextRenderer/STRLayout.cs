using System;

namespace SpriteTextRenderer
{
    /// <summary>
    /// The sprite text renderer uses a library-independent layout struct for characters.
    /// </summary>
    public struct STRLayout
    {
        public STRVector TopLeft { get; set; }

        public STRVector LayoutSize { get; set; }

        public STRVector Size { get; set; }

        public float WidthIncludingTrailingWhitespaces { get; set; }

        public float OverhangLeft { get; set; }

        public float OverhangRight { get; set; }

        public float OverhangTop { get; set; }

        public float OverhangBottom { get; set; }

        public IDisposable TextLayout { get; set; }
    }
}
