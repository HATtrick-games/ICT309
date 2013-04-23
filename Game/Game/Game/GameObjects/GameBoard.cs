using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;

namespace ICT309Game.GameObjects
{
    class GameBoard: GameObject
    {
        private int[,] boardData = new int[10,10];

        public GameBoard()
        {

        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            base.OnUpdate(deltaTime);
        }
    }
}
