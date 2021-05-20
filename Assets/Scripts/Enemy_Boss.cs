using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private GameObject _laserPrefab;
    private UIManager _uiManager;
    private Player _player;
    private SpriteRenderer _renderer;
    private AudioManager _audioManager;
    private Color _originalColor;
    [SerializeField]
    private int _lives = 15;
    private List<int> _tempList;
    private Transform[] _firingPorts;
    private float _canFire = 3f;
    [SerializeField]
    private float _fireDelay = 3f;
    private float _spawnFireDelay = 3f;
    private float _speed = 3f;
    [SerializeField]
    private float _overlapDuration = 1f;
    private float _elapsed = 0f;
    private bool _isDead;

    void Start()
    {
        _canFire = Time.time + _spawnFireDelay;
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
        {
            Debug.LogError("Boss enemy Sprite Renderer is null");
        }
        else
        {
            _originalColor = _renderer.color;
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("Boss enemy _uiManager is null");
        }
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Boss enemy _player is null");
        }
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if (_audioManager == null)
        {
            Debug.LogError("Audio Manager is NULL.");
        }

        _firingPorts = GetComponentsInChildren<Transform>();
        _tempList = new List<int>();
        //Debug.Log("_firingPorts size " + _firingPorts.Length);
    }

    void Update()
    {
        if(transform.position.y > 2.85f)
        {
            transform.Translate(Vector3.up * -_speed * Time.deltaTime);
        }

        if (!_isDead && _player!=null)
        {
            FiringSequence();
        }

    }

    void FiringSequence()
    {
        if (Time.time > _canFire)
        {
            _canFire = Time.time + _fireDelay;
            _tempList.Clear();
            while (_tempList.Count != 5)
            {
                int i = Random.Range(1, 9);
                if (!_tempList.Contains(i))
                    _tempList.Add(i);
            }
            StartCoroutine(FiringSequenceRoutine(_tempList));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            //Debug.Log("Damaged!");
            StartCoroutine(DamageFlashRoutine());
            Destroy(other.gameObject);
            _lives--;
            if (_lives <= 0)
            {
                StartCoroutine(BlowUpRoutine());
                _uiManager.WinGameOverSequence();
                _player.AddScore(100);
                GetComponent<Collider2D>().enabled = false;
                _renderer.enabled = false;
                _isDead = true;
                _audioManager.PlayExplosion();
                //Destroy(this.gameObject);
            }
        }

        if (other.tag == "Player")
        {
            //Debug.Log("Player overlap-turning red");
            _player.overlapWithBoss(true);
            //Debug.Log("Resetting elapsed time");
            _elapsed = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _elapsed += Time.deltaTime;
            //Debug.Log(_elapsed);

            if (_elapsed > _overlapDuration)
            {
                //Debug.Log("Damage the player");
                _player.Damage();
                _elapsed = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //Debug.Log("Player exit overlap-original color");
            _player.overlapWithBoss(false);
        }
    }

    IEnumerator FiringSequenceRoutine(List<int> list)
    {
        foreach(int port in list)
        {
            //Debug.Log("port " + port + " " +_firingPorts[port].transform.name);
            GameObject enemyLaser = Instantiate(_laserPrefab, _firingPorts[port].transform.position, Quaternion.identity);
            Laser laser = enemyLaser.GetComponent<Laser>();
            laser.AssignEnemyLaser();
            laser.tag = "Enemy_Fire";
            yield return new WaitForSeconds(0.3f);
        }
        //Debug.Log("End FiringSequenceRoutine");
    }

    IEnumerator DamageFlashRoutine()
    {
        //Debug.Log("Flashing color!");
        _renderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        _renderer.color = _originalColor;
        //Debug.Log("Return to original color");
    }

    IEnumerator BlowUpRoutine()
    {
        //Debug.Log("Exploding!");
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        yield return null;
        Instantiate(_explosionPrefab, transform.position + new Vector3(3f,2f,0), Quaternion.identity);
        yield return null;
        Instantiate(_explosionPrefab, transform.position + new Vector3(-2f, -1f, 0), Quaternion.identity);

    }
}
