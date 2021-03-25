using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private float _spawnRate = 2f;

    public bool startSpawn = false;

    private bool _isPlayerDead = false;


    // Start is called before the first frame update
    void Start()
    {
        if(startSpawn==true)
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    //spawn enemy every 5 seconds
    IEnumerator SpawnRoutine()
    {
        while(!_isPlayerDead)
        {
            Vector3 spawnPositions = new Vector3(Random.Range(-9.6f, 9.6f), 7.3f, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPositions, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_spawnRate);

        }
    }

    public void PlayerDied()
    {
        _isPlayerDead = true;
    }

   
}
