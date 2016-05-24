namespace MusicVisualizer
{
    using Microsoft.Xna.Framework;
    using System.Diagnostics;

    [DebuggerDisplay("<{Name}, {Color1}, {Color2}, {Color3}, {Color4}, {Color5}>")]
    public struct ColorPalette
    {
        public readonly string Name;

        public readonly Color Color1;
        public readonly Color Color2;
        public readonly Color Color3;
        public readonly Color Color4;
        public readonly Color Color5;

        public ColorPalette(string name, Color color1, Color color2, Color color3, Color color4, Color color5)
        {
            this.Name = name;
            this.Color1 = color1;
            this.Color2 = color2;
            this.Color3 = color3;
            this.Color4 = color4;
            this.Color5 = color5;
        }

        public ColorPalette(string name, string color1, string color2, string color3, string color4, string color5)
        {
            this.Name = name;
            this.Color1 = Helper.FromHex(color1);
            this.Color2 = Helper.FromHex(color2);
            this.Color3 = Helper.FromHex(color3);
            this.Color4 = Helper.FromHex(color4);
            this.Color5 = Helper.FromHex(color5);
        }
    }
}
