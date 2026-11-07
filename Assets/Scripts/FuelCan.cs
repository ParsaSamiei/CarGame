using UnityEngine;

public class FuelCan : Interactable
{
    [SerializeField] float fuelAmount = 25f;
    [SerializeField] float bobHeight = 0.15f;
    [SerializeField] float bobSpeed = 4f;
    [SerializeField] GameObject pickupVfx;

    float baseY;

    public override void OnSpawn()
    {
        base.OnSpawn();
        baseY = transform.position.y;
    }

    protected override void Update()
    {
        base.Update();
        var p = transform.position;
        p.y = baseY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = p;
    }

    public override void OnPlayerHit(PlayerController player)
    {
        player.AddFuel(fuelAmount);
        AudioManager.Instance.PlayFuel();
        if (pickupVfx != null)
            PoolManager.Instance.Get(pickupVfx, transform.position, Quaternion.identity);
        Despawn();
    }
}
