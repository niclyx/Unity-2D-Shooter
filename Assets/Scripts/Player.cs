using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private float _speed = 10.0f;
    private float _speedMultiplier = 3f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private Vector3 _laserOffset = new Vector3(0, 1f, 0);
    [SerializeField]
    private float _rateOfFire = 0.1f;
    private float _canFire = -1f;

    private float yMax = 6f;
    private float yMin = -4f;
    [SerializeField]
    private bool _isTripleShotActive;
    [SerializeField]
    private bool _isSpeedPowerupActive;
    [SerializeField]
    private bool _isShieldPowerupActive;

    [SerializeField]
    private GameObject _shieldVisual;

    private SpawnManager _spawnManager;

    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL.");
        }
        transform.position = new Vector3(0, -3, 0);
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
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

    }

    public void Damage()
    {
        if(_isShieldPowerupActive)
        {
            _isShieldPowerupActive = false;
            _shieldVisual.SetActive(false);
            return;
        }

        _lives--;
        if (_lives < 1)
        {
            _spawnManager.PlayerDied();
            Destroy(this.gameObject);
        }



    }

    public void TripleShotPowerupActivate()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void SpeedPowerupActivate()
    {
        _isSpeedPowerupActive = true;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void ShieldPowerupActivate()
    {
        _isShieldPowerupActive = true;
        _shieldVisual.SetActive(true);
        //StartCoroutine(ShieldPowerDownRoutine());

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

    IEnumerator ShieldPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isShieldPowerupActive = false;
    }
}
