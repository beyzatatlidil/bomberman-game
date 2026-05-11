public class BombModel
{
    public string OwnerId { get; private set; }

    public float FuseSeconds { get; set; } = 2.0f;
    public int Power { get; set; } = 1;

    public BombModel(string ownerId, float fuseSeconds, int power)
    {
        OwnerId = ownerId;
        FuseSeconds = fuseSeconds;
        Power = power;
    }
}
