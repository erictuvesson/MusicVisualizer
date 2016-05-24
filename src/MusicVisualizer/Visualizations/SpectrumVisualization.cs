namespace MusicVisualizer.Visualizations
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class SpectrumVisualization : Visualization
    {
        public override string Title => "Spectrum";

        public float Scale = 300.0f;

        public override void Draw(Audio.AnalyzedAudio data)
        {
            SpriteBatch.Begin();

            int pointSamples = data.FFT.Length;
            if (data.SmoothFFT.Length >= pointSamples)
            {
                float centerHeight = AppShell.Height / 2.0f;
                float sampleWidth = AppShell.Width / (float)pointSamples;

                float currentX = 0;
                for (int i = 0; i < pointSamples; i++)
                {
                    float nextX = currentX + sampleWidth;

                    float currentData = data.FFT[i].X * Scale;
                    float nextData = currentData;
                    if (i + 1 < pointSamples)
                        nextData = data.FFT[i + 1].X * Scale;

                    SpriteBatch.DrawLine(
                        new Vector2(currentX, centerHeight + currentData),
                        new Vector2(nextX, centerHeight + nextData), AppShell.ColorPalette.Color2);

                    float currentSmoothData = data.SmoothFFT[i].X * Scale;
                    float nextSmoothData = currentSmoothData;
                    if (i + 1 < pointSamples)
                        nextSmoothData = data.SmoothFFT[i + 1].X * Scale;

                    SpriteBatch.DrawLine(
                        new Vector2(currentX, centerHeight + currentSmoothData),
                        new Vector2(nextX, centerHeight + nextSmoothData), AppShell.ColorPalette.Color3);

                    currentX = nextX;
                }
            }

            SpriteBatch.End();
        }
    }
}
