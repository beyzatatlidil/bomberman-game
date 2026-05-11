using UnityEngine;

public class InMemoryPowerUpRepository : IPowerUpRepository
{
    public PowerUpType GetRandomPowerUp()
    {
        return (PowerUpType)Random.Range(0, 3);
    }
}
