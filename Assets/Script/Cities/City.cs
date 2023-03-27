using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviourPunCallbacks
{
    public int? IdOwner;
    public int LevelCity { get => _levelCity; private set => _levelCity = value; }
    public bool WasBought { get => _wasBought; private set => _wasBought = value; }
    public bool HasPlayer;
    public float InitialPrice;
    public float RentPrice { get; private set; }

    public GameObject Player { get; set; }
    private GameObject _flag;

    private bool _wasBought;
    public int _levelCity;


    [SerializeField] private Transform _spawn;
    [SerializeField] private GameObject _uiBuyCity;
    [SerializeField] private GameObject _uiBuildHouse;
    [SerializeField] private GameObject _uiRebuy;

    private void Awake()
    {
        _uiBuyCity.SetActive(false);
        _levelCity = 0;
    }

    private void Update()
    {

    }

    [PunRPC]
    public void CityWasBought()
    {
        _wasBought = true;
    }


    public void BuildFlag()
    {

        if (gameObject.CompareTag("LeftCity") || gameObject.CompareTag("RightCity"))
        {
            _spawn.position = new Vector3(transform.position.x + 1, transform.position.y + 0.4f, transform.position.z);

            _flag = PhotonNetwork.Instantiate("PurchasedFlag", _spawn.position, Quaternion.identity) as GameObject;

            _flag.name = "PurchasedFlag" + gameObject.GetComponent<IdCity>().ID ;
        }
        else if (gameObject.CompareTag("UpCity") || gameObject.CompareTag("DownCity"))
        {
            _spawn.position = new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z + 1);

            _flag = PhotonNetwork.Instantiate("PurchasedFlag", _spawn.position, Quaternion.identity) as GameObject;

            _flag.name = "PurchasedFlag" + gameObject.GetComponent<IdCity>().ID;
        }

        _levelCity++;
    }

    public void BuildHouse()
    {

        GameObject flag = GameObject.Find("PurchasedFlag" + gameObject.GetComponent<IdCity>().ID);
        if (flag != null)
        {
            PhotonNetwork.Destroy(flag);
        }

        GameObject house = null;

        if (_levelCity == 1)
        {
            if (gameObject.CompareTag("LeftCity") || gameObject.CompareTag("RightCity"))
            {
                _spawn.position = new Vector3(transform.position.x + 1, transform.position.y + 0.4f, transform.position.z);

                house = PhotonNetwork.Instantiate("HouseLevel1", _spawn.position, Quaternion.identity) as GameObject;
                house.name = "HouseLevel1-" + gameObject.GetComponent<IdCity>().ID;

            }
            else if (gameObject.CompareTag("UpCity") || gameObject.CompareTag("DownCity"))
            {
                _spawn.position = new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z + 1);

                house = PhotonNetwork.Instantiate("HouseLevel1", _spawn.position, Quaternion.identity) as GameObject;
                house.name = "HouseLevel1-" + gameObject.GetComponent<IdCity>().ID;

            }
        }

        _levelCity++;
    }

    void EnableUIBuyCity()
    {
        _uiBuyCity.SetActive(true);
    }

    void EnableUIBuildHouse()
    {
        _uiBuildHouse.SetActive(true);
    }


    private void OnTriggerEnter(Collider other)
    {

            if (other.gameObject.CompareTag("Player"))
            {
                HasPlayer = true;
                Player = other.gameObject;

                if (Player.GetComponent<PlayerScript>().RecipientIdCity == gameObject.GetComponent<IdCity>().ID && _wasBought == false)
                {
                    Invoke("EnableUIBuyCity", 0.5f);
                }
                else if (Player.GetComponent<PlayerScript>().RecipientIdCity == gameObject.GetComponent<IdCity>().ID && _wasBought && Player.GetComponent<PlayerScript>().ID == IdOwner)
                {
                    Invoke("EnableUIBuildHouse", 0.5f);
                }
                else if (Player.GetComponent<PlayerScript>().RecipientIdCity == gameObject.GetComponent<IdCity>().ID && _wasBought && Player.GetComponent<PlayerScript>().ID != IdOwner)
                {
                    _uiRebuy.SetActive(true);
                }

            }

    }

    private void OnTriggerExit(Collider other)
    {
        HasPlayer = false;
        Player = null;
    }
}
