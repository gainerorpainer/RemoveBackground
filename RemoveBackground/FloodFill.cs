using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoveBackground
{
    record class FloodFillResult
    {
        public required Bitmap Bitmap { get; internal set; }
        public Rectangle ROI { get; internal set; }
    }

    internal static class FloodFill
    {
        const uint VISITED_MASK = 0xFFu << 24;
        const uint IS_NOT_VISITED_VALUE = 0u;
        const uint IS_VISITED_VALUE = 0xFFu << 24;

        const float MAX_DIFFERENCE = 585225.0f;

        public static int GetIndex(int width, ref readonly Point point) => point.X + width * point.Y;

        public static int GetDifference(uint color1, uint color2)
        {
            // decompose argb into r,g,b
            int r1 = (int)((color1 & 0x00FF0000) >> 16);
            int r2 = (int)((color2 & 0x00FF0000) >> 16);
            int g1 = (int)((color1 & 0x0000FF00) >> 08);
            int g2 = (int)((color2 & 0x0000FF00) >> 08);
            int b1 = (int)((color1 & 0x000000FF) >> 00);
            int b2 = (int)((color2 & 0x000000FF) >> 00);

            unchecked
            {
                // intermediates
                int r_hat = (r1 / 2) + (r2 / 2);
                int deltaR = r1 - r2;
                int deltaG = g1 - g2;
                int deltaB = b1 - b2;

                if (r_hat < 128)
                    return 2 * deltaR * deltaR + 4 * deltaG * deltaG + 3 * deltaB * deltaB;

                return 3 * deltaR * deltaR + 4 * deltaG * deltaG + 2 * deltaB * deltaB;
            }
        }

        public unsafe static FloodFillResult MagicWand(Bitmap input, Point startPoint, float threshold)
        {
            // make a copy
            var result = new RawBitmap(input);

            int width = input.Width;
            int height = input.Height;

            int minX = startPoint.X, minY = startPoint.Y, maxX = startPoint.X, maxY = startPoint.Y;

            // pin memory location
            fixed (uint* pixels = result.RawData)
            {
                // source: https://stackoverflow.com/a/367253/18181748

                // mark each pixel as not visited within alpha channel
                for (int i = 0; i < width * height; i++)
                    pixels[i] = (pixels[i] & ~VISITED_MASK) | IS_NOT_VISITED_VALUE;

                // first pixel
                int startIndex = GetIndex(width, in startPoint);
                uint refColor = pixels[startIndex];
                // stack for recursion
                const int RINGBUFFER_SIZE = 256 * 1024;
                int* ringBuffer = stackalloc int[RINGBUFFER_SIZE];
                uint ringWrite = 0;
                uint ringRead = 0;
                ringBuffer[ringWrite] = GetIndex(width, in startPoint);
                ringWrite = (ringWrite + 1) % RINGBUFFER_SIZE;

                uint pixelsVisited = 0;
                uint pixelsSelected = 0;
                uint maxBufferSize = 0;

                // recursion
                while (ringRead != ringWrite)
                {
                    pixelsVisited++;

                    int index = ringBuffer[ringRead];
                    ringRead = (ringRead + 1) % RINGBUFFER_SIZE;
                    uint color = pixels[index];

                    // break out if already visited
                    if ((color & VISITED_MASK) == IS_VISITED_VALUE)
                        continue;

                    // mark as visited
                    pixels[index] = (color & ~VISITED_MASK) | IS_VISITED_VALUE;

                    // break out if difference is too high
                    float colorDiff = MathF.Sqrt(GetDifference(color, refColor) / MAX_DIFFERENCE);
                    if (colorDiff > threshold)
                        continue;

                    pixelsSelected++;

                    // update roi
                    int x = index % input.Width;
                    int y = index / input.Width;
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
                    bool isNotBottom = y < height - 1;
                    bool isNotRight = x < width - 1;

                    // three above
                    if (isNotTop)
                    {
                        int indexAbove = index - width;
                        // left
                        if (isNotLeft)
                        {
                            ringBuffer[ringWrite] = indexAbove - 1;
                            ringWrite = (ringWrite + 1) % RINGBUFFER_SIZE;
                        }
                        // top
                        ringBuffer[ringWrite] = indexAbove;
                        ringWrite = (ringWrite + 1) % RINGBUFFER_SIZE;
                        // right
                        if (isNotRight)
                        {
                            ringBuffer[ringWrite] = indexAbove + 1;
                            ringWrite = (ringWrite + 1) % RINGBUFFER_SIZE;
                        }
                    }

                    // left + right
                    if (isNotLeft)
                    {
                        ringBuffer[ringWrite] = index - 1;
                        ringWrite = (ringWrite + 1) % RINGBUFFER_SIZE;
                    }
                    if (isNotRight)
                    {
                        ringBuffer[ringWrite] = index + 1;
                        ringWrite = (ringWrite + 1) % RINGBUFFER_SIZE;
                    }

                    // three bewlo
                    if (isNotBottom)
                    {
                        int indexBelow = index + width;
                        // left
                        if (isNotLeft)
                        {
                            ringBuffer[ringWrite] = indexBelow - 1;
                            ringWrite = (ringWrite + 1) % RINGBUFFER_SIZE;
                        }
                        // bottom
                        ringBuffer[ringWrite] = indexBelow;
                        ringWrite = (ringWrite + 1) % RINGBUFFER_SIZE;
                        // right
                        if (isNotRight)
                        {
                            ringBuffer[ringWrite] = indexBelow + 1;
                            ringWrite = (ringWrite + 1) % RINGBUFFER_SIZE;
                        }
                    }

                    uint bufferSize = unchecked((ringWrite - ringRead) % RINGBUFFER_SIZE);
                    maxBufferSize = bufferSize > maxBufferSize ? bufferSize : maxBufferSize;
                }

                Debug.WriteLine($"Overscan factor {(double)pixelsVisited / pixelsSelected:F01}x (selected = {pixelsSelected:N0}, visited = {pixelsVisited:N0})");
                Debug.WriteLine($"Max buffer size = {maxBufferSize}");
            }

            return new FloodFillResult()
            {
                Bitmap = result.Bitmap,
                ROI = new Rectangle(minX, minY, maxX - minX, maxY - minY)
            };
        }
    }
}
