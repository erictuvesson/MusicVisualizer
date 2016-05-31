namespace MusicVisualizer
{
    using OpenTK.Graphics;

    static class Helper
    {
        public static Color4 FromHex(string hex)
        {
            int rgb = int.Parse(hex.Replace("#", ""), System.Globalization.NumberStyles.HexNumber);

            int r = (rgb >> 16) & 255;
            int g = (rgb >> 8) & 255;
            int b = rgb & 255;

            return new Color4((byte)r, (byte)g, (byte)b, (byte)255);
        }

        public static string ToHex(Color4 color)
        {
            return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        /// <summary> Return evenly spaced values within a given interval. </summary>
        public static float[] Arange(float start, float stop, float step)
        {
            int num = (int)((stop - start) / step);

            var result = new float[num];
            for (int i = 0; i < num; i++)
            {
                result[i] = start + step * i;
            }

            return result;
        }

        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }
    }
}
