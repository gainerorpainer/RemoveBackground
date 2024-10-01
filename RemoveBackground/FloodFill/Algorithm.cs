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

namespace RemoveBackground.FloodFill
{
    internal record class FloodFillResult
    {
        public required RawBitmap RawBitmap { get; internal set; }
        public Rectangle ROI { get; internal set; }
    }

    internal static class Algorithm
    {
        public static unsafe void ClearAlphaChannel(RawBitmap result)
        {
            // fast way to set each alpha channel to invisible initially
            fixed (uint* pixels = result.RawData)
            {
                // set alpha to invisible for each pixel
                for (int i = 0; i < result.Width * result.Height; i++)
                    pixels[i] = pixels[i] & Constants.RGB_MASK;
            }
        }

        public static FloodFillResult MagicWand(Bitmap input, Point startPoint, float threshold)
        {
            // make a clone and then preset alpha channel
            var raw = new RawBitmap(input);
            ClearAlphaChannel(raw);

            return new()
            {
                RawBitmap = raw,
                ROI = MagicWand(raw, startPoint, threshold),
            };
        }

        public static Rectangle MagicWand(RawBitmap raw, Point startPoint, float threshold)
        {
            // calculate squared absolute threshold for later comparison
            int absThreshold = (int)(MathF.Pow(threshold, 2) * Constants.MAX_COLOR_DIFFERENCE);

            // parameters
            uint refColor = raw.GetPixel(startPoint);

            List<Task<Rectangle>> backgroundTasks = [];
            unsafe
            {
                // container for all threads
                int* stackArray = stackalloc int[Constants.RINGBUFFER_SIZE];
                RingBuffer ringBuffer = new(stackArray);
                ringBuffer.Data[ringBuffer.WritePos++] = startPoint.X + startPoint.Y * raw.Width;

                fixed (uint* pixels = raw.RawData)
                {
                    do
                    {
                        var worker = new BackgroundWorker(new Size(raw.Width, raw.Height), pixels, ringBuffer, absThreshold, refColor);
                        backgroundTasks.Add(Task.Run(worker.DoWork));
                    }
                    // work takes long, add one more worker?
                    while (!Task.WaitAll([.. backgroundTasks], 100));
                }
            }

            // consume worker results
            Rectangle roi = new(startPoint.X, startPoint.Y, 1, 1);
            foreach (Rectangle currentRoi in backgroundTasks.Select(x => x.Result))
            {
                if (currentRoi == Rectangle.Empty)
                    continue;
                roi = CombineRois(roi, currentRoi);
            }

            return roi;
        }

        internal static Rectangle CombineRois(Rectangle a, Rectangle b)
        {
            int left = Math.Min(a.Left, b.Left);
            int top = Math.Min(a.Top, b.Top);
            int right = Math.Max(a.Right, b.Right);
            int bottom = Math.Max(a.Bottom, b.Bottom);
            return new(left, top, right - left, bottom - top);
        }
    }
}
