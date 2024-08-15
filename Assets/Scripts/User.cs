using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    public Image Profile, Timer;
    public TextMeshProUGUI Name, Points;
    public GameObject Turn, Medal;

    public void SetDetails(Sprite profile, int badge, string name)
    {
        for (int i = 0; i < Medal.transform.childCount; i++) 
        {
            Medal.transform.GetChild(i).gameObject.SetActive(i == badge);
        }
        Name.text = name;
    }

    public void SetTurn()
    {
        Timer.fillAmount = 1;
        Turn.SetActive(true);
    }

    public void UpdatePoints(int points)
    {
        Points.text = points.ToString();
    }
}
