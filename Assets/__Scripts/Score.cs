using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [Header("Dynamic")]
    public float score = 0;
    private Text uiText;
    void Start()
    {
        uiText = GetComponent<Text>();
    }

    void Update()
    {
        score = Hero.S.score;
        uiText.text = score.ToString("Score: #,0");
    }
}
