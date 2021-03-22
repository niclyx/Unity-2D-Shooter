using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private float _speed = 10.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private Vector3 _laserOffset = new Vector3(0, 0.8f, 0);
    [SerializeField]
    private float _rateOfFire = 0.1f;
    private float _canFire = -1f;

    private float yMax = 6f;
    private float yMin = -4f;
 
    void Start()
    {
        transform.position = new Vector3(0, -3, 0);
    }

    void Update()
    {
        CalculateMovement();

        if(Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
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
        Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);
    }

    public void Damage()
    {
        _lives--;
        if (_lives < 1)
            Destroy(this.gameObject);
    }
}
