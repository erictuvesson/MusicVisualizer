namespace MusicVisualizer.Visualizations
{
    public abstract class Visualization
    {
        public abstract string Title { get; }

        public IApplicationShell AppShell { get; internal set; }
        
        public virtual void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e) { }

        public abstract void Draw(float elapsedTime, Audio.AnalyzedAudio data);
    }
}
