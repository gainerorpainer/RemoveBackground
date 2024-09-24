using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RemoveBackground
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

        public unsafe RawBitmap(Bitmap input) : this(input.Width, input.Height)
        {
            // memcpy
            var inputData = input.LockBits(new Rectangle(new Point(), input.Size), ImageLockMode.ReadOnly | ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            for (int i = 0; i < Width * Height; i++)
                RawData[i] = ((uint*)inputData.Scan0)[i];
            input.UnlockBits(inputData);
        }

        public uint GetPixel(Point point) => RawData[point.X + point.Y * Width];

        void IDisposable.Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            GC.SuppressFinalize(this);
            BitsHandle.Free();
        }
    }
}
