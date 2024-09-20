using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoveBackground
{
    internal static class FloodFill
    {
        public unsafe static BinaryMask MagicWand(Bitmap input, Point startPoint, float threshold)
        {
            var result = new BinaryMask(input.Size.Width, input.Size.Height);

            var rawInput = new RawBitmap(input);

            Color color = rawInput.GetPixel(startPoint);

            // source: https://losingfight.com/blog/2007/08/28/how-to-implement-a-magic-wand-tool/

            return result;
        }

    }
}
