using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource gameSound;
    public AudioSource tickSound;
    public AudioSource Click;

    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

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
        if (gameSound != null)
            bgmSlider.value = gameSound.volume;

        if (Click != null)
            sfxSlider.value = Click.volume;

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetBGMVolume(float volume)
    {
        if (gameSound != null)
            gameSound.volume = volume;
    }

    // Method to set SFX volume based on slider value
    public void SetSFXVolume(float volume)
    {
        if (Click != null)
            Click.volume = volume;
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
