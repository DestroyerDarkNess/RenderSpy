namespace SpriteTextRenderer
{
    /// <summary>
    /// The SpriteTextRenderer uses a library-independent color structure because the base lib must not depend on platform specific libraries
    /// </summary>
    public class STRColor
    {
        public float Red;
        public float Green;
        public float Blue;
        public float Alpha;

        public STRColor() { }

        public STRColor(float a, float r, float g, float b)
        {
            this.Red = r;
            this.Green = g;
            this.Blue = b;
            this.Alpha = a;
        }

        public int ToArgb()
        {
            uint a, r, g, b;

            a = (uint)(Alpha * 255.0f);
            r = (uint)(Red * 255.0f);
            g = (uint)(Green * 255.0f);
            b = (uint)(Blue * 255.0f);

            uint value = b;
            value += g << 8;
            value += r << 16;
            value += a << 24;

            return (int)value;
        }
    }
}
