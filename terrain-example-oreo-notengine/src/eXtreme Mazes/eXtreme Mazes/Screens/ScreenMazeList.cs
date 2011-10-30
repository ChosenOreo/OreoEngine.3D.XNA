using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eXtreme_Mazes.Screens
{
    class ScreenMazeList : Screen
    {
        ScreenManager screenManager;
        GameManager gameManager;

        public ScreenMazeList(ScreenManager manager, GameManager game)
        {
            screenManager = manager;
            gameManager = game;
        }
    }
}
