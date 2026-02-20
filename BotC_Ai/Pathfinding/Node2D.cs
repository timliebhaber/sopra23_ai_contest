using System;

public class Node2D
{
    public int gCost, hCost;
    public bool obstacle;

    public int GridX, GridY;
    public Node2D parent;
    private int[] EndTile;
    
    public int distance;


    public Node2D(bool _obstacle, int _gridX, int _gridY, int[] endtile)
    {
        obstacle = _obstacle;
        GridX = _gridX;
        GridY = _gridY;
        EndTile = endtile;
        distance = getDistance(new int[]{GridX, GridY});
    }
    public int getDistance(int[] x)
    {
        var D = 1;
        int dx = Math.Abs(this.EndTile[0] - x[0]);
        int dy = Math.Abs(this.EndTile[1] - x[1]);
        return D * (dx + dy);
    }
    public int FCost => gCost + hCost;


    public void SetObstacle(bool isOb)
    {
        obstacle = isOb;
    }
}
