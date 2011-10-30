using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eXtreme_Mazes.Screens
{
    class ScreenCredits : Screen
    {
        ScreenManager screenManager;
        GameManager gameManager;

        public ScreenCredits(ScreenManager manager, GameManager game)
        {
            screenManager = manager;
            gameManager = game;
        }
    }
}
