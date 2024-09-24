using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoveBackground.FloodFill
{
    internal unsafe class RingBuffer
    {
        public RingBuffer(int* data)
        {
            Data = data;
            WritePos = ReadPos = 0;
        }

        public int* Data { get; private set; }
        public nint WritePos { get; set; }
        public nint ReadPos { get; set; }
    }

    internal unsafe class BackgroundWorker(Size size, uint* Pixels, RingBuffer RingBuffer, float absThreshold, uint refColor)
    {
        const int ITEMS_PER_WORKER = 1000;

        private readonly int Width = size.Width;
        private readonly int Height = size.Height;

        private static int GetColorDifference(uint color1, uint color2)
        {
            unchecked
            {
                // decompose argb into r,g,b
                int r1 = (int)((color1 & 0x00FF0000) >> 16);
                int r2 = (int)((color2 & 0x00FF0000) >> 16);
                int g1 = (int)((color1 & 0x0000FF00) >> 08);
                int g2 = (int)((color2 & 0x0000FF00) >> 08);
                int b1 = (int)((color1 & 0x000000FF) >> 00);
                int b2 = (int)((color2 & 0x000000FF) >> 00);

                // intermediates
                int r_hat = r1 / 2 + r2 / 2;
                int deltaR = r1 - r2;
                int deltaG = g1 - g2;
                int deltaB = b1 - b2;

                if (r_hat < 128)
                    return 2 * deltaR * deltaR + 4 * deltaG * deltaG + 3 * deltaB * deltaB;

                return 3 * deltaR * deltaR + 4 * deltaG * deltaG + 2 * deltaB * deltaB;
            }
        }

        public unsafe Rectangle DoWork()
        {
            // roi state vars
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            int* workItems = stackalloc int[ITEMS_PER_WORKER];
            int* recursiveWorkItems = stackalloc int[ITEMS_PER_WORKER * 8];

            // thread lives until ring buffer is full
            while (RingBuffer.ReadPos != RingBuffer.WritePos)
            {
                // pick up X items per worker
                int numItems = 0;
                lock (RingBuffer)
                {
                    for (int i = 0; i < ITEMS_PER_WORKER; i++)
                    {
                        if (RingBuffer.ReadPos == RingBuffer.WritePos)
                            // nothing to read from buffer
                            break;

                        workItems[numItems++] = RingBuffer.Data[RingBuffer.ReadPos];
                        RingBuffer.ReadPos = (RingBuffer.ReadPos + 1) % Constants.RINGBUFFER_SIZE;
                    }
                }

                // each work item can create up to 8 recursive items
                int numRecursiveItems = 0;

                // work on items
                for (int i = 0; i < numItems; i++)
                {
                    int index = workItems[i];
                    uint color = Pixels[index];

                    // break out if already visited
                    if (color > 0x00FFFFFFu)
                        continue;

                    // mark as visited
                    Pixels[index] = color & Constants.RGB_MASK | Constants.MAX_ALPHA;

                    // break out if difference is too high
                    if (GetColorDifference(color, refColor) > absThreshold)
                        continue;

                    // update roi
                    int x = index % Width;
                    int y = index / Width;
                    if (x < minX)
                        minX = x;
                    if (y < minY)
                        minY = y;
                    if (x > maxX)
                        maxX = x;
                    if (y > maxY)
                        maxY = y;

                    // add neighbours
                    bool isNotTop = y > 0;
                    bool isNotLeft = x > 0;
                    bool isNotBottom = y < Height - 1;
                    bool isNotRight = x < Width - 1;

                    // three above
                    if (isNotTop)
                    {
                        int indexAbove = index - Width;
                        // left
                        if (isNotLeft)
                            recursiveWorkItems[numRecursiveItems++] = indexAbove - 1;
                        // top
                        recursiveWorkItems[numRecursiveItems++] = indexAbove;
                        // right
                        if (isNotRight)
                            recursiveWorkItems[numRecursiveItems++] = indexAbove + 1;
                    }

                    // left + right
                    if (isNotLeft)
                        recursiveWorkItems[numRecursiveItems++] = index - 1;
                    if (isNotRight)
                        recursiveWorkItems[numRecursiveItems++] = index + 1;

                    // three below
                    if (isNotBottom)
                    {
                        int indexBelow = index + Width;
                        // left
                        if (isNotLeft)
                            recursiveWorkItems[numRecursiveItems++] = indexBelow - 1;
                        // bottom
                        recursiveWorkItems[numRecursiveItems++] = indexBelow;
                        // right
                        if (isNotRight)
                            recursiveWorkItems[numRecursiveItems++] = indexBelow + 1;
                    }
                }

                // queue recursive items
                lock (RingBuffer)
                {
                    for (int i = 0; i < numRecursiveItems; i++)
                    {
                        RingBuffer.Data[RingBuffer.WritePos] = recursiveWorkItems[i];
                        RingBuffer.WritePos = (RingBuffer.WritePos + 1) % Constants.RINGBUFFER_SIZE;
                    }
                }
            }

            // in case not a single point was processed
            if (new int[] { minX, minY, maxX, maxY }.Any(x => x == int.MinValue || x == int.MaxValue))
                return new Rectangle();
            // normal case
            return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }
    }
}
