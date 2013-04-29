using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICT309Game.PathFinding
{
    class PathFinder
    {
        int onClosedList = 0;
        //length and width of the map in number of squares.
        private int mapWidth = 10, mapHeight = 10;
        //constants
        const int walkable = 0, unwalkable = 1;
        const bool found = true, notfound = false;
        //arrays.
        private int[,] walkability;
        private int[] openList;
        private int[,] whichList;
        private int[] openX;
        private int[] openY;
        private int[,] parentX;
        private int[,] parentY;
        private int[] fCost;
        private int[,] gCost;
        private int[] hCost;
        private int[] pathLength;
        private int[] pathLocation;

        //sets up all arrays
        public void Intialise()
        {
            walkability = new int[mapWidth, mapHeight];
            openList = new int[mapWidth * mapHeight + 2];
            whichList = new int[mapWidth+1,mapHeight+1];
            openX = new int[mapWidth * mapHeight + 2];
            openY = new int[mapWidth * mapHeight + 2];
            parentX = new int[mapWidth + 1, mapHeight + 1];
            parentY = new int[mapWidth + 1, mapHeight + 1];
            fCost = new int[mapWidth * mapHeight + 2];
            gCost = new int[mapWidth + 1, mapHeight + 1];
            hCost = new int[mapWidth * mapHeight];
            pathLength = new int[1];
            pathLocation = new int[1];
        }

        //main function for finding a  path, takes in a starting point and finishing point
        public bool FindPath(int startingX, int startingY, int targetpassX, int targetpassY)
        {
            int startX = startingX;
            int startY = startingY;
            int targetX = targetpassX;
            int targetY = targetpassY;

            int onOpenList = 0, parentXval = 0, parentYval = 0,
             a = 0, b = 0, m = 0, u = 0, v = 0, temp = 0, corner = 0, numberOfOpenListItems = 0,
             addedGCost = 0, tempGcost = 0, path = 0, x = 0, y = 0,
             tempx, pathX, pathY, cellPosition,
             newOpenListItemID = 0;


            //if the target points is unwalkable, return not found.
            if (walkability[targetX, targetY] == unwalkable)
            {
                return notfound;
            }

            //resest the which list array
            for (x = 0; x < mapWidth; x++)
            {
                for (y = 0; y < mapHeight; y++)
                    whichList[x,y] = 0;
            }

            onClosedList = 2;
            onOpenList = 1;
            gCost[startX, startY] = 0;

            numberOfOpenListItems = 1;
            openList[1] = 1;
            openX[1] = startX; openY[1] = startY;


            //***************************************MAIN LOOP STARTS HERE*************************************************//
           do
           {
                //if there are items in the open list do this
                if (numberOfOpenListItems != 0)
                {
                    parentXval = openX[openList[1]];
                    parentYval = openY[openList[1]];
                    whichList[parentXval, parentYval] = onClosedList;

                    numberOfOpenListItems = numberOfOpenListItems - 1;
                    openList[1] = openList[numberOfOpenListItems + 1];
                    v = 1;



                    do
                    {
                        u = v;
                        if (2 * u + 1 <= numberOfOpenListItems)
                        {
                            if (fCost[openList[u]] >= fCost[openList[2 * u]])
                            {
                                v = 2 * u;
                            }
                            if (fCost[openList[v]] >= fCost[openList[2 * u + 1]])
                            {
                                v = 2 * u + 1;
                            }
                        }
                        else
                        {
                            if (28U <= numberOfOpenListItems)
                            {
                                if(fCost[openList[u]] >= fCost[openList[2*u]])
                                v = 2 * u;
                            }
                        }

                        if (u != v)
                        {
                            temp = openList[u];
                            openList[u] = openList[v];
                            openList[v] = temp;
                        }
                        else
                        {
                            break;
                        }

                        //break;
                    } while (true);


                }





                break;

            } while (true);






            return found;
         }




    }
}
