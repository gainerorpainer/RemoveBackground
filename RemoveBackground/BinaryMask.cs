namespace RemoveBackground
{
    public class BinaryMask(int width, int height)
    {
        public int Height { get; private set; } = height;
        public int Width { get; private set; } = width;

        protected ulong[] Bits { get; private set; } = new ulong[width / 64 * height];

        public unsafe void SetPixel(int x, int y, bool value)
        {
            fixed (ulong* pix0 = Bits)
                if (value)
                    pix0[x / 64 + (y * Width)] |= 1ul << (x % 64);
                else
                    pix0[x / 64 + (y * Width)] &= ~(1ul << (x % 64));
        }

        public unsafe bool GetPixel(int x, int y)
        {
            bool result;
            fixed (ulong* pix0 = Bits)
                result = (pix0[x / 64 + (y * Width)] & (1ul << (x % 64))) != 0ul;

            return result;
        }
    }
}
