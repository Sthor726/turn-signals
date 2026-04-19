using System.Collections.Generic;
using UnityEngine;

public class NpcSpawner : MonoBehaviour
{
    [Header("References")] public Transform player;
    public GameObject npcCarPrefab;
    public GameObject conePrefab;

    [Header("Lane Settings")] public float laneWidth = 2f;

    [Header("Spawn Distance Window")] public float spawnAheadDistance = 80f;
    public float spawnBehindDistance = 30f;

    [Header("Traffic Control")] public float spawnInterval = 1.5f;
    public int maxCarsPerWave = 2;

    [Header("Z Placement")] public float minZSpacing = 10f;

    private float timer;
    public float yOffset = 1f;

    private List<GameObject> activeObjects = new List<GameObject>();

    void Start()
    {
        if (player == null)
            player = Object.FindFirstObjectByType<PlayerMovement>().transform;
    }

    void Update()
    {
        HandleSpawning();
        CleanupBehindPlayer();
    }

    void HandleSpawning()
    {
        timer -= Time.deltaTime;

        if (timer > 0f)
            return;

        timer = spawnInterval;

        SpawnWave();
    }

    void SpawnWave()
    {
        int carCount = Random.Range(1, maxCarsPerWave + 1);

        List<int> lanes = new List<int> { -1, 0, 1 };
        Shuffle(lanes);

        float baseZ = player.position.z + spawnAheadDistance;

        float currentZ = baseZ;

        for (int i = 0; i < carCount; i++)
        {
            int lane = lanes[i];

            SpawnCar(lane, currentZ);

            currentZ += minZSpacing;
        }
    }

    void SpawnCar(int lane, float z)
    {
        if (Random.Range(0f, 1f) <= 0.25f)
        {
            Vector3 pos = new Vector3(
                lane * laneWidth,
                0f,
                z
            );
            GameObject cone = Instantiate(conePrefab, pos, Quaternion.identity);
            activeObjects.Add(cone);
        }
        else
        {
            Vector3 pos = new Vector3(
                lane * laneWidth,
                yOffset,
                z
            );
            GameObject car = Instantiate(npcCarPrefab, pos, Quaternion.identity);
            activeObjects.Add(car);
        }
    }

    void CleanupBehindPlayer()
    {
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            if (activeObjects[i] == null)
            {
                activeObjects.RemoveAt(i);
                continue;
            }

            if (activeObjects[i].transform.position.z < player.position.z - spawnBehindDistance)
            {
                Destroy(activeObjects[i]);
                activeObjects.RemoveAt(i);
            }
        }
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
}