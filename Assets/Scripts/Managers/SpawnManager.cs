using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject[] _powerupsArray;
    [SerializeField]
    private GameObject[] _collectibleArray;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private float _spawnRate = 3f;

    private int _enemiesToSpawn = 3;
    private int _enemiesLeft;
    private int _wave = 1;
    private UIManager _uiManager;

    //public bool startSpawn = false;

    private bool _isPlayerDead = false;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL.");
        }
    }
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine(_enemiesToSpawn));
        StartCoroutine(SpawnPowerUpRoutine());
        StartCoroutine(SpawnCollectibleRoutine());
        StartCoroutine(SpawnUltimateLaserRoutine());
        StartCoroutine(SpawnDebuffRoutine());
    }

    IEnumerator SpawnEnemyRoutine(int amountToSpawn)
    {
        _uiManager.StartWaveTextRoutine(_wave);
        yield return new WaitForSeconds(3f);
        Debug.Log("Start spawning " + amountToSpawn + " enemies");
        _enemiesLeft = amountToSpawn;
        
        while(!_isPlayerDead && amountToSpawn > 0)
        {
            Vector3 spawnPositions = new Vector3(Random.Range(-9.6f, 9.6f), 7f, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPositions, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            amountToSpawn--;
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
            Instantiate(_powerupsArray[powerupToSpawn], powerUpSpawnPos, Quaternion.identity);
        }
    }

    IEnumerator SpawnCollectibleRoutine()
    {
        while (!_isPlayerDead)
        {
            yield return new WaitForSeconds(Random.Range(3f, 6f));
            Vector3 pickupSpawnPos = new Vector3(Random.Range(-9f, 9f), 7.3f, 0);
            int collectibleToSpawn = Random.Range(0, 2);
            Instantiate(_collectibleArray[collectibleToSpawn], pickupSpawnPos, Quaternion.identity);
        }
    }

    IEnumerator SpawnDebuffRoutine()
    {
        while (!_isPlayerDead)
        {
            yield return new WaitForSeconds(Random.Range(15f, 30f));
            Vector3 pickupSpawnPos = new Vector3(Random.Range(-9f, 9f), 7.3f, 0);
            Instantiate(_powerupsArray[4], pickupSpawnPos, Quaternion.identity);
        }
    }

    //Phase 1:Framework -- Secondary Fire Powerup
    IEnumerator SpawnUltimateLaserRoutine()
    {
        while (!_isPlayerDead)
        {
            yield return new WaitForSeconds(Random.Range(30f, 45f));
            Vector3 powerUpSpawnPos = new Vector3(Random.Range(-9f, 9f), 7.3f, 0);
            Instantiate(_powerupsArray[3], powerUpSpawnPos, Quaternion.identity);
        }
    }

    public void PlayerDied()
    {
        _isPlayerDead = true;
    }

    public void DecrementEnemyCount()
    {
        _enemiesLeft--;
        if (_enemiesLeft == 0 && _wave < 3)
        {
            _wave++;
            _enemiesToSpawn += 3;
            StartCoroutine(SpawnEnemyRoutine(_enemiesToSpawn));
        }
    }


}
