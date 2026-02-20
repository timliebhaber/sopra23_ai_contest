using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
//using Codice.CM.Common;
//using Mono.Cecil;
using utils;

public class Pathfinding2D
{
    
    public Pathfinding2D(Grid2D grid2D)
    {
        this.Grid = grid2D;
    }

    private Grid2D Grid { get; }

    public List<Node2D> FindPath()
    {
        Priority_Queue<Node2D> queue = new Priority_Queue<Node2D>();
        var start = Grid.Grid[Grid.StartTile[0], Grid.StartTile[1]];
        var end = Grid.Grid[Grid.NextPoint[0], Grid.NextPoint[1]];
        Node2D current;
        HashSet<Node2D> set = new HashSet<Node2D>();
        queue.enqueue(0, start);
        bool pathFound = false;
        while (!queue.isEmpty())
        {
            current = queue.dequeue();
            if (current.Equals(end))
            {
                pathFound = true;
                break;
            }
            
            var neighbours = this.ExpandNode(current);
            set.Add(current);
            foreach (var successor in neighbours) 
            {
                if (set.Contains(successor)) continue;
                //Costs to travel one Node = 1
                var cost = current.gCost+1;
                
                if(queue.contains(successor) && cost >= successor.gCost) continue;

                successor.parent = current;
                successor.gCost = cost;

                var newCost = cost + successor.hCost;
                if (queue.contains(successor))
                {
                    queue.updatePriority(successor, newCost);
                }
                else
                {
                    queue.enqueue(newCost, successor);
                }
            }
        }
        
        List<Node2D> pfad = new List<Node2D>();
        if (pathFound)
        {
            var newCurrent = end;
            pfad.Add(end);
            while (!newCurrent.Equals(start))
            {
                newCurrent = newCurrent.parent;
                pfad.Add(newCurrent);
            }
            
        }
        foreach (var node2D in pfad)
        {
            
            //Console.WriteLine("PATH X: "+ node2D.GridX +" Y: " + node2D.GridY);
        }

        if (pfad.Count < 1)
        {
            // TODO: Handle null Exception
            try
            {
                pfad.RemoveAt(pfad.Count - 1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return pfad;
    }

    public List<Node2D> ExpandNode(Node2D node)
    {
        bool useDiagonal = false;
        List<Node2D> neighbours;
        if (useDiagonal)
        {
            neighbours = Grid.GetDiagonalNeighbors(node);
        }
        else
        {
            neighbours = Grid.GetNeighbors(node);
        }
        
        return neighbours;
    }

    public bool avoidObstacle(List<Node2D> path)
    {
        Node2D current = path[0];
        int pathLength = path.Count;
        
        for (var x = 0; x < pathLength; x++)
        {
            current = path[x];
            if (current.obstacle) return false;
        }

        return true;
    }
    
    public bool pathInBounds(List<Node2D> path, int[] endTile)
    {
        Node2D current = path[0];
        int pathLength = path.Count;
        
        for (var x = 0; x < pathLength; x++)
        {
            current = path[x];
            if (current.GridX < 0 || current.GridY < 0 || current.GridX > endTile[0] || current.GridY > endTile[1])
            {
                return false;
            }
        }

        return true;
    }
    
    public bool isTower(List<Node2D> path)
    {
        Node2D current = path[0];
        int pathLength = path.Count;
        for (var x = 0; x == pathLength; x++)
        {
            current = path[x];
            //TODO: Check for Walls & Holes           
            //if (board.IsTowerSpotOccupied(current.GridX,current.GridY)) return false;
        }

        return true;
    }
   
    public bool reachesEnd(List<Node2D> path, int[] end)
    {
        Node2D current = path[0];
        int pathLength = path.Count;
        for (var x = 0; x <= pathLength; x++)
        {
            current = path[x];
            if (current.GridX == end[0] && current.GridY == end[1]) return true;
        }

        return false;
    }
    

    public void SetObstacle(bool isObstacle, int x, int y)
    {
        Grid.Grid[x,y].SetObstacle(isObstacle);
    }
    
    //public bool InBounds(Grid x y) {
    //    return (x <= id.x) && (id.x < width) && (y <= id.y) && (id.y < height);
    //}
}