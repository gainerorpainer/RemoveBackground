namespace RemoveBackground.FloodFill
{
    internal static class Constants
    {
        // a mask to filter out alpha from ARGB
        public const uint RGB_MASK = 0x00FFFFFFu;
        // pixel color component with alpha = 255
        public const uint ALPHA_VISIBLE = 0xFF000000;
    }
}
