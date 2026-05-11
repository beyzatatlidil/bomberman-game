using UnityEngine;

public static class PowerUpFactory
{
    public static GameObject Create(
        PowerUpType type,
        Vector3 position,
        GameObject[] prefabs)
    {
        int index = (int)type;

        if (index < 0 || index >= prefabs.Length)
            return null;

        return Object.Instantiate(prefabs[index], position, Quaternion.identity);
    }
}
