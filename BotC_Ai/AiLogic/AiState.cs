using System.Collections.Generic;
using Newtonsoft.Json.Linq;

//stores information about the players in the current game
public class AiState
{
    public static string Name;
    public static int[] CurrentPosition = new[] { 0, 0 };
    public static directionEnum Direction { get; set; }
    public static characterEnum Character { get; internal set; }
    public static int Lives { get; set; }
    public static int LembasCount { get; set; }
    public static int Suspended { get; set; }
    public static int ReachedCheckpoints { get; set; }
    public static List<cardEnum> PlayedCards = new List<cardEnum>();
    public static List<cardEnum> ReceivedCards = new List<cardEnum>();
    public static int TurnOrder { get; internal set; }

    public static void UpdateAiState(JObject newAiState)
    {
        try
        {
            CurrentPosition = newAiState.GetValue("currentPosition").ToObject<int[]>();
            Direction = Enum.Parse<directionEnum>(newAiState.GetValue("direction").ToObject<string>());
            Character = Enum.Parse<characterEnum>(newAiState.GetValue("character").ToObject<string>());
            Lives = newAiState.GetValue("lives").ToObject<int>();
            LembasCount = newAiState.GetValue("lembasCount").ToObject<int>();
            Suspended = newAiState.GetValue("suspended").ToObject<int>();
            ReachedCheckpoints = newAiState.GetValue("reachedCheckpoints").ToObject<int>();
        }
        catch (Exception e)
        {
            Console.WriteLine("AiState: " + e);
        }
    }
}