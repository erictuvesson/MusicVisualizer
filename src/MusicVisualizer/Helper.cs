namespace MusicVisualizer
{
    using Microsoft.Xna.Framework;
    using System.Globalization;

    static class Helper
    {
        public static Color FromHex(string hex)
        {
            int rgb = int.Parse(hex.Replace("#", ""), NumberStyles.HexNumber);

            int r = (rgb >> 16) & 255;
            int g = (rgb >> 8) & 255;
            int b = rgb & 255;

            return new Color(r, g, b, 255);
        }

        public static string ToHex(Color color)
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
    }
}
