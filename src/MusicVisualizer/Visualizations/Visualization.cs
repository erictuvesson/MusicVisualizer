namespace MusicVisualizer.Visualizations
{
    public abstract class Visualization
    {
        public abstract string Title { get; }

        public IApplicationShell AppShell { get; internal set; }

        protected Microsoft.Xna.Framework.Graphics.SpriteBatch SpriteBatch => AppShell.SpriteBatch;

        public virtual void InView()
        {

        }

        public virtual void OutView()
        {

        }

        public virtual void Update(Input input)
        {

        }

        public abstract void Draw(Microsoft.Xna.Framework.GameTime gameTime, Audio.AnalyzedAudio data);
    }
}
