using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrototype : MonoBehaviour
{
    private float _speed = 5f;
    [SerializeField]
    private GameObject _laser;

    void Start()
    {
        
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, vertical, 0);
        transform.Translate(direction * _speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(_laser, transform.position, Quaternion.identity);
    }
}
