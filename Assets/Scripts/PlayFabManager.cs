using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabManager : MonoBehaviour
{
    
    public static PlayFabManager instance;
    [SerializeField] PlayerData playerData;
    PlayerManager playerManager;
    [SerializeField] GameObject leaderboardScreen;
    [SerializeField] Transform leaderboardContent;
    [SerializeField] GameObject leaderboardItemPrefab;

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

    void Start()
    {
        if (PlayerPrefs.GetInt("Remember") == 1)
        {
            if(PlayerPrefs.GetString("LoginAs") == "Guest")
            {
                GuestLogin();
            }
        }
        else
        {
            GuestCreateAccount();
        }
        playerManager = FindObjectOfType<PlayerManager>();
    }

    #region GuestLogin

    public void GuestCreateAccount()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnGuestCreate, OnError);
    }

    void OnGuestCreate(LoginResult result)
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest(),
          result => {
              string playerName = "Player" + Random.Range(0, 1000);
              SavePlayerName(playerName);
              SavePlayerData(
                  new Credentials
                  {
                      playerName = playerName,
                      playerAvatar = null,
                      playerBadge = 0,
                      playerRank = 0,
                      playerScore = 0
                  }
                  );
              PlayerPrefs.SetInt("Remember", 1);
              PlayerPrefs.SetString("LoginAs", "Guest");
              GuestLogin();
          },
          error =>
          {
              OnError(error);
          });

    }

    void GuestLogin()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = false
        };

        PlayFabClientAPI.LoginWithCustomID(request, result =>
        {
            PlayerPrefs.SetInt("GuestLogin", 1);
            GetPlayerData();
        }, OnError);
    }

    #endregion
    #region PlayerName
    void SavePlayerName(string name)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            result =>
            {
                Debug.Log("Successfully updated display name to: " + name);
            },
            error =>
            {
                OnError(error);
            });
    }

    public string GetPlayerName()
    {
        return playerData.credentials.playerName;
    }

    #endregion
    #region PlayFab Defaults
    void OnSuccess(LoginResult result)
    {
        Debug.Log("Successful Login/Create Account");
    }

    void OnError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    #endregion
    #region Player Data

    public void UpdateUserScore(int score)
    {
        playerData.credentials.playerScore += score;
        SavePlayerData(playerData.credentials);
    }

    public void SavePlayerData(Credentials pData)
    {
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>
            {
                { "Player", JsonUtility.ToJson(pData) },
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }
    void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Data Send");
    }


    public void GetPlayerData()
    {
        Debug.Log("GetPlayerData() Called");
        var request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, OnGetData, OnDataError);
    }

    void OnDataError(PlayFabError error)
    {
        Debug.Log("PlayFab Can't Get Data");
        Debug.Log(error.GenerateErrorReport());
    }

    void OnGetData(GetUserDataResult result)
    {
        if (result.Data.TryGetValue("Player", out UserDataRecord userDataRecord))
        {
            string jsonData = userDataRecord.Value;
            playerData.credentials = JsonUtility.FromJson<Credentials>(jsonData);
            playerManager.LoadPlayerData(playerData);
            SendLeaderBoard(playerData.credentials.playerScore);
        }
    }

    #endregion
    #region LeaderBoard
    public void SendLeaderBoard(int value)
    {
        Debug.Log("Saveing Leaderboard: " + value);
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "MultiplayerHighestScore",
                    Value = value
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderBoardUpdate, OnError);
    }

    void OnLeaderBoardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successful Update LeaderBoard");
    }

    public void GetLeaderBoard()
    {
        leaderboardScreen.SetActive(true);
        if (leaderboardContent.childCount > 0)
        {
            for (int i = 0; i < leaderboardContent.childCount; i++)
            {
                Destroy(leaderboardContent.GetChild(i).gameObject);
            }
        }
        var request = new GetLeaderboardRequest
        {
            StatisticName = "MultiplayerHighestScore",
            StartPosition = 0
        };
        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboard, OnError);
    }

    void OnGetLeaderboard(GetLeaderboardResult result)
    {
        foreach (var item in result.Leaderboard)
        {
            var obj = Instantiate(leaderboardItemPrefab, leaderboardContent);
            LeaderBoardPrefab o = obj.GetComponent<LeaderBoardPrefab>();
            o.SetPlayerDetails(item.Position, item.DisplayName, item.StatValue);
        }
    }
    #endregion

    void Update()
    {

    }
}
