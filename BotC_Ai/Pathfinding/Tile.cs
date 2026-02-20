

//saves information about the tiles
public class Tile
{
    public Tile()
    {
        SetGrass();
    }

    public tileEnum TileType { get; private set; }

    public AiState? Player { get; set; }

    //only relevant if the tileType is START, EYE or RIVER
    public directionEnum Direction { get; private set; }

    //different purpose for different tile types:
    //lembas field: amount of lembas left
    //check point: check point number (starting with 1)
    //start field: number of the player who starts on that field
    //other field types should ignore this attribute.
    public int Number { get; set; }

    //methods to change the TileType
    public void SetGrass()
    {
        TileType = tileEnum.GRASS;
    }

    public void SetStart(directionEnum direction)
    {
        TileType = tileEnum.START;
        Direction = direction;
    }

    public void SetCheckPoint(int checkPointNumber)
    {
        TileType = tileEnum.CHECK_POINT;
        Number = checkPointNumber;
    }

    public void SetEye(directionEnum direction)
    {
        TileType = tileEnum.EYE;
        Direction = direction;
    }

    public void SetHole()
    {
        TileType = tileEnum.HOLE;
    }

    public void SetRiver(directionEnum direction)
    {
        TileType = tileEnum.RIVER;
        Direction = direction;
    }

    public void SetLembas(int amount)
    {
        TileType = tileEnum.LEMBAS;
        Number = amount;
    }

    public void TakeLembas()
    {
        Number--;
    }

    public void SetEagle()
    {
        TileType = tileEnum.EAGLE;
    }

    public void SetPlayer(AiState? player)
    {
        Player = player;
    }
}