namespace MusicVisualizer.Visualizations
{
    using System;
    using Microsoft.Xna.Framework;
    using Audio;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    [Obsolete("Test Class")]
    public class TestShape : Visualization
    {
        public override string Title => "TestShape";

        private VertexPositionColor[] Vertices;
        
        public override void Draw(GameTime gameTime, AnalyzedAudio data)
        {
            if (Vertices == null)
            {
                Vertices = new VertexPositionColor[256];
            }


            var heartData = Shapes.CreateHeart();
            for (int i = 0; i < heartData.Length; i++)
            {
                Vertices[i].Position = new Vector3(heartData[i], 0) * 15;
                Vertices[i].Color = AppShell.ColorPalette.Color1;
            }

            int drawCount = heartData.Length - 1;


            var basicEffect = AppShell.BasicEffect;

            var View = Matrix.Identity;
            var Projection = Matrix.CreateOrthographic(AppShell.Width, AppShell.Height, -1.0f, 1.0f);

            basicEffect.View = View;
            basicEffect.Projection = Projection;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                AppShell.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, Vertices, 0, drawCount, VertexPositionColor.VertexDeclaration);
            }
        }
    }
}
