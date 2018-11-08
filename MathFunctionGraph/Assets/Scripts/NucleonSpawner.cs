using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucleonSpawner : MonoBehaviour {

    public float timeBetweenSpawns;

    public float spawnDistance;

    public Nucleon[] nucleonPrefabs;

    float timeSinceLastSpwan;

    private void Awake()
    {
        Application.targetFrameRate = 100;
    }

    private void FixedUpdate()
    {
        timeSinceLastSpwan += Time.deltaTime;
        if(timeSinceLastSpwan >= timeBetweenSpawns)
        {
            timeSinceLastSpwan -= timeBetweenSpawns;
            SpawnNucleon();
        }
    }

    void SpawnNucleon()
    {
        Nucleon prefab = nucleonPrefabs[Random.Range(0, nucleonPrefabs.Length)];
        Nucleon spawn = Instantiate<Nucleon>(prefab);
        spawn.transform.localPosition = Random.onUnitSphere * spawnDistance;
    }
}
