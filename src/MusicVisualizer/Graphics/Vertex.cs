namespace MusicVisualizer.Graphics
{
    using OpenTK;
    using OpenTK.Graphics;

    struct Vertex
    {
        public Vector3 Position;
        public Color4 Color;

        public static int SizeInBytes = 12 + 16;
    }
}
