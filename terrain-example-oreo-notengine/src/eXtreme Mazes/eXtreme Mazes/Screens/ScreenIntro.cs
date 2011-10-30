using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eXtreme_Mazes.Screens
{
    class ScreenIntro : Screen
    {
        ScreenManager screenManager;
        GameManager gameManager;

        public ScreenIntro(ScreenManager manager, GameManager game)
        {
            screenManager = manager;
            gameManager = game;
        }
    }
}
