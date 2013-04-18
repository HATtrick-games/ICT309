using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game.States;
using Microsoft.Xna.Framework;

namespace ICT309Game.Screens
{
    class IScreen : State
    {
        protected override void OnEnter(StateEventArgs eventArgs)
        {
            Initialize();
            LoadContent();

            base.OnEnter(eventArgs);
        }

        protected override void OnExit(StateEventArgs eventArgs)
        {
            UnloadContent();

            base.OnExit(eventArgs);
        }

        protected override void OnUpdate(StateEventArgs eventArgs)
        {
            Update(eventArgs.DeltaTime);
            Draw(eventArgs.DeltaTime);

            base.OnUpdate(eventArgs);
        }

        public virtual void Initialize() { }

        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        public virtual void Update(TimeSpan gameTime) { }

        public virtual void Draw(TimeSpan gameTime) { }
    }
}
