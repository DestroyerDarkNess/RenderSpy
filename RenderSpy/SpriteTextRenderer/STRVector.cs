using System.Runtime.InteropServices;

namespace SpriteTextRenderer
{
    /// <summary>
    /// The SpriteTextRenderer uses a library-independent vector structure because the base lib must not depend on platform specific libraries
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STRVector
    {
        public static STRVector Zero = new STRVector(0, 0);

        public float X;
        public float Y;

        public STRVector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static STRVector operator +(STRVector lhs, STRVector rhs)
        {
            return new STRVector(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }
    }
}
