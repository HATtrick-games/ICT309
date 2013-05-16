using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;
using ICT309Game.GameObjects.Board;

namespace ICT309Game.GameObjects.AI
{
    class AIHandlerObject : GameObject
    {
        int turn = 1;
        bool PickTarget = false;
        bool MoveToTarget = false;
        bool AttackTarget = false;
        bool TurnCompleted = false;
        int target;
        float max = -200;
        float min = 200;
        int minx;
        int miny;
        int maxindex = 0;
        public bool IsAITurn { get; set; }
        public bool EndAITurn { get; set; }
        public GameBoardManagerObject GameBoardObj;
        public AIHandlerObject(GameBoardManagerObject GameBoard)
        {
            GameBoardObj = GameBoard;
            IsAITurn = false;
            EndAITurn = false;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        private int Distance(CharacterObject target, CharacterObject source)
        {
            int disx = source.PosX - target.PosX;
            int disy = source.PosY - target.PosY;

            if (disx < 0) disx = disx * -1;
            if (disy < 0) disy = disy * -1;

            int dis = disx + disy;
            return dis;
        }

        private int Distance(int x1, int y1, int x2, int y2)
        {
            int disx = x1 - x2;
            int disy = y1 - y2;

            if (disx < 0) disx = disx * -1;
            if (disy < 0) disy = disy * -1;

            int dis = disx + disy;
            return dis;
        }
        /*
        Total Damage=Attackers Damage-(Attackers Damage × Defenders Armor)/100

	
Unit	Unit Importance
Main Character	10
Ranged	7
Melee Healer	3
Melee Fighter	4
Mage	5

	Movement Difference=Movement Amount-Distance to Target

	Decision Score=(Total Damage)/10+Unit Importance+Movement Difference

        */
        public int SelectTarget()
        {
            max = -1000;
            float[] scores = new float[GameBoardObj.TurnManager.characterList.Count];
            float[] distances = new float[GameBoardObj.TurnManager.characterList.Count];
            float[] damages = new float[GameBoardObj.TurnManager.characterList.Count];

            for (int i = 0; i < GameBoardObj.TurnManager.characterList.Count; i++)
            {
                distances[i] = Distance(GameBoardObj.TurnManager.CurrentTurn, GameBoardObj.TurnManager.characterList[i]) - GameBoardObj.TurnManager.CurrentTurn.Movement;
                damages[i] = (GameBoardObj.TurnManager.CurrentTurn.Damage - ((GameBoardObj.TurnManager.CurrentTurn.Damage*GameBoardObj.TurnManager.characterList[i].Armor) / 100));
                if (GameBoardObj.TurnManager.characterList[i].isAlly)
                {
                    scores[i] = damages[i] / (10 + GameBoardObj.TurnManager.characterList[i].importance + distances[i]);
                    if ((scores[i] > max)&&(distances[i] != GameBoardObj.TurnManager.CurrentTurn.Movement))
                    {
                        Console.WriteLine(scores[i]);
                        max = scores[i];
                        maxindex = i;
                    }
                }
                else
                {
                    scores[i] = 1000000;
                }
            }
            return maxindex;
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            //for (int i = 0; i < TurnManager.characterList.Count; i++)
            if (IsAITurn)
            {
                if (turn == -10)
                {
                    target = SelectTarget();
                    if (Distance(GameBoardObj.TurnManager.CurrentTurn, GameBoardObj.TurnManager.characterList[target]) == 1)
                    {
                        //Console.WriteLine("I AM BESIDE MY TARGET");

                    }
                    else
                    {
                        min = 200;
                        for (int x = 0; x < 10; x++)
                        {
                            //Console.WriteLine("DERp");
                            for (int y = 0; y < 10; y++)
                            {
                                if (GameBoardObj.GameBoard[x, y] == SquareData.HIGHLIGHTEDRED)
                                {
                                   // Console.WriteLine("DERp");
                                   
                                    if (Distance(x, y, GameBoardObj.TurnManager.characterList[target].PosX, GameBoardObj.TurnManager.characterList[target].PosY)<min)
                                    {
                                        min = Distance(x, y, GameBoardObj.TurnManager.characterList[target].PosX, GameBoardObj.TurnManager.characterList[target].PosY);
                                        minx = x;
                                        miny = y;
                                        MoveToTarget = true;
                                    }
                                }
                            }
                        }
                    }
                    
                    if (MoveToTarget == true)
                    {
                        GameBoardObj.Pathfinding(minx, miny);
                    }
                }
                
                
                turn--;
                
            }

            if ((GameBoardObj.MovementInProgress == false)&&(turn<-15))
            {
                if (Distance(GameBoardObj.TurnManager.CurrentTurn, GameBoardObj.TurnManager.characterList[target]) == 1)
                {
                    Console.WriteLine("I AM BESIDE MY TARGET");
                    GameActions.ResolveCombat(GameBoardObj.TurnManager.CurrentTurn, GameBoardObj.TurnManager.characterList[target]);
                    
                }

                EndAITurn = true;
                MoveToTarget = false;
                turn = 1;
                minx = 0;
                miny = 0;
            }


            base.OnUpdate(deltaTime);
        }



        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
