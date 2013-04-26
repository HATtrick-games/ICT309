using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;
using Microsoft.Practices.ServiceLocation;
using ICT309Game.GameObjects;
using DigitalRune.Mathematics.Algebra;

namespace ICT309GameGame.GameObjects
{
    class GameBoardManagerObject:GameObject
    {
        private const float startPos = -114.5f;
        private const float gap = 25.5f;

        private SquareObject[,] gameBoard = new SquareObject[10,10];

        public GameBoardManagerObject()
        {
            var gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();

            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    gameBoard[i, j] = new SquareObject(new Vector3F(
                                            gap * i + startPos,
                                            0.0f,
                                            gap * j + startPos));
                    gameObjectService.Objects.Add(gameBoard[i,j]);
                }

            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
