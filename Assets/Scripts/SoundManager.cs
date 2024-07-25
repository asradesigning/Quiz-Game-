using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource gameSound;
    public AudioSource tickSound;
    public AudioSource Click;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            GameSound();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
       
    }

    public void PlayClick()
    {
        Click.Play();
    }
    public void GameSound()
    {
        gameSound.Play();
    }
    public void TickSound()
    {
        tickSound.Play();
    }

    
}
