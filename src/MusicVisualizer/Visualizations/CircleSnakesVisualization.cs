namespace MusicVisualizer.Visualizations
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;

    [Obsolete("Not implemented yet.")]
    public class CircleSnakesVisualization : Visualization
    {
        public override string Title => "Circle Snakes";

        public float Radius = 150.0f;

        public float Min = -1.0f;
        public float Max = 1.0f;

        private List<CirSnake> snakes;
        private BasicEffect basicEffect;

        private int tesselation = 256;

        private Random rd = new Random();

        public override void InView()
        {
            basicEffect = new BasicEffect(AppShell.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.Identity;
            
            snakes = new List<CirSnake>();

            snakes.Add(new CirSnake(tesselation)
            {
                Radius = 150.0f,
                StartDegree = 248.0f,
                Degrees = 200.0f,
                WaveHeight = 10,
                WaveLength = 0.5f,
                WaveSpeed = 20
            });

            snakes.Add(new CirSnake(tesselation)
            {
                Radius = 160.0f,
                StartDegree = 23.0f,
                Degrees = 124.0f,
                WaveHeight = 8,
                WaveLength = 0.5f,
                WaveSpeed = 25
            });

            snakes.Add(new CirSnake(tesselation)
            {
                Radius = 170.0f,
                StartDegree = 12.0f,
                Degrees = 98.0f,
                WaveHeight = 6,
                WaveLength = 0.25f,
                WaveSpeed = 60
            });

            base.InView();
        }

        public override void Draw(GameTime gameTime, Audio.AnalyzedAudio data)
        {
            foreach (var snake in snakes)
            {
                snake.Move((float)rd.NextDouble()); // TODO: Change

                // TODO: Add the sound effects

                snake.Update((float)gameTime.TotalGameTime.TotalSeconds, AppShell.ColorPalette.Color1);
            }


            var View = Matrix.Identity;
            var Projection = Matrix.CreateOrthographic(AppShell.Width, AppShell.Height, -1.0f, 1.0f);

            basicEffect.View = View;
            basicEffect.Projection = Projection;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach (var snake in snakes)
                {
                    AppShell.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, snake.Vertices, 0, snake.Vertices.Length - 1, VertexPositionColor.VertexDeclaration);
                }
            }
        }

        class CirSnake
        {
            public VertexPositionColor[] Vertices;

            public float Radius = 150;
            public float StartDegree = 30;
            public float Degrees = 60;

            public float WaveHeight = 10;
            public float WaveLength = 0.5f;
            public float WaveSpeed = 20;

            public CirSnake(int tessellation)
            {
                Vertices = new VertexPositionColor[tessellation];
            }

            /// <summary> Move along the circle. </summary>
            public void Move(float degree)
            {
                StartDegree = (degree >= 0) ? (StartDegree + degree) : (StartDegree - degree + 360) % 360;
            }

            public void Update(float elapsedTime, Color color)
            {
                float max = 2.0f * MathHelper.ToRadians(Degrees);
                float step = max / Vertices.Length;

                float startStep = MathHelper.ToRadians(StartDegree); 

                int i = 0;
                for (double theta = 0.0; theta < max; theta += step, i++)
                {
                    if (i >= Vertices.Length)
                        break;

                    var newTheta = startStep + theta;

                    var currentPosition = new Vector3(Radius * (float)Math.Cos(newTheta), Radius * (float)Math.Sin(newTheta), 0);
                    var currentNormal = currentPosition - new Vector3((Radius + 1) * (float)Math.Cos(newTheta), (Radius + 1) * (float)Math.Sin(newTheta), 0);

                    SetVertex(i, elapsedTime, currentPosition, currentNormal, color);
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
