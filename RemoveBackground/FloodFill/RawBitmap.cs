using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace RemoveBackground.FloodFill
{
    public class RawBitmap : IDisposable
    {
        public bool Disposed { get; private set; }
        public Bitmap Bitmap { get; private set; }

        public readonly int Width;
        public readonly int Height;
        public readonly uint[] RawData;
        private GCHandle BitsHandle;

        public RawBitmap(int width, int height)
        {
            Width = width;
            Height = height;

            RawData = new uint[Width * Height];

            BitsHandle = GCHandle.Alloc(RawData, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());
        }

        void IDisposable.Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            GC.SuppressFinalize(this);
            BitsHandle.Free();
        }

        public unsafe RawBitmap(Bitmap input, bool clearAlpha = true) : this(input.Width, input.Height)
        {
            // memcpy
            var inputData = input.LockBits(new Rectangle(new Point(), input.Size), ImageLockMode.ReadOnly | ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            if (clearAlpha)
                for (int i = 0; i < Width * Height; i++)
                    RawData[i] = ((uint*)inputData.Scan0)[i] & Constants.RGB_MASK;
            else
                for (int i = 0; i < Width * Height; i++)
                    RawData[i] = ((uint*)inputData.Scan0)[i];
            input.UnlockBits(inputData);
        }

        /// <summary>
        /// Determine linear index from x,y coordinate
        /// </summary>
        /// <param name="point">Input coordinates</param>
        /// <returns>Output Index</returns>
        public int GetIndex(Point point) => point.X + point.Y * Width;

        /// <summary>
        /// Return the image cropped to a rectangle
        /// </summary>
        /// <param name="crop">cropping regions</param>
        /// <returns>Bitmap containing the cropped region</returns>
        public Bitmap Crop(Rectangle crop) => Bitmap.Clone(crop, PixelFormat.Format32bppArgb);

        /// <summary>
        /// Return the image cropped to only the visible pixels
        /// </summary>
        /// <returns></returns>
        public unsafe Bitmap Crop()
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            fixed (uint* pixels = RawData)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int i = x + y * Width;

                        // take advantage of the fact that alpha is most significant byte
                        if (pixels[i] <= Constants.RGB_MASK)
                            // has alpha value of 0
                            continue;

                        if (x < minX)
                            minX = x;
                        if (y < minY)
                            minY = y;
                        if (x > maxX)
                            maxX = x;
                        if (y > maxY)
                            maxY = y;
                    }
                }
            }

            // crop fails if all pixels are invisible
            if (minX == int.MaxValue)
                return Bitmap;

            Rectangle roi = new(minX, minY, maxX - minX, maxY - minY);
            return Bitmap.Clone(roi, PixelFormat.Format32bppArgb);
        }

        /// <summary>
        /// Inverts the alpha channel, and crops the image to the visible pixels
        /// </summary>
        /// <returns></returns>
        public unsafe Bitmap IntervertAndCrop()
        {
            var copy = new RawBitmap(Bitmap, clearAlpha: false);
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;

            fixed (uint* pixels = copy.RawData)
            {
                for (int y = 0; y < copy.Height; y++)
                {
                    for (int x = 0; x < copy.Width; x++)
                    {
                        int i = x + y * copy.Width;
                        uint alphaInverted = Constants.ALPHA_VISIBLE ^ pixels[i] & ~Constants.RGB_MASK;
                        pixels[i] = pixels[i] & Constants.RGB_MASK | alphaInverted;

                        if (alphaInverted == 0)
                            continue;

                        if (x < minX)
                            minX = x;
                        if (y < minY)
                            minY = y;
                        if (x > maxX)
                            maxX = x;
                        if (y > maxY)
                            maxY = y;
                    }
                }
            }

            // crop fails if all pixels are invisible
            if (minX == int.MaxValue)
                return copy.Bitmap;
            // crop
            Rectangle roi = new(minX, minY, maxX - minX, maxY - minY);
            return copy.Bitmap.Clone(roi, PixelFormat.Format32bppArgb);
        }
    }
}
