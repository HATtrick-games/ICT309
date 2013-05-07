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
        HIGHLIGHTEDRED = 3,
        HIGHLIGHTEDBLUE = 4,
    };

    class GameBoardManagerObject:GameObject
    {
        private const float startPos = -114.5f;
        private const float gap = 25.5f;
        private const int boardSize = 10;

        // BOARD PROPERTIES
        public static readonly Vector3F[,] Positions = new Vector3F[boardSize, boardSize];

        private RedSquareObject[,] redGameBoard = new RedSquareObject[boardSize,boardSize];
        private WhiteSquareObject[,] whiteGameBoard = new WhiteSquareObject[boardSize, boardSize];
        private BlueSquareObject[,] blueGameBoard = new BlueSquareObject[boardSize, boardSize];

        public SquareData[,] GameBoard = new SquareData[boardSize, boardSize];

        // CHARACTER OBJECTS
        private MainCharacter _mainCharacter;
        private RangedCharacter _rangedCharacter;
        private AIRangedCharacter _aiRangedCharacter;
        private AIWeakCharacter _aiWeakCharacter;

        public TurnManager TurnManager { get; private set; }

        public GameBoardManagerObject()
        {
            ResetBoard();  
            // LOAD IN LEVEL FILES FROM EXTERNAL FILE

            for (int i = 0; i < Positions.GetLength(0); i++)
            {
                for (int j = 0; j < Positions.GetLength(1); j++)
                {
                    Positions[i, j] = new Vector3F(gap * i + startPos, 0.0f, gap * j + startPos);
                }
            }

            
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

            _mainCharacter = new MainCharacter();
            _rangedCharacter = new RangedCharacter();
            _aiRangedCharacter = new AIRangedCharacter();
            _aiWeakCharacter = new AIWeakCharacter();

            gameObjectService.Objects.Add(_mainCharacter);
            gameObjectService.Objects.Add(_rangedCharacter);
            gameObjectService.Objects.Add(_aiRangedCharacter);
            gameObjectService.Objects.Add(_aiWeakCharacter);

            TurnManager = new TurnManager();
            TurnManager.AddToList(_mainCharacter);
            TurnManager.AddToList(_aiRangedCharacter);
            TurnManager.AddToList(_rangedCharacter);
            TurnManager.AddToList(_aiWeakCharacter);
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            ResetBoard();

            for (int i = 0; i < TurnManager.characterList.Count; i++)
            {
                GameBoard[TurnManager.characterList[i].PosX, TurnManager.characterList[i].PosY] = SquareData.OCCUPIED;
            }

            if (TurnManager.CurrentTurn.isAlly)
            {
                var gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();
                var inputService = ServiceLocator.Current.GetInstance<IInputService>();
                CameraObject camera;
                gameObjectService.Objects.TryGet("Camera", out camera);

                Vector3F mousePos = new Vector3F();

                if (TurnManager.CurrentTurnStatus == TurnStatus.MOVEMENT)
                {
                    ShowMovementRange(TurnManager.CurrentTurn.PosX, TurnManager.CurrentTurn.PosY, TurnManager.CurrentTurn.Movement);
                }

                if (TurnManager.CurrentTurnStatus == TurnStatus.ACTION)
                {
                    ShowAttackRange(TurnManager.CurrentTurn.PosX, TurnManager.CurrentTurn.PosY, TurnManager.CurrentTurn.Range);
                }
                
                if (inputService.IsPressed(MouseButtons.Right, false))
                {
                    mousePos = camera.MousePosition;

                    int IndexI = (int)((mousePos.X - startPos + (gap / 2.0f)) / gap);
                    int IndexJ = (int)((mousePos.Z - startPos + (gap / 2.0f)) / gap);

                    if (IndexI >= 0 && IndexI <= 9 && IndexJ >= 0 && IndexJ <= 9)
                    {
                        if (GameBoard[IndexI, IndexJ] == SquareData.HIGHLIGHTEDRED)
                        {
                            // Move character to selected square
                            TurnManager.CurrentTurn.PosX = IndexI;
                            TurnManager.CurrentTurn.PosY = IndexJ;

                            TurnManager.ChangeStatus();
                        }

                        if (GameBoard[IndexI, IndexJ] == SquareData.HIGHLIGHTEDBLUE)
                        {
                            // Attacking the target at the selected square
                            for (int i = 0; i < TurnManager.characterList.Count; i++)
                            {
                                if (TurnManager.characterList[i].PosX == IndexI && TurnManager.characterList[i].PosY == IndexJ)
                                {
                                    GameActions.ResolveCombat(TurnManager.CurrentTurn, TurnManager.characterList[i]);
                                    TurnManager.ChangeTurn();
                                }
                            }
                        }
                    }
                }
            }

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
                        case SquareData.HIGHLIGHTEDRED:
                            redGameBoard[i, j].InUse = true;
                            whiteGameBoard[i, j].InUse = false;
                            blueGameBoard[i, j].InUse = false;
                            break;
                        case SquareData.HIGHLIGHTEDBLUE:
                            redGameBoard[i, j].InUse = false;
                            whiteGameBoard[i, j].InUse = false;
                            blueGameBoard[i, j].InUse = true;
                            break;
                        case SquareData.OCCUPIED:
                            redGameBoard[i, j].InUse = false;
                            whiteGameBoard[i, j].InUse = true;
                            blueGameBoard[i, j].InUse = false;
                            break;
                    }
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
                    if (GameBoard[nodeList[i].First, nodeList[i].Second] != SquareData.OCCUPIED && GameBoard[nodeList[i].First, nodeList[i].Second] != SquareData.BLOCKED)
                    {
                        GameBoard[nodeList[i].First, nodeList[i].Second] = SquareData.HIGHLIGHTEDRED;
                    }
            }
        }

        public void ShowAttackRange(int x, int z, int range)
        {
            if (!CheckRange(x) ||
                !CheckRange(z) ||
                !CheckRange(range))
            {
                return;
            }

            List<Pair<int>> nodeList = new List<Pair<int>>();
            nodeList.Add(new Pair<int>(x, z));

            for (int i = 1; i <= range; i++)
            {
                if (CheckRange(x + i))
                    nodeList.Add(new Pair<int>(x + i, z));

                if (CheckRange(x - i))
                    nodeList.Add(new Pair<int>(x - i, z));

                if (CheckRange(z + i))
                    nodeList.Add(new Pair<int>(x, z + i));

                if (CheckRange(z - i))
                    nodeList.Add(new Pair<int>(x, z - i));
            }

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (GameBoard[nodeList[i].First, nodeList[i].Second] != SquareData.BLOCKED)
                {
                    if (nodeList[i].First == x && nodeList[i].Second == z) continue;

                    if (GameBoard[nodeList[i].First, nodeList[i].Second] != SquareData.OCCUPIED) continue;

                    for (int j = 0; j < TurnManager.characterList.Count; j++)
                    {
                        if (TurnManager.characterList[j].PosX == nodeList[i].First && TurnManager.characterList[j].PosY == nodeList[i].Second)
                        {
                            if (!TurnManager.characterList[j].isAlly)
                            {
                                GameBoard[nodeList[i].First, nodeList[i].Second] = SquareData.HIGHLIGHTEDBLUE;
                            }
                        }
                    }    
                }
            }
        }

        private bool CheckRange(int x)
        {
            if (x >= boardSize || x < 0)
            {
                return false;
            }

            return true;
        }
    }
}
