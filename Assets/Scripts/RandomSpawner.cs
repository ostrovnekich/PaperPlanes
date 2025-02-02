using UnityEngine;
using System.Collections.Generic;

public class RandomSpawner : MonoBehaviour
{
    public Vector3[] spawnZones;
    public Vector3[] spawnAreaSizes;
    public GameObject objectToSpawn;
    public int totalObjectsToSpawn;
    public bool useRandomRotation = false; // Флаг для включения/выключения случайной ротации

    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        if (spawnZones.Length != spawnAreaSizes.Length)
        {
            Debug.LogError("Количество зон спавна не совпадает с количеством размеров зон!");
            return;
        }

        SpawnObjectsInZones();
    }

    public void SpawnObjectsInZones()
    {
        if (spawnZones.Length == 0)
        {
            Debug.LogWarning("Зоны спавна не заданы!");
            return;
        }

        if (totalObjectsToSpawn == 1)
        {
            int randomZoneIndex = Random.Range(0, spawnZones.Length);
            SpawnObjectsInZone(spawnZones[randomZoneIndex], spawnAreaSizes[randomZoneIndex], 1);
        }
        else
        {
            int objectsPerZone = totalObjectsToSpawn / spawnZones.Length;

            int leftoverObjects = totalObjectsToSpawn % spawnZones.Length;

            for (int i = 0; i < spawnZones.Length; i++)
            {
                int objectsToSpawnInThisZone = objectsPerZone;

                if (leftoverObjects > 0)
                {
                    objectsToSpawnInThisZone++;
                    leftoverObjects--;
                }

                SpawnObjectsInZone(spawnZones[i], spawnAreaSizes[i], objectsToSpawnInThisZone);
            }
        }
    }

    void SpawnObjectsInZone(Vector3 zoneCenter, Vector3 areaSize, int objectsToSpawn)
    {
        for (int i = 0; i < objectsToSpawn; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                Random.Range(-areaSize.y / 2, areaSize.y / 2),
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            ) + zoneCenter;

            Quaternion rotation = useRandomRotation 
                ? Random.rotation 
                : Quaternion.identity;

            GameObject spawnedObject = Instantiate(objectToSpawn, randomPosition, rotation);
            spawnedObjects.Add(spawnedObject);
        }
    }

    public void RemoveAllSpawnedObjects()
    {
        foreach (GameObject spawnedObject in spawnedObjects)
        {
            if (spawnedObject != null)
            {
                Destroy(spawnedObject);
            }
        }
        spawnedObjects.Clear();
        Debug.Log("Все заспавненные объекты удалены.");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < spawnZones.Length; i++)
        {
            Gizmos.DrawWireCube(spawnZones[i], spawnAreaSizes[i]);
        }
    }
}