public class BoardState
{
    public BoardState(int[,] lembasFields)
    {
        LembasFields = lembasFields;
    }

    public static int[,] LembasFields { get; set; }
}