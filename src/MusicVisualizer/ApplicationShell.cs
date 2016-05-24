namespace MusicVisualizer
{
    public interface ApplicationShell
    {
        int Width { get; }
        int Height { get; }

        AppSettings AppSettings { get; }

        ColorPalette ColorPalette { get; }

        Microsoft.Xna.Framework.Graphics.SpriteBatch SpriteBatch { get; }
    }
}
