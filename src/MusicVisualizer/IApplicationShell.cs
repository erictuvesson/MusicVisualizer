namespace MusicVisualizer
{
    public interface IApplicationShell
    {
        int Width { get; }
        int Height { get; }

        AppSettings AppSettings { get; }

        ColorPalette ColorPalette { get; }

        Microsoft.Xna.Framework.Graphics.GraphicsDevice GraphicsDevice { get; }
        Microsoft.Xna.Framework.Graphics.SpriteBatch SpriteBatch { get; }
        Microsoft.Xna.Framework.Graphics.SpriteFont Font { get; }
    }
}
