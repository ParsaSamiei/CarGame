using UnityEngine;

public class Coin : Interactable
{
    [SerializeField] int points = 50;
    [SerializeField] float spinSpeed = 180f;
    [SerializeField] GameObject pickupVfx;       // pooled particle prefab (with AutoRelease)

    protected override void Update()
    {
        base.Update();
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.Self);
    }

    public override void OnPlayerHit(PlayerController player)
    {
        GameManager.Instance.AddScore(points);
        AudioManager.Instance.PlayCoin();
        if (pickupVfx != null)
            PoolManager.Instance.Get(pickupVfx, transform.position, Quaternion.identity);
        Despawn();
    }
}
