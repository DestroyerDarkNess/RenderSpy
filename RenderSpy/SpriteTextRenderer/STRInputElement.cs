namespace SpriteTextRenderer
{
    /// <summary>
    /// The SpriteTextRenderer uses a library-independent input element structure because the base lib must not depend on platform specific libraries
    /// </summary>
    public struct STRInputElement
    {
        public string Semantic { get; set; }
        public STRFormat Format { get; set; }
        public int Offset { get; set; }

        public STRInputElement(string semantic, STRFormat format, int offset) : this()
        {
            this.Semantic = semantic;
            this.Format = format;
            this.Offset = offset;
        }
    }

    /// <summary>
    /// Defines the format of an input element
    /// </summary>
    public enum STRFormat
    {
        R32G32_Float,
        B8G8R8A8_UNorm,
    }
}
