using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private GameObject[] _enemyPrefabs;
    [SerializeField]
    private GameObject[] _pickupsArrayT1;
    [SerializeField]
    private GameObject[] _pickupsArrayT2;
    [SerializeField]
    private GameObject[] _pickupsArrayT3;
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
        StartCoroutine(SpawnPickupsRoutine());
    }

    IEnumerator SpawnEnemyRoutine(int amountToSpawn)
    {
        _uiManager.StartWaveTextRoutine(_wave);
        yield return new WaitForSeconds(3f);
        Debug.Log("Start spawning " + amountToSpawn + " enemies");
        _enemiesLeft = amountToSpawn;
        
        while(!_isPlayerDead && amountToSpawn > 0)
        {
            GameObject newEnemy = null;
            Vector3 spawnPositions = new Vector3(Random.Range(-9.6f, 9.6f), 7f, 0);
            int enemyToSpawn = Random.Range(0, 101);
            switch (enemyToSpawn)
            {
                case int n when (n >= 0 && n <=40):
                    newEnemy = Instantiate(_enemyPrefabs[0], spawnPositions, Quaternion.identity);
                    break;
                case int n when (n >= 41 && n <= 60):
                    newEnemy = Instantiate(_enemyPrefabs[1], spawnPositions, Quaternion.identity);
                    break;
                case int n when (n >= 61 && n <= 80):
                    newEnemy = Instantiate(_enemyPrefabs[2], spawnPositions, Quaternion.identity);
                    break;
                case int n when (n >= 81 && n <= 95):
                    newEnemy = Instantiate(_enemyPrefabs[3], spawnPositions, Quaternion.identity);
                    break;
                case int n when (n >= 96 && n <= 100):
                    newEnemy = Instantiate(_enemyPrefabs[4], new Vector3(-11.5f,1.5f,0), Quaternion.identity);
                    break;
                default:
                    break;
            }
            newEnemy.transform.parent = _enemyContainer.transform;
            amountToSpawn--;
            yield return new WaitForSeconds(_spawnRate);
        }
    }

    IEnumerator SpawnPickupsRoutine()
    {
        while (!_isPlayerDead)
        {
            yield return new WaitForSeconds(Random.Range(3f, 6f));
            int pickupGenerator = Random.Range(0, 100); //T1 0 - 70, T2 71 - 90, T3 91 - 99
            //Debug.Log("Pickup RNG: " + pickupGenerator);
            Vector3 pickupSpawnLocRange = new Vector3(Random.Range(-9f, 9f), 7.3f, 0);
            switch (pickupGenerator)
            {
                case int n when (n >= 0 && n <= 70):
                    Instantiate(_pickupsArrayT1[Random.Range(0,4)], pickupSpawnLocRange, Quaternion.identity);
                    break;
                case int n when (n >= 71 && n <= 90):
                    Instantiate(_pickupsArrayT2[Random.Range(0, 2)], pickupSpawnLocRange, Quaternion.identity);
                    break;
                case int n when (n >= 91 && n < 100):
                    //Instantiate(_pickupsArrayT3[Random.Range(0, 2)], pickupSpawnLocRange, Quaternion.identity);
                    Instantiate(_pickupsArrayT3[0], pickupSpawnLocRange, Quaternion.identity);
                    break;
                default:
                    break;
            }
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
