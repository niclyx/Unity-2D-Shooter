using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private float _speed = 10.0f;
    private float _speedMultiplier = 2f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private Vector3 _laserOffset = new Vector3(0, 1f, 0);
    [SerializeField]
    private float _rateOfFire = 0.1f;
    private float _canFire = -1f;
    private int _shieldsLeft;
    private int _ammo = 15;

    private float yMax = 6f;
    private float yMin = -4f;

    private bool _isTripleShotActive;
    private bool _isSpeedPowerupActive;
    private bool _isShieldPowerupActive;

    [SerializeField]
    private GameObject _shieldVisual;
    [SerializeField]
    private GameObject _leftWingDamage, _rightWingDamage;
    [SerializeField]
    private AudioClip _fireLaserClip;
    [SerializeField]
    private AudioClip _powerupPickupClip;
    [SerializeField]
    private int _score;

    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private AudioManager _audioManager;

    private SpriteRenderer _shieldSprite;
    private AudioSource _audio;

    

    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL.");
        }
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL.");
        }
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if (_audioManager == null)
        {
            Debug.LogError("Audio Manager is NULL.");
        }
        _shieldSprite = _shieldVisual.GetComponent<SpriteRenderer>();
        if(_shieldSprite == null)
        {
            Debug.LogError("Shield sprite renderer is NULL");
        }
        _audio = GetComponent<AudioSource>();
        if (_audio == null)
        {
            Debug.LogError("Player AudioSource is NULL.");
        }
        else
        {
            _audio.clip = _fireLaserClip;
        }

        transform.position = new Vector3(0, -3, 0);
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _ammo > 0)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (_isSpeedPowerupActive)
        {
            transform.Translate(direction * (_speed * _speedMultiplier) * Time.deltaTime);
        }

        //Phase I:Framework - Thrusters
        if(Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(direction * (_speed + 5f) * Time.deltaTime);
        }

        transform.Translate(direction * _speed * Time.deltaTime);

        /// Movement restrictions/special
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, yMin, yMax), 0);

        if (transform.position.x >= 10f)
        {
            transform.position = new Vector3(-10f, transform.position.y, 0);
        }
        else if (transform.position.x <= -10f)
        {
            transform.position = new Vector3(10f, transform.position.y, 0);
        }
        ///
    }

    void FireLaser()
    {
        _canFire = Time.time + _rateOfFire;
        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);
        }

        if (_audio.clip != _fireLaserClip)
        {
            _audio.clip = _fireLaserClip;
        }
        _audio.Play();

        _ammo--;
        _uiManager.UpdateAmmo(_ammo);
    }

    public void Damage()
    {
        //Phase 1:Framework -- Shield Strength
        if(_isShieldPowerupActive)
        {
            _shieldsLeft--;
            switch (_shieldsLeft)
            {
                case 2:
                    Debug.Log("2 shields left");
                    _shieldSprite.color = new Color(1f, 1f, 1f, 0.7f);
                    break;
                case 1:
                    Debug.Log("1 shields left");
                    _shieldSprite.color = new Color(1f, 1f, 1f, 0.4f);
                    break;
                case 0:
                    _isShieldPowerupActive = false;
                    _shieldVisual.SetActive(false);
                    break;
                default:
                    Debug.Log("DEFAULT");
                    break;
            }
            return;
        }

        _lives--;
        if (_lives == 2)
        {
            _leftWingDamage.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightWingDamage.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.PlayerDied();
            _audioManager.PlayExplosion();
            Destroy(this.gameObject);
        }
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    void playPowerupPickupClip()
    {
        _audio.clip = _powerupPickupClip;
        _audio.Play();
    }

    public void TripleShotPowerupActivate()
    {
        playPowerupPickupClip();
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void SpeedPowerupActivate()
    {
        playPowerupPickupClip();
        _isSpeedPowerupActive = true;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void ShieldPowerupActivate()
    {
        playPowerupPickupClip();
        _shieldSprite.color = new Color(1f, 1f, 1f, 1f);
        _shieldsLeft = 3;
        _isShieldPowerupActive = true;
        _shieldVisual.SetActive(true);
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isSpeedPowerupActive = false;

    }
}
