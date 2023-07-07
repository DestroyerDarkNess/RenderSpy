using System.Numerics;

namespace SpriteTextRenderer
{
    /// <summary>
    /// Represents an abstract command to draw a char
    /// </summary>
    public struct CharRenderCall
    {
        /// <summary>
        /// The position of the char
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The library-specific text layout of the char
        /// </summary>
        public object TextLayout { get; set; }
    }
}
