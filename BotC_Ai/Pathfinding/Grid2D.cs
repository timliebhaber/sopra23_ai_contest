using System;
using System.Collections.Generic;


    public class Grid2D
    {
        public Node2D[,] Grid;

        private int _gridSizeX;
        private int _gridSizeY;


        public int[] StartTile { get; }

        public int[] NextPoint { get; }

        public Grid2D(Tile[,] tiles, int[] startTile, int[] nextPoint)
        {
            StartTile = startTile;
            NextPoint = nextPoint;
            _gridSizeX = tiles.GetLength(0);
            _gridSizeY = tiles.GetLength(1);
           
            Grid = CreateGrid(tiles, _gridSizeX, _gridSizeY, nextPoint);
        }


        private Node2D[,] CreateGrid(Tile[,] tiles, int gridSizeX, int gridSizeY, int[] endpoint)
        {
            var grid = new Node2D[gridSizeX, gridSizeY];

            for (var x = 0; x < gridSizeX; x++)
            {
                for (var y = 0; y < gridSizeY; y++)
                {
                    var tile = tiles[x, y];
                    var obstacle = tile.TileType == tileEnum.HOLE || tile.TileType == tileEnum.EYE || tile.TileType == tileEnum.EAGLE;
                    grid[x, y] = new Node2D(obstacle, x, y, endpoint);
                }
            }

            return grid;
        }
        
        public List<Node2D> GetDiagonalNeighbors(Node2D node)
        {
            var neighbors = new List<Node2D>();
            
            
            for (var x =-1; x <= 1; x++)
            {
                for (var y =-1; y <= 1; y++)
                {
                    var tileX = node.GridX+x;
                    var tileY = node.GridY+y;
                    
                    if ((x == 0 && y == 0)|| tileX < 0 || tileY < 0 || tileX >= _gridSizeX || tileY >= _gridSizeY)
                    {
                        continue;
                    }

                    var node2D = Grid[tileX, tileY];
                    if(!node2D.obstacle) neighbors.Add(node2D);
                }
            }
            return neighbors;
         
        }
        
        
        public List<Node2D> GetNeighbors(Node2D node)
        {
            var neighbors = new List<Node2D>();
            
            
            for (var x =-1; x <= 1; x++)
            {
                for (var y =-1; y <= 1; y++)
                {
                    var tileX = node.GridX+x;
                    var tileY = node.GridY+y;
                    
                    if ((x == 0 && y == 0)|| (x == -1 && y == -1) || (x == -1 && y == 1) || (x == 1 && y == -1) || (x == 1 && y == 1) || tileX < 0 || tileY < 0 || tileX >= _gridSizeX || tileY >= _gridSizeY)
                    {
                        continue;
                    }

                    var node2D = Grid[tileX, tileY];
                    if(!node.obstacle) neighbors.Add(node2D);
                }
            }
            return neighbors;
         
        }  
    }