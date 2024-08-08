using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public enum PlayerLevels
{
    Beginner,
    Competent,
    Veteran,
    Elite,
    Genius
}

public enum PlayerMode
{
    Offline,
    Multiplayer,
    Team
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    PlayerData playerdata;
    public string levelName;
    [SerializeField] GameObject[] Medals;
    Slider levelSlider;
    public PlayerLevels state;
    public PlayerMode mode;
    [SerializeField] CanvasGroup CatergoryPanel, OnlineCategories, ChooseMode;

    public int required_xp;
    public int Score = 0;
    public int rankOfPlayer = 0;
    public int playerBagde = 0;
    //Player Details
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] Image playerAvatar;
    [SerializeField] TextMeshProUGUI playerScore;
    [SerializeField] TextMeshProUGUI playerRank;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        CatergoryPanel.gameObject.SetActive(false);
        DontDestroyOnLoad(this);
        Score = 0;
    }

    public void LoadPlayerData(PlayerData pData)
    {
        playerdata = pData;
        playerName.text = playerdata.credentials.playerName;
        playerAvatar.sprite = playerdata.credentials.playerAvatar;
        playerBagde = playerdata.credentials.playerBadge;
        Score = playerdata.credentials.playerScore;
        rankOfPlayer = playerdata.credentials.playerRank;
        /*playerdata.credentials.playerName = playerName.text;
        playerdata.credentials.playerAvatar = playerAvatar.sprite;
        playerdata.credentials.playerBadge = playerBagde;*/
       /* if (int.TryParse(playerScore.text, out Score))
        {
            playerdata.credentials.playerScore = Score;
        }
        if (int.TryParse(playerRank.text, out rankOfPlayer))
        {
            playerdata.credentials.playerRank = rankOfPlayer;
        }*/
      
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public void OfflineMode()
    {
        mode = PlayerMode.Offline;
        PhotonManager.instance.playerMode = mode;
        ChooseCategory();
    }

    public void MultiplayerMode()
    {
        mode = PlayerMode.Multiplayer;
        PhotonManager.instance.ConnectToPhoton();
        PhotonManager.instance.playerMode = mode;
        ChooseOnlineModes();
    }

    public void TeamMode()
    {
        mode = PlayerMode.Team;
        PhotonManager.instance.ConnectToPhoton();
        PhotonManager.instance.playerMode = mode;
        ChooseOnlineModes();
    }

    public void LoadPlayerItems()
    {
        if (GameManager.instance != null)
        {
            playerScore.text = GameManager.instance.score_TXT.text;
            levelSlider = GameManager.instance.level_Slider;
            

            // Initialize required_xp and levelSlider maxValue
            if (required_xp <= 0)
            {
                required_xp = 5000;
                levelSlider.maxValue = required_xp;
            }

            levelSlider.value = Score;
            playerScore.text = Score.ToString();
        }
        
    }

    public void ChooseCategory(string category)
    {
        
        
        if (mode == PlayerMode.Offline)
        {
            if (category == "Ancient")
            {
                levelName = category;
                SceneManager.LoadScene(1);
                LoadPlayerItems();

            }
            else if (category == "Science")
            {
                SceneManager.LoadScene(1);
                levelName = category;
                LoadPlayerItems();
            }
            else if (category == "Arts")
            {
                levelName = category;
                SceneManager.LoadScene(1);
                LoadPlayerItems();
            }
            else if (category == "Wars")
            {
                levelName = category;
                SceneManager.LoadScene(1);
                LoadPlayerItems();
            }
        }
        else
        {
            if (category == "Classic")
            {
                levelName = category;
                SceneManager.LoadScene(2);
                LoadPlayerItems();
            }
            else if (category == "Survival")
            {
                levelName = category;
                SceneManager.LoadScene(2);
                LoadPlayerItems();
            }
            else if (category == "SpeedRound")
            {
                levelName = category;
                SceneManager.LoadScene(2);
                LoadPlayerItems();
            }
        }
    }

    public void IncreaseScore(int score)
    {
        Score += score;
        playerScore.text = Score.ToString();
        levelSlider.value += score;
        ChangePlayerState();
        //PlayerPrefs.SetInt("player_Score", Score);
    }

    public void ChangePlayerState()
    {
       if(mode != PlayerMode.Multiplayer)
        {
            switch (state)
            {
                case PlayerLevels.Beginner:

                    playerBagde = 0;
                    ActivateMedals();
                    if (Score >= 5000)
                    {
                        state = PlayerLevels.Competent;
                        required_xp = 10000;
                        levelSlider.maxValue = required_xp;
                        if (Score == 5000)
                        {
                            GameManager.instance.StateChanged();
                        }
                    }
                    break;
                case PlayerLevels.Competent:
                    playerBagde = 1;
                    ActivateMedals();
                    if (Score >= 10000)
                    {
                        state = PlayerLevels.Veteran;
                        required_xp = 15000;
                        levelSlider.maxValue = required_xp;
                        if (Score == 10000)
                        {
                            GameManager.instance.StateChanged();
                        }
                    }
                    break;
                case PlayerLevels.Veteran:
                    playerBagde = 2;
                    ActivateMedals();
                    if (Score >= 15000)
                    {
                        state = PlayerLevels.Elite;
                        required_xp = 20000;
                        levelSlider.maxValue = required_xp;
                        if (Score == 15000)
                        {
                            GameManager.instance.StateChanged();
                        }
                    }
                    break;
                case PlayerLevels.Elite:
                    playerBagde = 3;
                    ActivateMedals();
                    if (Score >= 20000)
                    {
                        state = PlayerLevels.Genius;
                        playerBagde = 4;
                        ActivateMedals();
                        if (Score == 20000)
                        {
                            GameManager.instance.StateChanged();
                        }
                        // No need to set required_xp here, as Genius is the highest state
                        // You can optionally update levelSlider.maxValue to a higher value or leave it as is
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            Debug.Log("You are Playing Multiplayer");
        }
        
    }

    void ActivateMedals()
    {
        if (Medals[0] != null)
        {
            for (int i = 0; i < Medals.Length; i++)
            {
                Medals[i].SetActive(false);
            }
            Medals[playerBagde].SetActive(true);
        }
    }

    public void ModePanel()
    {
        ChooseMode.gameObject.SetActive(true);
        LeanTween.alphaCanvas(ChooseMode, 1, 0.6f);
    }

    public void ChooseCategory()
    {
        CatergoryPanel.gameObject.SetActive(true);
        LeanTween.alphaCanvas(CatergoryPanel, 1, 0.6f);
    }

    public void ChooseOnlineModes()
    {
       /* if(PhotonManager.instance.playerMode == PlayerMode.Multiplayer)
        {
            PhotonManager.instance.PlayRandomMultiplayer();
        }*/
    }
}
