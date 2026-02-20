namespace KI.KiLogic;

public class DummyState
{
    public static int[] CurrentPosition = new[] { 0, 0 };
    public static directionEnum Direction { get; set; }
    public static List<cardEnum> PlayedCards = new List<cardEnum>();
    public static List<cardEnum> ReceivedCards = new List<cardEnum>();

    // For debugging
    public static int shortestDistance;
    public static int[] BestPosition = new[] { 0, 0 };

    public static void CloneAi()
    {
        CurrentPosition = new[] { AiState.CurrentPosition[0], AiState.CurrentPosition[1] };
        Direction = AiState.Direction;
        PlayedCards = new List<cardEnum>();
        ReceivedCards = AiState.ReceivedCards;
    }
}