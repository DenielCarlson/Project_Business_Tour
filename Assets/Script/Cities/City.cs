using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviourPunCallbacks
{
    public int IdCity;
    public int? IdOwner;
    public int LevelCity { get => _levelCity; private set => _levelCity = value; }
    public bool HasPlayer;
    public bool WasBought;
    public float InitialPrice;
    public float RentPrice { get; private set; }

    public GameObject Player { get; set; }

    private GameObject _flag;

    public int _levelCity;
    private bool _isOwn;

    [SerializeField] private Transform _spawn;
    [SerializeField] private GameObject _uiBuyCity;
    [SerializeField] private GameObject _uiBuildHouse;

    private void Awake()
    {
        _uiBuyCity.SetActive(false);
        _levelCity = 0;
    }

    private void Update()
    {

        if (IdOwner > 0)
        {
            _isOwn = true;
            WasBought = true;
        }
    }


    public void BuildFlag()
    {
        _flag = Resources.Load("PurchasedFlag") as GameObject;
        _flag.name = "PurchasedFlag" + IdCity;

        if (gameObject.CompareTag("LeftCity") || gameObject.CompareTag("RightCity"))
        {
            _spawn.position = new Vector3(transform.position.x + 1, transform.position.y + 0.4f, transform.position.z);
            Instantiate(_flag, _spawn.position, Quaternion.identity);
        }
        else if (gameObject.CompareTag("UpCity") || gameObject.CompareTag("DownCity"))
        {
            _spawn.position = new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z + 1);
            Instantiate(_flag, _spawn.position, Quaternion.identity);
        }
        
        _levelCity++;
    }


    public void BuildHouse()
    {

        GameObject flag = GameObject.Find("PurchasedFlag" + IdCity + "(Clone)");
        Destroy(flag);

        if (_levelCity == 1)
        {
            if (gameObject.CompareTag("LeftCity") || gameObject.CompareTag("RightCity"))
            {
                _spawn.position = new Vector3(transform.position.x + 1, transform.position.y + 0.4f, transform.position.z);
                GameObject house = Resources.Load("HouseLevel1") as GameObject;
                Instantiate(house, _spawn.position, Quaternion.identity);

            }
            else if (gameObject.CompareTag("UpCity") || gameObject.CompareTag("DownCity"))
            {
                _spawn.position = new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z + 1);
                GameObject house = Resources.Load("HouseLevel1") as GameObject;
                Instantiate(house, _spawn.position, Quaternion.identity);

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
            Player = other.gameObject;
            if (Player.GetComponent<PlayerScript>().RecipientIdCity == this.IdCity && WasBought == false)
            {
                HasPlayer = true;
                Player = other.gameObject;
                Invoke("EnableUIBuyCity", 0.5f);
            }
            else if (Player.GetComponent<PlayerScript>().RecipientIdCity == this.IdCity && Player.GetComponent<PlayerScript>().ID == IdOwner)
            {
                HasPlayer = true;
                Player = other.gameObject;
                Invoke("EnableUIBuildHouse", 0.5f);
            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
        HasPlayer = false;
        Player = null;
    }
}
