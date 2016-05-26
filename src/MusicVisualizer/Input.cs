namespace MusicVisualizer
{
    using Microsoft.Xna.Framework.Input;

    public class Input
    {
        internal KeyboardState currentKeybardState;
        internal KeyboardState previousKeyboardState;

        public bool IsNewKeyDown(Keys key)
        {
            return currentKeybardState.IsKeyDown(key) &&
                !previousKeyboardState.IsKeyDown(key);
        }
    }
}
