using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionDetection
{
    class CharacterObject : ForegroundObject
    {
        private enum Motion
        {
            private enum Moving
                {
                    Up,
                    UpRight,
                    Right,
                    DownRight,
                    Down,
                    DownLeft,
                    Left,
                    UpLeft
                },
            Static
        }

        private Motion characterMotion = Motion.Static;

        

        private Moving characterMoving = Moving.NotMoving;

        private int xCoord = 0;
        private int yCoord = 0;
        
    }
}
