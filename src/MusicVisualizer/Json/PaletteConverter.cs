namespace MusicVisualizer.Json
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    class PaletteConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ColorPalette);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(ColorPalette))
            {
                var colorPaletteObj = JObject.Load(reader);

                // TODO: Make crash proof
                var name = colorPaletteObj["Name"].Value<string>();
                var color1 = Helper.FromHex(colorPaletteObj["Color1"].Value<string>());
                var color2 = Helper.FromHex(colorPaletteObj["Color2"].Value<string>());
                var color3 = Helper.FromHex(colorPaletteObj["Color3"].Value<string>());
                var color4 = Helper.FromHex(colorPaletteObj["Color4"].Value<string>());
                var color5 = Helper.FromHex(colorPaletteObj["Color5"].Value<string>());

                var colorPalette = new ColorPalette(name, color1, color2, color3, color4, color5);
                
                serializer.Populate(colorPaletteObj.CreateReader(), colorPalette);

                return colorPalette;
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is ColorPalette)
            {
                var colorPalette = (ColorPalette)value;

                var colorPaletteObj = new JObject();
                colorPaletteObj.Add(new JProperty("Name", colorPalette.Name));
                colorPaletteObj.Add(new JProperty("Color1", "#" + Helper.ToHex(colorPalette.Color1)));
                colorPaletteObj.Add(new JProperty("Color2", "#" + Helper.ToHex(colorPalette.Color2)));
                colorPaletteObj.Add(new JProperty("Color3", "#" + Helper.ToHex(colorPalette.Color3)));
                colorPaletteObj.Add(new JProperty("Color4", "#" + Helper.ToHex(colorPalette.Color4)));
                colorPaletteObj.Add(new JProperty("Color5", "#" + Helper.ToHex(colorPalette.Color5)));

                colorPaletteObj.WriteTo(writer);
            }
        }
    }
}
