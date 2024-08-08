using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PhotonEventCodes
{
    public const byte TimerUpdate = 1;
}

public class TimeController : MonoBehaviour, IOnEventCallback
{
    public Sprite[] timerSprites; // Array of sprites for each second
    public float timePerSprite = 1f; // Time duration per sprite
    private Image timerImage;
    [SerializeField] private float timer;
    [SerializeField] float buzzerTimer = 5f;
    [SerializeField] float playTimer = 10f;
    private int currentSpriteIndex = 0;
    public bool isBuzzerTimerRunning = true;
    public bool isPlayTimerRunning = false;
    bool canPlay = false;

    void Start()
    {
        timerImage = GetComponent<Image>();
        InitializeTimer();
        CheckCanPlay();
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Update()
    {
        if (canPlay)
        {
            if (isBuzzerTimerRunning)
            {
                if (buzzerTimer > 0)
                {
                    buzzerTimer -= Time.deltaTime;
                    currentSpriteIndex += (int)Time.deltaTime;
                    timer = buzzerTimer;
                    timerImage.sprite = timerSprites[currentSpriteIndex];
                    SoundManager.instance.TickSound();
                    if (PhotonNetwork.IsConnected)
                    {
                        object[] content = new object[] { buzzerTimer, currentSpriteIndex, "Buzzer"};
                        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                        PhotonNetwork.RaiseEvent(PhotonEventCodes.TimerUpdate, content, raiseEventOptions, SendOptions.SendReliable);
                    }
                }
                else
                {
                    currentSpriteIndex = 0;
                    LevelManager.instance.LoseByTime("LoseByTimeNotAccepted");
                }
            }

            if (isPlayTimerRunning)
            {
                if (playTimer > 0)
                {
                    playTimer -= Time.deltaTime;
                    currentSpriteIndex += (int)Time.deltaTime;
                    timerImage.sprite = timerSprites[currentSpriteIndex];            
                    SoundManager.instance.TickSound();
                    timer = playTimer;
                    timerImage.sprite = timerSprites[currentSpriteIndex];
                    SoundManager.instance.TickSound();
                    if (PhotonNetwork.IsConnected)
                    {
                        object[] content = new object[] { buzzerTimer, currentSpriteIndex, "Play" };
                        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                        PhotonNetwork.RaiseEvent(PhotonEventCodes.TimerUpdate, content, raiseEventOptions, SendOptions.SendReliable);
                    }
                }
                else
                {
                    currentSpriteIndex = 0;
                    LevelManager.instance.LoseByTime("LoseByTimeAccepted");
                }
            }
        }
    }


    void InitializeTimer()
    {
        buzzerTimer = 5f;
        playTimer = 10f;
        timer = 0f;
        currentSpriteIndex = 0;
        timerImage.sprite = timerSprites[currentSpriteIndex];
    }

    void CheckCanPlay()
    {
        canPlay = !PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if (photonEvent.Code == PhotonEventCodes.TimerUpdate)
            {
                object[] data = (object[])photonEvent.CustomData;
                string timerType = data[2].ToString();

                if (timerType == "Buzzer")
                {
                    buzzerTimer = (float)data[0];
                    isBuzzerTimerRunning = true;
                }
                else if (timerType == "Play")
                {
                    playTimer = (float)data[0];
                    isPlayTimerRunning = true;
                }

                currentSpriteIndex = (int)data[1];
                timerImage.sprite = timerSprites[currentSpriteIndex];
            }
        }
    }

    public float GetCurrentTime()
    {
        return timer;
    }

    public void StartTimer(string type)
    {
        if (type == "Buzzer")
        {
            isBuzzerTimerRunning = true;
        }
        else if (type == "Play")
        {
            isPlayTimerRunning = true;
        }
    }

    public void StopTimer(string type)
    {
        if (type == "Buzzer")
        {
            isBuzzerTimerRunning = false;
        }
        else if (type == "Play")
        {
            isPlayTimerRunning = false;
        }
    }

    public void ResetTimer(string type)
    {
        StopTimer(type);
        InitializeTimer();
    }
}
