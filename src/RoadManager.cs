using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public GameObject roadPrefab;
    public Transform player;

    public int initialChunks = 6;
    public float chunkLength = 30f;
    public int maxActiveChunks = 8;

    private float spawnZ = 0f;
    private List<GameObject> activeChunks = new List<GameObject>();

    void Start()
    {
        // spawn initial road
        for (int i = 0; i < initialChunks; i++)
        {
            SpawnChunk();
        }
    }

    void Update()
    {
        // spawn ahead if player is near end
        if (player.position.z + (chunkLength * 10) > spawnZ)
        {
            SpawnChunk();
            DeleteOldChunk();
        }
    }

    void SpawnChunk()
    {
        GameObject chunk = Instantiate(roadPrefab, Vector3.forward * spawnZ, Quaternion.identity);
        activeChunks.Add(chunk);

        spawnZ += chunkLength;
    }

    void DeleteOldChunk()
    {
        if (activeChunks.Count <= maxActiveChunks)
            return;

        Destroy(activeChunks[0]);
        activeChunks.RemoveAt(0);
    }
}