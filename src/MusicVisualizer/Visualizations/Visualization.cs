namespace MusicVisualizer.Visualizations
{
    public abstract class Visualization
    {
        public abstract string Title { get; }

        public abstract void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sp, ApplicationShell shell, ColorPalette colorPalette, Audio.AnalyzedAudio data);
    }
}
