using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class Gameboard
{
    public readonly int Height;
    public readonly int Width;
    public int[,] checkPoints;
    public int[] Eye;
    public directionEnum EyeDirection;

    public string Name;
    public int NumberOfCheckPoints;
    public List<int[]> spawnFields;
    public List<directionEnum> spawnFieldsDirections;
    private readonly int[,,] Walls;

    public Gameboard(string boardConfigurationString)
    {
        var boardConfiguration = JObject.Parse(boardConfigurationString);
        try
        {
            //reads name, height and width
            Name = boardConfiguration.GetValue("name").ToObject<string>();
            Height = boardConfiguration.GetValue("height").ToObject<int>();
            Width = boardConfiguration.GetValue("width").ToObject<int>();

            //generates an empty grass board
            Tiles = new Tile[Width, Height];
            for (var i = 0; i < Width; i++)
            for (var j = 0; j < Height; j++)
                Tiles[i, j] = new Tile();

            int[] position;
            string direction;

            //sets start fields
            spawnFields = new List<int[]>();
            spawnFieldsDirections = new List<directionEnum>();
            var startFields = (JArray)boardConfiguration.GetValue("startFields");
            foreach (JObject field in startFields)
            {
                position = field.GetValue("position").ToObject<int[]>();
                direction = field.GetValue("direction").ToObject<string>();
                Tiles[position[0], position[1]].SetStart(Enum.Parse<directionEnum>(direction));
                spawnFields.Add(new[] { position[0], position[1] });
                spawnFieldsDirections.Add(Enum.Parse<directionEnum>(direction));
            }

            //sets checkpoints
            checkPoints = boardConfiguration.GetValue("checkPoints").ToObject<int[,]>();
            for (var i = 0; i < checkPoints.GetLength(0); i++)
                Tiles[checkPoints[i, 0], checkPoints[i, 1]].SetCheckPoint(i + 1);

            NumberOfCheckPoints = checkPoints.GetLength(0);

            //sets the eye of sauron
            var eye = boardConfiguration.GetValue("eye").ToObject<JObject>();
            position = eye.GetValue("position").ToObject<int[]>();
            direction = eye.GetValue("direction").ToObject<string>();
            Tiles[position[0], position[1]].SetEye(Enum.Parse<directionEnum>(direction));

            //sets holes
            var holes = boardConfiguration.GetValue("holes").ToObject<int[,]>();
            for (var i = 0; i < holes.GetLength(0); i++) Tiles[holes[i, 0], holes[i, 1]].SetHole();

            //sets rivers
            var riverFields = (JArray)boardConfiguration.GetValue("riverFields");
            foreach (JObject field in riverFields)
            {
                position = field.GetValue("position").ToObject<int[]>();
                direction = field.GetValue("direction").ToObject<string>();
                Tiles[position[0], position[1]].SetRiver(Enum.Parse<directionEnum>(direction));
            }

            //sets the walls
            var wallPositions = (JArray)boardConfiguration.GetValue("walls");
            var walls = new int[wallPositions.Count, 2, 2];
            var k = 0;
            foreach (JArray wall in wallPositions)
            {
                var l = 0;
                foreach (JArray field in wall)
                {
                    walls[k, l, 0] = field.First.ToObject<int>();
                    walls[k, l, 1] = field.Last.ToObject<int>();
                    l++;
                }

                k++;
            }

            Walls = walls;

            //sets lembas fields
            BoardState.LembasFields = new int[Width, Height];
            var lembasFields = (JArray)boardConfiguration.GetValue("lembasFields");
            foreach (JObject field in lembasFields)
            {
                position = field.GetValue("position").ToObject<int[]>();
                var amount = field.GetValue("amount").ToObject<int>();
                Tiles[position[0], position[1]].SetLembas(amount);
                BoardState.LembasFields[position[0], position[1]] = amount;
            }


            generateWallArrays();
        }
        catch (NullReferenceException)
        {
        }
    }

    public Tile[,] Tiles { get; set; }

    //verticalWalls[x, y] = true <=> Tile[x, y] has a wall to its right
    public bool[,] VerticalWalls { get; set; }

    //horizontalWalls[x, y] = true <=> Tile[x, y] has a wall below it
    public bool[,] HorizontalWalls { get; set; }

//generates verticalWalls[,] and horizontalWalls[,] from Walls[, ,] 
    private void generateWallArrays()
    {
        VerticalWalls = new bool[Width - 1, Height];
        HorizontalWalls = new bool[Width, Height - 1];

        for (var i = 0; i < Walls.GetLength(0); i++)
            if (Walls[i, 0, 0] != Walls[i, 1, 0])
                VerticalWalls[Math.Min(Walls[i, 0, 0], Walls[i, 1, 0]), Walls[i, 0, 1]] = true;
            else
                HorizontalWalls[Walls[i, 0, 0], Math.Min(Walls[i, 0, 1], Walls[i, 1, 1])] = true;
    }
}