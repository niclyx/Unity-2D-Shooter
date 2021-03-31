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


    // Start is called before the first frame update
    void Start()
    {
        //if(startSpawn==true)
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    //spawn enemy every 5 seconds
    IEnumerator SpawnEnemyRoutine()
    {
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
        //every 3-7 seconds spawn powerup
        
        while (!_isPlayerDead)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f));
            Vector3 powerUpSpawnPos = new Vector3(Random.Range(-9f, 9f), 7.3f, 0);
            int powerupToSpawn = Random.Range(0, 3);
            Instantiate(_powerupsArr[powerupToSpawn], powerUpSpawnPos, Quaternion.identity);
            //Instantiate(_tripleShotPowerupPrefab, powerUpSpawnPos, Quaternion.identity);

        }
    }

    public void PlayerDied()
    {
        _isPlayerDead = true;
    }

   
}
