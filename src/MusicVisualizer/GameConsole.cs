namespace MusicVisualizer
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;

    class GameConsole : DrawableGameComponent
    {
        private List<Message> messages;

        private readonly IApplicationShell appShell;

        public GameConsole(Game game) 
            : base(game)
        {
            appShell = (IApplicationShell)game;

            messages = new List<Message>();
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < messages.Count; i++)
            {
                messages[i].CurrentLifetime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (messages[i].CurrentLifetime > messages[i].Lifetime.TotalMilliseconds)
                {
                    messages.RemoveAt(i);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var defaultHeight = appShell.Font.MeasureString("A").Y * 1.4f;

            appShell.SpriteBatch.Begin();

            for (int i = 0; i < messages.Count; i++)
            {
                appShell.SpriteBatch.DrawString(appShell.Font, messages[i].Text, new Vector2(10, defaultHeight * i), messages[i].Color);
            }

            appShell.SpriteBatch.End();

            base.Draw(gameTime);
        }

        public void WriteLine(string text, Color color, TimeSpan? lifetime = null)
        {
            messages.Add(new Message()
            {
                Text = text,
                Color = color,
                Lifetime = lifetime ?? TimeSpan.FromSeconds(1)
            });
        }
        
        class Message
        {
            public double LifetimeLeft => CurrentLifetime / Lifetime.TotalMilliseconds;

            public string Text;
            public Color Color;
            public TimeSpan Lifetime;

            public double CurrentLifetime;
        }
    }
}
