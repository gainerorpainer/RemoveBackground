using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RemoveBackground.FloodFill
{
    public class RawBitmap : IDisposable
    {
        public bool Disposed { get; private set; }

        public Bitmap Bitmap { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public uint[] RawData { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

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

        public unsafe RawBitmap(Bitmap input) : this(input.Width, input.Height)
        {
            // memcpy
            var inputData = input.LockBits(new Rectangle(new Point(), input.Size), ImageLockMode.ReadOnly | ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            for (int i = 0; i < Width * Height; i++)
                RawData[i] = ((uint*)inputData.Scan0)[i];
            input.UnlockBits(inputData);
        }

        public uint GetPixel(Point point) => RawData[point.X + point.Y * Width];

        public void SetPixel(Point point, uint color) => RawData[point.X + point.Y * Width] = color;

        internal Bitmap GetCropped(Rectangle crop) => Bitmap.Clone(crop, PixelFormat.Format32bppArgb);

        internal Bitmap GetInvertedAndCropped()
        {
            var copy = new RawBitmap(Bitmap);
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
            unsafe
            {
                fixed (uint* pixels = copy.RawData)
                {
                    for (int y = 0; y < copy.Height; y++)
                    {
                        for (int x = 0; x < copy.Width; x++)
                        {
                            int i = x + y * copy.Width;
                            uint alphaInverted = Constants.MAX_ALPHA ^ pixels[i] & ~Constants.RGB_MASK;
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
            }
            // crop fails if alll pixels are invisible
            if (minX == int.MaxValue)
                return copy.Bitmap;
            // crop
            Rectangle roi = new(minX, minY, maxX - minX, maxY - minY);
            return copy.Bitmap.Clone(roi, PixelFormat.Format32bppArgb);
        }
    }
}
