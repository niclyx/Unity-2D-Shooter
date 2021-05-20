using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Special : MonoBehaviour
{
    private Transform _player;
    private Transform _closestEnemy;
    private float _speed = 6f;
    private float _playerLaserSpeed = 10f;
    private bool _isEnemyLaser;
    void Start()
    {
        if (gameObject.tag == "Enemy_Fire")
        {
            _isEnemyLaser = true;
            _player = GameObject.Find("Player").GetComponent<Transform>();
            if (_player == null)
            {
                Debug.LogError("Player not found");
            }
            StartCoroutine(SelfDestructRoutine());
        }
        else if (gameObject.tag == "Laser")
        {
            _isEnemyLaser = false;
            _closestEnemy = FindClosestEnemy().transform;
        }

    }

    void Update()
    {
        if (_isEnemyLaser)
        {
            Vector3 direction = _player.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            transform.eulerAngles = Vector3.forward * angle;
            //direction.Normalize();
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
        }
        else
        {
            if (_closestEnemy==null || _closestEnemy.tag=="Being_Destroyed")
            {
                transform.Translate(Vector3.up * _playerLaserSpeed * Time.deltaTime);
                return;
            }
            Vector3 direction = _closestEnemy.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            transform.eulerAngles = Vector3.forward * angle;
            transform.Translate(Vector3.up * _playerLaserSpeed * Time.deltaTime);
        }

    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach(GameObject enemy in enemies)
        {
            Vector3 difference = enemy.transform.position - position;
            float currentDistance = difference.sqrMagnitude;
            if(currentDistance < distance)
            {
                closestEnemy = enemy;
                distance = currentDistance;
            }
        }
        return closestEnemy;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && gameObject.tag=="Enemy_Fire")
        {
            Player player = other.GetComponent<Player>();
            player.Damage();
            Destroy(this.gameObject);
        }
    }

    IEnumerator SelfDestructRoutine()
    {
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
    }
}
