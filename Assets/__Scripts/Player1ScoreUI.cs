using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1ScoreUI : MonoBehaviour
{
    private Text p1;

    private void Awake()
    {
        p1 = GetComponent<Text>();
        p1.text = "";
    }

    private void Update()
    {
        if (LostCities.S.phase != TurnPhase.gameOver)
        {
            p1.text = "";
            return;
        }
        p1.text = "Player 1 Score: " + LostCities.S.p1Score;
    }
}
