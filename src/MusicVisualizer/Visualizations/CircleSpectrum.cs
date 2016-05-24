namespace MusicVisualizer.Visualizations
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;

    public class CircleSpectrum : Visualization
    {
        public override string Title => "Circle Spectrum";

        public float Scale = 1.0f;
        public float Radius = 150.0f;

        // TODO: Add Min & Max for smoother output

        private Vector2[] points;
        private Vector2[] pointsNormals;

        private int ActualPoints => points.Length - 1;

        public override void Draw(SpriteBatch sp, ApplicationShell shell, ColorPalette colorPalette, Audio.AnalyzedAudio data)
        {
            sp.Begin();

            int pointSamples = data.FFT.Length;
            if (data.SmoothFFT.Length >= pointSamples)
            {
                if (points == null)
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
                }

                float centerWidth = shell.Width / 2.0f;
                float centerHeight = shell.Height / 2.0f;

                Vector2 centerScreen = new Vector2(centerWidth, centerHeight);
                for (int i = 0; i < ActualPoints; i++)
                {
                    int prevIndex = GetPointIndex(i, -1);
                    int nextIndex = GetPointIndex(i, 1);

                    Vector2 previous = points[prevIndex];
                    Vector2 current = points[i];
                    Vector2 next = points[nextIndex];

                    Vector2 start = centerScreen + current + (pointsNormals[i] * data.FFT[i].X * Scale);
                    Vector2 end = centerScreen + next + (pointsNormals[nextIndex] * data.FFT[nextIndex].X * Scale);

                    sp.DrawLine(start, end, colorPalette.Color2);

                    Vector2 smoothStart = centerScreen + current + (pointsNormals[i] * data.SmoothFFT[i].X * Scale);
                    Vector2 smoothEnd = centerScreen + next + (pointsNormals[nextIndex] * data.SmoothFFT[nextIndex].X * Scale);

                    sp.DrawLine(smoothStart, smoothEnd, colorPalette.Color3);
                }
            }

            sp.End();
        }

        private int GetPointIndex(int index, int offset)
        {
            if (offset < 0)
                return (index - offset + ActualPoints) % ActualPoints;
            return (index + offset) % ActualPoints;
        }
    }
}
