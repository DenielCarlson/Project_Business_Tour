using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChampionship : MonoBehaviour
{
    [SerializeField] private GameObject _uiWorldChampionship;
    GameObject _currentPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _currentPlayer = other.gameObject;
            if (_currentPlayer.GetComponent<PlayerScript>().RecipientIdCity == gameObject.GetComponent<IdCity>().ID)
            {
                _uiWorldChampionship.SetActive(true);
            }
        }
    }
}
