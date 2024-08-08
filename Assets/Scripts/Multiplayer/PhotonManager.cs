using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager instance;
    public PlayerMode playerMode;
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    public int Question = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    public void ConnectToPhoton()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Successfully connected to Photon server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Successfully Joined Lobby");
        if (playerMode == PlayerMode.Multiplayer)
        {
            PlayRandomMultiplayer();
        }
    }


    public void PlayRandomMultiplayer()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(returnCode + " " + message);
        CreateRoom();
    }

    public void CreateRoom()
    {
        string RoomName = "Room" + Random.Range(0, 1000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 1;
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();
        customRoomProperties.Add("Question", 3);
        roomOptions.CustomRoomProperties = customRoomProperties;
        PhotonNetwork.CreateRoom(RoomName, roomOptions, TypedLobby.Default);
        Debug.Log("Joining Room");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined");
        Debug.Log("Current Players " + PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log("Max Players " + PhotonNetwork.CurrentRoom.MaxPlayers);
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
           PhotonNetwork.LoadLevel(1);
        }
        Question = (int)PhotonNetwork.CurrentRoom.CustomProperties["Question"];
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(1);
            }
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
        foreach (RoomInfo room in roomList)
        {
            Debug.LogFormat("Room: {0}, Selected Level: {1}", room.Name, room.CustomProperties["selectedLevel"]);
        }
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }

    public void Questions(string type)
    {
        if(type == "Increase")
        {
            Question++;
        }
        else if(type == "Decrease")
        {
            if (Question > 0)
                Question--;
        }
    }

    public int AccessRoomProperties()
    {
        return Question;
    }
}
