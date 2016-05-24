namespace MusicVisualizer.Visualizations
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class SpectrumVisualization : Visualization
    {
        public override string Title => "Spectrum";

        public float Scale = 300.0f;

        public float Min = -1.0f;
        public float Max = 1.0f;

        private VertexPositionColor[] vertices1;
        private VertexPositionColor[] vertices2;
        private BasicEffect basicEffect;

        public override void InView()
        {
            basicEffect = new BasicEffect(AppShell.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.Identity;

            base.InView();
        }

        public override void Draw(Audio.AnalyzedAudio data)
        {
            int pointSamples = data.FFT.Length;
            if (vertices1 == null || vertices2 == null) // TODO: If change
            {
                vertices1 = new VertexPositionColor[pointSamples];
                vertices2 = new VertexPositionColor[pointSamples];
            }

            float centerHeight = AppShell.Height / 2.0f;
            float sampleWidth = AppShell.Width / (float)pointSamples;

            for (int i = 0; i < pointSamples; i++)
            {
                float currentX = sampleWidth * i;

                float fft = MathHelper.Clamp(data.FFT[i].X, Min, Max) * Scale;

                vertices1[i].Position = new Vector3(currentX, centerHeight + fft, 0);
                vertices1[i].Color = AppShell.ColorPalette.Color3;

                float sfft = MathHelper.Clamp(data.SmoothFFT[i].X, Min, Max) * Scale;

                vertices2[i].Position = new Vector3(currentX, centerHeight + sfft, 0);
                vertices2[i].Color = AppShell.ColorPalette.Color2;
            }

            var View = Matrix.Identity;
            var Projection = Matrix.CreateOrthographicOffCenter(0, AppShell.Width, AppShell.Height, 0, -1.0f, 1.0f);

            basicEffect.View = View;
            basicEffect.Projection = Projection;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                AppShell.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices1, 0, vertices1.Length - 1, VertexPositionColor.VertexDeclaration);
                AppShell.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices2, 0, vertices2.Length - 1, VertexPositionColor.VertexDeclaration);
            }
        }
    }
}
