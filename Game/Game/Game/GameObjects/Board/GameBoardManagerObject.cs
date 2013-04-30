using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;
using Microsoft.Practices.ServiceLocation;
using ICT309Game.GameObjects;
using ICT309Game.GameObjects.Board;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Game.Input;
using DigitalRune.Geometry;
using DigitalRune.Graphics;
using Microsoft.Xna.Framework;
using DigitalRune.Collections;

namespace ICT309Game.GameObjects.Board
{
    enum SquareData
    {
        BLOCKED = 0,
        EMPTY = 1,
        OCCUPIED = 2,
        HIGHLIGHTED = 3,
    };

    class GameBoardManagerObject:GameObject
    {
        private const float startPos = -114.5f;
        private const float gap = 25.5f;

        public static readonly Vector3F[,] Positions = new Vector3F[10, 10];

        private RedSquareObject[,] redGameBoard = new RedSquareObject[10,10];
        private WhiteSquareObject[,] whiteGameBoard = new WhiteSquareObject[10, 10];
        private BlueSquareObject[,] blueGameBoard = new BlueSquareObject[10, 10];

        public SquareData[,] GameBoard = new SquareData[10, 10];

        public GameBoardManagerObject()
        {
            for (int i = 0; i < Positions.GetLength(0); i++)
            {
                for (int j = 0; j < Positions.GetLength(1); j++)
                {
                    Positions[i, j] = new Vector3F(gap * i + startPos, 0.0f, gap * j + startPos);
                }
            }

            ResetBoard();

            // TODO make the game board data load from an external file
            // in the meantime heres this array

           
        }

        protected override void  OnLoad()
        {
            var gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();

            for (int i = 0; i < GameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < GameBoard.GetLength(1); j++)
                {
                    blueGameBoard[i, j] = new BlueSquareObject(new Vector3F(
                                            Positions[i,j].X,
                                            0.0f,
                                            Positions[i, j].Z));
                    redGameBoard[i, j] = new RedSquareObject(new Vector3F(
                                            Positions[i, j].X,
                                            0.0f,
                                            Positions[i, j].Z));
                    whiteGameBoard[i, j] = new WhiteSquareObject(new Vector3F(
                                            Positions[i, j].X,
                                            0.0f,
                                            Positions[i, j].Z));
                    
                    gameObjectService.Objects.Add(redGameBoard[i, j]);
                    gameObjectService.Objects.Add(blueGameBoard[i, j]);
                    gameObjectService.Objects.Add(whiteGameBoard[i, j]);
                }
            }
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            for (int i = 0; i < GameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < GameBoard.GetLength(1); j++)
                {
                    switch(GameBoard[i,j])
                    {
                        case SquareData.BLOCKED:
                            redGameBoard[i, j].InUse = false;
                            whiteGameBoard[i, j].InUse = false;
                            blueGameBoard[i, j].InUse = false;
                            break;
                        case SquareData.EMPTY:
                            redGameBoard[i, j].InUse = false;
                            whiteGameBoard[i, j].InUse = true;
                            blueGameBoard[i, j].InUse = false;
                            break;
                        case SquareData.HIGHLIGHTED:
                            redGameBoard[i, j].InUse = true;
                            whiteGameBoard[i, j].InUse = false;
                            blueGameBoard[i, j].InUse = false;
                            break;
                        case SquareData.OCCUPIED:
                            redGameBoard[i, j].InUse = false;
                            whiteGameBoard[i, j].InUse = false;
                            blueGameBoard[i, j].InUse = true;
                            break;
                    }
                }
            }

            var gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();
            var inputService = ServiceLocator.Current.GetInstance<IInputService>();
            CameraObject camera; 
            gameObjectService.Objects.TryGet("Camera", out camera);

            Vector3F mousePos = new Vector3F();

            if (inputService.IsPressed(MouseButtons.Left, true))
            {
                mousePos = camera.MousePosition;

                int IndexI = (int)((mousePos.X - startPos + (gap / 2.0f)) / gap);
                int IndexJ = (int)((mousePos.Z - startPos + (gap / 2.0f)) / gap);

                if (IndexI >= 0 && IndexI <= 9 && IndexJ >= 0 && IndexJ <= 9)
                {
                    ResetBoard();
                    ShowMovementRange(IndexI, IndexJ, 2);
                }
            } 
           

            base.OnUpdate(deltaTime);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        private void ResetBoard()
        {
            for (int i = 0; i < GameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < GameBoard.GetLength(1); j++)
                {
                    GameBoard[i, j] = SquareData.EMPTY;
                }
            }
        }

        public void ShowMovementRange(int x, int z, int range)
        {
            if(!CheckRange(x) ||
                !CheckRange(z) ||
                !CheckRange(range))
            {
                return;
            }

            List<Pair<int>> nodeList = new List<Pair<int>>();
            nodeList.Add(new Pair<int>(x,z));

            while (range > 0)
            {
                int count = nodeList.Count;

                for (int i = 0; i < count; i++)
                {
                    // UP
                    if (CheckRange(nodeList[i].First + 1))
                        nodeList.Add(new Pair<int>(nodeList[i].First + 1, nodeList[i].Second));

                    // DOWN
                    if (CheckRange(nodeList[i].First - 1))
                        nodeList.Add(new Pair<int>(nodeList[i].First - 1, nodeList[i].Second));

                    // LEFT
                    if (CheckRange(nodeList[i].Second + 1))
                        nodeList.Add(new Pair<int>(nodeList[i].First, nodeList[i].Second + 1));

                    // RIGHT
                    if (CheckRange(nodeList[i].Second - 1))
                        nodeList.Add(new Pair<int>(nodeList[i].First, nodeList[i].Second - 1));
                }

                range--;
            }

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (CheckRange(nodeList[i].First) && CheckRange(nodeList[i].Second))
                    // TODO 
                    GameBoard[nodeList[i].First, nodeList[i].Second] = SquareData.HIGHLIGHTED;
            }

            GameBoard[x, z] = SquareData.OCCUPIED;
        }

        private bool CheckRange(int x)
        {
            if (x >= 10 || x < 0)
            {
                return false;
            }

            return true;
        }
    }
}
