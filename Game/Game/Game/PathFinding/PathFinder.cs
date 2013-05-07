using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        int[] whereOpen;
        int[] whereClose;
        node[] openList;
        node[] closedList;
        int[] closeX;
        int[] closeY;
        int[] openX;
        int[] openY;
        node[] path;
        int[,] WhichList;
        public node current;
        node[] Neighbours;
        int pathlength;
        int camefromsize;


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
            camefromsize = 0;
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
            // whereOpen[0] = 0;
            current = tempNode;
            tempNode2.x = endX;
            tempNode2.y = endY;

            GcostMap.Add(current, 0);
            FcostMap.Add(current, GcostMap[current] + Gcost(startX, startY, tempNode2));
            // FcostMap[current] = GcostMap[current] + Gcost(startX, startY, tempNode2);
            //Console.WriteLine(Gcost(startX, startY, tempNode2));

            while (openNum != 0)
            {
                // Console.WriteLine("OPEN NUM =");
                //  Console.WriteLine(openNum);
                int minspot = 0;
                min = 20;

                if (openNum > 1)
                    for (int a = 0; a < openNum; a++)
                    {
                        //  Console.WriteLine(openList[a].x);
                        // Console.WriteLine(openList[a].y);
                        //  Console.WriteLine(openList[a].x);
                        // Console.WriteLine(openList[a].y);
                        // Console.WriteLine(GcostMap[openList[a]]);
                        // Console.WriteLine("========");
                        if (GcostMap[openList[a]] + Gcost(openList[a].x, openList[a].y, tempNode2) <= min)
                        {

                            min = GcostMap[openList[a]] + Gcost(openList[a].x, openList[a].y, tempNode2);
                            minspot = a;
                        }
                    }
                else
                    minspot = 0;

                current = openList[minspot];
                // Console.WriteLine()
                // Console.WriteLine(current.x);
                // Console.WriteLine(current.y);
                // Console.WriteLine("NEXt");

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
                    // Console.WriteLine(current.x);
                    //  Console.WriteLine(current.y);
                    //  Console.WriteLine(Neighbours[x].x);
                    //  Console.WriteLine(Neighbours[x].y);
                    // Console.WriteLine("////////////////");
                    int tentativeGscore = Gcost(startX, startY, current) + Gcost(current.x, current.y, Neighbours[x]);
                    if (WhichList[Neighbours[x].x, Neighbours[x].y] == 2)
                    {
                        //Console.Write("ON CLOSED LIST");
                        if (tentativeGscore >= GcostMap[current] + Gcost(current.x, current.y, Neighbours[x]))
                        {
                            // Console.Write("ON CLOSED LIST");
                            continue;
                        }
                    }
                    if (WhichList[Neighbours[x].x, Neighbours[x].y] != 1)
                    {
                        cameFrom.Add(Neighbours[x], current);
                        GcostMap.Add(Neighbours[x], tentativeGscore);
                        FcostMap.Add(Neighbours[x], GcostMap[Neighbours[x]] + Gcost(Neighbours[x].x, Neighbours[x].y, tempNode2));
                        //cameFrom[Neighbours[x]] = current;
                        //GcostMap[Neighbours[x]] = tentativeGscore;
                        //FcostMap[Neighbours[x]] = GcostMap[Neighbours[x]] + Gcost(Neighbours[x].x, Neighbours[x].y, tempNode2);
                        if (WhichList[Neighbours[x].x, Neighbours[x].y] != 1)
                        {
                            // Console.WriteLine(Neighbours[x].x);
                            // Console.WriteLine(Neighbours[x].y);
                            openList[openNum] = Neighbours[x];
                            WhichList[Neighbours[x].x, Neighbours[x].y] = 1;
                            openNum++;
                        }
                    }
                }
                //Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");



            }


            // Console.WriteLine(current.x);
            //  Console.WriteLine(current.y);
            Console.WriteLine("DID NOT FIND PATH");
            return false;
        }




        public void reconstruct_path(node nodepass)//camefrom, current)
        {

            if (cameFrom.ContainsKey(nodepass))
            {
                //Console.WriteLine(pathlength);
                path[pathlength] = nodepass;
                pathlength++;
                reconstruct_path(cameFrom[nodepass]);//came_from, came_from[current_node])
                //return (p + current);
            }
            else
            {
                //return current;
            }
        }




    }
}

