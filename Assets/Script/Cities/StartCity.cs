using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCity : MonoBehaviour
{

    private GameObject _currentPlayer;
    [SerializeField] private GameObject _uiStart;


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            _currentPlayer = other.gameObject;

            if (_currentPlayer.GetComponent<PlayerScript>().RecipientIdCity == gameObject.GetComponent<IdCity>().ID)
            {

                Debug.Log(_currentPlayer.GetComponent<PlayerScript>().RecipientIdCity + ", " + gameObject.GetComponent<IdCity>().ID);

                _uiStart.SetActive(true);

            }
        }
    }
}
