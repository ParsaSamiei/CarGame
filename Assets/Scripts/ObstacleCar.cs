using UnityEngine;

/// <summary>Static (parked / slow) obstacle car. Costs one life on contact.</summary>
public class ObstacleCar : Interactable
{
    public override void OnPlayerHit(PlayerController player)
    {
        player.TakeHit(transform.position);
    }
}
