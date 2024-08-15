using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class TimeController : MonoBehaviour
{
    [SerializeField] private float timer;
    [SerializeField] float buzzerTimer = 5f;
    [SerializeField] float playTimer = 10f;
    public bool isBuzzerPressed = false;
    public bool isPlayTimerRunning = false;
    Player player;

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            player = LevelManager.instance.player.GetComponent<Player>();
            ResetTime();
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsConnected) 
        {
            if (isPlayTimerRunning && !isBuzzerPressed)
            {
                timer -= Time.deltaTime;
                UpdateSliderValues();

                if (timer <= 0)
                {
                    EndBuzzerPhase();
                }
            }
            else if (isPlayTimerRunning && isBuzzerPressed && player.turn)
            {
                timer -= Time.deltaTime;
                LevelManager.instance.PlayerTimer(timer, playTimer);

                if (timer <= 0)
                {
                    EndPlayPhase();
                }
            }
            else if(isPlayTimerRunning && isBuzzerPressed && !player.turn)
            {
                timer -= Time.deltaTime;
                LevelManager.instance.OpponentTimer(timer, playTimer);
            }
        }
        else
        {
            if (isPlayTimerRunning && isBuzzerPressed)
            {
                timer -= Time.deltaTime;
                LevelManager.instance.PlayerTimer(timer, playTimer);

                if (timer <= 0)
                {
                    isPlayTimerRunning = false;
                    isBuzzerPressed = false;
                    LevelManager.instance.LoseByTime("LoseByTimeOffline");
                }
            }
        }
    }

    public void StartGame()
    {
        LevelManager.instance.player.RPC("StartCountdown", RpcTarget.All);
    }

    public void ResetTime()
    {
        isPlayTimerRunning = false;
        isBuzzerPressed = false;
        player.turn = false;
        timer = buzzerTimer;
        LevelManager.instance.SetTimerImage();
    }

    public void RPC_StartCountdown()
    {
        timer = buzzerTimer;
        isPlayTimerRunning = true;
        isBuzzerPressed = false;
    }

    public void DirectPlay()
    {
        isPlayTimerRunning = true;
        isBuzzerPressed = true;
        timer = playTimer;
        LevelManager.instance.PlayerTimer(playTimer, playTimer);
    }

    public void ResetForOffline()
    {
        isPlayTimerRunning = false;
        isBuzzerPressed = false;
        timer = playTimer;
        LevelManager.instance.PlayerTimer(playTimer, playTimer);
    }

    public void RPC_HandleBuzzerPress(int playerID)
    {
        isBuzzerPressed = true;
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerID)
        {
            // The local player pressed the buzzer
            player.turn = true;
            timer = playTimer;
            LevelManager.instance.PlayerTimer(playTimer, playTimer);
            LevelManager.instance.OpponentTimer(0, playTimer);
        }
        else
        {
            // The opponent pressed the buzzer
            player.turn = false;
            timer = 10;
            LevelManager.instance.PlayerTimer(0, playTimer);
            LevelManager.instance.OpponentTimer(playTimer, playTimer);
        }
    }

    void EndBuzzerPhase()
    {
        isPlayTimerRunning = false;
        if (!isBuzzerPressed)
        {
            LevelManager.instance.LoseByTime("LoseByTimeNotAccepted");
        }
    }

    void EndPlayPhase()
    {
        isPlayTimerRunning = false;
        isBuzzerPressed = false;
        LevelManager.instance.LoseByTime("LoseByTimeAccepted"); 
    }

    void UpdateSliderValues()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            LevelManager.instance.UserTimer(timer, buzzerTimer);
        }
        else
        {
            LevelManager.instance.UserTimer(timer, buzzerTimer);
        }
    }
}
