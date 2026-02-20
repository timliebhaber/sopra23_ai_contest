using KI.KiLogic;

namespace KI.UnitTests;
using Xunit;

[Collection("Sequential")]
public class AiControllerTests
{
    public AiControllerTests(){}

    [Fact]
    public void Test1()
    {
        // Small test where there is only one good move
        
        // Arrange
        Gameboard gameboard = new Gameboard(File.ReadAllText("../../../BotC_Ai/Utils/boardconfig.json"));
        
        AiController._gameboard = gameboard;
        AiState.CurrentPosition = new[] { 2, 0 };
        AiState.Direction = directionEnum.EAST;
        AiState.ReceivedCards = new List<cardEnum>()
        {
            cardEnum.MOVE_3, cardEnum.MOVE_2, cardEnum.MOVE_3, cardEnum.AGAIN, cardEnum.MOVE_1, cardEnum.LEMBAS,
            cardEnum.LEMBAS, cardEnum.LEMBAS, cardEnum.MOVE_3
        };

        var path = new List<Node2D>()
        {
            new Node2D(false, 0, 0, new[] { 0, 0 }),
            new Node2D(false, 0, 1, new[] { 0, 0 }),
            new Node2D(false, 0, 2, new[] { 0, 0 }),
            new Node2D(false, 0, 3, new[] { 0, 0 }),
        };

        var expectedCards = new List<cardEnum>(){ cardEnum.AGAIN, cardEnum.MOVE_1, cardEnum.LEMBAS, cardEnum.LEMBAS, cardEnum.LEMBAS };
        
        // Act
        var actualCards = AiController.SelectCards(path);
        
        // Test
        Assert.Equal(expectedCards, actualCards);
        Assert.Equal(3, DummyState.shortestDistance);
        
        // CleanUp
        CleanData.CleanUpData();
    }
    
    [Fact]
    public void Test2()
    {
        // Test if Ai wants to walk through a wall
        
        // Arrange
        Gameboard gameboard = new Gameboard(File.ReadAllText("../../../BotC_Ai/Utils/boardconfigSelectCardsTest2.json"));
        
        AiController._gameboard = gameboard;;
        AiState.CurrentPosition = new[] { 2, 1 };
        AiState.Direction = directionEnum.SOUTH;
        AiState.ReceivedCards = new List<cardEnum>()
        {
            cardEnum.LEFT_TURN, cardEnum.MOVE_1, cardEnum.U_TURN, 
            cardEnum.U_TURN, cardEnum.MOVE_1, cardEnum.U_TURN
        };

        var path = new List<Node2D>()
        {
            new Node2D(false, 1, 0, new[] { 0, 0 }),
            new Node2D(false, 1, 1, new[] { 0, 0 }),
            new Node2D(false, 1, 2, new[] { 0, 0 }),
            new Node2D(false, 2, 2, new[] { 0, 0 }),
            new Node2D(false, 2, 3, new[] { 0, 0 }),
        };

        var expectedCards = new List<cardEnum>(){ cardEnum.U_TURN, cardEnum.MOVE_1, cardEnum.LEFT_TURN, cardEnum.MOVE_1, cardEnum.U_TURN};
        
        // Act
        var actualCards = AiController.SelectCards(path);
        
        // Test
        Assert.Equal(expectedCards, actualCards);
        Assert.Equal(0, DummyState.shortestDistance);
        
        // CleanUp
        CleanData.CleanUpData();
    }
    
    [Fact]
    public void Test3()
    {
        // Test if Ai runs in hole with MOVE_2 card
        
        // Arrange
        Gameboard gameboard = new Gameboard(File.ReadAllText("../../../BotC_Ai/Utils/boardconfig.json"));
        
        AiController._gameboard = gameboard;;
        AiState.CurrentPosition = new[] { 3, 3 };
        AiState.Direction = directionEnum.SOUTH;
        AiState.ReceivedCards = new List<cardEnum>()
        {
            cardEnum.MOVE_2, cardEnum.MOVE_1, cardEnum.LEFT_TURN,
            cardEnum.LEMBAS, cardEnum.LEMBAS, cardEnum.LEMBAS
        };

        var path = new List<Node2D>()
        {
            new Node2D(false, 3, 5, new[] { 0, 0 }),
        };

        var expectedCards = new List<cardEnum>(){ cardEnum.LEFT_TURN, cardEnum.MOVE_1, cardEnum.LEMBAS, cardEnum.LEMBAS, cardEnum.LEMBAS };
        
        // Act
        var actualCards = AiController.SelectCards(path);
        
        // Test
        Assert.Equal(expectedCards, actualCards);
        Assert.Equal(3, DummyState.shortestDistance);
        
        // CleanUp
        CleanData.CleanUpData();
    }
    
