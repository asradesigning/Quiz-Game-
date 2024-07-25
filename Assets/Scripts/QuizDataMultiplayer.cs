using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuizData_M", menuName = "ScriptableObjects/QuizData_M", order = 2)]
public class QuizDataMultiplayer : ScriptableObject
{
    public QuizData[] multiplayerQuizs;
}

[System.Serializable]
public class QuizData
{
    public Sprite[] levelImg;
    public string[] question;
    public int correctAnswerIndex;
}
