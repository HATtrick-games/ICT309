﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;

namespace ICT309Game.GameObjects.AI
{
    class AIHandlerObject : GameObject
    {
        public bool IsAITurn { get; set; }

        public AIHandlerObject()
        {
            IsAITurn = false;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if(IsAITurn) Console.WriteLine("Is AI Turn");

            base.OnUpdate(deltaTime);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}