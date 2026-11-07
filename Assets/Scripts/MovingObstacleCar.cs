using UnityEngine;

/// <summary>Moving obstacle: drives forward on its own, slower than the player.</summary>
public class MovingObstacleCar : ObstacleCar
{
    [SerializeField] Vector2 ownSpeedRange = new Vector2(6f, 12f);   // keep below GameManager.startSpeed
    [SerializeField] float maxLead = 58f;   // a segment is 60 m; never overtake the next segment's rows
    float ownSpeed;

    public override void OnSpawn()
    {
        base.OnSpawn();
        ownSpeed = Random.Range(ownSpeedRange.x, ownSpeedRange.y);
    }

    protected override void Update()
    {
        base.Update();
        if (!IsPlaying) return;
        transform.position += Vector3.forward * (ownSpeed * Time.deltaTime);

        // The car belongs to one segment and is released when that segment is recycled. Driving
        // indefinitely would push it into the next segment's rows, where it could overlap another
        // obstacle and be despawned early by a segment that no longer owns it, so it holds station
        // at the far end of its own stretch of road instead.
        if (owner != null)
        {
            float anchorZ = owner.transform.position.z;
            if (transform.position.z - anchorZ > maxLead)
            {
                Vector3 p = transform.position;
                p.z = anchorZ + maxLead;
                transform.position = p;
            }
        }
    }
}
