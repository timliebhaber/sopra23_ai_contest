using Newtonsoft.Json.Linq;
using KI.KiLogic;

public class AiController
{
    public static Gameboard _gameboard;
    
    public static Queue<int[]> checkPointsQueue = new Queue<int[]>();
    public static void DrawBoard(Gameboard gameboard)
    {
        Console.WriteLine("Drawing Gameboard: \n");
   
        for (int i = 0; i < gameboard.Width; i++)
        {
            for (int j = 0; j < gameboard.Height; j++)
            {
                if (j == AiState.CurrentPosition[0] && i == AiState.CurrentPosition[1])
                {
                    Console.Write("\t [");
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("BOT");
                    Console.ResetColor();
                    Console.Write("] \t");
                    continue;
                }
                if (gameboard.Tiles[j, i].TileType == tileEnum.EYE)
                {
                    Console.Write("\t ["+ gameboard.Tiles[j,i].TileType + "] \t");
                    continue;
                }
                Console.Write("\t ["+ gameboard.Tiles[j,i].TileType + "] ");
            }
                Console.WriteLine("");
        }
        Console.WriteLine("\n");
    }
    
    public static void DrawBoard(Gameboard gameboard, List<Node2D> path)
    {
        Console.WriteLine("Drawing Path to next Checkpoint: \n");
        
        for (int i = 0; i < gameboard.Width; i++)
        {
            for (int j = 0; j < gameboard.Height; j++)
            {
                for (int k = 0; k < path.Count; k++)
                {
                    if (path[k].GridY == i && path[k].GridX == j)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        break;
                    }
                    else Console.ResetColor();
                }

                if (j == AiState.CurrentPosition[0] && i == AiState.CurrentPosition[1])
                {
                    Console.Write("\t [");
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("BOT");
                    Console.ResetColor();
                    Console.Write("] \t");
                    continue;
                }
                if (gameboard.Tiles[j, i].TileType == tileEnum.EYE)
                {
                    Console.Write("\t ["+ gameboard.Tiles[j,i].TileType + "] \t");
                    continue;
                }
                Console.Write("\t ["+ gameboard.Tiles[j,i].TileType + "] ");
            }
            Console.WriteLine("");
        }
        Console.WriteLine("\n");
    }

    public static void FindCheckpoints()
    {
        Console.WriteLine("Finding order of checkpoints: \n");
        int Count = 1;
        
        var boardConfiguration = JObject.Parse(Variables.boardConfigJSON);
        JArray JsonCheckpoints = (JArray)boardConfiguration.GetValue("checkPoints");
        foreach (var checkpoint in JsonCheckpoints)
        {
            checkPointsQueue.Enqueue(new int[]{(int)checkpoint[0],(int)checkpoint[1]});
        }
        
        foreach (int[] checkpointArray in checkPointsQueue)
        {
            Console.WriteLine("Checkpoints: "+ string.Join(",", checkpointArray));
        }
        
        Console.WriteLine("");
    }
    public static List<Node2D> PathToCheckpoint(Tile [,] tiles)
    {
        try
        {
            Grid2D grid = new Grid2D(tiles, AiState.CurrentPosition, checkPointsQueue.Peek());
            Pathfinding2D _pathfinding = new Pathfinding2D(grid);
            List<Node2D> path = new List<Node2D>(_pathfinding.FindPath());
            return path;
        }
        catch (IndexOutOfRangeException e)
        {
           Console.WriteLine(e);
           List<Node2D> emptyPath = new List<Node2D>();
           return emptyPath;
        }
    }
    
    public static List<cardEnum> SelectCards(List<Node2D> path)
    {
        /*
        Console.WriteLine("Ai got these cards: ");
        foreach (cardEnum card in AiState.ReceivedCards)
        {
            Console.WriteLine(card.ToString());
        }
        // Uncomment if receivedCards have to be filtered
        /*
        var receivedCards = AiState.ReceivedCards;
        var hand = FilterReceivedCards(receivedCards);
        if (hand.Count < 5)
        {
            receivedCards.RemoveRange(0, receivedCards.Count - 5);
            return receivedCards;
        }
        */
        var hand = AiState.ReceivedCards;
        var reachablePath = new Dictionary<Node2D,List<cardEnum>>();

        var bestPermutation = hand;
        var shortestDistance = 1000; 
        int longDis = 1000;
        Node2D bestField = path[0];
        
        var combinationsAmount = 0;
        var combinations = CardCombinations.CombinationsArray;
        
        // Regulate combinationsAmount by amount of cards in Hand
        switch (hand.Count)
        {
            case 6:
                combinationsAmount = 6;
                break;
            case 7:
                combinationsAmount = 21;
                break;
            case 8:
                combinationsAmount = 56;
                break;
            case 9:
                combinationsAmount = combinations.Count;
                break;
        }
        
        // Simulate every permutation of every combination of cards
        for(int i = 0; i < combinationsAmount; i++)
        {
            var permutations = DoPermute(combinations[i], 0, combinations[i].Length - 1, new List<List<int>>());
            var ignorePermutation = false;
            
            foreach (var permutation in permutations)
            {
                // Reset DummyState
                DummyState.CloneAi();
                
                foreach (var cardIndex in permutation)
                {
                    var card = hand[cardIndex];
                    if (!SimulateCard(card))
                    {
                        // If ai lands on hole than ignore permutation
                        ignorePermutation = true;
                        aiOnCheckpoint = false;
                        break;
                    }
                    
                    // When Ai walks over checkpoint, immediately return the card combination
                    if (aiOnCheckpoint)
                    {
                        Console.WriteLine("Select Cards: Found way on Checkpoint!");
                        aiOnCheckpoint = false;
                        
                        var cards = new List<cardEnum>();
                        permutation.ForEach(cardIndex => cards.Add(hand[cardIndex]));
                        return cards;
                    }
                    
                    DummyState.PlayedCards.Add(card);
                }

                if (ignorePermutation)
                {
                    ignorePermutation = false;
                    continue;
                }

                // Calculate min distance to path
                var newDistance = GetDistanceToPath(path, DummyState.CurrentPosition);
                if (shortestDistance > newDistance)
                {
                    var cards = new List<cardEnum>();
                    shortestDistance = newDistance;
                    permutation.ForEach(cardIndex => cards.Add(hand[cardIndex]));
                    bestPermutation = cards;
                }

                if (shortestDistance == 0)
                {
                    for (int j = 0; j < path.Count; j++)
                    {
                        if (path[j].GridX == DummyState.CurrentPosition[0] && path[j].GridY == DummyState.CurrentPosition[1])
                        {
                            if (!reachablePath.ContainsKey(path[j]))
                            {
                                reachablePath.Add(path[j], bestPermutation);
                            }
                        }
                    }
                    DummyState.shortestDistance = 0;
                    //return bestPermutation;
                }
            }
        }
        foreach (var pair in reachablePath)
        {
            if (pair.Key.distance < longDis)
            {
                bestField = pair.Key; 
                Console.WriteLine("NEW BEST FIELD = ["+ pair.Key.GridX+","+pair.Key.GridY+"]");
                longDis = pair.Key.distance;
            }
            bestPermutation = reachablePath[bestField];
        }
        // For debugging
        DummyState.shortestDistance = shortestDistance;
        return bestPermutation;
    }
    public static List<cardEnum> SelectCards2()
    {
        var hand = AiState.ReceivedCards;

        // set default to first 5 cards
        var bestPermutation = new List<cardEnum>();
        for (int i = 0; i < 5; i++)
        {
            bestPermutation.Add(hand[i]);
        }
        
        var shortestDistance = 1000; 
        
        var combinationsAmount = 0;
        var combinations = CardCombinations.CombinationsArray;
        
        // Regulate combinationsAmount by amount of cards in Hand
        switch (hand.Count)
        {
            case 6:
                combinationsAmount = 6;
                break;
            case 7:
                combinationsAmount = 21;
                break;
            case 8:
                combinationsAmount = 56;
                break;
            case 9:
                combinationsAmount = combinations.Count;
                break;
        }
        
        // Simulate every permutation of every combination of cards
        for(int i = 0; i < combinationsAmount; i++)
        {
            var permutations = DoPermute(combinations[i], 0, combinations[i].Length - 1, new List<List<int>>());
            var ignorePermutation = false;

            foreach (var permutation in permutations)
            {
                // Reset DummyState
                DummyState.CloneAi();

                foreach (var cardIndex in permutation)
                {
                    var card = hand[cardIndex];
                    if (!SimulateCard(card))
                    {
                        // If ai lands on hole than ignore permutation
                        ignorePermutation = true;
                        aiOnCheckpoint = false;
                        break;
                    }

                    DummyState.PlayedCards.Add(card);
                }

                if (ignorePermutation)
                {
                    ignorePermutation = false;
                    continue;
                }

                // Calculate min distance to Checkpoint
                var newDistance = Math.Abs(DummyState.CurrentPosition[0] - checkPointsQueue.Peek()[0]) + Math.Abs(DummyState.CurrentPosition[1] - checkPointsQueue.Peek()[1]);
                if (shortestDistance > newDistance)
                {
                    DummyState.BestPosition = DummyState.CurrentPosition;
                    shortestDistance = newDistance;
                    bestPermutation = new List<cardEnum>();
                    permutation.ForEach(cardIndex => bestPermutation.Add(hand[cardIndex]));
                }
            }
        }
        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------"+DummyState.BestPosition[0]+","+DummyState.BestPosition[1]);
        return bestPermutation;
    }

    /*
    public static List<cardEnum> FilterReceivedCards(List<cardEnum> receivedCards)
    {
        var filteredHand = new List<cardEnum>();
        var allowedCards = new List<cardEnum>()
        {
            cardEnum.MOVE_1, cardEnum.MOVE_2, cardEnum.MOVE_3, cardEnum.AGAIN, cardEnum.U_TURN, cardEnum.LEFT_TURN,
            cardEnum.RIGHT_TURN
        };
        receivedCards.ForEach(card => { if (allowedCards.Contains(card)) { filteredHand.Add(card); } });

        return filteredHand;
    }
    */

    public static int GetDistanceToPath(List<Node2D> path, int[] position)
    {
        List<int> distances = new List<int>();
        path.ForEach(node => {distances.Add(Math.Abs(node.GridX - position[0]) + Math.Abs(node.GridY - position[1]));});
        return distances.AsQueryable().Min();
    }

    private static void Swap(ref int a, ref int b)
    {
        var temp = a;
        a = b;
        b = temp;
    }
    
    public static List<List<int>> DoPermute(int[] nums, int start, int end, List<List<int>> list)
    {
        if (start == end)
        {
            // We have one of our possible n! solutions,
            // add it to the list.
            list.Add(new List<int>(nums));
        }
        else
        {
            for (var i = start; i <= end; i++)
            {
                Swap(ref nums[start], ref nums[i]);
                DoPermute(nums, start + 1, end, list);
                Swap(ref nums[start], ref nums[i]); // reset for next pass
            }
        }

        return list;
    }

    private static bool aiOnCheckpoint;
    public static bool SimulateCard(cardEnum card)
    {
        switch (card)
        {
            case cardEnum.MOVE_1:
                if (!Move(DummyState.Direction))
                {
                    return false;
                }
                break;

            case cardEnum.MOVE_2:
                if (!SimulateCard(cardEnum.MOVE_1))
                {
                    return false;
                }
                if (!Move(DummyState.Direction))
                {
                    return false;
                }
                break;

            case cardEnum.MOVE_3:
                if (!SimulateCard(cardEnum.MOVE_1))
                {
                    return false;
                }
                if (!SimulateCard(cardEnum.MOVE_1))
                {
                    return false;
                }
                if (!Move(DummyState.Direction))
                {
                    return false;
                }
                
                break;
            
            case cardEnum.MOVE_BACK:
                if (!MoveBack())
                {
                    return false;
                }
                break;

            case cardEnum.U_TURN:
                Turn(1);
                Turn(1);
                break;

            case cardEnum.LEFT_TURN:
                Turn(-1);
                break;

            case cardEnum.RIGHT_TURN:
                Turn(1);
                break;

            case cardEnum.AGAIN:
                if (DummyState.PlayedCards.Any())
                {
                    var lastCard = DummyState.PlayedCards.Last();
                    if (!lastCard.Equals(cardEnum.AGAIN))
                    {
                        return SimulateCard(lastCard);
                    }
                }
                break;
        }
        
        var field = _gameboard.Tiles[DummyState.CurrentPosition[0], DummyState.CurrentPosition[1]];
        switch (field.TileType)
        {
            // return false if ai would fall in hole
            case tileEnum.HOLE:
                return false;
        }

        return true;
    }
    
    private static void Turn(int rotation)
    {
        var newDirection = ((int)DummyState.Direction + rotation +4) % 4;
        DummyState.Direction = (directionEnum)newDirection;
    }

    private static bool MoveBack()
    {
        var switchDirection = ((int)DummyState.Direction + 2) % 4;
        return Move((directionEnum)switchDirection); 
    }
    private static bool Move(directionEnum direction)
    {
        var oldPosition = new int[] { DummyState.CurrentPosition[0], DummyState.CurrentPosition[1] };
        var newPosition = oldPosition;
        try
        {
            switch (direction)
            {
                case directionEnum.NORTH:
                    newPosition[1] = newPosition[1] - 1;
                    if (_gameboard.HorizontalWalls[newPosition[0],newPosition[1]]) return true;
                    break;
                case directionEnum.EAST:
                    newPosition[0] = newPosition[0] + 1;
                    if (_gameboard.VerticalWalls[newPosition[0] - 1,newPosition[1]]) return true;
                    break;
                case directionEnum.SOUTH:
                    newPosition[1] = newPosition[1] + 1;
                    if (_gameboard.HorizontalWalls[newPosition[0],newPosition[1] - 1]) return true;
                    break;
                case directionEnum.WEST:
                    newPosition[0] = newPosition[0] - 1;
                    if (_gameboard.VerticalWalls[newPosition[0],newPosition[1]]) return true;
                    break;
            }
        }
        catch (IndexOutOfRangeException e)
        {
            return false;
        }
        
        var field = _gameboard.Tiles[newPosition[0], newPosition[1]];

        if (field.TileType.Equals(tileEnum.EYE))
        { return true;}
        
        DummyState.CurrentPosition = newPosition;
        return true;
    }

    public static void UpdateCheckpoints()
    {
        Console.WriteLine("Checkpoint [" + checkPointsQueue.Peek()[0] + "," +
                          checkPointsQueue.Peek()[1] + "] removed from the queue.\n");
        checkPointsQueue.Dequeue();
        Console.WriteLine("The next Checkpoint is [" + checkPointsQueue.Peek()[0] + "," +
                          checkPointsQueue.Peek()[1] + "]");
    }
    
    public static void paused()
    {
        while (MessageReceiver.CurrentState == gameStateEnum.Paused)
        {
            Thread.Sleep(3000);
            Console.WriteLine("Game is paused...");
        }
    }
}

