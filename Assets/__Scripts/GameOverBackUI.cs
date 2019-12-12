using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverBackUI : MonoBehaviour
{
    public GameObject back;
    void Update()
    {
        if(LostCities.S.popUpGameOver == true)
        {
            back.SetActive(true);
        } else
        {
            back.SetActive(false);
        }
    }
}
