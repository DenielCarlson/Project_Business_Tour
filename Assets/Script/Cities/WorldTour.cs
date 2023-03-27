using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTour : MonoBehaviour
{
    [SerializeField] private GameObject _uiWorldTour;




    private void OnTriggerEnter(Collider other)
    {
        GameObject _currentPlayer;

        if (other.gameObject.CompareTag("Player"))
        {
            _currentPlayer = other.gameObject;

            if (_currentPlayer.GetComponent<PlayerScript>().RecipientIdCity == gameObject.GetComponent<IdCity>().ID)
            {
                _uiWorldTour.SetActive(true);
            }
        }
    }
}
