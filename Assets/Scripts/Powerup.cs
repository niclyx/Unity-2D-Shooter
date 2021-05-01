using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField] // 0=Triple shot, 1=Speed, 2=Shield, 3=360 Laser, 4=Fire Lock
    private int _powerupID;
    //[SerializeField]
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
        
        if(transform.position.y < -5.2f)
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
        if(other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if(player!=null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotPowerupActivate();
                        break;
                    case 1:
                        player.SpeedPowerupActivate();
                        break;
                    case 2:
                        player.ShieldPowerupActivate();
                        break;
                    case 3:
                        player.UltimateLaserPowerupActivate();
                        break;
                    case 4:
                        player.DebuffTriggerJamActivate();
                        break;
                    default:
                        Debug.Log("DEFAULT");
                        break;
                }
            }
            Destroy(this.gameObject);
        }
    }
    
}
