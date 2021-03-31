using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField] // 0=Triple shot, 1=Speed, 2=Shield
    private int _powerupID;

    // Update is called once per frame
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
                        Debug.Log("Triple shot powerup activated");
                        break;
                    case 1:
                        player.SpeedPowerupActivate();
                        Debug.Log("Speed powerup activated");
                        break;
                    case 2:
                        player.ShieldPowerupActivate();
                        Debug.Log("Shield powerup activated");
                        break;
                    default:
                        Debug.Log("DEFAULT");
                        break;
                }
                /*
                if(_powerupID==0)
                {
                    player.TripleShotActivate();
                }
                else if(_powerupID==1)
                {
                    Debug.Log("Speed powerup activated");
                }
                */
            }
            Destroy(this.gameObject);
        }
    }
    
}
