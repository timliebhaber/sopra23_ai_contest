using System.Diagnostics;

namespace KI.UnitTests;
using Xunit.Abstractions;
using Xunit;

[Collection("Sequential")]

public class PathfindingTest
{
    private readonly ITestOutputHelper output;

    public PathfindingTest(ITestOutputHelper output)
    {
        this.output = output;
    }
    [Fact]
    public void Test1()
    {
        // Arrange
        Gameboard gameboard = new Gameboard(File.ReadAllText("../../../BotC_Ai/Utils/boardconfig.json"));
        
        AiController._gameboard = gameboard;
        AiState.CurrentPosition = new[] { 2, 0 };
        AiState.Direction = directionEnum.EAST;
        AiController.DrawBoard(gameboard);
        // Act
        
        // Test
        Assert.Equivalent(true, true);
        
        // CleanUp
        CleanData.CleanUpData();
    }
}