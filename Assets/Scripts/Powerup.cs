using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField] // 0=Triple shot, 1=Speed, 2=Shield, 3=360 Laser
    private int _powerupID;

    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if(transform.position.y < -5.2f)
        {
            Destroy(this.gameObject);
        }

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
                    default:
                        Debug.Log("DEFAULT");
                        break;
                }
            }
            Destroy(this.gameObject);
        }
    }
    
}
