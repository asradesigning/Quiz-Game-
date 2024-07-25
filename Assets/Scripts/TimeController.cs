using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    public Sprite[] timerSprites; // Array of sprites for each second
    public float timePerSprite = 1f; // Time duration per sprite
    private Image timerImage;
    private float timer;
    private int currentSpriteIndex = 0;
    private bool isPaused = false;
    private bool isTimerRunning = true;

    void Start()
    {
        timerImage = GetComponent<Image>(); // Assuming this script is attached to an Image component

        // Initialize timer
        timer = 0f;
        currentSpriteIndex = 0;
        timerImage.sprite = timerSprites[currentSpriteIndex];
    }

    void Update()
    {
        if (!isTimerRunning)
            return;

        // Increment timer only if not paused
        if (!isPaused)
        {
            timer += Time.deltaTime;

            // Check if it's time to change sprite
            if (timer >= timePerSprite)
            {
                // Reset timer
                timer -= timePerSprite;

                // Increment current sprite index
                currentSpriteIndex++;

                // If we've reached the end of the array, reset to the beginning
                if (currentSpriteIndex >= timerSprites.Length)
                {
                    currentSpriteIndex = 0;
                    GameManager.instance.YouLoose();
                }

                // Change sprite
                timerImage.sprite = timerSprites[currentSpriteIndex];
                SoundManager.instance.TickSound();
            }
        }
    }

    public void PauseTimer()
    {
        isPaused = true;
    }

    public void ResumeTimer()
    {
        isPaused = false;
    }

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        isTimerRunning = false;
        timer = 0;
        currentSpriteIndex = 0;
        timerImage.sprite = timerSprites[currentSpriteIndex];
    }
}
