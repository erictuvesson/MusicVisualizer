namespace MusicVisualizer.Visualizations
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;

    [Obsolete("Not implemented yet.")]
    public class ThicklinesVisualization : Visualization
    {
        public override string Title => "Thick Lines";

        public float Scale = 300.0f;

        public float Min = -1.0f;
        public float Max = 1.0f;

        private List<Line> lines;
        private BasicEffect basicEffect;

        public override void InView()
        {
            basicEffect = new BasicEffect(AppShell.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.Identity;

            lines = new List<Line>();

            // TODO: Just a random number make this mathematically correct
            for (int i = 0; i < 28; i++)
            {
                lines.Add(new Line());
            }

            base.InView();
        }

        public override void Draw(GameTime gameTime, Audio.AnalyzedAudio data)
        {
            int widthOffset = AppShell.Width / 2 - 100;
            var centerList = lines.Count / 2;
            for (int i = 0; i < lines.Count; i++)
            {
                var x = (i * 40) - widthOffset;
                var diameter = (Math.Abs(centerList - i) - lines.Count) * 5;

                lines[i].UpdateLine(new Vector2(x, diameter), new Vector2(x, -diameter), 10, AppShell.ColorPalette.Color1);
            }


            var View = Matrix.Identity;
            var Projection = Matrix.CreateOrthographic(AppShell.Width, AppShell.Height, -1.0f, 1.0f);

            basicEffect.View = View;
            basicEffect.Projection = Projection;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                for (int i = 0; i < lines.Count; i++)
                {
                    lines[i].Draw(AppShell.GraphicsDevice);
                }
            }
        }
        

        class Line
        {
            private VertexPositionColor[] vertices;
            private int[] indices;

            private int currentVertex = 0;
            private int currentIndex = 0;

            public Line(int tessellation = 16)
            {
                int verticesCount = 4 + (tessellation + 2) * 2;
                int indicesCount = 6 + (tessellation + 1) * 6;

                vertices = new VertexPositionColor[verticesCount];
                indices = new int[indicesCount];
            }
            
            public void UpdateLine(Vector2 start, Vector2 end, float thickness, Color color)
            {
                var start3 = new Vector3(start, 0);
                var end3 = new Vector3(end, 0);

                var normal = end3 - start3;
                normal.Normalize();

                normal = new Vector3(-normal.Y, normal.X, 0);

                indices[currentIndex++] = 0;
                indices[currentIndex++] = 1;
                indices[currentIndex++] = 2;
                indices[currentIndex++] = 2;
                indices[currentIndex++] = 1;
                indices[currentIndex++] = 3;

                vertices[currentVertex++] = new VertexPositionColor(start3 - thickness * normal, color);
                vertices[currentVertex++] = new VertexPositionColor(start3 + thickness * normal, color);
                vertices[currentVertex++] = new VertexPositionColor(end3 - thickness * normal, color);
                vertices[currentVertex++] = new VertexPositionColor(end3 + thickness * normal, color);

                CreateCircle(start3, thickness, 16, color);
                CreateCircle(end3, thickness, 16, color);

                currentVertex = 0;
                currentIndex = 0;
            }

            // This should not be a proper circle
            private void CreateCircle(Vector3 center, float radius, int tessellation, Color color)
            {
                int startIndex = currentVertex;
                vertices[currentVertex++] = new VertexPositionColor(center, color);

                int horizontalSegments = tessellation;
                for (int j = 0; j <= horizontalSegments; j++)
                {
                    float longitude = (j * (float)MathHelper.TwoPi) / horizontalSegments;

                    float dx = (float)Math.Cos(longitude);
                    float dy = (float)Math.Sin(longitude);

                    Vector3 normal = new Vector3(dx, dy, 0);

                    indices[currentIndex++] = startIndex + j;
                    indices[currentIndex++] = startIndex;
                    indices[currentIndex++] = startIndex + j + 1;

                    vertices[currentVertex++] = new VertexPositionColor()
                    {
                        Position = normal * radius + center,
                        Color = color
                    };
                }
            }

            public void Draw(GraphicsDevice graphics)
            {
                graphics.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
            }
        }
    }
}
