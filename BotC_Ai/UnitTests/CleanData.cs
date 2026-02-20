using KI.KiLogic;

namespace KI.UnitTests;

public class CleanData
{
    public static void CleanUpData()
    {
        AiState.PlayedCards.Clear();
        AiState.ReceivedCards.Clear();
        
        AiController.checkPointsQueue.Clear();
        
        DummyState.CloneAi();
    }
}