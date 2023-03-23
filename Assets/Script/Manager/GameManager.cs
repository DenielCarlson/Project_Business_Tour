using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        Instance = this;
    }

    public void OnRollDiceClick()
    {
        GameObject player = GameObject.Find("Player");
        int dice = Random.Range(2, 13);
        player.GetComponent<Movement>().CityIndex(dice);
    }
}
