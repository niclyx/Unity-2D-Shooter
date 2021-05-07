using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int _enemyID; //0=basic,1=rammer,2=fire backwards, 3=mini boss
    [SerializeField]
    private float _speed = 4f;

    private float yMin = -6f;
    private float _xMax = 9.6f;
    private float _xMin = -9.6f;
    private Vector3 _laserOffset = new Vector3(0, -2f, 0);

    private Player _player;
    private Animator _animator;
    private Collider2D _collider;
    private AudioManager _audioManager;
    private SpawnManager _spawnManager;
    private float _canFire = -1f;
    private float _fireDelay;
    private bool _isDead;
    private int _randomizer;
    private Vector3 _direction;
    private Vector3 _directionPositive;
    private Vector3 _directionNegative;
    private bool _shieldActive;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _laserSpecialPrefab;
    [SerializeField]
    private GameObject _shield =null;
    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]
    private float _amplitude = 2f;
    [SerializeField]
    private float _frequency = 2f;

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
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL.");
        }

        if (_enemyID != 3)
        {
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError("Enemy Animator is NULL");
            }
        }

        _directionPositive = new Vector3(1f, -0.1f, 0);
        _directionNegative = new Vector3(-1f, -0.1f, 0);
        _direction = _directionPositive;
        
        int randomizer = Random.Range(1, 5);
        if(randomizer == 1 && _enemyID==0)
        {
            _shieldActive = true;
            _shield.SetActive(true);
        }
    }

    void Update()
    {
        switch (_enemyID)
        {
            case 0:
            case 2:
                EnemyMovementSideToSide();
                FireLaser();
                break;
            case 1:
                EnemyMovementStraight();
                if (Vector3.Distance(transform.position, _player.transform.position) < 3f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _speed * Time.deltaTime);
                }
                break;
            case 3:
                EnemyMovementWave();
                FireLaserSpecial();
                break;
            default:
                EnemyMovementStraight();
                break;
        }
        


    }

    void FireLaser()
    {
        if (Time.time > _canFire && !_isDead)
        {
            _fireDelay = Random.Range(3f, 7f);
            _canFire = Time.time + _fireDelay;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            
            foreach (Laser l in lasers)
            {
                l.AssignEnemyLaser();
                if(_enemyID==2 && transform.position.y < _player.transform.position.y)
                {
                    l.IsEnemyReverseShot();
                }
            }
        }
    }

    void FireLaserSpecial()
    {
        if (Time.time > _canFire && !_isDead)
        {
            _fireDelay = Random.Range(5f, 7f);
            _canFire = Time.time + _fireDelay;
            GameObject enemyLaser = Instantiate(_laserSpecialPrefab, transform.position + _laserOffset, Quaternion.identity);
        }
    }


    void EnemyMovementStraight()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= yMin)
        {
            float randomX = Random.Range(-9.6f, 9.6f);
            transform.position = new Vector3(randomX, 7.3f, 0);
        }
    }

    void EnemyMovementSideToSide()
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

    void EnemyMovementWave()
    {
        float x = 1f;
        float y = Mathf.Sin(Time.time * _frequency) * _amplitude;
        float z = transform.position.z;
        transform.Translate(new Vector3(x, y, z) * 2f * Time.deltaTime) ;
        if (transform.position.x >= 10f)
        {
            transform.position = new Vector3(-10f, transform.position.y, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_shieldActive)
            {
                _shieldActive = false;
                _shield.SetActive(false);
                return;
            }
            if (_player != null)
            {
                _player.Damage();
                _player.AddScore(10);
            }
            PostDeathSequence();
        }
        else if (other.CompareTag("Laser"))
        {
            if (_shieldActive)
            {
                _shieldActive = false;
                _shield.SetActive(false);
                return;
            }

            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }
            PostDeathSequence();
        }
    }

    void PostDeathSequence()
    {
        if(_animator!=null) { 
            _animator.SetTrigger("OnEnemyDeath");
        }
        else
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(GetComponent<SpriteRenderer>());
        }
        _audioManager.PlayExplosion();
        _speed = 0;
        _isDead = true;
        _spawnManager.DecrementEnemyCount();
        Destroy(GetComponent<Collider2D>());
        Destroy(this.gameObject, 2.4f);
    }
}
