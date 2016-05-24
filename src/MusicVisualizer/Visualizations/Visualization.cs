namespace MusicVisualizer.Visualizations
{
    public abstract class Visualization
    {
        public abstract string Title { get; }

        public ApplicationShell AppShell { get; internal set; }

        protected Microsoft.Xna.Framework.Graphics.SpriteBatch SpriteBatch => AppShell.SpriteBatch;

        public abstract void Draw(Audio.AnalyzedAudio data);
    }
}
