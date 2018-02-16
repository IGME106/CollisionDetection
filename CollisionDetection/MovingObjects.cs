using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionDetection
{
    class MovingObjects : GameObjects
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
            UpLeft,
            NotMoving
        }

        private Moving currentMovement = Moving.NotMoving;

        private enum ObjectType
        {
            Player,
            Enemy,
            Other
        }

        private ObjectType objectType = ObjectType.Other;

        private int xCoord;
        private int yCoord;

    }
}
