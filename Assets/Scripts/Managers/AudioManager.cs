using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSourceExplosion;
    void Start()
    {
        _audioSourceExplosion = transform.Find("Explosion_sound").GetComponent<AudioSource>();
        if(_audioSourceExplosion == null)
        {
            Debug.LogError("Explosion_sound AudioSource is null");
        }
    }

    public void PlayExplosion()
    {
        _audioSourceExplosion.Play();
    }
}
