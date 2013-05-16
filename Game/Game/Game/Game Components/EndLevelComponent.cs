using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ICT309Game.GameObjects;
using ICT309Game.Save;

namespace ICT309Game.Game_Components
{
    class EndLevelComponent : DrawableGameComponent
    {

        public EndLevelComponent(Game game, bool allyWin, int levelComplete, List<CharacterObject> remaining)
            : base(game)
        {
            SaveLoadGame.InitiateSave(levelComplete, remaining);
        }
    }
}
