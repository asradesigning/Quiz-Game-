using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Waiting : MonoBehaviour
{
    public TextMeshProUGUI[] Names;

    private void Update()
    {
        Photon.Realtime.Player[] player = PhotonNetwork.PlayerList;
        for (int i = 0; i < player.Length; i++)
        {
            Names[player[i].ActorNumber - 1].text = player[i].NickName;
        }
    }
}
