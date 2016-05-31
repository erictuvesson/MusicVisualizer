namespace MusicVisualizer
{
    public interface IApplicationShell
    {
        int Width { get; }
        int Height { get; }

        AppSettings AppSettings { get; }

        ColorPalette ColorPalette { get; }
    }
}
