using System.Diagnostics;

namespace RemoveBackground.FloodFill
{
    internal unsafe class RingBuffer(int* data, int capacity)
    {
        readonly int* Data = data;
        readonly int Capacity = capacity;
        int WritePos = 0;
        int ReadPos = 0;

        public void Push(int item)
        {
            Data[WritePos] = item;
            WritePos = (WritePos + 1) % Capacity;
            // Do not check before increment as this would raise immediately at first Push()
            Debug.Assert(WritePos != ReadPos, "Buffer reached full capacity");
        }

        public int Pop()
        {
            // Do not check before increment as this would raise immediately at first Push()
            Debug.Assert(WritePos != ReadPos, "Popped from empty buffer");

            int item = Data[ReadPos];
            ReadPos = (ReadPos + 1) % Capacity;
            return item;
        }

        public int GetSize() => (WritePos - ReadPos + Capacity) % Capacity;

        public bool IsEmpty() => ReadPos == WritePos;
    }

    internal unsafe class FloodFill
    {
        // the highest possible color difference == GetColorDifference(/BLACK/, /WHITE/)
        public const int MAX_COLOR_DIFFERENCE = 585_225;

        public readonly RawBitmap RawImage;

        readonly int AbsThreshold;

        public FloodFill(Bitmap input, float threshold)
        {
            RawImage = new(input);
            AbsThreshold = (int)(MathF.Pow(threshold, 2) * MAX_COLOR_DIFFERENCE);
        }

        /// <summary>
        /// Runs the flood fill algorithm (based on color similarity), overwriting pixel values of the bitmap
        /// </summary>
        /// <param name="startPoint">Starting point for flood fill</param>
        /// <returns>The roi of the flood fill</returns>
        public Rectangle Run(Point startPoint)
        {
            // global reference color
            uint refColor = RawImage.RawData[RawImage.GetIndex(startPoint)];

            // roi state vars
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            // instead of using C# List to track active pixels, use a ring buffer
            // size can be estimated roughly by the circumference with a safety factor of 2
            int[] floodingPixels = new int[10 * (2 * RawImage.Width + 2 * RawImage.Height)];

            // use ptrs in hot section
            fixed (uint* pixels = RawImage.RawData)
            fixed (int* floodingPixelsPtr = floodingPixels)
            {
                RingBuffer floodingPixelsBuffer = new(floodingPixelsPtr, floodingPixels.Length);

                // write initial
                floodingPixelsBuffer.Push(RawImage.GetIndex(startPoint));

                // run flood filling iteratively
                uint iterations = 0;
                do
                {
#if DEBUG
                    iterations++;
                    if ((iterations % 50000) == (50000 - 1))
                    {
                        RawImage.IntervertAndCrop().Save(iterations + ".jpg");
                    }

                    if ((iterations % 50000) == (50000 - 1))
                    Debug.WriteLine($"Size={floodingPixelsBuffer.GetSize()} ({floodingPixelsBuffer.GetSize() / (double)floodingPixels.Length:P1})");
#endif

                    // pop item from buffer
                    int index = floodingPixelsBuffer.Pop();
                    uint color = pixels[index];

                    // do not visit visited pixels again
                    // take advantage of the fact that alpha is most significant byte
                    if (color > Constants.RGB_MASK)
                        continue;

                    // mark as visited by making it visible
                    pixels[index] = color + Constants.ALPHA_VISIBLE; // (color & Constants.RGB_MASK) | Constants.ALPHA_VISIBLE;

                    // break out if difference is too high
                    if (GetColorDifference(color, refColor) > AbsThreshold)
                        continue;

                    // update roi
                    int x = index % RawImage.Width;
                    int y = index / RawImage.Width;
                    if (x < minX)
                        minX = x;
                    if (y < minY)
                        minY = y;
                    if (x > maxX)
                        maxX = x;
                    if (y > maxY)
                        maxY = y;

                    // add neighbours:
                    // check against bounds
                    bool isNotTop = y > 0;
                    bool isNotLeft = x > 0;
                    bool isNotBottom = y < RawImage.Height - 1;
                    bool isNotRight = x < RawImage.Width - 1;
                    // three neighbours above
                    if (isNotTop)
                    {
                        int indexAbove = index - RawImage.Width;
                        // left
                        if (isNotLeft)
                            floodingPixelsBuffer.Push(indexAbove - 1);
                        // top
                        floodingPixelsBuffer.Push(indexAbove);
                        // right
                        if (isNotRight)
                            floodingPixelsBuffer.Push(indexAbove + 1);
                    }
                    // neighbours left + right
                    if (isNotLeft)
                        floodingPixelsBuffer.Push(index - 1);
                    if (isNotRight)
                        floodingPixelsBuffer.Push(index + 1);
                    // three neighbours  below
                    if (isNotBottom)
                    {
                        int indexBelow = index + RawImage.Width;
                        // left
                        if (isNotLeft)
                            floodingPixelsBuffer.Push(indexBelow - 1);
                        // bottom
                        floodingPixelsBuffer.Push(indexBelow);
                        // right
                        if (isNotRight)
                            floodingPixelsBuffer.Push(indexBelow + 1);
                    }
                } while (!floodingPixelsBuffer.IsEmpty());
            }

            if (minX == int.MaxValue)
                // in case not a single point was processed, return an empty rectangle instead of a max size rectangle
                return Rectangle.Empty;

            // need to add the height and width of one pixel
            return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

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
    }
}
