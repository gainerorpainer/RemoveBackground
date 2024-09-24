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
    internal record class FloodFillResult
    {
        public required Bitmap Bitmap { get; internal set; }
        public Rectangle ROI { get; internal set; }
    }


    internal static class FloodFill
    {
        private record class ThreadStartInfo
        {

        }

        const uint RGB_MASK = ~(0xFFu << 24);
        const uint MAX_ALPHA = 0xFFu << 24;

        const float MAX_DIFFERENCE = 585225.0f;

        static int GetIndex(int width, ref readonly Point point) => point.X + width * point.Y;

        static int GetColorDifference(uint color1, uint color2)
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

        static unsafe void ClearAlphaChannel(RawBitmap result)
        {
            // fast way to set each alpha channel to invisible initially
            fixed (uint* pixels = result.RawData)
            {
                // set alpha to invisible for each pixel
                for (int i = 0; i < result.Width * result.Height; i++)
                    pixels[i] = pixels[i] & RGB_MASK;
            }
        }

        private static unsafe Rectangle SideThread(RawBitmap input, float threshold, Point startPoint, uint refColor, Rectangle bounds)
        {
            // calculate squared absolute threshold for later comparison
            int absThreshold = (int)(MathF.Pow(threshold, 2) * MAX_DIFFERENCE);

            // roi state vars
            int minX = startPoint.X, minY = startPoint.Y, maxX = startPoint.X, maxY = startPoint.Y;

            // container for new flood starting points
            const int RINGBUFFER_SIZE = 256 * 1024;
            int* ringBuffer = stackalloc int[RINGBUFFER_SIZE];
            uint ringWrite = 0;
            uint ringRead = 0;
            ringBuffer[ringWrite] = GetIndex(input.Width, in startPoint);
            ringWrite = (ringWrite + 1) % RINGBUFFER_SIZE;

            // recursion
            fixed (uint* pixels = input.RawData)
                while (ringRead != ringWrite)
                {
                    int index = ringBuffer[ringRead];
                    ringRead = (ringRead + 1) % RINGBUFFER_SIZE;
                    uint color = pixels[index];

                    // break out if already visited
                    if (color > 0x00FFFFFFu)
                        continue;

                    // mark as visited
                    pixels[index] = (color & RGB_MASK) | MAX_ALPHA;

                    // break out if difference is too high
                    if (GetColorDifference(color, refColor) > absThreshold)
                        continue;

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
                    bool isNotTop = y > bounds.Top;
                    bool isNotLeft = x > bounds.Left;
                    bool isNotBottom = y < bounds.Bottom - 1;
                    bool isNotRight = x < bounds.Right - 1;

                    // three above
                    if (isNotTop)
                    {
                        int indexAbove = index - input.Width;
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
                        int indexBelow = index + input.Width;
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
                }

            return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

        public static FloodFillResult MagicWand(Bitmap input, Point startPoint, float threshold)
        {
            // make a clone and then preset alpha channel
            var maskedImage = new RawBitmap(input);
            ClearAlphaChannel(maskedImage);

            // result vars
            int minX = startPoint.X, minY = startPoint.Y, maxX = startPoint.X, maxY = startPoint.Y;

            // parameters
            uint refColor = maskedImage.GetPixel(startPoint);
            Rectangle[] bounds = new Rectangle[4];
            bounds[0] = new(0, 0, startPoint.X + 1, startPoint.Y + 1);
            bounds[1] = new(0, bounds[0].Height, bounds[0].Width, input.Height - bounds[0].Height);
            bounds[2] = new(bounds[0].Width, 0, input.Width - bounds[0].Width, bounds[0].Height);
            bounds[3] = new(bounds[2].X, bounds[2].Height, bounds[2].Width, input.Height - bounds[2].Height);
            Point[] points =
            [
                startPoint,
                startPoint + new Size(0, 1),
                startPoint + new Size(1, 0),
                startPoint + new Size(1, 1),
            ];

            // spawn up to 4 worker threads, depending on starting points lay within image ("edge" cases)
            Task<Rectangle>[] workers = new Task<Rectangle>[4];
            Rectangle imageBounds = new(new Point(), input.Size);
            foreach (int i in Enumerable.Range(0, workers.Length).Where(i => imageBounds.Contains(points[i])))
                workers[i] = Task.Run(() => delegateTask(bounds[i], points[i]));

            // consume worker results
            Task.WaitAll(workers);
            for (int i = 0; i < workers.Length; i++)
            {
                var roi = workers[i].Result;
                if (roi.X < minX)
                    minX = roi.X;
                if (roi.Y < minY)
                    minY = roi.Y;
                if (roi.Right > maxX)
                    maxX = roi.Right;
                if (roi.Bottom > maxY)
                    maxY = roi.Bottom;
            }

            return new FloodFillResult()
            {
                Bitmap = maskedImage.Bitmap,
                ROI = new Rectangle(minX, minY, maxX - minX, maxY - minY)
            };

            Rectangle delegateTask(Rectangle bounds, Point start) => SideThread(maskedImage, threshold, start, refColor, bounds);
        }


    }
}
