using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Score : MonoBehaviour
{
    public static Score S;

    public int computedScore;
    public int multiplier;

    void Awake()
    {
        if (S == null)
        {
            S = this; // Set the private singleton
        }
        else
        {
            Debug.LogError("ERROR: Score.Awake(): S is already set!");
        }
    }

    public int ComputeScore(List<CardLostCities> testList)
    {
        computedScore = 0;

        for (int i = 0; i < testList.Count; i++)
        {
            computedScore = computedScore + testList[i].rank;
        }
        return computedScore;

    }

    public int Multiplier(List<CardLostCities> testList)
    {
        int multiplier;

        multiplier = 1;

        int i = 0;
        while (testList[i].rank == 1)
        {
            multiplier = multiplier + 1;
            i = i + 1;
        }

        return multiplier;
    }
}
