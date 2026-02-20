/// <summary>
///     Enumeration for the different stages of the game (roughly correlates to the scenes).
/// </summary>
public enum gameStateEnum
{
    Unconnected,
    Reconnecting,
    Joining,
    InLobby,
    SelectingCharacter,
    InGame,
    Paused
}