namespace MusicVisualizer.Visualizations
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;

    [Obsolete("Not implemented yet.")]
    public class WavesVisualization : Visualization
    {
        public override string Title => "Abstract Waves";

        public float Min = -1.0f;
        public float Max = 1.0f;
        
        private List<Wave> waves;
        private BasicEffect basicEffect;

        private int tesselation = 256;

        public override void InView()
        {
            basicEffect = new BasicEffect(AppShell.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.Identity;

            waves = new List<Wave>();

            waves.Add(new Wave(WaveLayer.Layer1, tesselation)
            {
                WaveHeight = 6.00f,
                WaveLength = 0.25f,
                WaveSpeed  = 60.0f
            });

            waves.Add(new Wave(WaveLayer.Layer2, tesselation)
            {
                WaveHeight = 50.0f,
                WaveLength = 0.25f,
                WaveSpeed  = 30.0f
            });

            waves.Add(new Wave(WaveLayer.Layer3, tesselation)
            {
                WaveHeight = 75.0f,
                WaveLength = 0.25f,
                WaveSpeed  = 15.0f
            });

            base.InView();
        }
        
        public override void Draw(GameTime gameTime, Audio.AnalyzedAudio data)
        {
            var offset = new Vector3(0, AppShell.Height / 2, 0);

            foreach (var wave in waves)
            {
                var minSample = data.Samples[0].MinSample + 1.0f;
                var maxSample = data.Samples[0].MaxSample + 1.0f;

                // TODO: Add the sound effects

                switch (wave.Layer)
                {
                    case WaveLayer.Layer1:
                        break;
                    case WaveLayer.Layer2:
                        break;
                    case WaveLayer.Layer3:
                        break;
                }

                wave.Update((float)gameTime.TotalGameTime.TotalSeconds, offset, AppShell.Width, AppShell.ColorPalette.Color1);
            }


            var View = Matrix.Identity;
            var Projection = Matrix.CreateOrthographicOffCenter(0, AppShell.Width, AppShell.Height, 0, -1.0f, 1.0f);
            
            basicEffect.View = View;
            basicEffect.Projection = Projection;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach (var wave in waves)
                {
                    AppShell.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, wave.Vertices, 0, wave.Vertices.Length - 1, VertexPositionColor.VertexDeclaration);
                }
            }
        }

        enum WaveLayer
        {
            Layer1,
            Layer2,
            Layer3,
        }

        class Wave
        {
            public VertexPositionColor[] Vertices;
            
            public float WaveHeight = 10;
            public float WaveLength = 0.5f;
            public float WaveSpeed = 20;

            public WaveLayer Layer = WaveLayer.Layer1;

            public Wave(WaveLayer layer, int tessellation)
            {
                this.Layer = layer;

                Vertices = new VertexPositionColor[tessellation];
            }

            public void Update(float elapsedTime, Vector3 offset, float widthSpan, Color color)
            {
                float step = widthSpan / Vertices.Length;

                for (int i = 0; i < Vertices.Length; i++)
                {
                    var cosOffset = (elapsedTime * WaveSpeed) + (i * 0.25f);

                    var cos = (float)Math.Cos(cosOffset * WaveLength) * WaveHeight;
                    var newPosition = new Vector3(step * i, cos, 0);

                    Vertices[i].Position = newPosition + offset;
                    Vertices[i].Color = color;
                }
            }

            private void SetVertex(int index, float time, Vector3 position, Vector3 normal, Color color)
            {
                var newPosition = position;

                var offset = (index + 1);
                var cosOffset = (time * WaveSpeed) + (offset * 0.25f);

                var cos = (float)Math.Cos(cosOffset * WaveLength) * WaveHeight;
                newPosition += normal * cos;

                Vertices[index].Position = newPosition;
                Vertices[index].Color = color;
            }
        }
    }
}
