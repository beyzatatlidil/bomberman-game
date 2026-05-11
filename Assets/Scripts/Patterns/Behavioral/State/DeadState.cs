using UnityEngine;

public class DeadState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.canMove = false;
        Debug.Log("Player died");
    }

    public void Update(PlayerController player)
    {
        // Dead state: no behavior
    }
}
