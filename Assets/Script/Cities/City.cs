using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour
{
    private GameObject player;

    private float _rentPrice;
    private int _levelCity;

    private int _idOwner;
    private bool _isOwn;

    [SerializeField] private float _initialPrice;
    [SerializeField] private Transform spawn;
    [SerializeField] private GameObject _uiCity;
    [SerializeField] private GameObject _buildHouse;

    private void Awake()
    {
        _uiCity.SetActive(false);
        _buildHouse.SetActive(false);
        _isOwn = false;
        _levelCity = 0;
    }

    public void OnBuyClick()
    {
        if (player.GetComponent<PlayerWallet>().Money > _initialPrice)
        {
            player.GetComponent<PlayerWallet>().Withdraw(_initialPrice);

            _idOwner = player.GetComponent<PlayerScript>().ID;
            _isOwn = true;
            _uiCity.SetActive(false);
            _buildHouse.SetActive(true);
        }
        else
        {

        }
    }

    public void OnBuildHouseOneClick()
    {
        _buildHouse.SetActive(false);

        GameObject house = Resources.Load("HouseLevel1") as GameObject;
        Instantiate(house, spawn.position, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject;

            Invoke("EnableUI", 0.5f);
        }
    }

    void EnableUI()
    {
        _uiCity.SetActive(true);
    }
}
