public class PlayerModel
{
    public string Id { get; private set; }

    public int Lives { get; set; } = 1;

    public float MoveSpeed { get; set; } = 5f;
    public int BombCount { get; set; } = 1;
    public int BombPower { get; set; } = 1;

    public PlayerModel(string id)
    {
        Id = id;
    }
}