    [Fact]
    public void Test4()
    {
        // Test if Ai runs in hole with AGAIN and MOVE_3 card
        
        // Arrange
        Gameboard gameboard = new Gameboard(File.ReadAllText("../../../BotC_Ai/Utils/boardconfig.json"));
        
        AiController._gameboard = gameboard;;
        AiState.CurrentPosition = new[] { 3, 3 };
        AiState.Direction = directionEnum.SOUTH;
        AiState.ReceivedCards = new List<cardEnum>()
        {
            cardEnum.MOVE_3, cardEnum.LEMBAS, cardEnum.LEFT_TURN,
            cardEnum.LEMBAS, cardEnum.AGAIN, cardEnum.LEMBAS
        };

        var path = new List<Node2D>()
        {
            new Node2D(false, 3, 9, new[] { 0, 0 }),
        };

        var expectedCards = new List<cardEnum>(){ cardEnum.AGAIN, cardEnum.LEFT_TURN, cardEnum.LEMBAS, cardEnum.LEMBAS, cardEnum.LEMBAS };
        var expectedCards2 = new List<cardEnum>(){ cardEnum.MOVE_3, cardEnum.AGAIN, cardEnum.LEFT_TURN, cardEnum.LEMBAS, cardEnum.LEMBAS };
        
        // Act
        var actualCards = AiController.SelectCards(path);
        var shortestDistance1 = DummyState.shortestDistance;
        gameboard.Tiles[3, 4].SetGrass();
        AiController._gameboard = gameboard;;
        var actualCards2 = AiController.SelectCards(path);
        
        // Test
        Assert.Equivalent(expectedCards, actualCards, true);
        Assert.Equal(6, shortestDistance1);
        Assert.Equal(expectedCards2, actualCards2);
        Assert.Equal(0, DummyState.shortestDistance);
        
        // CleanUp
        CleanData.CleanUpData();
    }
    
    [Fact]
    public void Test5()
    {
        // Arrange
        Gameboard gameboard = new Gameboard(File.ReadAllText("../../../BotC_Ai/Utils/boardconfig.json"));
        
        AiController._gameboard = gameboard;;
        AiController.checkPointsQueue.Enqueue(new []{3,1});
        AiState.CurrentPosition = new[] { 1, 1 };
        AiState.Direction = directionEnum.NORTH;
        AiState.ReceivedCards = new List<cardEnum>()
        {
            cardEnum.MOVE_3, cardEnum.MOVE_2, cardEnum.LEFT_TURN,
            cardEnum.LEMBAS, cardEnum.AGAIN, cardEnum.LEMBAS, cardEnum.RIGHT_TURN, cardEnum.MOVE_1, cardEnum.MOVE_1
        };
        

        var expectedCards = new List<cardEnum>(){ cardEnum.LEFT_TURN, cardEnum.LEMBAS, cardEnum.RIGHT_TURN, cardEnum.AGAIN, cardEnum.MOVE_2};
        
        // Act
        var actualCards = AiController.SelectCards2();
        
        // Test
        Assert.Equal(expectedCards, actualCards);
        
        // CleanUp
        CleanData.CleanUpData();
    }
    
    [Fact]
    public void Test6()
    {
        // Arrange
        Gameboard gameboard = new Gameboard(File.ReadAllText("../../../BotC_Ai/Utils/boardconfig.json"));
        
        AiController._gameboard = gameboard;;
        AiController.checkPointsQueue.Enqueue(new []{1,5});
        AiState.CurrentPosition = new[] { 2, 1 };
        AiState.Direction = directionEnum.NORTH;
        AiState.ReceivedCards = new List<cardEnum>()
        {
            cardEnum.LEFT_TURN, cardEnum.MOVE_2, cardEnum.LEFT_TURN,
            cardEnum.LEMBAS, cardEnum.AGAIN, cardEnum.LEMBAS, cardEnum.RIGHT_TURN, cardEnum.MOVE_1, cardEnum.MOVE_1
        };
        

        var expectedCards = new List<cardEnum>(){ cardEnum.LEFT_TURN, cardEnum.MOVE_1, cardEnum.LEFT_TURN, cardEnum.MOVE_2, cardEnum.AGAIN};
        
        // Act
        var actualCards = AiController.SelectCards2();
        
        // Test
        Assert.Equal(expectedCards, actualCards);
        
        // CleanUp
        CleanData.CleanUpData();
    }
    
