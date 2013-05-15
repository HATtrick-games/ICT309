using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using ICT309Game.Game_Components.UI;

namespace ICT309Game.GameObjects.Board
{
    enum TurnStatus
    {
        MOVEMENT = 0,
        ACTION = 1
    };

    class TurnManager
    {
        public List<CharacterObject> characterList { get; private set; }

        
        public CharacterObject CurrentTurn { get; private set; }
        public TurnStatus CurrentTurnStatus { get; private set; }

        public TurnManager()
        {
            CurrentTurnStatus = TurnStatus.MOVEMENT;
            characterList = new List<CharacterObject>();
        }

        public void AddToList(CharacterObject element)
        {
            characterList.Add(element);

            if (characterList.Count == 1) CurrentTurn = characterList[0];
        }

        public void UpdateList()
        {
            for (int i = 0; i < characterList.Count; i++)
            {
                if (characterList[i].HitPoints <= 0)
                {
                    System.Console.WriteLine(characterList[i].CharacterName);
                    System.Console.WriteLine(characterList.ElementAt(i).CharacterName);
                    characterList[i].Unload();                                        
                    characterList.RemoveAt(i);

                }
            }
        }

        public void ChangeTurn()
        {
            UpdateList();

            if (characterList.Count == 1) return;

            characterList.RemoveAt(0);

            characterList.Insert(characterList.Count, CurrentTurn);

            CurrentTurn = characterList[0];
            CurrentTurnStatus = TurnStatus.MOVEMENT;
        }

        public void ChangeStatus()
        {
            if (CurrentTurnStatus == TurnStatus.MOVEMENT) CurrentTurnStatus = TurnStatus.ACTION;
        }
    }
}
