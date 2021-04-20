using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;

    private float yMin = -6f;
    private float _xMax = 9.6f;
    private float _xMin = -9.6f;

    private Player _player;
    private Animator _animator;
    private Collider2D _collider;
    private AudioManager _audioManager;
    private float _canFire = -1f;
    private float _fireDelay;
    private bool _isDead;
    private int _randomizer;
    private Vector3 _direction;
    [SerializeField]
    private Vector3 _directionPositive;
    [SerializeField]
    private Vector3 _directionNegative;

    [SerializeField]
    private GameObject _laserPrefab;
    void Start()
    {
        //Cache to reduce get component calls
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.LogError("Player is NULL");
        }
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if (_audioManager == null)
        {
            Debug.LogError("Audio Manager is NULL.");
        }

        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Enemy Animator is NULL");
        }

        _directionPositive = new Vector3(1f, -0.1f, 0);
        _directionNegative = new Vector3(-1f, -0.1f, 0);
        _direction = _directionPositive;
    }

    void Update()
    {
        EnemyMovement();

        if(Time.time > _canFire && !_isDead)
        {
            _fireDelay = Random.Range(3f, 7f);
            _canFire = Time.time + _fireDelay;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            foreach(Laser l in lasers)
            {
                l.AssignEnemyLaser();
            }
        }
    }

    void EnemyMovement()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);

        if(transform.position.x > _xMax)
        {
            _direction = _directionNegative;
        }
        else if(transform.position.x < _xMin)
        {
            _direction = _directionPositive;
        }

        if (transform.position.y <= yMin)
        {
            //float randomX = Random.Range(-9.6f, 9.6f);
            transform.position = new Vector3(transform.position.x, 7.3f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
                _player.AddScore(10);
            }
            _animator.SetTrigger("OnEnemyDeath");
            _audioManager.PlayExplosion();
            _speed = 0;
            _isDead = true;
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.4f);
        }
        else if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }
            _animator.SetTrigger("OnEnemyDeath");
            _audioManager.PlayExplosion();
            _speed = 0;
            _isDead = true;
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.4f);
        }
    }
}
