using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="PlayerData", menuName = "ScriptableObjects/PlayerData", order = 3)]
public class PlayerData : ScriptableObject
{
    public Credentials credentials;
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
