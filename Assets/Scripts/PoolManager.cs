using System.Collections.Generic;
using UnityEngine;

/// <summary>Marks an object as pooled and remembers which prefab it came from.</summary>
public class PooledObject : MonoBehaviour
{
    public GameObject Prefab;

    /// <summary>
    /// Rotation the prefab root is authored with. Imported glTF models carry their
    /// axis correction on the root, so a spawner that sets its own rotation must keep
    /// this as the base or the model ends up lying on its back.
    /// </summary>
    public Quaternion BaseRotation = Quaternion.identity;
}

/// <summary>Generic object pool (road segments, obstacles, coins, fuel cans, VFX).</summary>
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    readonly Dictionary<GameObject, Queue<GameObject>> pools = new();
    Transform root;

    void Awake()
    {
        Instance = this;
        root = new GameObject("PooledObjects").transform;
        root.SetParent(transform);
    }

    public void Prewarm(GameObject prefab, int count)
    {
        var list = new List<GameObject>();
        for (int i = 0; i < count; i++) list.Add(Get(prefab, Vector3.zero, Quaternion.identity));
        foreach (var go in list) Release(go);
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!pools.TryGetValue(prefab, out var queue))
            pools[prefab] = queue = new Queue<GameObject>();

        GameObject go = null;
        while (queue.Count > 0 && go == null) go = queue.Dequeue();

        if (go == null)
        {
            go = Instantiate(prefab);                       // only when the pool is empty
            var pooled = go.AddComponent<PooledObject>();
            pooled.Prefab = prefab;
            pooled.BaseRotation = go.transform.rotation;    // orientation the prefab is authored with
        }

        // Caller's rotation goes on top of the orientation the prefab was authored with.
        Quaternion baseRotation = go.GetComponent<PooledObject>().BaseRotation;

        go.transform.SetParent(parent, false);
        go.transform.SetPositionAndRotation(position, rotation * baseRotation);
        go.SetActive(true);
        return go;
    }

    public void Release(GameObject go)
    {
        if (!go.TryGetComponent<PooledObject>(out var po)) { Destroy(go); return; }
        go.SetActive(false);
        go.transform.SetParent(root, false);
        pools[po.Prefab].Enqueue(go);
    }
}
