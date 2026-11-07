using UnityEngine;

/// <summary>Returns a pooled VFX object to the pool after a lifetime.</summary>
public class AutoRelease : MonoBehaviour
{
    [SerializeField] float lifetime = 3f;
    float timer;

    void OnEnable() => timer = lifetime;

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) PoolManager.Instance.Release(gameObject);
    }
}
