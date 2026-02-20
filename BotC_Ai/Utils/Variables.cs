/// <summary>
///     Stores variables.
/// </summary>
public class Variables
{
    public static bool isPlayer;
    public static bool reconnectPossible;
    public static bool disconnected;

    public static string IP;
    public static string port;

    public static string boardConfigJSON = "";
    public static string gameConfig = "";
    public static string reconnectToken = "";

    public static float inGameDelayMillisec = 0f;
}