namespace MusicVisualizer
{
    public struct AppSettings
    {
        public readonly int FFTLength;

        public AppSettings(int fftLength)
        {
            this.FFTLength = fftLength;
        }
    }
}
