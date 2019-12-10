using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for the uGUI classes like Text

public class RoundResultUI : MonoBehaviour {
    private Text txt;
    private Text p1;
    private Text p2;

    private void Awake()
    {
        txt = GetComponent<Text>();
        p1 = GetComponent<Text>();
        p2 = GetComponent<Text>();
        txt.text = "";
        p1.text = "";
        p2.text = "";
    }

    private void Update()
    {
        if(LostCities.S.phase != TurnPhase.gameOver)
        {
            txt.text = "";
            p1.text = "";
            p2.text = "";
            return;
        }
        if(LostCities.S.draw)
        {
            txt.text = "Draw! Score: " + LostCities.S.p1Score;
            p1.text = "";
            p2.text = "";
        }
        else if(LostCities.S.winner)
        {
            txt.text = "Player 1 won";
            p1.text = "Player 1 Score: " + LostCities.S.p1Score;
            p2.text = "Player 2 Score: " + LostCities.S.p2Score;
            
        }
        else if(!LostCities.S.winner)
        {
            txt.text = "Player 2 won";
            p1.text = "Player 1 Score: " + LostCities.S.p1Score;
            p2.text = "Player 2 Score: " + LostCities.S.p2Score;
        }
        

        //Player winner = LostCities.winner;
    }
}
