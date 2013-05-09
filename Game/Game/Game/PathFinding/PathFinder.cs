using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/*
 * to use this class create an instance of this object
 * then call the function FindPath, before every call of this function make sure to call initialise function first
 * the parameters for FindPath are the x and y of the starting point and the x and y of the finish point. if the function returns true, a path has been found
 * calling return path will then return an array of x and y structs starting at the end point and ending at the first step after the start point.
 * the position in the array of the end point is returned by the returnpathlength function. 
 * 
 * 
 * 
 */




namespace ICT309Game.Pathfinding
{
    class PathFinder
    {

        public struct node
        {
            public int x;
            public int y;
        }
        Dictionary<node, node> cameFrom = new Dictionary<node, node>();
        Dictionary<node, int> GcostMap = new Dictionary<node, int>();
        Dictionary<node, int> FcostMap = new Dictionary<node, int>();
        int length = 10;
        int width = 10;
        int min = 2000;
        public int[,] walkable;
        int openNum;
        int closeNum;
        node[] openList;
        node[] closedList;
        node[] path;
        int[,] WhichList;
        public node current;
        node[] Neighbours;
        int pathlength;
     


        public void SetWalkable(int xpos, int ypos, int walkablepass) //Setting to 1 will make that position unwalkable, any other value will make it walkable, places are walkable by default
        {
            walkable[xpos, ypos] = walkablepass;
        }

        public node[] returnpath()
        {
            Console.WriteLine("PATH WAS FOUND");
            reconstruct_path(current);
            for (int n = pathlength - 1; n > -1; n--)
            {
                Console.Write("STEP = ");
                Console.Write(path[n].x);
                Console.WriteLine(path[n].y);
            }
            return path;
        }

        //this returns the length of the array containing the path
        // the path begins at the end of the array so you can just use a backwards for loop
        // starting at pathlength and continuing while iterator is greater than -1
        public int returnpathlength()
        {
            return pathlength - 1;
        }


        public PathFinder()
        {
          
            pathlength = 0;
            path = new node[length * width + 10];
            cameFrom = new Dictionary<node, node>();
            GcostMap = new Dictionary<node, int>();
            FcostMap = new Dictionary<node, int>();
            walkable = new int[length, width];
            openList = new node[length * width + 3];
            closedList = new node[length * width + 3];
            WhichList = new int[length, width];
            Neighbours = new node[4];
        }

        public void Intiialise()
        {
            pathlength = 0;
            path = new node[length * width + 10];
            cameFrom = new Dictionary<node, node>();
            GcostMap = new Dictionary<node, int>();
            FcostMap = new Dictionary<node, int>();
            walkable = new int[length, width];
            openList = new node[length * width + 3];
            closedList = new node[length * width + 3];
            WhichList = new int[length, width];
            Neighbours = new node[4];
        }

        int Gcost(int x, int y, node pass)
        {
            int Gx = (x - pass.x);
            if (Gx < 0)
            {
                Gx = Gx * -1;
            }
            int Gy = (y - pass.y);
            if (Gy < 0)
            {
                Gy = Gy * -1;
            }

            return (Gx + Gy);

        }

        int numNeighbours(node spot)
        {
            int place = 0;
            node temp = spot;
            if (spot.x + 1 < 11)
            {
                if (walkable[spot.x + 1, spot.y] != 1)
                {
                    temp.x += 1;
                    Neighbours[place] = temp;
                    place++;
                    temp = spot;
                }
            }
            if (spot.y + 1 < 11)
            {
                if (walkable[spot.x, spot.y + 1] != 1)
                {
                    temp.y += 1;
                    Neighbours[place] = temp;
                    place++;
                    temp = spot;
                }
            }
            if (spot.x - 1 > 0)
            {
                if (walkable[spot.x - 1, spot.y] != 1)
                {
                    temp.x -= 1;
                    Neighbours[place] = temp;
                    place++;
                    temp = spot;
                }
            }
            if (spot.y - 1 > 0)
            {
                if (walkable[spot.x, spot.y - 1] != 1)
                {
                    temp.y -= 1;
                    Neighbours[place] = temp;
                    place++;
                    temp = spot;
                }
            }
            return place;
        }

        public bool FindPath(int startX, int startY, int endX, int endY)
        {
            node tempNode;
            node tempNode2;
            tempNode.x = startX;
            tempNode.y = startY;

            openList[0] = tempNode;
            closeNum = 0;
            openNum = 1;
            current = tempNode;
            tempNode2.x = endX;
            tempNode2.y = endY;

            GcostMap.Add(current, 0);
            FcostMap.Add(current, GcostMap[current] + Gcost(startX, startY, tempNode2));
            

            while (openNum != 0)
            {
                
                int minspot = 0;
                min = 20;

                if (openNum > 1)
                    for (int a = 0; a < openNum; a++)
                    {
                       
                        if (GcostMap[openList[a]] + Gcost(openList[a].x, openList[a].y, tempNode2) <= min)
                        {

                            min = GcostMap[openList[a]] + Gcost(openList[a].x, openList[a].y, tempNode2);
                            minspot = a;
                        }
                    }
                else
                    minspot = 0;

                current = openList[minspot];
                

                if (current.x == endX && current.y == endY)
                {
                    path = returnpath();
                    return true;

                }

                openNum--;

                closedList[closeNum] = current;
                WhichList[current.x, current.y] = 2;
                closeNum += 1;

                for (int x = 0; x < numNeighbours(current); x++)
                {
                    
                    int tentativeGscore = Gcost(startX, startY, current) + Gcost(current.x, current.y, Neighbours[x]);
                    if (WhichList[Neighbours[x].x, Neighbours[x].y] == 2)
                    {
                        
                        if (tentativeGscore >= GcostMap[current] + Gcost(current.x, current.y, Neighbours[x]))
                        {
                            
                            continue;
                        }
                    }
                    if (WhichList[Neighbours[x].x, Neighbours[x].y] != 1)
                    {
                        cameFrom.Add(Neighbours[x], current);
                        GcostMap.Add(Neighbours[x], tentativeGscore);
                        FcostMap.Add(Neighbours[x], GcostMap[Neighbours[x]] + Gcost(Neighbours[x].x, Neighbours[x].y, tempNode2));
                        if (WhichList[Neighbours[x].x, Neighbours[x].y] != 1)
                        {
                            openList[openNum] = Neighbours[x];
                            WhichList[Neighbours[x].x, Neighbours[x].y] = 1;
                            openNum++;
                        }
                    }
                }
               



            }
            
            
            Console.WriteLine("DID NOT FIND PATH");
            return false;
        }




        public void reconstruct_path(node nodepass)
        {

            if (cameFrom.ContainsKey(nodepass))
            {
                
                path[pathlength] = nodepass;
                pathlength++;
                reconstruct_path(cameFrom[nodepass]);
                
            }
            
        }




    }
}

