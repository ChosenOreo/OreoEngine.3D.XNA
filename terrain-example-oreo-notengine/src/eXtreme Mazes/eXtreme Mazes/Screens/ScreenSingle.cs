﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eXtreme_Mazes.Screens
{
    class ScreenSingle : Screen
    {
        ScreenManager screenManager;
        GameManager gameManager;

        public ScreenSingle(ScreenManager manager, GameManager game)
        {
            screenManager = manager;
            gameManager = game;
        }
    }
}