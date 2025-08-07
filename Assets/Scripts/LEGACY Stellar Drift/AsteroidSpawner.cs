using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private Rigidbody2D[] asteroidPool;
    [SerializeField] private SpawnPositionWithDirection[] spawningPositions;
    [SerializeField] private float asteroidDefaultSpeed;
    [SerializeField] private float spawningTime;

    private List<SpawnPositionWithDirection> randomSpawnPositions = new List<SpawnPositionWithDirection>();
    private float timer;
    

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
            SpawnAsteroid();
    }

    private void SpawnAsteroid()
    {
        timer = spawningTime;

        Rigidbody2D asteroid = ChosenAsteroid();

        if (asteroid == null)
            return;

        SpawnPositionWithDirection spawnPosition = GetSpawnPositionWithDirection();
        asteroid.transform.position = spawnPosition.SpawnPosition.position;
        asteroid.velocity = Vector2.zero;

        asteroid.gameObject.SetActive(true);
        asteroid.AddForce(spawnPosition.VelocityDirection * asteroidDefaultSpeed, ForceMode2D.Impulse);
    }

    private SpawnPositionWithDirection GetSpawnPositionWithDirection()
    {
        if (randomSpawnPositions.Count < 1)
        {
            randomSpawnPositions.Clear();
            randomSpawnPositions = new List<SpawnPositionWithDirection>(spawningPositions);
        }

        SpawnPositionWithDirection spawnPos = randomSpawnPositions[Random.Range(0, randomSpawnPositions.Count)];
        randomSpawnPositions.Remove(spawnPos);
        return spawnPos;
    }

    private Rigidbody2D ChosenAsteroid()
    {
        Rigidbody2D asteroid = null;

        for (int i=0; i < asteroidPool.Length; i++)
        {
            if (asteroidPool[i].gameObject.activeInHierarchy)
                continue;

            asteroid = asteroidPool[i];
            break;
        }

        return asteroid;
    }
}

[System.Serializable]
public struct SpawnPositionWithDirection
{
    public Transform SpawnPosition;
    public Vector2 VelocityDirection;
}
