namespace MusicVisualizer
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using System;

    using Audio;
    using Visualizations;
    using Newtonsoft.Json;
    
    public class Game1 : Game, ApplicationShell
    {
        static string BaseTitleText = "Music Visualization";


        public int Width => graphics.PreferredBackBufferWidth;
        public int Height => graphics.PreferredBackBufferHeight;

        public AppSettings AppSettings { get; private set; }

        public ColorPalette ColorPalette => colorPalettes[nextColorPalette];

        public Microsoft.Xna.Framework.Graphics.SpriteBatch SpriteBatch => spriteBatch;


        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        
        private AudioPlayback audioPlayback;
        private AudioAnalyzer audioAnalyzer;

        private List<ColorPalette> colorPalettes;
        private int nextColorPalette;

        private List<Visualization> visualizations;
        private int nextVisualization;
        private Visualization currentVisualization;

        private KeyboardState pkeyboardState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 800;

            Window.Title = BaseTitleText;
            Window.AllowUserResizing = true;

            IsMouseVisible = true;

            isFullscreen = false;
            previousLocation = new Point(0, 0);
            previousResolution = new Point(0, 0);

            AppSettings = new AppSettings(4096 * 2);
        }
        
        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            /// Color Palettes

            var inputColorPalettesJson = System.IO.File.ReadAllText("Content/ColorPalettes.json"); // TODO: Make safe
            var inputColorPalettes = JsonConvert.DeserializeObject<IList<ColorPalette>>(inputColorPalettesJson, new Json.PaletteConverter());

            nextColorPalette = 0;
            colorPalettes = new List<ColorPalette>(inputColorPalettes);

            if (colorPalettes.Count == 0)
            {
                colorPalettes.Add(new ColorPalette("coup de grâce", "99B898", "FECEA8", "FF847C", "E84A5F", "2A363B"));
            }

            /// Visualizations

            nextVisualization = 0;
            visualizations = new List<Visualization>();
            visualizations.AddRange(
                from type in Assembly.GetExecutingAssembly().GetTypes().OrderBy(x => x.Name)
                where type.IsSubclassOf(typeof(Visualization))
                select (Visualization)Activator.CreateInstance(type));

            NextVisualization();

            /// Audio
            
            this.audioPlayback = new AudioPlayback();
            this.audioAnalyzer = new AudioAnalyzer(audioPlayback);
            
            this.audioPlayback.Load(@"Song.mp3");
            this.audioPlayback.Play();
        }

        private void NextVisualization()
        {
            if (nextVisualization <= visualizations.Count)
            {
                currentVisualization = visualizations[nextVisualization];
                currentVisualization.AppShell = this;
            }

            Window.Title = string.Format("{0} - {1}", BaseTitleText, currentVisualization.Title);
            nextVisualization = (nextVisualization + 1) % visualizations.Count;
        }

        private void NextColorPattern()
        {
            nextColorPalette = (nextColorPalette + 1) % colorPalettes.Count;
        }

        protected override void UnloadContent()
        {
            this.audioAnalyzer.Dispose();
            this.audioPlayback.Dispose();
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.IsKeyDown(Keys.Space) && !pkeyboardState.IsKeyDown(Keys.Space))
            {
                NextVisualization();
            }

            if (keyboardState.IsKeyDown(Keys.C) && !pkeyboardState.IsKeyDown(Keys.C))
            {
                NextColorPattern();
            }

            if (keyboardState.IsKeyDown(Keys.F11) && !pkeyboardState.IsKeyDown(Keys.F11))
            {
                ToggleBorderless();
            }

            pkeyboardState = keyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(colorPalettes[nextColorPalette].Color5);

            currentVisualization.Draw(audioAnalyzer.CurrentAnalyzedAudio);

            base.Draw(gameTime);
        }

        private bool isFullscreen;
        private Point previousLocation;
        private Point previousResolution;

        private void ToggleBorderless()
        {
            if (!isFullscreen)
            {
                previousLocation = Window.Position;
                previousResolution = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

                Window.Position = Point.Zero;
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                Window.Position = previousLocation;
                graphics.PreferredBackBufferWidth = previousResolution.X;
                graphics.PreferredBackBufferHeight = previousResolution.Y;
            }

            graphics.ApplyChanges();

            Window.IsBorderless = !isFullscreen;
            isFullscreen = !isFullscreen;
        }

        #region Entry Point
#if WINDOWS || LINUX
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
#endif
        #endregion
    }
}
