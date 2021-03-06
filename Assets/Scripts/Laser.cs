using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 7.0f;

    private bool _isEnemyLaser = false;
    private bool _reverseShot;

    void Update()
    {
        if((!_isEnemyLaser && !_reverseShot) || _reverseShot)
        {
            MoveUp();
        } 
        else
        {
            MoveDown();
        }
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -7f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > 7f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    public void IsEnemyReverseShot()
    {
        _reverseShot = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if(other.CompareTag("Player") && _isEnemyLaser)
        if (other.CompareTag("Player") && gameObject.tag=="Enemy_Fire")
        {
            Player player = other.GetComponent<Player>();
            player.Damage();
            Destroy(this.gameObject);
        }
        if ((other.CompareTag("Powerup") || other.CompareTag("Collectible")) && _isEnemyLaser)
        {
            if (other.CompareTag("Powerup"))
            {
                Powerup pickup = other.GetComponent<Powerup>();
            }
            else
            {
                Collectible pickup = other.GetComponent<Collectible>();
            }
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }

    }
}
