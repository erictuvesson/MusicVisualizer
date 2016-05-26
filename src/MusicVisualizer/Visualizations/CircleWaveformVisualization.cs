namespace MusicVisualizer.Visualizations
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    
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
        private float radius;

        public float Scale = 150.0f;

        public float Min = -1.0f;
        public float Max = 1.0f;

        private VertexPositionColor[] vertices1;
        private VertexPositionColor[] vertices2;
        private BasicEffect basicEffect;

        /// <summary>
        /// 
        /// </summary>
        public CircleWaveformVisualization()
        {
            this.Radius = 150.0f;
        }

        public override void InView()
        {
            basicEffect = new BasicEffect(AppShell.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.Identity;

            base.InView();
        }

        public override void Draw(GameTime gameTime, Audio.AnalyzedAudio data)
        {
            int pointSamples = data.FFT.Length;
            if (vertices1 == null || vertices2 == null) // TODO: If change
            {
                vertices1 = new VertexPositionColor[pointSamples];
                vertices2 = new VertexPositionColor[pointSamples];
            }

            const double max = 2.0 * Math.PI;
            double step = max / (pointSamples - 1);

            int i = 0;
            for (double theta = 0.0; theta < max; theta += step, i++)
            {
                var currentPosition = new Vector3((float)(Radius * Math.Cos(theta)), (float)(Radius * Math.Sin(theta)), 0);
                var currentNormal = currentPosition - new Vector3((float)((Radius + 1) * Math.Cos(theta)), (float)((Radius + 1) * Math.Sin(theta)), 0);

                float fft = MathHelper.Clamp(data.FFT[i].X, Min, Max);

                vertices1[i].Position = currentPosition + (currentNormal * Scale * fft);
                vertices1[i].Color = AppShell.ColorPalette.Color3;

                float sfft = MathHelper.Clamp(data.SmoothFFT[i].X, Min, Max);

                vertices2[i].Position = currentPosition + (currentNormal * Scale * sfft);
                vertices2[i].Color = AppShell.ColorPalette.Color2;
            }

            var View = Matrix.Identity;
            var Projection = Matrix.CreateOrthographic(AppShell.Width, AppShell.Height, -1.0f, 1.0f);
            
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
