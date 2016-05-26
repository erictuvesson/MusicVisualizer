namespace MusicVisualizer
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using System;
    using Newtonsoft.Json;
    using System.IO;

    using Audio;
    using Visualizations;

    public class Game1 : Game, IApplicationShell
    {
        static string BaseTitleText = "Music Visualization";


        public int Width => graphics.PreferredBackBufferWidth;
        public int Height => graphics.PreferredBackBufferHeight;

        public AppSettings AppSettings { get; private set; }

        public ColorPalette ColorPalette => colorPalettes[nextColorPalette];

        Microsoft.Xna.Framework.Graphics.GraphicsDevice IApplicationShell.GraphicsDevice => GraphicsDevice;
        public Microsoft.Xna.Framework.Graphics.SpriteBatch SpriteBatch => spriteBatch;
        public Microsoft.Xna.Framework.Graphics.SpriteFont Font => font;
        public Microsoft.Xna.Framework.Graphics.BasicEffect BasicEffect => basicEffect;
        
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private BasicEffect basicEffect;
                
        private AudioPlayback audioPlayback;
        private AudioAnalyzer audioAnalyzer;

        private List<ColorPalette> colorPalettes;
        private int nextColorPalette;

        private List<Visualization> visualizations;
        private int nextVisualization;
        private Visualization currentVisualization;

        private KeyboardState pkeyboardState;

        private GameConsole gameConsole;
        private Input input;

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

        protected override void Initialize()
        {
            Components.Add(gameConsole = new GameConsole(this));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            this.font = Content.Load<SpriteFont>("Content/Consolas");

            this.basicEffect = new BasicEffect(GraphicsDevice);
            this.basicEffect.VertexColorEnabled = true;
            this.basicEffect.World = Matrix.Identity;

            this.input = new Input();

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
                where type.IsSubclassOf(typeof(Visualization)) && type.GetCustomAttribute<ObsoleteAttribute>() == null
                select (Visualization)Activator.CreateInstance(type));

            // Experimental Visualizations
            //visualizations = new List<Visualization>(
            //    new Visualization[]
            //    {
            //        new TestShape(),
            //        new CircleSnakesVisualization(),
            //        new ThicklinesVisualization(),
            //        new WavesVisualization()
            //    });

            NextVisualization();

            /// Audio
            
            this.audioPlayback = new AudioPlayback(this);
            this.audioAnalyzer = new AudioAnalyzer(audioPlayback);
            
#if DEBUG
            var songPath = @"F:\Music\ODESZA - Sun Models (feat. Madelyn Grant).mp3";
#else
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "All Supported Files (*.wav;*.mp3)|*.wav;*.mp3|All Files (*.*)|*.*";

            var result = openFileDialog.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK)
            {
                Exit();
            }

            var songPath = openFileDialog.FileName;
#endif

            this.audioPlayback.Load(songPath);
            this.audioPlayback.Play();

            gameConsole.WriteLine($"-- Playing {Path.GetFileNameWithoutExtension(songPath)}", Color.White, TimeSpan.FromSeconds(10));
        }

        private void NextVisualization()
        {
            if (nextVisualization <= visualizations.Count)
            {
                currentVisualization?.OutView();

                currentVisualization = visualizations[nextVisualization];
                currentVisualization.AppShell = this;

                currentVisualization.InView();
            }

            gameConsole.WriteLine($"-- Visualization, {currentVisualization.Title}", ColorPalette.Color1);

            Window.Title = string.Format("{0} - {1}", BaseTitleText, currentVisualization.Title);
            nextVisualization = (nextVisualization + 1) % visualizations.Count;
        }

        private void NextColorPattern()
        {
            nextColorPalette = (nextColorPalette + 1) % colorPalettes.Count;
            gameConsole.WriteLine($"-- ColorPalette, {ColorPalette.Name}", ColorPalette.Color1);
        }

        protected override void UnloadContent()
        {
            this.audioAnalyzer.Dispose();
            this.audioPlayback.Dispose();
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            input.currentKeybardState = Keyboard.GetState();

            if (input.IsNewKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (input.IsNewKeyDown(Keys.Space))
            {
                NextVisualization();
            }

            if (input.IsNewKeyDown(Keys.C))
            {
                NextColorPattern();
            }

            if (input.IsNewKeyDown(Keys.F11))
            {
                ToggleBorderless();
            }

            if (currentVisualization != null)
                currentVisualization.Update(input);

            input.previousKeyboardState = input.currentKeybardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(colorPalettes[nextColorPalette].Color5);

            if (audioAnalyzer.CurrentAnalyzedAudio.FFT != null)
            {
                currentVisualization.Draw(gameTime, audioAnalyzer.CurrentAnalyzedAudio);
            }

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
