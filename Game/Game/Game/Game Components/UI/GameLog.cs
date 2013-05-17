using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;

namespace ICT309Game.Game_Components.UI
{
    /// <summary> Game log.</summary>
    ///
    /// Displays a summary of all major actions that take place within the 
    /// game.
    class GameLog
    {
        /// <summary> Gets or sets the log.</summary>
        ///
        /// <value> The log.</value>
        public String _log { get; private set; }
        int messageCount = 0;

        /// <summary> Default constructor.</summary>
        public GameLog()
        {
            _log = "";
        }

        /// <summary> Adds a message to the game log.</summary>
        ///
        /// <param name="message"> The message.</param>
        public void AddMessage(string message)
        {
            _log += message + "\n";
            messageCount++;
        }

        /// <summary> Clears/resets the game log.</summary>
        public void ResetLog()
        {
            _log = "";
        }
    }
}
