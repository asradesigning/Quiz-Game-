using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class FetchData : MonoBehaviour
{
    public static FetchData instance;
    public string apiUrl = "http://localhost/Quiz/fetch.php";
    public List<LevelData> Ancient_levels = new List<LevelData>();
    public List<LevelData> Science_levels = new List<LevelData>();
    public List<LevelData> Arts_levels = new List<LevelData>();
    public List<LevelData> Wars_levels = new List<LevelData>();
    public List<LevelData> AllQuestions = new List<LevelData>();

    void Start()
    {
        StartCoroutine(GetDataFromApi());
    }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        
        
    }

    private IEnumerator GetDataFromApi()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Parse plain text response
                string[] rows = www.downloadHandler.text.Split('\n');
                foreach (string row in rows)
                {
                    if (string.IsNullOrWhiteSpace(row))
                        continue;

                    string[] columns = row.Split('|');
                    if (columns.Length == 7)
                    {
                        string imagePath1 = columns[0].Trim();
                        string imagePath2 = columns[1].Trim();
                        string question1 = columns[2].Trim();
                        string question2 = columns[3].Trim();
                        int levelNumber = int.Parse(columns[4].Trim());
                        int correctAnswer = int.Parse(columns[5].Trim());
                        string mode = columns[6].Trim();
                        Debug.Log(mode);

                        Sprite image1 = null;
                        Sprite image2 = null;

                        // Load and assign images
                        yield return StartCoroutine(LoadSprite(imagePath1, sprite =>
                        {
                            image1 = sprite;
                        }));

                        yield return StartCoroutine(LoadSprite(imagePath2, sprite =>
                        {
                            image2 = sprite;
                        }));

                        // Check if images are loaded
                        if (image1 != null && image2 != null)
                        {
                            LevelData levelData = new LevelData
                            {
                                levelImg1 = image1,
                                levelImg2 = image2,
                                question1 = question1,
                                question2 = question2,
                                levelNumber = levelNumber,
                                correctAnswerIndex = correctAnswer
                            };
                            switch (mode)
                            {
                                case "Ancient":
                                    Ancient_levels.Add(levelData);
                                    break;
                                case "Scientific":
                                    Science_levels.Add(levelData);
                                    break;
                                case "War":
                                    Wars_levels.Add(levelData);
                                    break;
                                case "Art_Culture":
                                    Arts_levels.Add(levelData);
                                    break;
                                default:
                                    break;
                            }
                            AllQuestions.Add(levelData);
                        }
                        else
                        {
                            Debug.LogWarning("Failed to load one or more images for level: " + levelNumber);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Invalid data format: " + row);
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to fetch data: " + www.error);
            }
        }
    }

    private IEnumerator LoadSprite(string imagePath, System.Action<Sprite> onLoaded)
    {
        string url = "http://localhost/Quiz/" + imagePath.Trim(); // Construct the full URL
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                if (texture == null)
                {
                    Debug.LogWarning($"Failed to load texture from URL: {url}");
                    onLoaded?.Invoke(null);
                    yield break;
                }

                Sprite sprite = TextureToSprite(texture);
                onLoaded?.Invoke(sprite);
            }
            else
            {
                Debug.LogError("Failed to load image: " + www.error);
                onLoaded?.Invoke(null);
            }
        }
    }

    private Sprite TextureToSprite(Texture2D texture)
    {
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
    }
}

[System.Serializable]
public class LevelData
{
    [Header("Images")]
    public Sprite levelImg1;
    public Sprite levelImg2;
    [Header("Questions")]
    public string question1;
    public string question2;
    [Header("Level")]
    public int levelNumber;
    [Header("Answer")]
    public int correctAnswerIndex;
}
