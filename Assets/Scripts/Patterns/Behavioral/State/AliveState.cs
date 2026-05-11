using UnityEngine;

public class AliveState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.canMove = true;
    }

    public void Update(PlayerController player)
    {
        player.HandleMovement();
    }
}
