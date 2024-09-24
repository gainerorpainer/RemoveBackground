using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoveBackground.FloodFill
{
    internal static class Constants
    {
        public const int RINGBUFFER_SIZE = 256 * 1024;

        public const uint RGB_MASK = ~(0xFFu << 24);
        public const uint MAX_ALPHA = 0xFFu << 24;

        public const float MAX_COLOR_DIFFERENCE = 585225.0f;
    }
}
