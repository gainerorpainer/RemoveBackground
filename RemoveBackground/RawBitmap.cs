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
        public Bitmap Bitmap { get; private set; }
        public int[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }
        protected BitmapData Data { get; private set; }

        public RawBitmap(int width, int height)
        {
            Width = width;
            Height = height;

            Bits = new int[Width * Height];

            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());
            Data = Bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly | ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        }

        public unsafe RawBitmap(Bitmap input) : this(input.Width, input.Height)
        {
            // crude memcpy
            var inputData = input.LockBits(new Rectangle(new Point(), input.Size), ImageLockMode.ReadOnly | ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            for (int y = 0; y < input.Height; y++)
                for (int x = 0; x < input.Width; x++)
                    Bits[x + y * Width] = ((int*)inputData.Scan0)[x + y * Width];

            input.UnlockBits(inputData);
        }

        public unsafe void SetPixel(int x, int y, Color colour)
        {
            fixed (int* pix0 = Bits)
                pix0[x + y * Width] = colour.ToArgb();
        }

        public unsafe Color GetPixel(in Point p)
        {
            fixed (int* pix0 = Bits)
                return Color.FromArgb(pix0[p.X + p.Y * Width]);
        }

        void IDisposable.Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            GC.SuppressFinalize(this);
            BitsHandle.Free();
        }
    }
}
