namespace MusicVisualizer
{
    using Microsoft.Xna.Framework;
    using System;

    static class Shapes
    {
        public static Vector2[] CreateHeart(float step = 0.05f)
        {
            var dataArray = Helper.Arange(0.0f, MathHelper.TwoPi, step);
            var result = new Vector2[dataArray.Length];

            for (int i = 0; i < dataArray.Length; i++)
            {
                var t = dataArray[i];

                float x = (float)(16 * Math.Pow(Math.Sin(t), 3));
                float y = (float)(13 * Math.Cos(t) - 5 * Math.Cos(2 * t) - 2 * Math.Cos(3 * t) - Math.Cos(4 * t));

                result[i] = new Vector2(x, y);
            }

            return result;
        }
    }
}
