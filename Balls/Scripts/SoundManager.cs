using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundManager
{
    public AudioClip safeHitSound;
    public AudioClip[] hitWallSounds;
    public AudioClip enemiesHitSound;
    public AudioClip playerExplodeSound;

    AudioSource soundEffectObject;

    public void Initialize()
    {
        soundEffectObject = GameObject.FindObjectOfType<AudioSource>();
    }

    public void PlaySafeHitSound()
    {
        soundEffectObject.PlayOneShot(safeHitSound);
    }

    public void PlayWallHitSound()
    {
        soundEffectObject.PlayOneShot(hitWallSounds[GetRandomInteger(0, hitWallSounds.Length-1)]);
    }

    public void PlayEnemiesHitSound()
    {
        soundEffectObject.PlayOneShot(enemiesHitSound);
    }

    public void PlayExplosionSound()
    {
        soundEffectObject.PlayOneShot(playerExplodeSound);
    }

    int GetRandomInteger(int from, int to)
    {
        return Random.Range(from, to);
    }
}
