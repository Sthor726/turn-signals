using System;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject conePrefab;
    public GameObject coinPrefab;

    public float spawnDuration;
    private float spawnTimer;
    public int previousCoinLocation;

    public float chanceOfCoin = 0.5f;

    public float chanceOfCone = 0.1f;

    private GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnTimer = spawnDuration;
        gameManager = FindFirstObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.gameState != GameManager.GameState.Active) return;
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnDuration;
            // Spawn Object
            SpawnObject();
        }
    }

    void SpawnObject()
    {
        if (chanceOfCoin > Random.Range(0f, 1f))
        {
            int lane;
            if (Random.Range(0f, 1f) <= 0.75f)
            {
                lane = previousCoinLocation;
            }
            else
            {
                do
                {
                    lane = Random.Range(-1, 2);
                } while (Math.Abs(lane - previousCoinLocation) > 1);
            }
            
            

            Vector3 pos = new Vector3(
                lane * 3f,
                0f,
                transform.position.z
            );
            Instantiate(coinPrefab, pos, transform.rotation);
        }
        else if (chanceOfCone > Random.Range(0f, chanceOfCoin))
        {
            Vector3 pos = new Vector3(
                Random.Range(-1f, 1f) * 3f,
                0f,
                transform.position.z
            );
            Instantiate(conePrefab, pos, transform.rotation);
        }
    }
}