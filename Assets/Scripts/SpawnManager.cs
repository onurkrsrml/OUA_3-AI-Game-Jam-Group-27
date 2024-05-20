using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private bool randomSpawn;
    [SerializeField] private Transform chaserTransform;
    [SerializeField] private Transform evaderTransform;
    [SerializeField] private Transform[] chaserSpawnPlatforms;
    [SerializeField] private Transform[] evaderSpawnPlatforms;
    [SerializeField] private Vector2 spawnDistanceRange;
 
    private Vector2 chaserStartPosition;
    private Vector2 evaderStartPosition;
    private List<Vector2> chaserSpawnPoints = new List<Vector2>();
    private List<Vector2> evaderSpawnPoints = new List<Vector2>();

    private void Start()
    {
        chaserStartPosition = chaserTransform.localPosition;
        evaderStartPosition = evaderTransform.localPosition;

        foreach (Transform spawnPlatform in chaserSpawnPlatforms)
        {
            for (float i = -spawnPlatform.localScale.x / 2; i < spawnPlatform.localScale.x / 2; i++)
            {
                chaserSpawnPoints.Add(new Vector2(spawnPlatform.localPosition.x + i + .5f, spawnPlatform.localPosition.y + spawnPlatform.localScale.y / 2 + 0.5f));
            }
        }

        foreach (Transform spawnPlatform in evaderSpawnPlatforms)
        {
            for (float i = -spawnPlatform.localScale.x / 2; i < spawnPlatform.localScale.x / 2; i++)
            {
                evaderSpawnPoints.Add(new Vector2(spawnPlatform.localPosition.x + i + .5f, spawnPlatform.localPosition.y + spawnPlatform.localScale.y / 2 + 0.5f));
            }
        }
    }

    public void SpawnAgents()
    {
        if (randomSpawn)
        {
            Vector2 chaserSpawnPoint = chaserSpawnPoints[Random.Range(0, chaserSpawnPoints.Count)];
            Vector2 evaderSpawnPoint = evaderSpawnPoints[Random.Range(0, evaderSpawnPoints.Count)];
            
            while (Vector2.Distance(chaserSpawnPoint, evaderSpawnPoint) < spawnDistanceRange.x || Vector2.Distance(chaserSpawnPoint, evaderSpawnPoint) > spawnDistanceRange.y)
            {
                evaderSpawnPoint = evaderSpawnPoints[Random.Range(0, evaderSpawnPoints.Count)];
            }

            chaserTransform.localPosition = chaserSpawnPoint;
            evaderTransform.localPosition = evaderSpawnPoint;
        }
        else
        {
            chaserTransform.localPosition = chaserStartPosition;
            evaderTransform.localPosition = evaderStartPosition;
        }
    }
}
