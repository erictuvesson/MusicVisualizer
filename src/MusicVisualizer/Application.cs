namespace MusicVisualizer
{
    using Newtonsoft.Json;
    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Audio;
    using Visualizations;

    public class Application : GameWindow, IApplicationShell
    {
        static string BaseTitleText = "Music Visualization";


        public AppSettings AppSettings { get; private set; }

        public ColorPalette ColorPalette => colorPalettes[nextColorPalette];

        int IApplicationShell.Width => base.Width;
        int IApplicationShell.Height => base.Height;

        
        private AudioPlayback audioPlayback;
        private AudioAnalyzer audioAnalyzer;

        private List<ColorPalette> colorPalettes;
        private int nextColorPalette;

        private List<Visualization> visualizations;
        private int nextVisualization;
        private Visualization currentVisualization;

        public Application()
            : base(1280, 900)
        {
            this.Title = BaseTitleText;
            
            AppSettings = new AppSettings(4096 * 2);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        Exit();
                        break;
                    case Key.Space:
                        NextVisualization();
                        break;
                    case Key.C:
                        NextColorPattern();
                        break;
                    case Key.F11:
                        //ToggleBorderless();
                        break;
                }
            }

            currentVisualization?.OnKeyDown(e);

            base.OnKeyDown(e);
        }
        
        protected override void OnLoad(EventArgs e)
        {
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

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            this.audioAnalyzer.Dispose();
            this.audioPlayback.Dispose();

            base.OnUnload(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(colorPalettes[nextColorPalette].Color5);

            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            if (audioAnalyzer.CurrentAnalyzedAudio.FFT != null)
            {
                currentVisualization.Draw((float)e.Time, audioAnalyzer.CurrentAnalyzedAudio);
            }

            SwapBuffers();

            base.OnRenderFrame(e);
        }

        private void NextVisualization()
        {
            if (nextVisualization <= visualizations.Count)
            {
                currentVisualization = visualizations[nextVisualization];
                currentVisualization.AppShell = this;
            }

            this.Title = string.Format("{0} - {1}", BaseTitleText, currentVisualization.Title);
            nextVisualization = (nextVisualization + 1) % visualizations.Count;
        }

        private void NextColorPattern()
        {
            nextColorPalette = (nextColorPalette + 1) % colorPalettes.Count;
        }

        #region Entry Point
        [STAThread]
        static void Main()
        {
            using (var game = new Application())
            {
                game.Run(60.0);
            }
        }
        #endregion
    }
}
