using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Graphics.SceneGraph;
using ICT309Game.GameObjects;

namespace ICT309Game.Levels
{
    class FirstLevel : Level
    {
        MainCharacter _MainCharacter;
        AIRangedCharacter _AIRangedCharacter;
        RangedCharacter _RangedCharacter;
        AIWeakCharacter _AIWeakCharacter;

        public FirstLevel()
            : base()
        {
            _levelModel = "Board/testmodel";
            _backgroundSong = "SoundFX/first_level";
            _boardData = new bool[10, 10];

            for (int i = 0; i < _boardData.GetLength(0); i++)
            {
                for (int j = 0; j < _boardData.GetLength(1); j++)
                {
                    _boardData[i, j] = true;
                }
            }

            _MainCharacter = new MainCharacter();
            _AIRangedCharacter = new AIRangedCharacter();
            _RangedCharacter = new RangedCharacter();
            _AIWeakCharacter = new AIWeakCharacter();

            _characters = new List<CharacterObject>();
            _characters.Add(_MainCharacter);
            _characters.Add(_AIRangedCharacter);
            _characters.Add(_RangedCharacter);
            _characters.Add(_AIWeakCharacter);
        }
    }
}
