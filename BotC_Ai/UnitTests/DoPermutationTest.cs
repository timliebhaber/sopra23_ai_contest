using System.Diagnostics;

namespace KI.UnitTests;
using Xunit;

[Collection("Sequential")]
public class DoPermutationTest
{
    public DoPermutationTest(){}

    [Fact]
    public void Test1()
    {
        // Arrange
        List<List<int>> expectedPermutations = new List<List<int>>() {new List<int>(){1,2,3},new List<int>(){1,3,2},new List<int>(){2,1,3},new List<int>(){2,3,1},new List<int>(){3,2,1},new List<int>(){3,1,2} };
        // Act
        List<List<int>> actualPermutations = AiController.DoPermute(new int[] { 1, 2, 3 }, 0, 2, new List<List<int>>());
        
        // Test
        Assert.Equivalent(expectedPermutations, actualPermutations,true);
        
        // CleanUp
        CleanData.CleanUpData();
    }
}