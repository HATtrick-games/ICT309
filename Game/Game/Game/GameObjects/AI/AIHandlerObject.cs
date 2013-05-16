using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;

namespace ICT309Game.GameObjects.AI
{
    class AIHandlerObject : GameObject
    {
        public bool IsAITurn { get; set; }
        public bool EndAITurn { get; set; }

        public AIHandlerObject()
        {
            IsAITurn = false;
            EndAITurn = false;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if (IsAITurn)
            {
                EndAITurn = true; 
                Console.WriteLine("Is AI Turn");
            }

            base.OnUpdate(deltaTime);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
