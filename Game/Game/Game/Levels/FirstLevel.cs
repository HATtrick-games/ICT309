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
        AIWeakCharacter _AIWeakCharacter1;
        AIWeakCharacter _AIWeakCharacter2;
        MeleeFighterCharacter _MeleeFighterCharacter;

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
            _MainCharacter.PosX = 5;
            _MainCharacter.PosY = 0;
            _AIRangedCharacter = new AIRangedCharacter();
            _AIRangedCharacter.PosX = 5;
            _AIRangedCharacter.PosY = 9;
            _RangedCharacter = new RangedCharacter();
            _RangedCharacter.PosX = 4;
            _RangedCharacter.PosY = 0;
            _AIWeakCharacter1 = new AIWeakCharacter();
            _AIWeakCharacter1.PosX = 4;
            _AIWeakCharacter1.PosY = 9;
            _AIWeakCharacter2 = new AIWeakCharacter();
            _AIWeakCharacter2.PosX = 6;
            _AIWeakCharacter2.PosY = 9;
            _MeleeFighterCharacter = new MeleeFighterCharacter();
            _MeleeFighterCharacter.PosX = 6;
            _MeleeFighterCharacter.PosY = 0;

            _characters = new List<CharacterObject>();
            _characters.Add(_MainCharacter);
            _characters.Add(_AIRangedCharacter);
            _characters.Add(_RangedCharacter);
            _characters.Add(_AIWeakCharacter1);
            _characters.Add(_AIWeakCharacter2);
            _characters.Add(_MeleeFighterCharacter);
        }
    }
}
