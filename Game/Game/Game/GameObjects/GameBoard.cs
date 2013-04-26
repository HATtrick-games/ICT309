using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics.Algebra;

namespace ICT309Game.GameObjects
{
    enum SquareData
    {
        EMPTY = 0,
        HIGHLIGHTED = 1,
        OCCUPIED = 2,
        BLOCKED = 3
    }

    class GameBoard: GameObject
    {
        //public SquareData[,] boardData = new SquareData[10,10];
        public SquareData[,] boardData = {{SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY},
                                          {SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY},
                                          {SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.OCCUPIED,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY},
                                          {SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.OCCUPIED,SquareData.OCCUPIED,SquareData.OCCUPIED,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY},
                                          {SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.OCCUPIED,SquareData.OCCUPIED,SquareData.HIGHLIGHTED,SquareData.OCCUPIED,SquareData.OCCUPIED,SquareData.EMPTY,SquareData.EMPTY},
                                          {SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.OCCUPIED,SquareData.OCCUPIED,SquareData.OCCUPIED,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY},
                                          {SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.OCCUPIED,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY},
                                          {SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY},
                                          {SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.BLOCKED,SquareData.BLOCKED,SquareData.EMPTY,SquareData.EMPTY},
                                          {SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY,SquareData.EMPTY}};
        public Vector3F[,] boardPositions = new Vector3F[10, 10];

        private ModelNode _boxModel;

        public GameBoard()
        {
            /*for (int i = 0; i < boardData.GetLength(0); i++)
            {
                for (int j = 0; j < boardData.GetLength(1); j++)
                {
                    boardData[i, j] = SquareData.HIGHLIGHTED;
                }
            }*/

            for (int i = 0; i < boardPositions.GetLength(0); i++)
            {
                for (int j = 0; j < boardPositions.GetLength(1); j++)
                {
                    boardPositions[i, j] = new Vector3F(25.5f * i - 114.5f,
                                                    0,
                                                    25.5f * j - 114.5f);
                }
            }

        }

        protected override void OnLoad()
        {
            Console.WriteLine("GameBoard OnLoad");
            base.OnLoad();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            base.OnUpdate(deltaTime);
        }


    }
}
