﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            CurrentTurnStatus = TurnStatus.MOVEMENT;
        }

        public void ChangeStatus()
        {
            if (CurrentTurnStatus == TurnStatus.MOVEMENT) CurrentTurnStatus = TurnStatus.ACTION;
        }
    }
}
