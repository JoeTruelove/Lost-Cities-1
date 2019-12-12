using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for the uGUI classes like Text

public class RoundResultUI : MonoBehaviour {
    private Text txt;

    private void Awake()
    {
        txt = GetComponent<Text>();
        txt.text = "";
    }

    private void Update()
    {
        if(LostCities.S.phase != TurnPhase.gameOver)
        {
            txt.text = "";
            return;
        }
        if(LostCities.S.draw)
        {
            txt.text = "Draw!";
        }
        else if(LostCities.S.winner)
        {
            txt.text = "Player 1 won";
            
        }
        else if(!LostCities.S.winner)
        {
            txt.text = "Player 2 won";
        }
        

        //Player winner = LostCities.winner;
    }
}
