using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eXtreme_Mazes.Screens
{
    class ScreenOptions : Screen
    {
        ScreenManager screenManager;
        GameManager gameManager;

        public ScreenOptions(ScreenManager manager, GameManager game)
        {
            screenManager = manager;
            gameManager = game;
        }
    }
}
