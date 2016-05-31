namespace MusicVisualizer.Visualizations
{
    using Graphics;
    using Audio;
    using OpenTK;

    public class WaveformVisualization : Visualization
    {
        public override string Title => "Waveform";

        public float Scale = 300.0f;

        public float Min = -1.0f;
        public float Max = 1.0f;

        private Line Line1;
        private Line Line2;
        private BasicEffect basicEffect;

        public WaveformVisualization()
        {
            basicEffect = new BasicEffect();
            Line1 = new Line();
            Line2 = new Line();
        }

        public override void Draw(float elapsedTime, AnalyzedAudio data)
        {
            int pointSamples = data.FFT.Length;
            int targetSamples = MathHelper.Clamp(pointSamples, 0, AppShell.Width);
            int step = pointSamples / targetSamples;

            Line1.Resize(targetSamples);
            Line2.Resize(targetSamples);

            float centerHeight = AppShell.Height / 2.0f;
            float sampleWidth = AppShell.Width / (float)targetSamples;
            
            for (int i = 0; i < targetSamples; i++)
            {
                int index = i * step;
                float fft = MathHelper.Clamp(data.FFT[index].X, Min, Max) * Scale;
                float sfft = MathHelper.Clamp(data.SmoothFFT[index].X, Min, Max) * Scale;

                float currentX = sampleWidth * i;
                Line1.SetAtPosition(i, new Vector2(currentX, centerHeight + fft), AppShell.ColorPalette.Color3);
                Line2.SetAtPosition(i, new Vector2(currentX, centerHeight + sfft), AppShell.ColorPalette.Color2);
            }

            var transform = Matrix4.CreateOrthographicOffCenter(0, AppShell.Width, AppShell.Height, 0, -1.0f, 1.0f);

            Line1.Draw(basicEffect, transform);
            Line2.Draw(basicEffect, transform);
        }
    }
}
