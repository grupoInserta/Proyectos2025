using UnityEngine;

public class RandomSpawnManager : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Transform[] spawnpoints;

    private void Start()
    {
        SpawnearObjeto();
    }

    public void SpawnearObjeto()
    {
        if (spawnpoints.Length == 0) return;
        int indiceAleatorio = Random.Range(0, spawnpoints.Length);
        Instantiate(objectToSpawn, spawnpoints[indiceAleatorio].position, Quaternion.identity);
    }
}
