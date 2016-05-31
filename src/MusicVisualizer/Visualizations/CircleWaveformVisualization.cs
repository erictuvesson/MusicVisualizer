namespace MusicVisualizer.Visualizations
{
    using System;
    using Audio;
    using Graphics;
    using OpenTK;

    public class CircleWaveformVisualization : Visualization
    {
        public override string Title => "Circle Waveform";

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
            }
        }
        private float radius = 150.0f;

        public float Scale = 150.0f;

        public float Min = -1.0f;
        public float Max = 1.0f;

        private Line Line1;
        private Line Line2;
        private BasicEffect basicEffect;

        public CircleWaveformVisualization()
        {
            basicEffect = new BasicEffect();
            Line1 = new Line();
            Line2 = new Line();
            Line1.LoopLines = true;
            Line2.LoopLines = true;
        }

        public override void Draw(float elapsedTime, AnalyzedAudio data)
        {
            int pointSamples = data.FFT.Length;
            int targetSamples = MathHelper.Clamp(pointSamples, 0, AppShell.Width);
            int sampleStep = pointSamples / targetSamples;

            Line1.Resize(targetSamples);
            Line2.Resize(targetSamples);

            const double max = 2.0 * Math.PI;
            double step = max / targetSamples;

            var center = new Vector2(AppShell.Width / 2, AppShell.Height / 2);

            int i = 0;
            for (double theta = 0.0; theta < max; theta += step, i++)
            {
                var currentPosition = new Vector2((float)(Radius * Math.Cos(theta)), (float)(Radius * Math.Sin(theta)));
                var currentNormal = currentPosition - new Vector2((float)((Radius + 1) * Math.Cos(theta)), (float)((Radius + 1) * Math.Sin(theta)));

                float fft = MathHelper.Clamp(data.FFT[i].X, Min, Max);
                float sfft = MathHelper.Clamp(data.SmoothFFT[i].X, Min, Max);

                Line1.SetAtPosition(i, center + currentPosition + (currentNormal * Scale * fft), AppShell.ColorPalette.Color3);
                Line2.SetAtPosition(i, center + currentPosition + (currentNormal * Scale * sfft), AppShell.ColorPalette.Color2);
            }

            var transform = Matrix4.CreateOrthographicOffCenter(0, AppShell.Width, AppShell.Height, 0, -1.0f, 1.0f);

            Line1.Draw(basicEffect, transform);
            Line2.Draw(basicEffect, transform);
        }
    }
}
