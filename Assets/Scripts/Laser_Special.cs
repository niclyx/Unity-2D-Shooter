using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Special : MonoBehaviour
{
    private Transform _player;
    private float _speed = 6f;
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Transform>();
        if (_player == null)
        {
            Debug.LogError("Player not found");
        }
    }

    void Update()
    {
        Vector3 direction = _player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        transform.eulerAngles = Vector3.forward * angle;
        //direction.Normalize();
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.Damage();
            Destroy(this.gameObject);
        }
    }
}