    [Fact]
    public void Test7()
    {
        // Test cards in SimulateCard individually
        
        // Arrange
        Gameboard gameboard = new Gameboard(File.ReadAllText("../../../BotC_Ai/Utils/boardconfig.json"));
        
        AiController._gameboard = gameboard;;
        
        AiState.CurrentPosition = new[] { 3, 6 };
        DummyState.CurrentPosition = new[] { 3, 6 };
        AiState.Direction = directionEnum.WEST;
        DummyState.Direction = directionEnum.WEST;
        
        
        // Act
        
        // Test
        
        // MOVE_1
        Assert.True(AiController.SimulateCard(cardEnum.MOVE_1));
        CleanData.CleanUpData();
        DummyState.CurrentPosition = new[] { 0, 6 };
        Assert.False(AiController.SimulateCard(cardEnum.MOVE_1));
        CleanData.CleanUpData();
        
        // MOVE_2
        Assert.True(AiController.SimulateCard(cardEnum.MOVE_2));
        CleanData.CleanUpData();
        DummyState.CurrentPosition = new[] { 0, 6 };
        Assert.False(AiController.SimulateCard(cardEnum.MOVE_2));
        CleanData.CleanUpData();
        DummyState.CurrentPosition = new[] { 1, 6 };
        Assert.False(AiController.SimulateCard(cardEnum.MOVE_2));
        CleanData.CleanUpData();
        
        // MOVE_3
        Assert.True(AiController.SimulateCard(cardEnum.MOVE_3));
        CleanData.CleanUpData();
        DummyState.CurrentPosition = new[] { 0, 6 };
        Assert.False(AiController.SimulateCard(cardEnum.MOVE_3));
        CleanData.CleanUpData();
        DummyState.CurrentPosition = new[] { 1, 6 };
        Assert.False(AiController.SimulateCard(cardEnum.MOVE_3));
        CleanData.CleanUpData();
        DummyState.CurrentPosition = new[] { 2, 6 };
        Assert.False(AiController.SimulateCard(cardEnum.MOVE_3));
        CleanData.CleanUpData();
        DummyState.Direction = directionEnum.NORTH;
        Assert.False(AiController.SimulateCard(cardEnum.MOVE_3));
        
        // MOVE_BACK
        Assert.True(AiController.SimulateCard(cardEnum.MOVE_BACK));
        CleanData.CleanUpData();
        DummyState.CurrentPosition = new[] { 0, 6 };
        DummyState.Direction = directionEnum.EAST;
        Assert.False(AiController.SimulateCard(cardEnum.MOVE_BACK));
        CleanData.CleanUpData();
        
        // U_TURN
        Assert.True(AiController.SimulateCard(cardEnum.U_TURN));
        CleanData.CleanUpData();
        
        // LEFT_TURN
        Assert.True(AiController.SimulateCard(cardEnum.LEFT_TURN));
        CleanData.CleanUpData();
        
        // RIGHT_TURN
        Assert.True(AiController.SimulateCard(cardEnum.RIGHT_TURN));
        CleanData.CleanUpData();
        
        // AGAIN
        Assert.True(AiController.SimulateCard(cardEnum.AGAIN));
        CleanData.CleanUpData();
        DummyState.CurrentPosition = new[] { 2, 6 };
        DummyState.PlayedCards.Add(cardEnum.MOVE_3);
        Assert.False(AiController.SimulateCard(cardEnum.AGAIN));
        DummyState.CurrentPosition = new[] { 2, 6 };
        DummyState.PlayedCards.Add(cardEnum.MOVE_2);
        Assert.True(AiController.SimulateCard(cardEnum.AGAIN));
        DummyState.PlayedCards.Add(cardEnum.AGAIN);
        Assert.True(AiController.SimulateCard(cardEnum.AGAIN));
        
        
        // CleanUp
        CleanData.CleanUpData();
    }

    [Fact]
    public void Test8()
    {
        // Test UpdateCheckpoints() Method
        
        // Arrange
        AiController.checkPointsQueue.Enqueue(new []{1,5});
        AiController.checkPointsQueue.Enqueue(new []{1,5});

        var expected = new Queue<int[]>();
        expected.Enqueue(new []{1,5});
        
        // Act
        AiController.UpdateCheckpoints();
        
        // Test
        Assert.Equivalent(expected , AiController.checkPointsQueue, true);
        
        // CleanUp
        CleanData.CleanUpData();
    }
    
    [Fact]
    public void Test9()
    {
        // Test FindCheckpoints() Method
        
        // Arrange
        Variables.boardConfigJSON = File.ReadAllText("../../../BotC_Ai/Utils/boardconfigSelectCardsTest2.json");

        var expected = new Queue<int[]>();
        expected.Enqueue(new []{0,0});
        expected.Enqueue(new []{9,9});
        
        // Act
        AiController.FindCheckpoints();
        
        // Test
        Assert.Equivalent(expected, AiController.checkPointsQueue, true);
        
        // CleanUp
        CleanData.CleanUpData();
    }
}