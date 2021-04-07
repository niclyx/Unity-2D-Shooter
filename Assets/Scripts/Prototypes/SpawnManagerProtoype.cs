using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerProtoype : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemy;
    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }

    void Update()
    {
        
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while(true)
        {
            Vector3 spawnPosRange = new Vector3(Random.Range(-5f, 5f), 7f, 0);
            Instantiate(_enemy, spawnPosRange, Quaternion.identity);
            yield return new WaitForSeconds(3f);
        }
    }
}
