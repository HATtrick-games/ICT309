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
        AIFinalBossCharacter _AIFinalBoss;
        MeleeFighterCharacter _MeleeFighterCharacter1;
        AIRangedCharacter _AIRangedCharacter1;
        RangedCharacter _RangedCharacter;
        AIWeakCharacter _AIWeakCharacter;
        MeleeFighterCharacter _MeleeFighterCharacter2;
        AIRangedCharacter _AIRangedCharacter2;

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
            _MainCharacter.PosX = 5;
            _MainCharacter.PosY = 1;
            _AIFinalBoss = new AIFinalBossCharacter();
            _AIFinalBoss.PosX = 5;
            _AIFinalBoss.PosY = 8;
            _MeleeFighterCharacter1 = new MeleeFighterCharacter();
            _MeleeFighterCharacter1.PosX = 6;
            _MeleeFighterCharacter1.PosY = 0;
            _AIRangedCharacter1 = new AIRangedCharacter();
            _AIRangedCharacter1.PosX = 6;
            _AIRangedCharacter1.PosY = 9;
            _RangedCharacter = new RangedCharacter();
            _RangedCharacter.PosX = 4;
            _RangedCharacter.PosY = 0;
            _AIWeakCharacter = new AIWeakCharacter();
            _AIWeakCharacter.PosX = 4;
            _AIWeakCharacter.PosY = 9;
            _MeleeFighterCharacter2 = new MeleeFighterCharacter();
            _MeleeFighterCharacter2.PosX = 3;
            _MeleeFighterCharacter2.PosY = 0;
            _AIRangedCharacter2 = new AIRangedCharacter();
            _AIRangedCharacter2.PosX = 7;
            _AIRangedCharacter2.PosY = 9;



            _characters = new List<CharacterObject>();
            _characters.Add(_MainCharacter);
            _characters.Add(_AIFinalBoss);
            _characters.Add(_MeleeFighterCharacter1);
            _characters.Add(_AIRangedCharacter1);
            _characters.Add(_RangedCharacter);
            _characters.Add(_AIWeakCharacter);
            _characters.Add(_MeleeFighterCharacter2);
            _characters.Add(_AIRangedCharacter2);
        }
    }
}
