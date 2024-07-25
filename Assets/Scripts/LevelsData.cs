using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "LevelsData", menuName = "ScriptableObjects/LevelsData", order = 1)]
public class LevelsData : ScriptableObject
{
    public LevelDatas[] Ancient_levels;
    public LevelDatas[] Science_levels;
    public LevelDatas[] Wars_levels;
    public LevelDatas[] Arts_levels;
    public LevelDatas[] TakeLevels;
}


[System.Serializable]
public class LevelDatas
{
    public Sprite[] levelImg;
    public string[] question1;
    public int levelNumber;
    public int correctAnswerIndex;
}
