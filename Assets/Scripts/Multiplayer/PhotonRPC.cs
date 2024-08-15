using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonRPC : MonoBehaviour
{
    [PunRPC]
    void RPC_LoadQuestion(int questionIndex, int levelIndex)
    {
        LevelManager.instance.RPC_LoadQuestion(questionIndex, levelIndex);
    }

    [PunRPC]
    void RPC_LoadLevel(int levelIndex)
    {
        LevelManager.instance.RPC_LoadLevel(levelIndex);
    }

    [PunRPC]
    void RPC_Answer(string state, string name, int points)
    {
        LevelManager.instance.RPC_Answer(state, name, points);
    }

    [PunRPC]
    void RPC_NextButtonClicked()
    {
        GameManager.instance.RPC_NextButtonClicked();
    }

    [PunRPC]
    void RPC_NextLevelDelay()
    {
        GameManager.instance.RPC_NextLevelDelay();
    }

    [PunRPC]
    void RPC_OpenQuestPanel()
    {
        LevelManager.instance.RPC_OpenQuestPanel();
    }

    [PunRPC]
    void RPC_CheckWin()
    {
        LevelManager.instance.CheckWin();
    }

    [PunRPC]
    void RPC_Question(string state)
    {
        PhotonManager.instance.Questions(state);
    }

    [PunRPC]
    void StartCountdown()
    {
        GameManager.instance.timerScript.RPC_StartCountdown();
    }

    [PunRPC]
    void HandleBuzzerPress(int playerID)
    {
        GameManager.instance.timerScript.RPC_HandleBuzzerPress(playerID);
    }

    [PunRPC]
    void SendAdOpponent(int actorNum, string name)
    {
        PhotonView[] players = FindObjectsOfType<PhotonView>();
        if (players.Length >= 0)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].Owner.ActorNumber == actorNum)
                {
                    LevelManager.instance.Opponent = players[i].GetComponent<Player>();
                    LevelManager.instance.Opponent.Name = name;
                    LevelManager.instance.SetupOpponent();
                    break;
                }
            }
        }
    }
}
