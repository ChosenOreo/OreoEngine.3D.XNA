using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eXtreme_Mazes.Screens
{
    class ScreenGameList : Screen
    {
        ScreenManager screenManager;
        GameManager gameManager;

        public ScreenGameList(ScreenManager manager, GameManager game)
        {
            screenManager = manager;
            gameManager = game;
        }
    }
}
