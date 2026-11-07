using UnityEngine;

/// <summary>Abstract base for everything the player can touch: obstacles, coins, fuel cans.</summary>
public abstract class Interactable : MonoBehaviour
{
    [HideInInspector] public RoadSegment owner;

    protected bool IsPlaying =>
        GameManager.Instance != null && GameManager.Instance.State == GameState.Playing;

    /// <summary>Called by the road generator every time the object is taken from the pool.</summary>
    public virtual void OnSpawn() { }

    /// <summary>What happens when the player touches this object.</summary>
    public abstract void OnPlayerHit(PlayerController player);

    protected virtual void Update() { }

    /// <summary>Return to the pool.</summary>
    public virtual void Despawn()
    {
        if (owner != null) { owner.Unregister(this); owner = null; }
        PoolManager.Instance.Release(gameObject);
    }
}
