using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour
{
    public int IdCity;
    public int IdOwner;
    public int LevelCity { get => _levelCity; private set => _levelCity = value; }
    public bool HasPlayer;
    public bool WasBought;
    public float InitialPrice;
    public float RentPrice { get; private set; }

    public GameObject Player { get; set; }

    private GameObject _flag;

    private int _levelCity;
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

        if (_levelCity >= 2)
        {
            _uiBuildHouse.GetComponentInChildren<Button>().interactable = false;
        }
        else
        {
            _uiBuildHouse.GetComponentInChildren<Button>().interactable = true;
        }

        if (IdOwner > 0)
        {
            _isOwn = true;
            WasBought = true;
        }
    }


    public void BuildFlag()
    {
        _flag = Resources.Load("PurchasedFlag") as GameObject;
        Instantiate(_flag, _spawn.position, Quaternion.identity);
    }

    public void BuildHouse()
    {
        Debug.Log("Destroyed");
        Destroy(_flag);

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

        _levelCity++;
    }

    void EnableUIBuyHouse()
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
                Invoke("EnableUIBuyHouse", 0.5f);
            }
            else if (Player.GetComponent<PlayerScript>().RecipientIdCity == this.IdCity && Player.GetComponent<PlayerScript>().ID == IdOwner)
            {
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
