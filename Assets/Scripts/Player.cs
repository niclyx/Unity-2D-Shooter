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
    private GameObject _360LaserPrefab;
    [SerializeField]
    private GameObject _laserTrackingPrefab;
    [SerializeField]
    private Vector3 _laserOffset = new Vector3(0, 1f, 0);
    [SerializeField]
    private float _rateOfFire = 0.1f;
    private float _canFire = -1f;
    private float _rateOfPickup = 5f;
    private float _canPickup = -1f;
    private bool _fireLocked;
    private int _shieldsLeft;
    private int _trackingLasersLeft;
    [SerializeField]
    private int _ammo = 15;

    private float yMax = 6f;
    private float yMin = -4f;

    private bool _isTripleShotActive;
    private bool _isSpeedPowerupActive;
    private bool _isShieldPowerupActive;
    private bool _is360LaserPowerupActive;
    private bool _isLaserTrackingActive;

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
    private float _fuel = 100f;
    private bool canUseThrusters = true;

    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private AudioManager _audioManager;

    private SpriteRenderer _shieldSprite;
    private SpriteRenderer _playerSprite;
    private Color _originalColor;
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
        if (_shieldSprite == null)
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
        _playerSprite = GetComponent<SpriteRenderer>();
        if (_playerSprite == null)
        {
            Debug.LogError("Player SpriteRenderer is NULL.");
        }
        else
        {
            _originalColor = _playerSprite.color;
        }

        transform.position = new Vector3(0, -3, 0);
    }

    void Update()
    {
        CalculateMovement();

        if (_fuel < 100f && canUseThrusters)
        {
            _fuel += 0.05f;
            _uiManager.UpdateFuel(_fuel);
        }

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _ammo > 0 && !_fireLocked)
        {
            FireLaser();
        }

        if(Time.time > _canPickup)
        {
            _uiManager.PickupAvailable();
        }
        if (Input.GetKeyDown(KeyCode.C) && Time.time > _canPickup)
        {
            _uiManager.PickupUnavailable();
            CollectPickups();
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
        if (Input.GetKey(KeyCode.LeftShift) && _fuel > 0 && canUseThrusters)
        {
            transform.Translate(direction * (_speed + 5f) * Time.deltaTime);
            _fuel -= 0.5f;
            _uiManager.UpdateFuel(_fuel);
            if (_fuel <= 0)
            {
                _uiManager.UpdateFuel(0);
                canUseThrusters = false;
                StartCoroutine(RefuelCoroutine());
            }
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
        else if (_is360LaserPowerupActive)
        {
            Instantiate(_360LaserPrefab, transform.position, Quaternion.identity);
        }
        else if (_isLaserTrackingActive)
        {
            GameObject laser = Instantiate(_laserTrackingPrefab, transform.position + _laserOffset, Quaternion.identity);
            laser.tag = "Laser";
            _trackingLasersLeft--;
            if (_trackingLasersLeft == 0)
            {
                _isLaserTrackingActive = false;
            }
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);
        }

        if (_audio.clip != _fireLaserClip)
        {
            _audio.clip = _fireLaserClip;
            if (_audio.pitch != 1f)
            {
                _audio.pitch = 1f;
                _audio.time = 0;
            }
        }
        _audio.Play();

        _ammo--;
        _uiManager.UpdateAmmo(_ammo);
    }

    void CollectPickups()
    {
        _canPickup = Time.time + _rateOfPickup;

        Collider2D[] pickups = Physics2D.OverlapCircleAll(transform.position, 10f);

        foreach (Collider2D collider in pickups)
        {
            if (collider.CompareTag("Powerup") || collider.CompareTag("Collectible"))
            {
                if (collider.CompareTag("Powerup"))
                {
                    Powerup pickup = collider.GetComponent<Powerup>();
                    pickup.ActivatePickupCollect();
                }
                else
                {
                    Collectible pickup = collider.GetComponent<Collectible>();
                    pickup.ActivatePickupCollect();
                }
            }
        }
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
                    //Debug.Log("2 shields left");
                    _shieldSprite.color = new Color(1f, 1f, 1f, 0.7f);
                    break;
                case 1:
                    //Debug.Log("1 shields left");
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
        StartCoroutine(_uiManager.CameraShakeRoutine());
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
        if (_audio.pitch != 1f)
        {
            _audio.pitch = 1f;
            _audio.time = 0;
        }
        _audio.clip = _powerupPickupClip;
        _audio.Play();
    }

    void playDebuffPickupClip()
    {
        _audio.clip = _powerupPickupClip;
        _audio.time = _audio.clip.length - 0.01f;
        _audio.pitch = -1f;
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

    public void UltimateLaserPowerupActivate()
    {
        playPowerupPickupClip();
        _is360LaserPowerupActive = true;
        StartCoroutine(UltimateLaserPowerDownRoutine());
    }

    public void DebuffTriggerJamActivate()
    {
        playDebuffPickupClip();
        StartCoroutine(DebuffTriggerJamRoutine());
    }

    public void LaserTrackingPowerupActivate()
    {
        playPowerupPickupClip();
        _isLaserTrackingActive = true;
        _trackingLasersLeft = 10;
    }

    public void RefillAmmo()
    {
        playPowerupPickupClip();
        _ammo = 15;
        _uiManager.UpdateAmmo(_ammo);
    }

    //Phase 1:Framework -- Health Collectible
    public void RefillHealth()
    {
        playPowerupPickupClip();
        if(_lives < 3)
        {
            _lives++;
            if(_lives == 2)
            {
                _rightWingDamage.SetActive(false);
            }
            else if(_lives == 3)
            {
                _leftWingDamage.SetActive(false);
            }
            _uiManager.UpdateLives(_lives);
        }
    }

    public void overlapWithBoss(bool overlap)
    {
        if (overlap)
        {
            _playerSprite.color = Color.red;
        }
        else
        {
            _playerSprite.color = _originalColor;
        }

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

    IEnumerator UltimateLaserPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _is360LaserPowerupActive = false;
    }

    //Phase 1:Framework -- Thruster: Scaling Bar HUD
    IEnumerator RefuelCoroutine()
    {
        Debug.Log("Start refuel coroutine");
        yield return new WaitForSeconds(3f);
        while (_fuel <= 100f)
        {
            _fuel += 0.5f;
            _uiManager.UpdateFuel(_fuel);
            yield return null;
        }

        canUseThrusters = true;
    }

    IEnumerator DebuffTriggerJamRoutine()
    {
        _fireLocked = true;
        _uiManager.TriggerJamActive();
        yield return new WaitForSeconds(5f);
        _fireLocked = false;
        _uiManager.UpdateAmmo(_ammo);
    }

}
