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
    [SerializeField] CanvasGroup CatergoryPanel, ChooseMode, multiplayerWaiting;
    [SerializeField] Panel_Manager uiPanels;
    public int required_xp;
    public int Score = 0;
    public int rankOfPlayer = 0;
    public int playerBagde = 0;
    //Player Details
    [Header("Default User Panel")]
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] Image playerAvatar;
    [SerializeField] TextMeshProUGUI playerScore;
    [SerializeField] TextMeshProUGUI playerRank;
    [SerializeField] Image[] PlayerBadges;
    [Header("User Panel")]
    [SerializeField] TextMeshProUGUI playerNamePanel;
    [SerializeField] Image playerAvatarPanel;
    [SerializeField] TextMeshProUGUI playerScorePanel;
    [SerializeField] TextMeshProUGUI playerRankPanel;
    [SerializeField] Image[] PlayerBadgesPanel;
    [SerializeField] Slider LoadingSlider;

    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
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
        PlayFabManager.instance.SetPlayerManager(this);
        CatergoryPanel.gameObject.SetActive(false);
        Score = 0;
    }

    public void LoadPlayerData(PlayerData pData)
    {
        playerdata = pData;
        playerName.text = playerdata.credentials.playerName;
        //playerAvatar.sprite = null;
        playerBagde = playerdata.credentials.playerBadge;
        Score = playerdata.credentials.playerScore;
        rankOfPlayer = playerdata.credentials.playerRank;
        if(playerBagde != 0)
        {
            for(int i = 0; i < PlayerBadges.Length; i++)
                PlayerBadges[i].gameObject.SetActive(i == playerBagde);
        }
        else
        {
            for (int i = 0; i < PlayerBadges.Length; i++)
                PlayerBadges[i].gameObject.SetActive(i == 0);
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public void OfflineMode()
    {
        mode = PlayerMode.Offline;
        PhotonManager.instance.playerMode = mode;
        ChooseCategory(true);
    }

    public void MultiplayerMode()
    {
        multiplayerWaiting.gameObject.SetActive(true);
        LeanTween.alphaCanvas(multiplayerWaiting, 1, 0.6f);
        mode = PlayerMode.Multiplayer;
        PhotonManager.instance.ConnectToPhoton();
        PhotonManager.instance.playerMode = mode;
        //ChooseOnlineModes();
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

    IEnumerator CategorySlider(string category)
    {
        LoadingOpenClose(true);
        LoadingSlider.maxValue = 5f;
        for(int i = 0; i < 10; i++)
        {
            LoadingSlider.value += 1f;
            yield return new WaitForSeconds(0.1f);
        }

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

    public void ChooseCategory(string category)
    {
        StartCoroutine(CategorySlider(category));
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

    public void ModePanel(bool isActive)
    {
        if (isActive)
        {
            ChooseMode.gameObject.SetActive(true);
            LeanTween.alphaCanvas(ChooseMode, 1, 0.6f);
            MainMenuOpenClose(false);
        }
        else
        {
            LeanTween.alphaCanvas(ChooseMode, 0, 0.6f);
            LeanTween.delayedCall(0.6f, DelayClose);
            MainMenuOpenClose(true);
        }
    }

    public void ChooseCategory(bool isActive)
    {
        if (isActive)
        {
            CatergoryPanel.gameObject.SetActive(true);
            LeanTween.alphaCanvas(CatergoryPanel, 1, 0.6f);
            MainMenuOpenClose(false);
        }
        else
        {
            LeanTween.alphaCanvas(CatergoryPanel, 0, 0.6f);
            LeanTween.delayedCall(0.6f, DelayClose);
            MainMenuOpenClose(true);
        }
       
    }

    void DelayClose()
    {
        CatergoryPanel.gameObject.SetActive(false);
        ChooseMode.gameObject.SetActive(false);
    }

    public void ChooseOnlineModes()
    {
       /* if(PhotonManager.instance.playerMode == PlayerMode.Multiplayer)
        {
            PhotonManager.instance.PlayRandomMultiplayer();
        }*/
    }

    #region UIPanelsActiveDeactive

    public void MainMenuOpenClose(bool isActive)
    {
        if (isActive)
        {
            uiPanels.mainPanel.GetComponent<Toggle_Panels>().SetActiveState(true);
        }
        else
        {
            uiPanels.mainPanel.GetComponent<Toggle_Panels>().SetActiveState(false);
        }
       
    }
    public void LeaderBoardOpenClose(bool isActive)
    {
        if (isActive)
        {
            uiPanels.leaderBoard.GetComponent<Toggle_Panels>().SetActiveState(true);
        }
        else
        {
            uiPanels.leaderBoard.GetComponent<Toggle_Panels>().SetActiveState(false);
        }
    }
    public void UserProfileOpenClose(bool isActive)
    {
        if (isActive)
        {
            uiPanels.userProfile.GetComponent<Toggle_Panels>().SetActiveState(true);
            playerNamePanel.text = playerdata.credentials.playerName;
            //playerAvatarPanel.sprite = null;
            playerScorePanel.text = playerdata.credentials.playerScore.ToString();
            playerRankPanel.text = playerdata.credentials.playerRank.ToString();
            if (playerBagde != 0)
            {
                for (int i = 0; i < PlayerBadgesPanel.Length; i++)
                    PlayerBadgesPanel[i].gameObject.SetActive(i == playerBagde);
            }
            else
            {
                for (int i = 0; i < PlayerBadgesPanel.Length; i++)
                    PlayerBadgesPanel[i].gameObject.SetActive(i == 0);
            }
        }
        else
        {
            uiPanels.userProfile.GetComponent<Toggle_Panels>().SetActiveState(false);
        }
    }
    public void SettingsOpenClose(bool isActive)
    {
        if (isActive)
        {
            uiPanels.settingsPanel.GetComponent<Toggle_Panels>().SetActiveState(true);
        }
        else
        {
            uiPanels.settingsPanel.GetComponent<Toggle_Panels>().SetActiveState(false);
        }
    }

    public void SurePanelOpenClose(bool isActive)
    {
        if (isActive)
        {
            uiPanels.surePanel.GetComponent<Toggle_Panels>().SetActiveState(true);
        }
        else
        {
            uiPanels.surePanel.GetComponent<Toggle_Panels>().SetActiveState(false);
        }
    }

    public void WaitingOpenClose(bool isActive)
    {
        if (isActive)
        {
            uiPanels.waitingPanel.GetComponent<Toggle_Panels>().SetActiveState(true);
        }
        else
        {
            uiPanels.waitingPanel.GetComponent<Toggle_Panels>().SetActiveState(false);
        }
    }
    public void LoadingOpenClose(bool isActive)
    {
        if (isActive)
        {
            uiPanels.loadingPanel.GetComponent<Toggle_Panels>().SetActiveState(true);
        }
        else
        {
            uiPanels.loadingPanel.GetComponent<Toggle_Panels>().SetActiveState(false);
        }
    }
    public void TournamentMode(bool isActive)
    {
        if (isActive)
        {
            uiPanels.tournamentMode.GetComponent<Toggle_Panels>().SetActiveState(true);
        }
        else
        {
            uiPanels.tournamentMode.GetComponent<Toggle_Panels>().SetActiveState(false);
        }
    }
    public void TournamentFormat(bool isActive)
    {
        if (isActive)
        {
            uiPanels.tournamentFormat.GetComponent<Toggle_Panels>().SetActiveState(true);
        }
        else
        {
            uiPanels.tournamentFormat.GetComponent<Toggle_Panels>().SetActiveState(false);
        }
    }
    #endregion

    public void QuitGame()
    {
        Application.Quit();
    }
}
