using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;

namespace ICT309Game.Game_Components.UI
{
    class GameLog
    {
        public String _log { get; private set; }
        int messageCount = 0;

        public GameLog()
        {
            _log = "";
        }

        public void AddMessage(string message)
        {
            _log += message + "\n";
            messageCount++;
        }

        public void ResetLog()
        {
            _log = "";
        }
    }
}
