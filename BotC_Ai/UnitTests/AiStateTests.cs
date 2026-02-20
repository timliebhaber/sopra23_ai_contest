using Newtonsoft.Json.Linq;

namespace KI.UnitTests;
using Xunit;

public class AiStateTests
{
    public AiStateTests(){}

    [Fact]
    public void Test1()
    {
        // Test UpdateAiState() Method
        
        // Arrange
        JObject newAiState = JObject.Parse(File.ReadAllText("../../../BotC_Ai/Utils/testAiState.txt"));
        
        // Act
        AiState.UpdateAiState(newAiState);

        // Test
        Assert.Equal(new []{5,7}, AiState.CurrentPosition);
        Assert.Equal(directionEnum.SOUTH, AiState.Direction);
        Assert.Equal(characterEnum.MERRY, AiState.Character);
        Assert.Equal(3, AiState.Lives);
        Assert.Equal(2, AiState.LembasCount);
        Assert.Equal(0, AiState.Suspended);
        Assert.Equal(0, AiState.ReachedCheckpoints);
    }
}