namespace KI.UnitTests;
using Xunit;

[Collection("Sequential")]
public class GetDisToPathTest
{
    public GetDisToPathTest(){}

    [Fact]
    public void Test1()
    {
        // Arrange
        List<Node2D> path = new List<Node2D>()
            { new Node2D(false, 0, 0, new []{0,0}), new Node2D(false, 0, 1, new []{0,0}), new Node2D(false, 0, 2, new []{0,0}) };
        int[] position = new[] { 3, 1 };
        
        // Act
        var expected = 3;
        var actual = AiController.GetDistanceToPath(path, position);
        
        // Test
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void Test2()
    {
        // Arrange
        List<Node2D> path = new List<Node2D>()
            { new Node2D(false, 1, 0, new []{0,0}), new Node2D(false, 1, 1, new []{0,0}), new Node2D(false, 1, 2, new []{0,0}) };
        int[] position = new[] { 3, 1 };
        
        // Act
        var expected = 2;
        var actual = AiController.GetDistanceToPath(path, position);
        
        // Test
        Assert.Equal(expected, actual);
        
        // CleanUp
        CleanData.CleanUpData();
    }
}