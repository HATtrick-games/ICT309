using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;
using Microsoft.Practices.ServiceLocation;
using ICT309Game.GameObjects;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Game.Input;
using DigitalRune.Geometry;
using DigitalRune.Graphics;
using Microsoft.Xna.Framework;

namespace ICT309GameGame.GameObjects
{
    class GameBoardManagerObject:GameObject
    {
        private const float startPos = -114.5f;
        private const float gap = 25.5f;

        private RedSquareObject[,] redGameBoard = new RedSquareObject[10,10];
        private WhiteSquareObject[,] whiteGameBoard = new WhiteSquareObject[10, 10];

        public GameBoardManagerObject()
        {
            var gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();

            for (int i = 0; i < redGameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < redGameBoard.GetLength(1); j++)
                {
                    redGameBoard[i, j] = new RedSquareObject(new Vector3F(
                                            gap * i + startPos,
                                            0.0f,
                                            gap * j + startPos));
                    whiteGameBoard[i, j] = new WhiteSquareObject(new Vector3F(
                                            gap * i + startPos,
                                            0.0f,
                                            gap * j + startPos));
                    gameObjectService.Objects.Add(redGameBoard[i,j]);
                    gameObjectService.Objects.Add(whiteGameBoard[i, j]);
                }
            }


            
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            var gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();
            var inputService = ServiceLocator.Current.GetInstance<IInputService>();

            CameraObject camera; 
            gameObjectService.Objects.TryGet("Camera", out camera);

            Vector3F mousePos = new Vector3F();

            if (inputService.IsPressed(MouseButtons.Left, true))
            {
                mousePos = camera.MousePosition;

                int IndexI = (int)((mousePos.X - startPos + 12.75f) / gap);
                int IndexJ = (int)((mousePos.Z - startPos + 12.75f) / gap);

                if (IndexI >= 0 && IndexI <= 9 && IndexJ >= 0 && IndexJ <= 9)
                {
                    //gameBoard[IndexI, IndexJ].InUse = !gameBoard[IndexI, IndexJ].InUse;

                    Console.WriteLine("Index I = " + IndexI);
                    Console.WriteLine("Index J = " + IndexJ);

                    redGameBoard[IndexI, IndexJ].InUse = true;
                    whiteGameBoard[IndexI, IndexJ].InUse = false;
                }
            }

            /**** Add debug rendering stuff here ****/
#if DEBUG
            var debugRenderer = ServiceLocator.Current.GetInstance<DebugRenderer>();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    debugRenderer.DrawLine(new Vector3F(gap * i + startPos, 2.0f, gap * j + startPos),
                        new Vector3F(gap * i + startPos + (114.5f), 2.0f, gap * j + startPos), Color.Red, false);
                    debugRenderer.DrawLine(new Vector3F(gap * i + startPos, 2.0f, gap * j + startPos),
                        new Vector3F(gap * i + startPos, 2.0f, gap * j + startPos + (114.5f)), Color.Red, false);
                }
            }
#endif

            base.OnUpdate(deltaTime);
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
