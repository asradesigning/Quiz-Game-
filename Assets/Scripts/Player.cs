using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

public class Player : MonoBehaviour, IPunObservable
{
    public PlayerData PlayerData;
    public bool turn = false;
    public bool opponentTurn = false;
    PhotonView pv;
    public int currentPoints;
    public string Name;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        Name = PlayFabManager.instance.GetPlayerName();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TurnCall()
    {
        if (!opponentTurn && pv.IsMine) {
            turn = true;
            pv.RPC("RPC_TurnCall", RpcTarget.OthersBuffered, pv.ViewID);
            Debug.Log("Sending Turn!!!");
            GameManager.instance.timerScript.StopTimer("Buzzer");
            GameManager.instance.timerScript.ResetTimer("Buzzer");
            GameManager.instance.timerScript.StartTimer("Play");
        }
    }

    void IPunObservable.OnPhotonSerializeView(Photon.Pun.PhotonStream stream, Photon.Pun.PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentPoints);
        }

        if (stream.IsReading) 
        {
            currentPoints = (int)stream.ReceiveNext();
        }
    }

    public string GetPlayerName()
    {
        return Name;
    }
       
}
