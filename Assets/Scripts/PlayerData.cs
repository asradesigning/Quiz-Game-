using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;
    public Credentials credentials;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else{
            Destroy(instance.gameObject);
            instance = this;
        }
    }
}

[System.Serializable]
public class Credentials
{
    public string playerName;
    public Sprite playerAvatar;
    public int playerBadge;
    public int playerScore;
    public int playerRank;
}
