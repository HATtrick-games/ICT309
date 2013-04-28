using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICT309Game.PathFinding
{
    class PathFinder
    {
        //length and width of the map in number of squares.
        private int mapWidth = 10, mapHeight = 10;
        //arrays.
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

        public void Intialise()
        {
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
        




    }
}
