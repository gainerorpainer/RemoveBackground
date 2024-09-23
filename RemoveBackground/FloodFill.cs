using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoveBackground
{
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

        public unsafe static RawBitmap MagicWand(Bitmap input, Point startPoint, float threshold)
        {
            // make a copy
            var result = new RawBitmap(input);

            int width = input.Width;
            int height = input.Height;

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
                Queue<int> stack = new([GetIndex(width, in startPoint)]);

                // recursion
                while (stack.Count > 0)
                {
                    int index = stack.Dequeue();
                    uint color = pixels[index];

                    // break out if already visited
                    if ((color & VISITED_MASK) == IS_VISITED_VALUE)
                        continue;

                    // mark as visited
                    pixels[index] = (color & ~VISITED_MASK) | IS_VISITED_VALUE;

                    // break out if difference is too high
                    float colorDiff = GetDifference(color, refColor) / MAX_DIFFERENCE;
                    if (colorDiff > threshold)
                        continue;

                    // add neighbours
                    int colIndex = index % input.Width;
                    int lineIndex = index / input.Width;

                    bool isNotTop = lineIndex > 0;
                    bool isNotLeft = colIndex > 0;
                    bool isNotBottom = lineIndex < height - 1;
                    bool isNotRight = colIndex < width - 1;

                    // three above
                    if (isNotTop)
                    {
                        int indexAbove = index - width;
                        // left
                        if (isNotLeft)
                            stack.Enqueue(indexAbove - 1);
                        // top
                        stack.Enqueue(indexAbove);
                        // right
                        if (isNotRight)
                            stack.Enqueue(indexAbove + 1);
                    }

                    // left + right
                    if (isNotLeft)
                        stack.Enqueue(index - 1);
                    if (isNotRight)
                        stack.Enqueue(index + 1);

                    // three bewlo
                    if (isNotBottom)
                    {
                        int indexBelow = index + width;
                        // left
                        if (isNotLeft)
                            stack.Enqueue(indexBelow - 1);
                        // bottom
                        stack.Enqueue(indexBelow);
                        // right
                        if (isNotRight)
                            stack.Enqueue(indexBelow + 1);
                    }
                }
            }

            return result;
        }

    }
}
