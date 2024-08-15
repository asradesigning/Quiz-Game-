using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

public class Player : MonoBehaviour, IPunObservable
{
    public PlayerData PlayerData;
    public bool turn = false;
    PhotonView pv;
    public int currentPoints;
    public string Name;
    public int Badge;
    public Texture2D avatar;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            Name = PlayFabManager.instance.GetPlayerName();
            Badge = PlayFabManager.instance.GetPlayerBagde();
            //avatar = PlayFabManager.instance.GetPlayerAvatar().texture;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void IPunObservable.OnPhotonSerializeView(Photon.Pun.PhotonStream stream, Photon.Pun.PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentPoints);
            stream.SendNext(Name);
            stream.SendNext(Badge); 
        }

        if (stream.IsReading) 
        {
            currentPoints = (int)stream.ReceiveNext();
            Name = stream.ReceiveNext().ToString();
            Badge = (int)stream.ReceiveNext();
            string tex = (string)stream.ReceiveNext();
            Debug.Log(avatar);
        }
    }

    public string GetPlayerName()
    {
        return Name;
    }

    public int GetPlayerBadge()
    {
        return Badge;
    } 
}
