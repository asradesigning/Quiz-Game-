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
    [SerializeField] GameObject[] sureBgImg;
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
    private int player_Xp = 0;
    private List<int> shuffledIndices;
    public List<LevelData> levels = new List<LevelData>();
    public string levelName;
    bool canPlay = false;
    public PhotonView player;
    public int LoadQuestionCount = 0;
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
                canPlay=false;
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
    }

    public void PlayerTurnCall()
    {
        player.GetComponent<Player>().TurnCall();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.GetComponent<Player>().opponentTurn && !player.GetComponent<Player>().turn)
        {
            buzzer.interactable = true;
        }
        else
        {
            buzzer.interactable = false;
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
                GameManager.instance.OpenBook(0);
            }
            else if (levelName == "Science")
            {
                levels = FetchData.instance.Science_levels;
                GameManager.instance.OpenBook(1);
            }
            else if (levelName == "Arts")
            {
                levels = FetchData.instance.Arts_levels;
                GameManager.instance.OpenBook(2);
            }
            else if (levelName == "Wars")
            {
                levels = FetchData.instance.Wars_levels;
                GameManager.instance.OpenBook(3);
            }
        }
        else
        {
            levels = FetchData.instance.AllQuestions;
            GameManager.instance.OpenBook(1);
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
        Winner.text = winnerName;
        
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
            player_Xp = 100;
            PlayerManager.instance.IncreaseScore(player_Xp);
            GameManager.instance.AnswerGiven("Win", "");
            if (PhotonNetwork.IsConnected)
            {
                if (player.IsMine)
                {
                    player.GetComponent<Player>().currentPoints++;
                    player.GetComponent<Player>().turn = false;
                    player.GetComponent<Player>().opponentTurn = false;
                    player.RPC("RPC_Answer", RpcTarget.OthersBuffered, "Win", PlayFabManager.instance.GetPlayerName());
                }
            }
            else
            {

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
                    player.GetComponent<Player>().turn = false;
                    player.GetComponent<Player>().opponentTurn = false;
                    player.RPC("RPC_Answer", RpcTarget.OthersBuffered, "Lose", PlayFabManager.instance.GetPlayerName());
                }
            }
        }
    }

    public void RPC_Answer(string state, string name)
    {
        switch (state)
        {
            case "Win":
                GameManager.instance.AnswerGiven("OpponentWin", name);
                break;
            case "Lose":
                GameManager.instance.AnswerGiven("OpponentLose", name);
                break;
            case "LoseByTimeAccepted":
                GameManager.instance.AnswerGiven("LoseByTimeAccepted", name);
                break;
            case "LoseByTimeNotAccepted":
                GameManager.instance.AnswerGiven("LoseByTimeNotAccepted", name);
                break;  
            case "LoseByTimeAcceptedOpponent":
                GameManager.instance.AnswerGiven("LoseByTimeAcceptedOpponent", name);
                break;
            default:
                break;
        }
        player.GetComponent<Player>().turn = false;
        player.GetComponent<Player>().opponentTurn = false;

    }

    public void CheckPanel(int index)
    {
        if (index == 0)
        {
            sureBgImg[1].SetActive(false);
            if (!PhotonNetwork.IsConnected)
            {
                sureBgImg[0].SetActive(true);
                surePanel[0].SetActive(true);
            }
            else
            {
                if (player.GetComponent<Player>().turn)
                {
                    sureBgImg[0].SetActive(true);
                    surePanel[0].SetActive(true);
                }
                else
                {
                    sureBgImg[0].SetActive(false);
                    surePanel[0].SetActive(false);
                }
            }
            surePanel[1].SetActive(false);
            surePanel[1].GetComponent<CanvasGroup>().alpha = 0;
            LeanTween.alphaCanvas(surePanel[0].GetComponent<CanvasGroup>(), 1, 0.6f);
        }
        else
        {
            sureBgImg[0].SetActive(false);
            if (!PhotonNetwork.IsConnected)
            {
                sureBgImg[1].SetActive(true);
                surePanel[1].SetActive(true);
            }
            else
            {
                if (player.GetComponent<Player>().turn)
                {
                    sureBgImg[1].SetActive(true);
                    surePanel[1].SetActive(true);
                }
                else
                {
                    sureBgImg[1].SetActive(false);
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
        if(state == "LoseByTimeAccepted")
        {
            if (player.GetComponent<Player>().turn)
            {
                player.GetComponent<Player>().currentPoints--;
                GameManager.instance.AnswerGiven("LoseByTimeAccepted", "");
                player.RPC("RPC_Answer", RpcTarget.OthersBuffered, "LoseByTimeAcceptedOpponent", PlayFabManager.instance.GetPlayerName());
            }
            else if (player.GetComponent<Player>().opponentTurn)
            {
                Player[] players = FindObjectsOfType<Player>();
                for (int i = 0; i < players.Length; i++) 
                {
                    if (!players[i].GetComponent<PhotonView>().IsMine)
                    {
                        players[i].currentPoints--;
                        GameManager.instance.AnswerGiven("LoseByTimeAcceptedOpponent", players[i].GetPlayerName());
                        break;
                    }
                }
                player.RPC("RPC_Answer", RpcTarget.OthersBuffered, "LoseByTimeAccepted", "");
                player.RPC("RPC_Answer", RpcTarget.OthersBuffered, "LoseByTimeAccepted", "");
            }
        }
        else if(state == "LoseByTimeNotAccepted")
        {
            GameManager.instance.AnswerGiven("LoseByTimeNotAccepted", "");
            player.RPC("RPC_Question", RpcTarget.AllBuffered, "Increase");
            player.RPC("RPC_Answer", RpcTarget.OthersBuffered, "LoseByTimeNotAccepted", "");
        }
        player.GetComponent<Player>().turn = false;
        player.GetComponent<Player>().opponentTurn = false;

    }

}
