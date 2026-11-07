using System.Collections.Generic;
using UnityEngine;

/// <summary>Procedural infinite road: scrolls pooled segments toward the player and fills them with items.</summary>
public class RoadGenerator : MonoBehaviour
{
    [Header("Segments")]
    [SerializeField] RoadSegment segmentPrefab;
    [SerializeField] int segmentCount = 7;
    [SerializeField] float segmentLength = 60f;       // must match the prefab's road length
    [SerializeField] float laneWidth = 3.5f;          // must match PlayerController
    [SerializeField] int emptySegments = 3;           // safe start

    [Header("Prefabs")]
    [SerializeField] GameObject[] staticObstaclePrefabs;
    [SerializeField] GameObject[] movingObstaclePrefabs;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] GameObject fuelPrefab;

    [Header("Spawn tuning")]
    [SerializeField] int rowsPerSegment = 3;
    [SerializeField] Vector2 rowChance = new Vector2(0.35f, 0.85f);        // easy -> hard
    [SerializeField] Vector2 secondObstacleChance = new Vector2(0.2f, 0.75f);
    [SerializeField] Vector2 movingChance = new Vector2(0.35f, 0.65f);
    [SerializeField] float fuelChancePerSegment = 0.3f;

    readonly List<RoadSegment> segments = new();
    int spawnedCount;

    void Start()
    {
        for (int i = 0; i < segmentCount; i++)
            segments.Add(CreateSegment((i - 1) * segmentLength));
    }

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm.State != GameState.Playing) return;

        Vector3 move = Vector3.back * (gm.Speed * Time.deltaTime);
        foreach (var s in segments) s.transform.position += move;

        var first = segments[0];
        if (first.transform.position.z + segmentLength < -30f)
        {
            float newZ = segments[segments.Count - 1].transform.position.z + segmentLength;
            first.ClearItems();
            PoolManager.Instance.Release(first.gameObject);
            segments.RemoveAt(0);
            segments.Add(CreateSegment(newZ));
        }
    }

    RoadSegment CreateSegment(float z)
    {
        var go = PoolManager.Instance.Get(segmentPrefab.gameObject, new Vector3(0, 0, z), Quaternion.identity, transform);
        var seg = go.GetComponent<RoadSegment>();
        Populate(seg);
        return seg;
    }

    void Populate(RoadSegment seg)
    {
        spawnedCount++;
        if (spawnedCount <= emptySegments) return;

        float d = GameManager.Instance.Difficulty01;
        float rowP = Mathf.Lerp(rowChance.x, rowChance.y, d);
        float secondP = Mathf.Lerp(secondObstacleChance.x, secondObstacleChance.y, d);
        float spacing = segmentLength / rowsPerSegment;

        for (int r = 0; r < rowsPerSegment; r++)
        {
            float z = spacing * (r + 0.5f);
            int freeLane = Random.Range(-1, 2);      // this lane is NEVER blocked -> level always passable

            if (Random.value < rowP)
            {
                var blocked = new List<int> { -1, 0, 1 };
                blocked.Remove(freeLane);
                PlaceObstacle(seg, blocked[0], z, d);
                if (Random.value < secondP) PlaceObstacle(seg, blocked[1], z, d);
                if (Random.value < 0.5f) PlaceCoinLine(seg, freeLane, z);
            }
            else if (Random.value < 0.4f)
            {
                PlaceCoinLine(seg, Random.Range(-1, 2), z);
            }
        }

        if (fuelPrefab != null && Random.value < fuelChancePerSegment)
        {
            float z = spacing * Random.Range(1, rowsPerSegment);   // between rows, away from obstacles
            Place(seg, fuelPrefab, Random.Range(-1, 2), z, 0.2f);  // the can sits on the tarmac and bobs ±0.15
        }
    }

    void PlaceObstacle(RoadSegment seg, int lane, float z, float difficulty)
    {
        bool moving = movingObstaclePrefabs.Length > 0 &&
                      Random.value < Mathf.Lerp(movingChance.x, movingChance.y, difficulty);
        var pool = moving ? movingObstaclePrefabs : staticObstaclePrefabs;
        if (pool.Length == 0) return;
        Place(seg, pool[Random.Range(0, pool.Length)], lane, z, 0f);
    }

    void PlaceCoinLine(RoadSegment seg, int lane, float centerZ)
    {
        if (coinPrefab == null) return;
        for (int i = -2; i <= 2; i++)
            Place(seg, coinPrefab, lane, centerZ + i * 3f, 1f);
    }

    void Place(RoadSegment seg, GameObject prefab, int lane, float z, float y)
    {
        Vector3 pos = seg.transform.TransformPoint(new Vector3(lane * laneWidth, y, z));
        var go = PoolManager.Instance.Get(prefab, pos, Quaternion.identity, seg.transform);
        var item = go.GetComponent<Interactable>();
        seg.Register(item);
        item.OnSpawn();
    }
}
