using UnityEngine;

/// <summary>Launches pooled firework prefabs at random intervals/positions in the night sky.</summary>
public class FireworksManager : MonoBehaviour
{
    [SerializeField] GameObject[] fireworkPrefabs;
    [SerializeField] Vector2 delayRange = new Vector2(1.5f, 4f);
    [SerializeField] Vector2 xRange = new Vector2(-70f, 70f);
    [SerializeField] Vector2 yRange = new Vector2(0f, 3f);        // launch height (rocket rises from here)
    [SerializeField] Vector2 zRange = new Vector2(120f, 220f);
    [SerializeField] float colorIntensity = 3f;                   // >1 so Bloom picks it up

    float timer;

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f || fireworkPrefabs.Length == 0) return;
        timer = Random.Range(delayRange.x, delayRange.y);
        Launch();
    }

    void Launch()
    {
        var prefab = fireworkPrefabs[Random.Range(0, fireworkPrefabs.Length)];
        Vector3 pos = new Vector3(Random.Range(xRange.x, xRange.y),
                                  Random.Range(yRange.x, yRange.y),
                                  Random.Range(zRange.x, zRange.y));
        var go = PoolManager.Instance.Get(prefab, pos, Quaternion.identity);

        Color c = Random.ColorHSV(0f, 1f, 0.75f, 1f, 1f, 1f);
        var hdr = new Color(c.r * colorIntensity, c.g * colorIntensity, c.b * colorIntensity, 1f);
        foreach (var ps in go.GetComponentsInChildren<ParticleSystem>())
        {
            var main = ps.main;
            main.startColor = hdr;
            ps.Clear();
            // Only the rocket is started. The burst is wired as a *death sub-emitter* of the
            // rocket, so it detonates exactly where a rocket particle dies (its apex) instead
            // of back at the launch point.
            if (ps.name == "Rocket") ps.Play();
        }
        AudioManager.Instance.PlayFirework();
    }
}
