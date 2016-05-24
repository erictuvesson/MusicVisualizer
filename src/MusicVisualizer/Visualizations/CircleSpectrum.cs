namespace MusicVisualizer.Visualizations
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class CircleSpectrum : Visualization
    {
        public override string Title => "Circle Spectrum";

        /// <summary>
        /// Initial Radius.
        /// </summary>
        public float Radius
        {
            get { return radius; }
            set
            {
                if (value < 1.0f) value = 1.0f;
                radius = value;
                dirty = true;
            }
        }
        private float radius;

        public float Min = -1.0f;
        public float Max = 1.0f;
        
        private Vector2[] points;
        private Vector2[] pointsNormals;
        
        private bool dirty = true;

        /// <summary>
        /// 
        /// </summary>
        public CircleSpectrum()
        {
            this.Radius = 150.0f;
        }

        public override void Draw(Audio.AnalyzedAudio data)
        {
            SpriteBatch.Begin();

            int pointSamples = data.FFT.Length;
            if (data.SmoothFFT.Length >= pointSamples)
            {
                if (points == null || dirty)
                {
                    const double max = 2.0 * Math.PI;
                    double step = max / pointSamples;

                    points = new Vector2[pointSamples + 1];
                    pointsNormals = new Vector2[pointSamples + 1];

                    int i = 0;
                    for (double theta = 0.0; theta < max; theta += step, i++)
                    {
                        points[i] = new Vector2((float)(Radius * Math.Cos(theta)), (float)(Radius * Math.Sin(theta)));
                        pointsNormals[i] = new Vector2((float)((Radius + 1) * Math.Cos(theta)), (float)((Radius + 1) * Math.Sin(theta)));
                    }

                    dirty = false;
                }

                float centerWidth = AppShell.Width / 2.0f;
                float centerHeight = AppShell.Height / 2.0f;

                Vector2 centerScreen = new Vector2(centerWidth, centerHeight);
                for (int i = 0; i < points.Length - 1; i++)
                {
                    int prevIndex = GetPointIndex(i, -1);
                    int nextIndex = GetPointIndex(i, 1);

                    Vector2 previous = points[prevIndex];
                    Vector2 current = points[i];
                    Vector2 next = points[nextIndex];

                    float cfft = MathHelper.Clamp(data.FFT[i].X, Min, Max);
                    float nfft = MathHelper.Clamp(data.FFT[nextIndex].X, Min, Max);
                    
                    Vector2 start = centerScreen + current + (pointsNormals[i] * cfft);
                    Vector2 end = centerScreen + next + (pointsNormals[nextIndex] * nfft);

                    SpriteBatch.DrawLine(start, end, AppShell.ColorPalette.Color2);

                    float scfft = MathHelper.Clamp(data.SmoothFFT[i].X, Min, Max);
                    float snfft = MathHelper.Clamp(data.SmoothFFT[nextIndex].X, Min, Max);

                    Vector2 smoothStart = centerScreen + current + (pointsNormals[i] * scfft);
                    Vector2 smoothEnd = centerScreen + next + (pointsNormals[nextIndex] * snfft);

                    SpriteBatch.DrawLine(smoothStart, smoothEnd, AppShell.ColorPalette.Color3);
                }
            }

            SpriteBatch.End();
        }

        private int GetPointIndex(int index, int offset)
        {
            if (offset < 0)
                return (index - offset + points.Length - 1) % (points.Length - 1);
            return (index + offset) % (points.Length - 1);
        }
    }
}
