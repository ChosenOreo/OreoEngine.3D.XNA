using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eXtreme_Mazes.Screens
{
    class ScreenGame : Screen
    { 
        ScreenManager screenManager;
        GameManager gameManager;

        public ScreenGame(ScreenManager manager, GameManager game)
        {
            screenManager = manager;
            gameManager = game;
        }
    }
}
