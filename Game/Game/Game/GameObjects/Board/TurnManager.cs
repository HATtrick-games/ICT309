using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICT309Game.GameObjects.Board
{
    class TurnManager
    {
        private List<CharacterObject> characterList = new List<CharacterObject>();

        public CharacterObject CurrentTurn { get; private set; }

        public TurnManager()
        {
        }

        public void AddToList(CharacterObject element)
        {
            characterList.Add(element);

            if (characterList.Count == 1) CurrentTurn = characterList[0];
        }

        public void RemoveFromList(CharacterObject element)
        {
            characterList.Remove(element);
        }

        public void ChangeTurn()
        {
            if (characterList.Count == 1) return;

            characterList.RemoveAt(0);

            characterList.Insert(characterList.Count, CurrentTurn);

            CurrentTurn = characterList[0];
        }
    }
}
