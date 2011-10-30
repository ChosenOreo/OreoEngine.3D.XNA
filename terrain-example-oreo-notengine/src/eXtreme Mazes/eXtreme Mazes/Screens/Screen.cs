using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace eXtreme_Mazes
{
    public enum ScreenType
    {
        ScreenIntro = 0,
        ScreenHelp,
        ScreenSingle,
        ScreenMulti,
        ScreenOptions,
        ScreenGameList,
        ScreenMazeList,
        ScreenGame,
        ScreenEnd,
        ScreenCredits
    };

    public abstract class Screen
    {
        public abstract void SetFocus(ContentManager content, bool focus);
        public abstract void ProcessInput(float elapsedTime, InputManager input);
        public abstract void Update(float elapsedTime);
        public abstract void Draw3D(GraphicsDevice gd);
        public abstract void Draw2d(GraphicsDevice gd, FontManager font);
    }
}
