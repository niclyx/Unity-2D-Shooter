using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField]
    private int _collectibleID;
    private GameObject _player;
    private bool _pickupCollectActive;

    private void Start()
    {
        _player = GameObject.Find("Player");
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }
    }
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (_pickupCollectActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, 10f * Time.deltaTime);
        }

        if (transform.position.y < -5.2f)
        {
            Destroy(this.gameObject);
        }
    }

    public void ActivatePickupCollect()
    {
        _pickupCollectActive = true;
    }

    public void DeactivatePickupCollect()
    {
        _pickupCollectActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if(_collectibleID == 0)
            {
               player.RefillAmmo();
            }
            else if(_collectibleID == 1)
            {
                player.RefillHealth();
            }
            Destroy(this.gameObject);
        }
    }
}
