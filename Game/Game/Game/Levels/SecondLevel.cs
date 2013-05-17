using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Graphics.SceneGraph;
using ICT309Game.GameObjects;

namespace ICT309Game.Levels
{
    class SecondLevel : Level
    {
        MainCharacter _MainCharacter;
        AIRangedCharacter _AIRangedCharacter;
        RangedCharacter _RangedCharacter;
        AIWeakCharacter _AIWeakCharacter;

        public SecondLevel()
            : base()
        {
            NumberOfTraps = 5;
            _levelModel = "Level/castlelevel";
            _backgroundSong = "SoundFX/second_level";
            _boardData = new bool[10, 10];

            for (int i = 0; i < _boardData.GetLength(0); i++)
            {
                for (int j = 0; j < _boardData.GetLength(1); j++)
                {
                    _boardData[i, j] = true;
                }
            }

            _MainCharacter = new MainCharacter();
            _MainCharacter.PosX = 2;
            _MainCharacter.PosY = 2;
            _AIRangedCharacter = new AIRangedCharacter();
            _AIRangedCharacter.PosX = 7;
            _AIRangedCharacter.PosY = 7;
            _RangedCharacter = new RangedCharacter();
            _RangedCharacter.PosX = 1;
            _RangedCharacter.PosY = 2;
            _AIWeakCharacter = new AIWeakCharacter();
            _AIWeakCharacter.PosX = 7;
            _AIWeakCharacter.PosY = 8;

            _characters = new List<CharacterObject>();
            _characters.Add(_MainCharacter);
            _characters.Add(_AIRangedCharacter);
            _characters.Add(_RangedCharacter);
            _characters.Add(_AIWeakCharacter);
        }
    }
}
