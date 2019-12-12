using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player2ScoreUI : MonoBehaviour
{
    private Text p2;

    private void Awake()
    {
        p2 = GetComponent<Text>();
        p2.text = "";
    }

    private void Update()
    {
        if (LostCities.S.phase != TurnPhase.gameOver)
        {
            p2.text = "";
            return;
        }
        p2.text = "Player 2 Score: " + LostCities.S.p2Score;
    }
}
