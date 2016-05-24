namespace MusicVisualizer.Visualizations
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class SpectrumVisualization : Visualization
    {
        public override string Title => "Spectrum";

        public float Scale = 300.0f;

        public override void Draw(SpriteBatch sp, ApplicationShell shell, ColorPalette colorPalette, Audio.AnalyzedAudio data)
        {
            sp.Begin();

            int pointSamples = data.FFT.Length;
            if (data.SmoothFFT.Length >= pointSamples)
            {
                float centerHeight = shell.Height / 2.0f;
                float sampleWidth = shell.Width / (float)pointSamples;

                float currentX = 0;
                for (int i = 0; i < pointSamples; i++)
                {
                    float nextX = currentX + sampleWidth;

                    float currentData = data.FFT[i].X * Scale;
                    float nextData = currentData;
                    if (i + 1 < pointSamples)
                        nextData = data.FFT[i + 1].X * Scale;

                    sp.DrawLine(
                        new Vector2(currentX, centerHeight + currentData),
                        new Vector2(nextX, centerHeight + nextData), colorPalette.Color2);

                    float currentSmoothData = data.SmoothFFT[i].X * Scale;
                    float nextSmoothData = currentSmoothData;
                    if (i + 1 < pointSamples)
                        nextSmoothData = data.SmoothFFT[i + 1].X * Scale;

                    sp.DrawLine(
                        new Vector2(currentX, centerHeight + currentSmoothData),
                        new Vector2(nextX, centerHeight + nextSmoothData), colorPalette.Color3);

                    currentX = nextX;
                }
            }

            sp.End();
        }
    }
}
