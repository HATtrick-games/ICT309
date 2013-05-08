using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;

namespace ICT309Game.Game_Components.UI
{
    class GameLog : TextBox
    {
        String _log = "";
        int messageCount = 0;

        public GameLog()
            : base()
        {
            HorizontalAlignment = DigitalRune.Game.UI.HorizontalAlignment.Center;
            Width = 700;
            Height = 80;
            Text = _log;
            Background = new Microsoft.Xna.Framework.Color(0.0f, 0.0f, 0.0f, 0.5f);
            Y = 630;
            MinLines = 3;
            MaxLines = 3;
            IsReadOnly = true;
            Font = "Consolas";
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            Text = _log;

            base.OnUpdate(deltaTime);
        }

        protected override void OnHandleInput(InputContext context)
        {

            base.OnHandleInput(context);
        }

        public void AddMessage(string message)
        {
            _log = message + "\n" + Text;
        }
    }
}
