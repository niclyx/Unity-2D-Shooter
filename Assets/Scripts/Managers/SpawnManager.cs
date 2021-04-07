using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _enemyPrefab;
    //[SerializeField]
    //private GameObject _tripleShotPowerupPrefab;
    [SerializeField]
    private GameObject[] _powerupsArr;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private float _spawnRate = 2f;

    //public bool startSpawn = false;

    private bool _isPlayerDead = false;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    //spawn enemy every 5 seconds
    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(2f);
        while(!_isPlayerDead)
        {
            Vector3 spawnPositions = new Vector3(Random.Range(-9.6f, 9.6f), 7.3f, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPositions, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_spawnRate);

        }
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(2f);
        while (!_isPlayerDead)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f));
            Vector3 powerUpSpawnPos = new Vector3(Random.Range(-9f, 9f), 7.3f, 0);
            int powerupToSpawn = Random.Range(0, 3);
            Instantiate(_powerupsArr[powerupToSpawn], powerUpSpawnPos, Quaternion.identity);
        }
    }

    public void PlayerDied()
    {
        _isPlayerDead = true;
    }

   
}
