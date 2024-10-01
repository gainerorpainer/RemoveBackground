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
        private static unsafe void ClearAlphaChannel(RawBitmap result)
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
            // calculate squared absolute threshold for later comparison
            int absThreshold = (int)(MathF.Pow(threshold, 2) * Constants.MAX_COLOR_DIFFERENCE);

            // make a clone and then preset alpha channel
            var maskedImage = new RawBitmap(input);
            ClearAlphaChannel(maskedImage);

            // parameters
            uint refColor = maskedImage.GetPixel(startPoint);

            List<Task<Rectangle>> backgroundTasks = [];
            unsafe
            {
                // container for all threads
                int* stackArray = stackalloc int[Constants.RINGBUFFER_SIZE];
                RingBuffer ringBuffer = new(stackArray);
                ringBuffer.Data[ringBuffer.WritePos++] = startPoint.X + startPoint.Y * maskedImage.Width;

                fixed (uint* pixels = maskedImage.RawData)
                {
                    do
                    {
                        var worker = new BackgroundWorker(input.Size, pixels, ringBuffer, absThreshold, refColor);
                        backgroundTasks.Add(Task.Run(worker.DoWork));
                    }
                    // work takes long, add one more worker?
                    while (!Task.WaitAll([.. backgroundTasks], 100));
                }
            }

            // consume worker results
            int minX = startPoint.X, minY = startPoint.Y, maxX = startPoint.X, maxY = startPoint.Y;
            foreach (Rectangle roi in backgroundTasks.Select(x => x.Result))
            {
                if (roi == Rectangle.Empty)
                    continue;
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
                RawBitmap = maskedImage,
                ROI = new Rectangle(minX, minY, maxX - minX, maxY - minY)
            };

        }


    }
}
