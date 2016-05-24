namespace MusicVisualizer
{
    public interface ApplicationShell
    {
        int Width { get; }
        int Height { get; }

        AppSettings AppSettings { get; }
    }
}
