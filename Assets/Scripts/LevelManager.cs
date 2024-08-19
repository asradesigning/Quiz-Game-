using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    [SerializeField] Image[] levelImg;
    [SerializeField] GameObject[] surePanel;
    [SerializeField] GameObject contentPanel;
    [SerializeField] GameObject QuestionsPanel;
    [SerializeField] GameObject GameOverPanel;
    [SerializeField] TextMeshProUGUI Winner;
    public GameObject[] levelTxt;
    [SerializeField] int levelIndex = 0;
    [SerializeField] int correct = 0;
    public Button buzzer;
    private int questionIndex = 0;
    private List<int> shuffledIndices;
    public List<LevelData> levels = new List<LevelData>();
    public string levelName;
    bool canPlay = false;
    public PhotonView player;
    public int LoadQuestionCount = 0;
    [SerializeField] GameObject User1, User2;
    public Player Opponent;
    int correctAnswers = 0;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                canPlay = true;
            }
            else
            {
                canPlay = false;
            }
        }
        else
        {
            canPlay = true;
        }
        SetLevels();
        if (canPlay)
        {
            ShuffleQuestions();
            LoadQuestion();
            OpenQuestPanel();
            //CorrectBtn();
        }
        surePanel[0].SetActive(false);
        surePanel[1].SetActive(false);
        CheckMode();
    }

    void CheckMode()
    {
        if (PhotonManager.instance.playerMode == PlayerMode.Offline)
        {
            buzzer.gameObject.SetActive(false);
            User1.SetActive(true);
            User2.SetActive(false);
            User1.GetComponent<User>().SetDetails(null, PlayFabManager.instance.GetPlayerBagde(), PlayFabManager.instance.GetPlayerName());
            User1.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }
        else
        {
            buzzer.gameObject.SetActive(true);
            User1.SetActive(true);
            User1.GetComponent<User>().SetDetails(null, PlayFabManager.instance.GetPlayerBagde(), PlayFabManager.instance.GetPlayerName());
        }
    }

    public void SetupOpponent()
    {
        User2.SetActive(true);
        User2.GetComponent<User>().SetDetails(null, Opponent.GetPlayerBadge(), Opponent.GetPlayerName());
    }                                                                                                                                                                                                                                              

    public void PlayerTurnCall()
    {
        if (!GameManager.instance.timerScript.isPlayTimerRunning || GameManager.instance.timerScript.isBuzzerPressed) return;
        player.RPC("HandleBuzzerPress", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        //User1.GetComponent<User>().SetTurn();
        //ResetTimers("mine");
    }

    public void UserTimer(float time, float totalTime)
    {
        float calculatedTime = time * ( 1 / totalTime);
        User1.GetComponent<User>().Timer.fillAmount = calculatedTime;
        User2.GetComponent<User>().Timer.fillAmount = calculatedTime;
    }

    public void PlayerTimer(float time, float totalTime)
    {
        float calculatedTime = time * (1 / totalTime);
        User1.GetComponent<User>().Timer.fillAmount = calculatedTime;
    }

    public void OpponentTimer(float time, float totalTime)
    {
        float calculatedTime = time * (1 / totalTime);
        User2.GetComponent<User>().Timer.fillAmount = calculatedTime;
    }

    public void SetTimerImage()
    {
        User1.GetComponent<User>().Timer.fillAmount = 1;
        User2.GetComponent<User>().Timer.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (!GameManager.instance.timerScript.isBuzzerPressed && !player.GetComponent<Player>().turn)
            {
                buzzer.interactable = true;
                User1.GetComponent<User>().Turn.SetActive(false);
                User2.GetComponent<User>().Turn.SetActive(false);
            }
            else
            {
                buzzer.interactable = false;
            }

            if (player.GetComponent<Player>().turn)
            {
                User1.GetComponent<User>().Turn.SetActive(true);
                User2.GetComponent<User>().Turn.SetActive(false);
                User2.GetComponent<User>().Timer.fillAmount = 0;
            }
            else if (GameManager.instance.timerScript.isBuzzerPressed && !player.GetComponent<Player>().turn)
            {
                User2.GetComponent<User>().Turn.SetActive(true);
                User1.GetComponent<User>().Turn.SetActive(false);
                User1.GetComponent<User>().Timer.fillAmount = 0;
            }
        }
        else
        {
            buzzer.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        if (canPlay)
        {
            LoadQuestion();
            OpenQuestPanel();
        }
        //CorrectBtn();
        surePanel[0].SetActive(false);
        surePanel[1].SetActive(false);
        User1.GetComponent<User>().Timer.fillAmount = 1;
        if(PhotonNetwork.IsConnected)
            User2.GetComponent<User>().Timer.fillAmount = 1;
    }

    public void SetLevels()
    {
        if(PhotonManager.instance.playerMode == PlayerMode.Offline)
        {
            if (PlayerManager.instance != null)
            {
                levelName = PlayerManager.instance.levelName;
            }
            if (levelName == "Ancient")
            {
                levels = FetchData.instance.Ancient_levels;
            }
            else if (levelName == "Science")
            {
                levels = FetchData.instance.Science_levels;
            }
            else if (levelName == "Arts")
            {
                levels = FetchData.instance.Arts_levels;
            }
            else if (levelName == "Wars")
            {
                levels = FetchData.instance.Wars_levels;
            }
        }
        else
        {
            levels = FetchData.instance.AllQuestions;
        }
       
    }


    public void OpenQuestPanel()
    {
        if (!PhotonNetwork.IsConnected)
        {
            LeanTween.alphaCanvas(contentPanel.GetComponent<CanvasGroup>(), 1, 0.5f);
            for (int i = 0; i < levelTxt.Length; i++)
            {
                levelTxt[i].GetComponent<TypewriterEffect>().TextAnimation();
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                player.RPC("RPC_OpenQuestPanel", RpcTarget.AllBuffered);
            }
        }
        
    }

    public void RPC_OpenQuestPanel()
    {
        LeanTween.alphaCanvas(contentPanel.GetComponent<CanvasGroup>(), 1, 0.5f);
        for (int i = 0; i < levelTxt.Length; i++)
        {
            levelTxt[i].GetComponent<TypewriterEffect>().TextAnimation();
        }
    }

    void ShuffleQuestions()
    {
        shuffledIndices = new List<int>();
        for (int i = 0; i < levels.Count; i++)
        {
            shuffledIndices.Add(i);
        }

        // Fisher-Yates shuffle
        for (int i = 0; i < shuffledIndices.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledIndices.Count);
            int temp = shuffledIndices[i];
            shuffledIndices[i] = shuffledIndices[randomIndex];
            shuffledIndices[randomIndex] = temp;
        }
    }

    public void LoadQuestion()
    {
        LoadQuestionCount++;
        int levelIndex = shuffledIndices[questionIndex];
        questionIndex++;
        if (!PhotonNetwork.IsConnected)
        {
            if (questionIndex < shuffledIndices.Count)
            {

                levelImg[0].sprite = levels[levelIndex].levelImg1;
                levelTxt[0].GetComponent<TypewriterEffect>().fullText = levels[levelIndex].question1;

                levelImg[1].sprite = levels[levelIndex].levelImg2;
                levelTxt[1].GetComponent<TypewriterEffect>().fullText = levels[levelIndex].question2;

                correct = levels[levelIndex].correctAnswerIndex;
            }
            else
            {
                Debug.Log("All questions have been answered!");
                // Optionally, you can reshuffle and start again or end the game
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (PhotonManager.instance.AccessRoomProperties() > 0)
                {
                    player.RPC("RPC_LoadQuestion", RpcTarget.AllBuffered, questionIndex, levelIndex);
                }
                else
                {
                    player.RPC("RPC_CheckWin", RpcTarget.AllBuffered);
                }
            }
        }      
    }

    public void CheckWin()
    {
        Player[] players = FindObjectsOfType<Player>();
        string winnerName = null;
        if(players.Length == 2)
        {
            if (players[0].currentPoints > players[1].currentPoints)
            {
                winnerName = players[0].GetPlayerName();
            }
            else if(players[1].currentPoints > players[0].currentPoints)
            {
                winnerName = players[1].GetPlayerName();
            }
            else if(players[1].currentPoints == players[0].currentPoints)
            {
                winnerName = "Draw";
            }
        }
        else
        {
            winnerName = players[0].GetPlayerName();
        }
        if (player.IsMine) 
        {
            PlayFabManager.instance.UpdateUserScore(player.GetComponent<Player>().currentPoints * 10);    
        }
        QuestionsPanel.SetActive(false);
        GameOverPanel.SetActive(true);
        if (winnerName == player.GetComponent<Player>().GetPlayerName()) 
        {
            GameOverPanel.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            GameOverPanel.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            GameOverPanel.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + player.GetComponent<Player>().currentPoints + " POINTS";
            GameOverPanel.transform.GetChild(1).GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + player.GetComponent<Player>().currentPoints * 10 + "xp";
        }
        else
        {
            GameOverPanel.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            GameOverPanel.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
            GameOverPanel.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+0";
            GameOverPanel.transform.GetChild(1).GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+0";
        }
        Winner.text = winnerName + " IS THE WINNER";
        
    }

    public void GameOver()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }

    public void RPC_LoadQuestion(int questionIndex, int levelIndex)
    {
      
            levelImg[0].sprite = levels[levelIndex].levelImg1;
            levelTxt[0].GetComponent<TypewriterEffect>().fullText = levels[levelIndex].question1;

            levelImg[1].sprite = levels[levelIndex].levelImg2;
            levelTxt[1].GetComponent<TypewriterEffect>().fullText = levels[levelIndex].question2;

            correct = levels[levelIndex].correctAnswerIndex;
            if (PhotonNetwork.IsMasterClient)
            {
                player.RPC("RPC_Question", RpcTarget.AllBuffered, "Decrease");
            }
    }


    public void LoadLevel()
    {
        if (!PhotonNetwork.IsConnected)
        {
            levelIndex++;
            levelImg[0].sprite = levels[levelIndex].levelImg1;
            levelTxt[0].GetComponent<TypewriterEffect>().fullText = levels[levelIndex].question1;

            levelImg[1].sprite = levels[levelIndex].levelImg2;
            levelTxt[1].GetComponent<TypewriterEffect>().fullText = levels[levelIndex].question2;

            correct = levels[levelIndex].correctAnswerIndex;
        }
        else {
            if (PhotonNetwork.IsMasterClient)
            {
                levelIndex++;
                player.RPC("RPC_LoadLevel", RpcTarget.AllBuffered, levelIndex);
            }
        }
    }

    public void RPC_LoadLevel(int levelIndex)
    {
        this.levelIndex = levelIndex;

        levelImg[0].sprite = levels[levelIndex].levelImg1;
        levelTxt[0].GetComponent<TypewriterEffect>().fullText = levels[levelIndex].question1;

        levelImg[1].sprite = levels[levelIndex].levelImg2;
        levelTxt[1].GetComponent<TypewriterEffect>().fullText = levels[levelIndex].question2;

        correct = levels[levelIndex].correctAnswerIndex;
    }

    public void Answer(int index)
    {
        if (index == correct)
        {
            GameManager.instance.AnswerGiven("Win", "");
            if (PhotonNetwork.IsConnected)
            {
                if (player.IsMine)
                {
                    player.GetComponent<Player>().currentPoints++;
                    User1.GetComponent<User>().UpdatePoints(player.GetComponent<Player>().currentPoints);
                    player.GetComponent<Player>().turn = false;
                    player.RPC("RPC_Answer", RpcTarget.OthersBuffered, "Win", PlayFabManager.instance.GetPlayerName(), player.GetComponent<Player>().currentPoints);
                }
            }
            else
            {
                correctAnswers++;
                User1.GetComponent<User>().UpdatePoints(correctAnswers);
            }
        }
        else
        {
            GameManager.instance.AnswerGiven("Lose", "");
            if (PhotonNetwork.IsConnected)
            {
                if (player.IsMine)
                {
                    player.GetComponent<Player>().currentPoints--;
                    User1.GetComponent<User>().UpdatePoints(player.GetComponent<Player>().currentPoints);
                    player.GetComponent<Player>().turn = false;
                    player.RPC("RPC_Answer", RpcTarget.OthersBuffered, "Lose", PlayFabManager.instance.GetPlayerName(), player.GetComponent<Player>().currentPoints);
                }
            }
            else
            {
                correctAnswers--;
                User1.GetComponent<User>().UpdatePoints(correctAnswers);
            }
        }
    }

    public void RPC_Answer(string state, string name, int points)
    {
        switch (state)
        {
            case "Win":
                GameManager.instance.AnswerGiven("OpponentWin", name);
                User2.GetComponent<User>().UpdatePoints(points);
                break;
            case "Lose":
                GameManager.instance.AnswerGiven("OpponentLose", name);
                User2.GetComponent<User>().UpdatePoints(points);
                break;
            case "LoseByTimeAcceptedOpponent":
                GameManager.instance.AnswerGiven("LoseByTimeAcceptedOpponent", name);
                User2.GetComponent<User>().UpdatePoints(points);
                break;
            default:
                break;
        }
        player.GetComponent<Player>().turn = false;
    }

    public void CheckPanel(int index)
    {
        if (index == 0)
        {
           
            if (!PhotonNetwork.IsConnected)
            {
               
                surePanel[0].SetActive(true);
            }
            else
            {
                if (player.GetComponent<Player>().turn)
                {
                   
                    surePanel[0].SetActive(true);
                }
                else
                {
                    
                    surePanel[0].SetActive(false);
                }
            }
            surePanel[1].SetActive(false);
            surePanel[1].GetComponent<CanvasGroup>().alpha = 0;
            LeanTween.alphaCanvas(surePanel[0].GetComponent<CanvasGroup>(), 1, 0.6f);
        }
        else
        {
           
            if (!PhotonNetwork.IsConnected)
            {
                
                surePanel[1].SetActive(true);
            }
            else
            {
                if (player.GetComponent<Player>().turn)
                {
                    
                    surePanel[1].SetActive(true);
                }
                else
                {
                    
                    surePanel[1].SetActive(false);
                }
            }
            surePanel[0].SetActive(false);
            surePanel[0].GetComponent<CanvasGroup>().alpha = 0;
            LeanTween.alphaCanvas(surePanel[1].GetComponent<CanvasGroup>(), 1, 0.7f);
        }
    }

    public void LoseByTime(string state)
    {
        if (PhotonNetwork.IsConnected)
        {
            if (state == "LoseByTimeAccepted")
            {
                if (player.GetComponent<Player>().turn)
                {
                    player.GetComponent<Player>().currentPoints--;
                    GameManager.instance.AnswerGiven("LoseByTimeAccepted", "");
                    player.RPC("RPC_Answer", RpcTarget.Others, "LoseByTimeAcceptedOpponent", PlayFabManager.instance.GetPlayerName(), player.GetComponent<Player>().currentPoints);
                }
            }
            else if (state == "LoseByTimeNotAccepted")
            {
                GameManager.instance.AnswerGiven("LoseByTimeNotAccepted", "");
                if (PhotonNetwork.IsMasterClient)
                {
                    player.RPC("RPC_Question", RpcTarget.AllBuffered, "Increase");
                }
            }
            player.GetComponent<Player>().turn = false;
        }
        else
        {
            if(state == "LoseByTimeOffline")
            {
                GameManager.instance.SkipAnswer();
            }
        }
    }

}
