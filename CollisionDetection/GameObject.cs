using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionDetection
{
    class GameObjects : Game1
    {
        private enum GameState
        {
            Menu,
            Game,
            GameOver
        }

        private GameState currentGameState = GameState.Menu;


    }
}
